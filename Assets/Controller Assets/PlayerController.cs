using UnityEngine;

public enum RoleType
{
  OppsCommander,
  WeaponsOfficer,
  Captain
}

public enum Axis { X, Y, Z }

public class PlayerController : MonoBehaviour
{
  public string Id { get; set; }
  public bool Available { get; set; }
  [HideInInspector]
  public Vector3 Acceleration { get; set; }
  [HideInInspector]
  public Quaternion Rotation { get; set; }

  private bool capturePhoneStraight;
  [HideInInspector]
  public bool CapturePhoneStraight
  {
    get { return capturePhoneStraight; }
    set { capturePhoneStraight = value; }
  }

  private bool captureFlashlightStraight;
  [HideInInspector]
  public bool CaptureFlashlightStraight
  {
    get { return captureFlashlightStraight; }
    set { captureFlashlightStraight = value; }
  }

  [SerializeField]
  private RoleType role;

  public bool OnAction { get; set; }

  [SerializeField]
  private GameObject submarine;
  private GameObject rotationDummy;
  
  [SerializeField]
  private float clampAngle = 45f;
  [SerializeField]
  private float threshold = 0.1f;
  [SerializeField]
  private float speed = 25f;

  private static int rotationDummyCounter = 0;

  private void Start()
  {
    OnAction = false;
    rotationDummy = new GameObject("RotationDummy" + ++rotationDummyCounter);
  }

  private void Update()
  {
    if (Available)
      return;

    if (role == RoleType.OppsCommander)
    {
      if (InCave())
        return;

      RotateSubmarine(Axis.Y, Acceleration.x);
    }
    else if (role == RoleType.WeaponsOfficer)
    {
      if (InCave())
        return;

      RotateSubmarine(Axis.X, -Acceleration.z);
    }
    else if (role == RoleType.Captain)
    {
      transform.GetComponent<GyroscopeController>().UpdateGyroscope(Rotation, ref capturePhoneStraight, ref captureFlashlightStraight);
    }
  }

  private void RotateSubmarine(Axis axis, float acceleration)
  {
    Vector3 dir = Vector3.zero;

    float dirAxis = 0f;
    if (axis == Axis.Y)
    {
      dir.y = acceleration;
      dirAxis = dir.y;
    }
    else if (axis == Axis.X)
    {
      dir.x = acceleration;
      dirAxis = dir.x;
    }
    else
      Debug.LogWarning("PlayerController: Input on axis " + axis.ToString() + " not supported");

    if (0f > dirAxis + threshold || 0f < dirAxis - threshold)
    {
      if (dir.sqrMagnitude > 1)
        dir.Normalize();

      rotationDummy.transform.position = submarine.transform.position;
      rotationDummy.transform.rotation = submarine.transform.rotation;
      rotationDummy.transform.forward = submarine.transform.forward;

      rotationDummy.transform.Rotate(dir * speed * Time.deltaTime);

      if (Vector3.Angle(rotationDummy.transform.forward, CollisionsWithoutImpact.forward) < clampAngle)
        submarine.transform.Rotate(dir * speed * Time.deltaTime);
    }
  }

  private bool InCave()
  {
    if (submarine.GetComponent<SubmarineController>().inCave)
    {
      transform.GetComponent<GyroscopeController>().UpdateGyroscope(Rotation, ref capturePhoneStraight, ref captureFlashlightStraight);
      return true;
    }

    transform.GetComponent<GyroscopeController>().DisableLight();
    return false;
  }
}
