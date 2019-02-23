using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/* 
	Source: http://wiki.unity3d.com/index.php?title=TinyXmlReader 
	See site for usage.
	Extended to handle comments and headers.
*/
using System.Linq;

public class TinyXmlReader
{
	private string xmlString = "";
	private int idx = 0;

	public TinyXmlReader(string newXmlString)
	{
		xmlString = newXmlString;
	}

	public enum TagType { OPENING = 0, CLOSING = 1, COMMENT = 2, HEADER = 3};

	public string tagName = "";
	public TagType tagType = TagType.OPENING;
	public string content = "";


	// properly looks for the next index of _c, without stopping at line endings, allowing tags to be break lines	
	int IndexOf(char _c, int _i)
	{
		int i = _i;
		while (i < xmlString.Length)
		{
			if (xmlString[i] == _c)
				return i;

			++i;
		}

		return -1;
	}

	int IndexOf(string _s, int _i)
	{
		if (string.IsNullOrEmpty(_s))
			return -1;

		int i = _i;
		while (i < (xmlString.Length - _s.Length))
		{
			if (xmlString.Substring(i, _s.Length) == _s)
				return i;

			++i;
		}

		return -1;
	}

	string ExtractCDATA(int _i)
	{
		return string.Empty;
	}

	public bool Read()
	{
		if (idx > -1)
			idx = xmlString.IndexOf("<", idx);

		if (idx == -1)
		{
			return false;
		}
		++idx;

		int endOfTag = IndexOf('>', idx);
		if (endOfTag == -1)
			return false;

		// All contents of the tag, incl. name and attributes
		string tagContents = xmlString.Substring(idx, endOfTag - idx);

		int endOfName = IndexOf(' ', idx);
		if ((endOfName == -1) || (endOfTag < endOfName))
			endOfName = endOfTag;

		tagName = xmlString.Substring(idx, endOfName - idx);
		idx = endOfTag;

		// Fill in the tag name
		if (tagName.StartsWith("/"))
		{
			tagType = TagType.CLOSING;
			tagName = tagName.Remove(0, 1);	// Remove the "/" at the front
		}
		else if (tagName.StartsWith("?"))
		{
			tagType = TagType.HEADER;
			tagName = tagName.Remove(0, 1);	// Remove the "?" at the front
		}
		else if(tagName.StartsWith("!--"))
		{
			tagType = TagType.COMMENT;
			tagName = string.Empty;	// A comment doesn't have a tag name
		}
		else
		{
			tagType = TagType.OPENING;
		}

		// Set the contents of the tag with respect to the type of the tag
		switch (tagType)
		{
			case TagType.OPENING:
				content = xmlString.Substring(idx + 1);

				int startOfCloseTag = IndexOf("<", idx);
				if (startOfCloseTag == -1)
					return false;
				content = xmlString.Substring(idx + 1, startOfCloseTag - idx - 1);

				break;
			case TagType.COMMENT:
				if ((tagContents.Length - 5) < 0)
					return false;

				content = tagContents.Substring(3, tagContents.Length - 5);
				break;
			case TagType.HEADER:
				if ((tagContents.Length - 1) < 0)
					return false;

				content = tagContents.Substring(tagName.Length + 1, tagContents.Length - tagName.Length - 2);
				break;
			default:
				content = string.Empty;
				break;
		}

		return true;
	}

	// returns false when the endingTag is encountered
	public bool Read(string endingTag)
	{
		bool retVal = Read();
		if ((tagName == endingTag) && (tagType == TagType.CLOSING))
		{
			retVal = false;
		}
		return retVal;
	}

	public static string GetProtocol(string xmlString)
	{
		TinyXmlReader xmlreader = new TinyXmlReader(xmlString);
		xmlreader.Read();
		xmlreader.Read();
//		Debug.Log(xmlreader.tagName+":"+xmlreader.content);
		return xmlreader.content;
	}
	
	public static Dictionary<string, string> DictionaryFromXMLString(string xmlString)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();

		TinyXmlReader xmlreader = new TinyXmlReader(xmlString);
		
		int depth = -1;
		
		// While still reading valid data
		string[] tag = new string[10];
		string curTag = "";
		while (xmlreader.Read())
		{
			
			if (xmlreader.tagType == TinyXmlReader.TagType.OPENING)
				++depth;
			else if (xmlreader.tagType == TinyXmlReader.TagType.CLOSING)
				--depth;
			
			if ((depth >0) && (xmlreader.tagType == TinyXmlReader.TagType.OPENING))
			{
				if (xmlreader.content != "")
				{
					curTag = tag[depth-1]+xmlreader.tagName;
					if( !data.ContainsKey(curTag) )
					{
						data.Add(curTag, xmlreader.content);
					}
					else
					{
						Debug.LogWarning("Data already contained key " + curTag + " with value "+ data[ curTag ] +". Replacing value " + xmlreader.content);
						data[ curTag ] = xmlreader.content;
					}	
				}
				tag[depth] = xmlreader.tagName+"-";
			}
		}

		return data;
	}

	
	public static string DictionaryToXMLString(Dictionary<string, string> data, string root = "Root")
	{
		if (data == null)
			return string.Empty;
		
		List<string> keys = data.Keys.ToList();
		List<string> values = data.Values.ToList();
		
		string rawdata = string.Empty;
		rawdata += "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n";
		rawdata += "<"+root +">\r\n";
		
		for (int i = 0; i < data.Count; ++i)
		{
			string key = keys[i];
			string value = values[i];

			if (value.Contains('<') || value.Contains('>') || value.Contains('&'))
				value = "<![CDATA[" + value + "]]>"; 


			rawdata += "\t<" + key + ">" + value + "</" + key + ">\r\n";
		}
		
		rawdata += "</"+ root +">\r\n";
		return rawdata;
	}

	public static string GetString(string name, string value,string name0=null, string value0=null,string name1=null, string value1=null,string name2=null, string value2=null,string name3=null, string value3=null,string name4=null, string value4=null)
	{
		string res="<" + name + ">" + value + "</" + name + ">";
		if (name0 == null)
			return res;
		res = res + GetString(name0, value0);
		if (name1 == null)
			return res;
		res = res + GetString(name1, value1);
		if (name2 == null)
			return res;
		res = res + GetString(name2, value2);
		if (name3 == null)
			return res;
		res = res + GetString(name3, value3);
		if (name4 == null)
			return res;
		res = res + GetString(name4, value4);
		return res;
	}
}