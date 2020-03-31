using UnityEngine;

public enum RoleType
{
  OppsCommander,
  WeaponsOfficer,
  Captain
}

public interface IPlayerController
{
  string Id { get; set; }
  bool Available { get; set; }
  Vector3 Acceleration { get; set; }
  Quaternion Rotation { get; set; }

  bool CapturePhoneStraight { get; set; }
  bool CaptureFlashlightStraight { get; set; }

  RoleType Role { get; set; }
}
