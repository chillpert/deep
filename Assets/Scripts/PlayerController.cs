using UnityEngine;

public enum RoleType
{
  OppsCommander,
  WeaponsOfficer,
  Captain
}

public enum Axis { X, Y, Z }

// they say this bool is the coolest one around!
public class CoolBool
{
  private bool cool;
  public bool Cool
  {
    get { return cool;  }
    set 
    {
      cool = value;
      TimeOnTrue = Time.time;
    } 
  }
  public float TimeOnTrue = 0f;
}

public class PlayerController : MonoBehaviour
{
  public string Id { get; set; }
  public bool Available { get; set; }
  public Vector3 Acceleration { get; set; }
  public Quaternion Rotation { get; set; }

  private bool capturePhoneStraight;
  public bool CapturePhoneStraight
  {
    get { return capturePhoneStraight; }
    set { capturePhoneStraight = value; }
  }

  private bool captureFlashlightStraight;
  public bool CaptureFlashlightStraight
  {
    get { return captureFlashlightStraight; }
    set { captureFlashlightStraight = value; }
  }

  [SerializeField]
  private RoleType role = RoleType.OppsCommander;

  public CoolBool OnAction { get; set; }

  private GameObject submarine;
  private GameObject rotationDummy;

  private SubmarineController submarineController;
  private GyroscopeController gyroscopeController;
  private FirmCollider firmCollider;

  [SerializeField]
  private float clampAngle = 45f;
  [SerializeField]
  private float threshold = 0.1f;
  [SerializeField]
  private float speed = 25f;

  private const string dummyNameH = "RotationDummyHorizontal";
  private const string dummyNameV = "RotationDummyVertical";

  private void Start()
  {
    OnAction = new CoolBool();

    submarine = GameObject.Find("Submarine");
    submarineController = submarine.GetComponent<SubmarineController>();
    gyroscopeController = transform.GetComponent<GyroscopeController>();
    firmCollider = GameObject.Find("FirmCollider").GetComponent<FirmCollider>();
  }

  private void UpdateRotationDummy(string name)
  {
    rotationDummy = GameObject.Find(name);

    if (rotationDummy == null)
      rotationDummy = new GameObject(name);
  }

  private void Update()
  {
    if (Available)
      return;

    if (role == RoleType.OppsCommander)
    {
      if (InCave())
        return;

      switch (submarineController.Level)
      {
        case 1:
          RotateSubmarine(Axis.Y, Acceleration.x);
          break;

        case 2:
          RotateSubmarine(Axis.X, -Acceleration.z);
          break;

        case 3:
          RotateHeadlight();
          break;

        case 4:
          RotateSubmarine(Axis.Y, Acceleration.x);
          break;

        case 5:
          RotateSubmarine(Axis.X, -Acceleration.z);
          break;
      }
    }
    else if (role == RoleType.WeaponsOfficer)
    {
      if (InCave())
        return;

      switch (submarineController.Level)
      {
        case 1:
          RotateSubmarine(Axis.X, -Acceleration.z);
          break;

        case 2:
          RotateHeadlight();
          break;

        case 3:
          RotateSubmarine(Axis.Y, Acceleration.x);
          break;

        case 4:
          RotateSubmarine(Axis.X, -Acceleration.z);
          break;

        case 5:
          RotateHeadlight();
          break;
      }
    }
    else if (role == RoleType.Captain)
    {
      switch (submarineController.Level)
      {
        case 1:
          RotateHeadlight();
          break;

        case 2:
          RotateSubmarine(Axis.Y, Acceleration.x);
          
          break;

        case 3:
          RotateSubmarine(Axis.X, -Acceleration.z);
          break;

        case 4:
          RotateHeadlight();
          break;

        case 5:
          RotateSubmarine(Axis.Y, Acceleration.x);
          break;
      }
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
      UpdateRotationDummy(dummyNameH);
    }
    else if (axis == Axis.X)
    {
      dir.x = acceleration;
      dirAxis = dir.x;
      UpdateRotationDummy(dummyNameV);
    }
    else
      Debug.LogWarning("PlayerController: Input on axis " + axis.ToString() + " not supported");

    if (-0.1f > dirAxis + threshold || 0.1f < dirAxis - threshold)
    {
      if (dir.sqrMagnitude > 1)
        dir.Normalize();
      
      rotationDummy.transform.position = submarine.transform.position;
      rotationDummy.transform.rotation = submarine.transform.rotation;
      rotationDummy.transform.forward = submarine.transform.forward;

      rotationDummy.transform.Rotate(dir * speed * Time.deltaTime);

      if (Vector3.Angle(firmCollider.Forward, rotationDummy.transform.forward) < clampAngle)
      {
        submarine.transform.Rotate(dir * speed * Time.deltaTime);
        //submarine.transform.rotation = Quaternion.Euler(dir * speed * Time.deltaTime);
      }
    }
  }

  private bool InCave()
  {
    if (submarineController.InCave)
    {
      gyroscopeController.UpdateGyroscope(Rotation, ref capturePhoneStraight, ref captureFlashlightStraight);
      return true;
    }

    gyroscopeController.DisableLight();
    return false;
  }

  private void RotateHeadlight()
  {
    gyroscopeController.UpdateGyroscope(Rotation, ref capturePhoneStraight, ref captureFlashlightStraight);
  }
}
