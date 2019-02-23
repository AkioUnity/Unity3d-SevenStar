using UnityEngine;
using System.Collections;
using System;

public class EchoTest : MonoBehaviour {
	public  WebSocket ws = new WebSocket(new Uri("ws://211.238.13.182:18080"));
	// Use this for initialization
	IEnumerator Start () {
		Debug.Log("start");
		yield return StartCoroutine(ws.Connect());
		Debug.Log("connect");
		ws.SendString("<protocol>roomidxlist</protocol><blindtype>1</blindtype>");
		int i=0;
		while (true)
		{
			string reply = ws.RecvString();

			if (reply != null)
			{
				Debug.Log ("Received: "+reply);
				i++;
//				if (i==1)
//					ws.SendString("<protocol>login</protocol><id>t1</id><pass>a</pass>");
//				if (i==2)
//					ws.SendString("<protocol>userinfo</protocol><useridx>2</useridx>");
			}
			if (ws.error != null)
			{
				Debug.LogError ("Error: "+ws.error);
				break;
			}
			yield return 0;
		}
		Debug.Log("close");
		ws.Close();
	}
}
