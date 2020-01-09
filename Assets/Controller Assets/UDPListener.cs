using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPListener : MonoBehaviour
{
  UdpClient client;

  [SerializeField]
  int port;
  [SerializeField]
  bool printToConsole;

  [HideInInspector]
  public string message;

  public void Start()
  {
    message = "";

    Thread receiveThread = new Thread(new ThreadStart(() =>
    {
      client = new UdpClient(port);
      while (true)
      {
        try
        {
          IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
          byte[] data = client.Receive(ref ip);

          message = Encoding.UTF8.GetString(data);

          // print message to console
          if (printToConsole)
            Debug.Log(message);
        }
        catch (Exception e)
        {
          print(e.ToString());
        }
      }
    }))
    {
      IsBackground = true
    };
    receiveThread.Start();
  }
}