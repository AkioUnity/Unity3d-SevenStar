using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtil;


public class AvataMgr : UtilObjectSingleton<AvataMgr>
{
    public string m_LoginUserID;
    public Sprite m_DefaultSprite;
    public List<Sprite> m_AvataSpriteList = new List<Sprite>();
    public List<Sprite> m_AvataBlackSpriteList = new List<Sprite>();
    public int[] m_AvataGender;

    public Sprite GetAvataSprite(int idx)
    {
        if (m_AvataSpriteList.Count <= idx || idx < 0)
            return null;
        return m_AvataSpriteList[idx];
    }

    public Sprite GetAvataBlackSprite(int idx)
    {
        if (m_AvataBlackSpriteList.Count <= idx || idx < 0)
            return null;
        return m_AvataBlackSpriteList[idx];
    }

    /// <summary>
    ///  0 : male
    ///  1 : female
    /// </summary>
    /// <param name="idx"></param>
    /// <returns></returns>
    public int GetAvataGender(int idx)
    {
        if (idx < 0 || m_AvataGender.Length <= idx)
            return -1;
        return m_AvataGender[idx];
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // set avata gender
        //m_AvataGender = new int[]{0,1,0,1,1,1,1,0,0};
        m_AvataGender = new int[] { 1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 };
    }

    public void StartDownload()
    {
        LoadTitleScene();
        return;


        string ftpPath = "ftp://211.238.13.182:23/Avata";
        string[] filePathList = GetFileList("Avata/");
        Array.Sort(filePathList);
        for (int i = 0; i < filePathList.Length; i++)
        {
            //Debug.Log(filePathList[i]);
            filePathList[i] = ftpPath + "/" + filePathList[i];
        }

        StartCoroutine(DownloadFiles(filePathList));
    }

    public void LoadTitleScene()
    {
        Debug.Log("Load 1_Title");
        SceneManager.LoadScene("1_Title");
    }

    IEnumerator DownloadFiles(string[] urls)
    {
        
        for (int i = 0; i < urls.Length; i++)
        {
            using (WWW www = new WWW(urls[i]))
            {
                yield return www;

                Texture2D texture = new Texture2D(1, 1);
                www.LoadImageIntoTexture(texture);
                Sprite image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                string[] imgName = urls[i].Split('/');
                image.name = imgName[imgName.Length-1];
                string[] splitName = image.name.Split('_');
                if(splitName[0].Equals("avata"))
                    m_AvataSpriteList.Add(image);
                else
                    m_AvataBlackSpriteList.Add(image);

                //m_Test.sprite = image;
            }
        }

        // Downloading done..
        LoadTitleScene();
    }

    public string[] GetFileList(string folderName)
    {
        string ftpServerIP = "ftp://211.238.13.182:23";// FTP Server IP ;
        string ftpUserID = "anonymous";// insert ID;
        string ftpPassword = "";// insert password;

        string[] downloadFiles;
        StringBuilder result = new StringBuilder();
        FtpWebRequest reqFTP;
        try
        {
            reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(ftpServerIP + "/" + folderName));
            reqFTP.UseBinary = true;
            reqFTP.UsePassive = true;
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
            WebResponse response = reqFTP.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());

            string line = reader.ReadLine();
            while (line != null)
            {
                result.Append(line);
                result.Append("\n");
                line = reader.ReadLine();
            }
            result.Remove(result.ToString().LastIndexOf('\n'), 1);
            reader.Close();
            response.Close();

            return result.ToString().Split('\n');
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            downloadFiles = null;
            return downloadFiles;
        }
    }



}
