using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text;

public class ClientNetwork : MonoBehaviour
{

    public void Start()
    {

    }

    /// <summary>
    /// Start Send Package
    /// </summary>
    /// <param name="selfPort"></param>
    /// <param name="targetPort"></param>
    public void StartSend(int selfPort, int targetPort)
    {
        if (canSend) return;

        udpClient = new UdpClient(selfPort);

        targetPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), targetPort);

        canSend = true;
    }

    private IPEndPoint endPoint;

    private byte[] recvBuf;
    private string recvStr;

    private bool readed;

    public void StartReceive(int targetPort)
    {
        if (canReceive) return;

        udpReceiv = new UdpClient();

        new Thread(() =>
        {
            while (canReceive)
            {
                try
                {
                    udpReceiv.BeginReceive(new System.AsyncCallback((System.IAsyncResult ar) =>
                    {
                        recvBuf = udpReceiv.EndReceive(ar, ref endPoint);
                        recvStr += Encoding.Default.GetString(recvBuf);
                        readed = true;
                    }), null);
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }).Start();
    }

    private void WriteData(string strData)
    {
        if (!canSend) return;
        byte[] writeData = Encoding.Default.GetBytes(strData);
        udpClient.Send(writeData, writeData.Length,targetPoint);
    }

    private string ReadData()
    {
        if(readed)
        {
            readed = false;
            recvStr = "";
            return recvStr;
        }
        return "";
    }

    /*  *   */

    UdpClient udpClient;

    UdpClient udpReceiv;

    /// <summary>
    /// Write data target point
    /// </summary>
    IPEndPoint targetPoint;

    /*  *   */

    private int selfPort { get; set; }

    private bool canSend { get; set; }

    private bool canReceive { get; set; }

}
