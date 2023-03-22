using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SocketServer : MonoBehaviour
{
    Socket serverSocket; //伺服器端socket
    Socket clientSocket; //客戶端socket
   

    IPEndPoint ipEnd; //偵聽埠
    string recvStr; //接收的字串
    string sendStr; //傳送的字串
    byte[] recvData = new byte[1000]; //接收的資料，必須為位元組
    byte[] sendData = new byte[1000]; //傳送的資料，必須為位元組
    int recvLen; //接收的資料長度
    Thread connectThread; //連線執行緒
    int packageCount = 0;
    byte[] colorBuffer;
 
    bool isFinish = false;
    public int textureTotalByteCount = 0;
    public GameObject prefab;
    // public GameObject signManager;
    public GameObject positionManager;
    public int weight;
    public int height;
    //private List<string> stringList = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
    private List<int> intList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
    public Text debugText;


    //初始化
    void InitSocket()
    {
        //定義偵聽埠,偵聽任何IP
        ipEnd = new IPEndPoint(IPAddress.Any, 5566);
        //定義套接字型別,在主執行緒中定義
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //連線
        serverSocket.Bind(ipEnd);
        //開始偵聽,最大10個連線
        serverSocket.Listen(10);
        //開啟一個執行緒連線，必須的，否則主執行緒卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    //連線
    void SocketConnect()
    {
        if (clientSocket != null)
            clientSocket.Close();
        //控制檯輸出偵聽狀態
        print("Waiting for a client");
        //一旦接受連線，建立一個客戶端
        clientSocket = serverSocket.Accept();
        //獲取客戶端的IP和埠
        IPEndPoint ipEndClient = (IPEndPoint)clientSocket.RemoteEndPoint;
    }

    void SocketSend(string sendStr)
    {
        //清空傳送快取
        sendData = new byte[1000];
        //資料型別轉換
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //傳送
        clientSocket.Send(sendData, sendData.Length, SocketFlags.None);
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
            recvLen = clientSocket.Receive(recvData);
            //如果收到的資料長度為0，則重連並進入下一個迴圈
            if (recvLen == 0)
            {
                SocketConnect();
                continue;
            }
            //輸出接收到的資料

            Array.Copy(recvData, 0, colorBuffer, textureTotalByteCount, recvLen);
            textureTotalByteCount += recvLen;

            //print(recvData.Length + " " + recvLen + " " + textureTotalByteCount + " " + colorBuffer.Length);
         
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            //debugText.text = textureTotalByteCount.ToString();
            if (textureTotalByteCount == 2097152)
            {
                isFinish = true;
                textureTotalByteCount = 0;

            }

        }
    }

    //連線關閉
    void SocketQuit()
    {
        //先關閉客戶端
        if (clientSocket != null)
            clientSocket.Close();
        //再關閉執行緒
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最後關閉伺服器
        serverSocket.Close();
        print("diconnect");
    }

    // Use this for initialization
    void Start()
    {
        colorBuffer = new byte[512 * 1024 * 4];
        InitSocket(); //在這裡初始化server
    


    }


    // Update is called once per frame
    void Update()
    {
     
            if (isFinish)
            {
                isFinish = false;

                Texture2D t = new Texture2D(512, 1024, TextureFormat.RGBA32, false);

                t.LoadRawTextureData(colorBuffer);
                t.Apply();

                GameObject sign = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity).transform.GetChild(0).gameObject;
                Vector3 signScale = sign.gameObject.transform.localScale;
                float scale = Random.Range(0.07f, 0.09f);
                sign.gameObject.transform.localScale = new Vector3(signScale.x * scale, signScale.y * scale, signScale.z * 1);

                // sign.transform.parent = PositionManager.transform.GetChild(result);
                sign.GetComponent<RawImage>().texture = t;
                int pos = Random.Range(0, intList.Count);
                int result = intList[pos];
              
                GameObject positionObj = positionManager.transform.GetChild(result).gameObject;

                if (positionObj.transform.childCount != 0)
                {
                    GameObject preSign = positionObj.transform.GetChild(0).gameObject;
                    Destroy(preSign);
                }
                Destroy(sign, 1800);


                sign.transform.parent = positionObj.transform;
                sign.transform.localPosition = Vector2.zero;

                intList.RemoveAt(pos);
                if (intList.Count == 0)
                {
                    intList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
                }
            
            

        }


      
    }
    void OnApplicationQuit()
    {
        SocketQuit();
    }
}
