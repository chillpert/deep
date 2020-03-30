using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum PackageType
{
  Connected,
  Sensor,
  ServerFull,
  Disconnected,
  Calibrate1,
  Calibrate2
}

[Serializable]
public class Package
{
  public List<object> data;
  public string senderId;
  public PackageType packetType;

  public Package(PackageType packetType, string senderId)
  {
    this.packetType = packetType;
    this.senderId = senderId;

    data = new List<object>();
  }

  public Package(byte[] bytes)
  {
    var formatter = new BinaryFormatter();
    var mem = new MemoryStream(bytes);
    Package p = null;
    try
    {
      p = (Package)formatter.Deserialize(mem);
      data = p.data;
      packetType = p.packetType;
      senderId = p.senderId;
    }
    catch (Exception e)
    {

    }
    finally
    {
      mem.Close();
    }
  }

  public byte[] ToBytes()
  {
    var formatter = new BinaryFormatter();
    var mem = new MemoryStream();

    formatter.Serialize(mem, this);
    var bytes = mem.ToArray();
    mem.Close();

    return bytes;
  }

  public static string GetIpAdress()
  {
    IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());

    foreach (var ip in ips)
    {
      if (ip.AddressFamily == AddressFamily.InterNetwork)
        return ip.ToString();
    }

    return "127.0.0.1";
  }
}
