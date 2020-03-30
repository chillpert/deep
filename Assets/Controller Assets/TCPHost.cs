using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ClientData 
{
  private Thread thread;

  public ClientData(Socket socket, Action<object> method)
  {
    Socket = socket;

    Id = Guid.NewGuid().ToString();
    thread = new Thread(method.Invoke);
    thread.Start(this);
  }

  public RoleType RoleType { get; set; }

  public string Id { get; set; }

  public Socket Socket { get; }
}

public class TCPHost : MonoBehaviour
{
  [SerializeField]
  GameObject player1;
  [SerializeField]
  GameObject player2;
  [SerializeField]
  GameObject player3;

  private Socket serverSocket;

  public List<ClientData> ConnectedClients { get; set; }

  private List<RoleType> roles;

  private int connectInt = 0;

  void Start()
  {
    ConnectedClients = new List<ClientData>();
    var roleTypes = Enum.GetValues(typeof(RoleType)).Cast<RoleType>().ToList();
    roles = new List<RoleType>();

    roleTypes.ForEach(x => roles.Add(x));

    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    serverSocket.Bind(new IPEndPoint(IPAddress.Parse(Package.GetIpAdress()), 11000));

    Thread listenThread = new Thread(ListenThread);
    listenThread.Start();
  }

  bool firstRun = true;

  public void ListenThread()
  {
    while (true)
    {
      serverSocket.Listen(0);

      if (!firstRun)
      {
        ConnectedClients.Add(new ClientData(serverSocket.Accept(), DataIn));
        Debug.Log("ROAD ROLLERU");
      }

      firstRun = false;
    }
  }

  public void DataIn(object data)
  {
    ClientData player = (ClientData)data;
    byte[] buffer;
    int readBytes;

    while (true)
    {
      try
      {
        if (SocketConnected(player.Socket))
        {
          buffer = new byte[player.Socket.SendBufferSize];
          readBytes = player.Socket.Receive(buffer);
          if (readBytes > 0)
          {
            Package p = new Package(buffer);
            DataManager(p);
          }
        }
        else
        {
          //Disconnection
          var p = new Package(PackageType.Disconnected, player.Id);

          ConnectedClients.Remove(player);

          for (int i = 0; i < ConnectedClients.Count; i++)
            ConnectedClients[i].Socket.Send(p.ToBytes());

          break;
        }
      }
      catch (Exception e)
      {
        Debug.LogError(e.Message);
        break;
      }
    }
  }

  private bool SocketConnected(Socket s)
  {
    bool part1 = s.Poll(1000, SelectMode.SelectRead);
    bool part2 = (s.Available == 0);
    if (part1 & part2)
      return false;

    return true;
  }

  public void DataManager(Package p)
  {
    switch (p.packetType)
    {
      case PackageType.Connected:
        var enumValue = (RoleType)Enum.Parse(typeof(RoleType), p.data[0].ToString());

        IPlayer player = null;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
          if (ConnectedClients.Count < roles.Count)
          {
            if (enumValue == RoleType.OppsCommander)
            {
              player = player1.GetComponent<ControllerPlayer1>();
            }
            else if (enumValue == RoleType.WeaponsOfficer)
            {
              player = player2.GetComponent<ControllerPlayer2>();
            }
            else if (enumValue == RoleType.Captain)
            {
              player = player3.GetComponent<ControllerPlayer3>();
            }
            else
              Debug.LogError("OH NOOOOOOOOOOOOO");

            player.Id = ConnectedClients[connectInt].Id;

            Debug.Log("D'ARBYYY HAS MOTHERFUCKING CONNECTED");
            
            ++connectInt;
          }
          else
          {
            var serverFullPackage = new Package(PackageType.ServerFull, "server");
            serverFullPackage.data.Add(player.Id);
            //player.Socket.Send(p.ToBytes());
          }

          p.data.Add(enumValue);

          ConnectedClients.ForEach(x => x.Socket.Send(p.ToBytes()));
        });

        break;
    }
  }
}
