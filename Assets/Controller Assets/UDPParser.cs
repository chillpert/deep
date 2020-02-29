using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class UDPParser : MonoBehaviour
{
  [SerializeField]
  int port;
  [SerializeField]
  UDPListener listener;
  [SerializeField]
  GameObject player1;
  [SerializeField]
  GameObject player2;
  [SerializeField]
  GameObject player3;
  [SerializeField]
  bool printConnectionAttempts;
  [SerializeField]
  bool printMessageOut;

  string prevMessageGyro;
  string prevMessageAccelerometer;
  string prevJoystick;

  int receivedRole = 1;

  [HideInInspector]
  public List<string> localIPs = new List<string>();

  void Start()
  {
    prevMessageGyro = "";
    prevMessageAccelerometer = "";
  }

  void Update()
  {
    if (listener.message.Length > 0)
    {
      // check for { x } - format
      if (listener.message[0] == '{' && listener.message[listener.message.Length - 1] == '}')
      {
        // parse local IP
        string localIP = listener.message.Substring(1, listener.message.IndexOf("}") - 1);

        bool alreadyExists = false;
        int hit = -1;

        for (int i = 0; i < localIPs.Count; i++)
        {
          if (localIPs[i] == localIP)
          {
            hit = i;
            alreadyExists = true;
          }
        }

        // new IPs will be added to the list of connected users
        if (!alreadyExists)
        {
          localIPs.Add(localIP);

          if (printConnectionAttempts)
            Debug.Log("Info: UDPParser: Registered new IP: " + localIP);
        }

        if (alreadyExists)
        {
          // check if message is asking for player selection
          int playerSelectionPos = listener.message.IndexOf("{R(?)}");
          int commanderPos = listener.message.IndexOf("{R(OC)}");
          int officerPos = listener.message.IndexOf("{R(WO)}");
          int captainPos = listener.message.IndexOf("{R(CPT)}");

          bool commanderAvailable = player1.GetComponent<ControllerPlayer1>().available;
          bool officerAvailable = player2.GetComponent<ControllerPlayer2>().available;
          bool captainAvailable = player3.GetComponent<ControllerPlayer3>().available;

          if (playerSelectionPos != -1)
          {
            string sendRoleMsg = "{R";

            if (commanderAvailable)
              sendRoleMsg += "(OC:1)";
            else
              sendRoleMsg += "(OC:0)";

            if (officerAvailable)
              sendRoleMsg += "(WO:1)";
            else
              sendRoleMsg += "(WO:0)";

            if (captainAvailable)
              sendRoleMsg += "(CPT:1)";
            else
              sendRoleMsg += "(CPT:0)";

            sendRoleMsg += "}";

            Send(sendRoleMsg, localIPs[hit]);

            listener.message = "";
          }
          else if (commanderPos != -1)
          {
            player1.GetComponent<ControllerPlayer1>().available = false;
          }
          else if (officerPos != -1)
          {
            player2.GetComponent<ControllerPlayer2>().available = false;
          }
          else if (captainPos != -1)
          {
            player3.GetComponent<ControllerPlayer3>().available = false;
          }
          else
          {
            int rolePos = listener.message.IndexOf("{R(");
            if (rolePos != -1)
            {
              string roleData = listener.message.Substring(rolePos + 3);
              receivedRole = int.Parse(roleData.Substring(0, roleData.IndexOf(")"))); // 1 == Opps Commander (Player 1) | 2 == Weapons Officer (Player 2) | 3 == Captain (Player 3)
            }

            // parse gyroscope data
            int gyroPos = listener.message.IndexOf("{G(");
            if (gyroPos != -1)
            {
              // check if field has changed
              if (listener.message != prevMessageGyro)
              {
                // cut off everything except the floats
                string gyroData = listener.message.Substring(gyroPos + 3);
                gyroData = gyroData.Substring(0, gyroData.IndexOf("}") - 1);

                string[] quat = gyroData.Split(',');

                if (receivedRole == 1)
                {
                  ControllerPlayer1 player = player1.GetComponent<ControllerPlayer1>();
                  player.rotation.x = -float.Parse(quat[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.rotation.y = -float.Parse(quat[1], CultureInfo.InvariantCulture.NumberFormat);
                  player.rotation.z = -float.Parse(quat[2], CultureInfo.InvariantCulture.NumberFormat);
                }
                else if (receivedRole == 2)
                {
                  ControllerPlayer2 player = player2.GetComponent<ControllerPlayer2>();
                  player.rotation.x = -float.Parse(quat[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.rotation.y = -float.Parse(quat[1], CultureInfo.InvariantCulture.NumberFormat);
                  player.rotation.z = -float.Parse(quat[2], CultureInfo.InvariantCulture.NumberFormat);
                }
                else if (receivedRole == 3)
                {
                  ControllerPlayer3 player = player3.GetComponent<ControllerPlayer3>();
                  player.rotation.x = -float.Parse(quat[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.rotation.y = -float.Parse(quat[1], CultureInfo.InvariantCulture.NumberFormat);
                  player.rotation.z = -float.Parse(quat[2], CultureInfo.InvariantCulture.NumberFormat);
                }
              }

              prevMessageGyro = listener.message;
            }

            // parse accelerometer data
            int accelerometerPos = listener.message.IndexOf("{A(");
            if (accelerometerPos != -1)
            {
              // check if field has changed
              if (listener.message != prevMessageAccelerometer)
              {
                // cut off everything except the floats
                string accelerometerData = listener.message.Substring(accelerometerPos + 3);
                accelerometerData = accelerometerData.Substring(0, accelerometerData.IndexOf("}") - 1);

                string[] vec = accelerometerData.Split(',');

                if (receivedRole == 1)
                {
                  ControllerPlayer1 player = player1.GetComponent<ControllerPlayer1>();
                  player.acceleration.x = float.Parse(vec[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.acceleration.y = float.Parse(vec[1], CultureInfo.InvariantCulture.NumberFormat);
                  player.acceleration.z = float.Parse(vec[2], CultureInfo.InvariantCulture.NumberFormat);
                }
                else if (receivedRole == 2)
                {
                  ControllerPlayer2 player = player2.GetComponent<ControllerPlayer2>();
                  player.acceleration.x = float.Parse(vec[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.acceleration.y = float.Parse(vec[1], CultureInfo.InvariantCulture.NumberFormat);
                  player.acceleration.z = float.Parse(vec[2], CultureInfo.InvariantCulture.NumberFormat);
                }
                else if (receivedRole == 3)
                {
                  ControllerPlayer3 player = player3.GetComponent<ControllerPlayer3>();
                  player.acceleration.x = float.Parse(vec[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.acceleration.y = float.Parse(vec[1], CultureInfo.InvariantCulture.NumberFormat);
                  player.acceleration.z = float.Parse(vec[2], CultureInfo.InvariantCulture.NumberFormat);
                }
              }

              prevMessageAccelerometer = listener.message;
            }

            // pares joystick data
            int joystickPos = listener.message.IndexOf("{J(");
            if (joystickPos != -1)
            {
              // check if field has changed
              if (listener.message != prevJoystick)
              {
                string joystickData = listener.message.Substring(joystickPos + 3);
                joystickData = joystickData.Substring(0, joystickData.IndexOf("}") - 1);

                string[] vec = joystickData.Split(',');
                if (receivedRole == 1)
                {
                  ControllerPlayer1 player = player1.GetComponent<ControllerPlayer1>();
                  player.joystick.x = float.Parse(vec[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.joystick.y = float.Parse(vec[1], CultureInfo.InvariantCulture.NumberFormat);
                }
                else if (receivedRole == 2)
                {
                  ControllerPlayer2 player = player2.GetComponent<ControllerPlayer2>();
                  player.joystick.x = float.Parse(vec[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.joystick.y = float.Parse(vec[1], CultureInfo.InvariantCulture.NumberFormat);
                }
                else if (receivedRole == 3)
                {
                  ControllerPlayer3 player = player3.GetComponent<ControllerPlayer3>();
                  player.joystick.x = float.Parse(vec[0], CultureInfo.InvariantCulture.NumberFormat);
                  player.joystick.y = float.Parse(vec[1], CultureInfo.InvariantCulture.NumberFormat);
                }
              }

              prevJoystick = listener.message;
            }

            // parse action button pressed
            int buttonPos = listener.message.IndexOf("{B(1)}");
            if (buttonPos != -1)
            {
              if (receivedRole == 1)
              {
                //Debug.Log("pressed reloaded button");
                ControllerPlayer1 player = player1.GetComponent<ControllerPlayer1>();
                player.actionPressed = true;
              }
              else if (receivedRole == 2)
              {
                //Debug.Log("pressed fired button");
                ControllerPlayer2 player = player2.GetComponent<ControllerPlayer2>();
                player.actionPressed = true;
              }
              else if (receivedRole == 3)
              {
                //Debug.Log("pressed fired button");
                ControllerPlayer3 player = player3.GetComponent<ControllerPlayer3>();
                player.actionPressed = true;
              }
            }
            else
            {
              if (receivedRole == 1)
              {
                ControllerPlayer1 player = player1.GetComponent<ControllerPlayer1>();
                player.actionPressed = false;
              }
              else if (receivedRole == 2)
              {
                ControllerPlayer2 player = player2.GetComponent<ControllerPlayer2>();
                player.actionPressed = false;
              }
              else if (receivedRole == 3)
              {
                ControllerPlayer3 player = player3.GetComponent<ControllerPlayer3>();
                player.actionPressed = false;
              }
            }
          }
        }
      }
    }
    else
    {
      player1.GetComponent<ControllerPlayer1>().actionPressed = false;
      player2.GetComponent<ControllerPlayer2>().actionPressed = false;
      player3.GetComponent<ControllerPlayer3>().actionPressed = false;
    }

    listener.message = "";
  }

  public void Send(string message, string ip)
  {
    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
    UdpClient client = new UdpClient();

    if (printMessageOut)
      Debug.Log("Sending to: " + ip + "| message: " + message);

    try
    {
      byte[] data = Encoding.UTF8.GetBytes(message);
      client.Send(data, data.Length, remoteEndPoint);
    }
    catch (Exception e)
    {
      print(e.ToString());
    }
  }
}
