﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using UnityEngine.UI;


public class CheckSocket : MonoBehaviour
{
    Socket checkSever;
    Socket checkClient;


    IPEndPoint ipEnd; //偵聽埠
    string recvStr; //接收的字串
    string sendStr; //傳送的字串
    byte[] recvData = new byte[1000]; //接收的資料，必須為位元組
    byte[] sendData = new byte[1000]; //傳送的資料，必須為位元組
    int recvLen; //接收的資料長度
    Thread connectThread; //連線執行緒
    int packageCount = 0;
    bool isFinish = false;
    private SocketServer imageSocketServer;
    private bool isDelayCheck = false;



    //初始化
    void InitSocket()
    {
        //定義偵聽埠,偵聽任何IP
        ipEnd = new IPEndPoint(IPAddress.Any, 7172);
        //定義套接字型別,在主執行緒中定義
        checkSever = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //連線
        checkSever.Bind(ipEnd);
        //開始偵聽,最大10個連線
        checkSever.Listen(10);
        //開啟一個執行緒連線，必須的，否則主執行緒卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    //連線
    void SocketConnect()
    {
        if (checkClient != null)
            checkClient.Close();
        //控制檯輸出偵聽狀態
        print("Waiting for a client");
        //一旦接受連線，建立一個客戶端
        checkClient = checkSever.Accept();
        //獲取客戶端的IP和埠
        IPEndPoint ipEndClient = (IPEndPoint)checkClient.RemoteEndPoint;
    }

    void SocketSend(string sendStr)
    {
        //清空傳送快取
        sendData = new byte[1000];
        //資料型別轉換
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //傳送
        checkClient.Send(sendData, sendData.Length, SocketFlags.None);
    }

    //伺服器接收
    void SocketReceive()
    {
        //連線
        SocketConnect();
        //進入接收迴圈
        while (true)
        {
            //對data清零
            recvData = new byte[512];
            //獲取收到的資料的長度
            recvLen = checkClient.Receive(recvData);
            //如果收到的資料長度為0，則重連並進入下一個迴圈
            if (recvLen == 0)
            {
                SocketConnect();
                continue;
            }
            //輸出接收到的資料

            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print("get check "+recvStr);
            if (recvStr == "Check")
            {
                print("get check");
                isDelayCheck = true;
            }


        }
    }
    void delayCheck()
    {
        
        if (imageSocketServer.textureTotalByteCount == 0)
        {
            SocketSend("Yes!");
        }
        else
        {
            SocketSend("NO!");
        }
        
    
    }
    
    //連線關閉
    void SocketQuit()
    {
        //先關閉客戶端
        if (checkClient != null)
            checkClient.Close();
        //再關閉執行緒
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最後關閉伺服器
     
        print("diconnect");
    }

    // Use this for initialization
    void Start()
    {
 
        InitSocket(); //在這裡初始化server
        imageSocketServer = GetComponent<SocketServer>();


    }


    // Update is called once per frame
    void Update()
    {
        if (isDelayCheck)
        {
            isDelayCheck = false;
            Invoke("delayCheck", 5);
        }

    }
    void OnApplicationQuit()
    {
        SocketQuit();
    }
}
