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

    //Id = Guid.NewGuid().ToString();
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
  private bool firstRun;
  private bool[] roles;
  private const int portOut = 11000;

  public List<ClientData> ConnectedClients { get; set; }

  private int connectInt = 0;

  void Start()
  {
    ConnectedClients = new List<ClientData>();
    var roleTypes = Enum.GetValues(typeof(RoleType)).Cast<RoleType>().ToList();
    roles = Enumerable.Repeat(true, 3).ToArray();

    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    serverSocket.Bind(new IPEndPoint(IPAddress.Parse(Package.GetIpAdress()), portOut));

    Thread listenThread = new Thread(ListenThread);
    listenThread.Start();
  }

  public void ListenThread()
  {
    while (true)
    {
      serverSocket.Listen(0);

      if (!firstRun)
      {
        ConnectedClients.Add(new ClientData(serverSocket.Accept(), DataIn));
        Debug.Log("Added Client at ConnectedClients[" + (ConnectedClients.Count - 1) + "]");
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
          // disconnect
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
    Debug.Log("TCPHost: Incoming package: " + p.packetType);
    switch (p.packetType)
    {
      case PackageType.Selection:
        var enumValue = (RoleType)Enum.Parse(typeof(RoleType), p.data[0].ToString());

        PlayerController player = null;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
          if (ConnectedClients.Count < roles.Length)
          {
            if (enumValue == RoleType.OppsCommander)
            {
              Debug.Log("TCPHost: Player 1 selected");
              player = player1.GetComponent<PlayerController>();
              player.Available = false;
              roles[0] = false;
            }
            else if (enumValue == RoleType.WeaponsOfficer)
            {
              Debug.Log("TCPHost: Player 2 selected");
              player = player2.GetComponent<PlayerController>();
              player.Available = false;
              roles[1] = false;
            }
            else if (enumValue == RoleType.Captain)
            {
              Debug.Log("TCPHost: Player 3 selected");
              player = player3.GetComponent<PlayerController>();
              player.Available = false;
              roles[2] = false;
            }

            player.Id = ConnectedClients[connectInt].Id;
            ++connectInt;
          }
          else
          {
            var serverFullPackage = new Package(PackageType.ServerFull, "server");
            serverFullPackage.data.Add(player.Id);
          }

          p.data.Add(enumValue);
          ConnectedClients.ForEach(x => x.Socket.Send(p.ToBytes()));
        });

        break;

      case PackageType.Connected:
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
          ConnectedClients.Last().Id = p.senderId;

          Package roleAvailability = new Package(PackageType.Connected, p.senderId);
          roleAvailability.data.Add(roles);

          var test = ConnectedClients.FirstOrDefault(x => x.Id == p.senderId);
          test?.Socket.Send(roleAvailability.ToBytes());
        });

        break;

      case PackageType.Disconnected:
        var res = Enum.TryParse(p.data[0]?.ToString(), out enumValue);

        if (res)
        {
          UnityMainThreadDispatcher.Instance().Enqueue(() =>
          {
            if (enumValue == RoleType.OppsCommander)
            {
              Debug.Log("TCPHost: Player 1 disconnected");
              player = player1.GetComponent<PlayerController>();
              player.Available = true;
              roles[0] = true;
            }
            else if (enumValue == RoleType.WeaponsOfficer)
            {
              Debug.Log("TCPHost: Player 2 disconnected");
              player = player2.GetComponent<PlayerController>();
              player.Available = true;
              roles[1] = true;
            }
            else if (enumValue == RoleType.Captain)
            {
              Debug.Log("TCPHost: Player 3 disconnected");
              player = player3.GetComponent<PlayerController>();
              player.Available = true;
              roles[2] = true;
            }            
          });
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
          for (int i = 0; i < ConnectedClients.Count; ++i)
          {
            if (ConnectedClients[i].Id == p.senderId)
            {
              Debug.Log("TCPHost: Removing Client at ConnectedClients[" + i + "]");
              ConnectedClients.RemoveAt(i);
              break;
            }
          }

          --connectInt;

          Package roleAvailability = new Package(PackageType.Disconnected, p.senderId);
          roleAvailability.data.Add(roles);

          ConnectedClients.ForEach(x => x.Socket.Send(roleAvailability.ToBytes()));
        });
        break;

      case PackageType.Calibrate1:
        enumValue = (RoleType)Enum.Parse(typeof(RoleType), p.data[0].ToString());

        player = null;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
          if (enumValue == RoleType.OppsCommander)
          {
            player = player1.GetComponent<PlayerController>();
          }
          else if (enumValue == RoleType.WeaponsOfficer)
          {
            player = player2.GetComponent<PlayerController>();
          }
          else if (enumValue == RoleType.Captain)
          {
            player = player3.GetComponent<PlayerController>();
          }

          player.CapturePhoneStraight = true;
        });

        break;

      case PackageType.Calibrate2:
        enumValue = (RoleType)Enum.Parse(typeof(RoleType), p.data[0].ToString());

        player = null;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
          if (enumValue == RoleType.OppsCommander)
          {
            player = player1.GetComponent<PlayerController>();
          }
          else if (enumValue == RoleType.WeaponsOfficer)
          {
            player = player2.GetComponent<PlayerController>();
          }
          else if (enumValue == RoleType.Captain)
          {
            player = player3.GetComponent<PlayerController>();
          }

          player.CaptureFlashlightStraight = true;
        });

        break;
    }
  }
}
