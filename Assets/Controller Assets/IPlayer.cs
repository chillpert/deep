using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public enum RoleType
{
  OppsCommander,
  WeaponsOfficer,
  Captain
}

public interface IPlayer
{
  string Id { get; set; }
  bool Available { get; set; }
  Vector3 Acceleration { get; set; }
  Quaternion Rotation { get; set; }

  bool CapturePhoneStraight { get; set; }
  bool CaptureFlashlightStraight { get; set; }
}
