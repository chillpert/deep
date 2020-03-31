using System.Globalization;
using UnityEngine;
using System;

public class UDPParser : UDPListener
{
  RoleType role;

  [SerializeField]
  GameObject player1;
  [SerializeField]
  GameObject player2;
  [SerializeField]
  GameObject player3;

  private PlayerController SetPlayer()
  {
    switch (role)
    {
      case RoleType.OppsCommander:
        return player1.GetComponent<PlayerController>();

      case RoleType.WeaponsOfficer:
        return player2.GetComponent<PlayerController>();

      case RoleType.Captain:
        return player3.GetComponent<PlayerController>();
    }

    Debug.LogError("UDPParser: Can not assign role");
    return null;
  }
  
  private void Update()
  {
    if (Message.Length <= 0)
      return;

    int foundRole = Message.IndexOf("{R(");
    if (foundRole != -1)
    {
      string uncut = Message.Substring(foundRole + 3);
      string cut = uncut.Substring(0, uncut.IndexOf(")"));

      role = (RoleType)Enum.Parse(typeof(RoleType), cut);
    }
    else
    {
      Debug.LogWarning("UDPParser: Missing role identfier in received sensor data");
      return;
    }

    int foundAccelerometer = Message.IndexOf("{A(");
    if (foundAccelerometer != -1)
    {
      string uncut = Message.Substring(foundAccelerometer + 3);
      string cut = uncut.Substring(0, uncut.IndexOf("}") - 1);
      string[] vec = cut.Split(',');

      SetPlayer().Acceleration = new Vector3(
        float.Parse(vec[0], CultureInfo.InvariantCulture.NumberFormat),
        float.Parse(vec[1], CultureInfo.InvariantCulture.NumberFormat),
        float.Parse(vec[2], CultureInfo.InvariantCulture.NumberFormat)
        );
    }

    int foundGyroscope = Message.IndexOf("{G(");
    if (foundGyroscope != -1)
    {
      string uncut = Message.Substring(foundGyroscope + 3);
      string cut = uncut.Substring(0, uncut.IndexOf("}") - 1);
      string[] quat = cut.Split(',');

      SetPlayer().Rotation = new Quaternion(
        float.Parse(quat[0], CultureInfo.InvariantCulture.NumberFormat),
        float.Parse(quat[1], CultureInfo.InvariantCulture.NumberFormat),
        float.Parse(quat[2], CultureInfo.InvariantCulture.NumberFormat),
        float.Parse(quat[3], CultureInfo.InvariantCulture.NumberFormat)
      );
    }
  }
}
