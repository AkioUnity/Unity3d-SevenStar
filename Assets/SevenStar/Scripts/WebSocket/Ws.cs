using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Ws : MonoBehaviour
{
    public static Ws Instance;

    public static WebSocket ws;

//    public string reply = null;
    
    private void Awake()
    {
        Instance = this;
        ws=new WebSocket(new Uri("ws://211.238.13.182:18080"));
    }
    
    private IEnumerator Start()
    {
        yield return StartCoroutine(ws.Connect());
        Debug.Log("connected");
        while (true)
        {
            string reply0 = ws.RecvString();
            if (reply0 != null)
            {
                Receive(reply0);
            }
            if (ws.error != null)
            {
                Debug.LogError("Error: " + ws.error);
                break;
            }
            yield return 0;
        }
        Debug.Log("close");
        ws.Close();
    }

    public void Login(string id, string pass)
    {
        Send("<protocol>login</protocol><id>"+id+"</id><pass>"+pass+"</pass>");
    }
    
    public void Send(string res)
    {
        Debug.Log("Send: " + res);
//        reply = null;
        ws.SendString(res);
    }
    public void Receive(string res)
    {
        Debug.Log("Received: " + res);
        TexasHoldemClient c = TexasHoldemClient.Instance;
        res = "<xml>" + res + "</xml>";
        string protocol = TinyXmlReader.GetProtocol(res);
        c.AddRecvData((int)Protocol.GetValue(protocol),Encoding.UTF8.GetBytes (res));
//        reply = res;
    }
}
