using UnityEngine;

public class PlayerController2 : MonoBehaviour, IPlayerController
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

  [HideInInspector]
  public RoleType Role { get; set; }


  [HideInInspector]
  public float answer = 0f;
  [HideInInspector]
  public bool actionPressed;

  [SerializeField]
  GameObject submarine;
  [SerializeField]
  GameObject headLight;
  [SerializeField]
  float verticalSpeed;
  [SerializeField]
  float headLightSpeed;
  [SerializeField]
  float threshold = 0.1f;
  [SerializeField]
  float clampAngle = 80f;
  [SerializeField]
  GameObject player1;

  readonly float initialVal = 0f;

  GameObject rotationDummy;

  readonly float coolDown = 5f;

  [HideInInspector]
  public float timeSinceLastFire = 0f;
  bool actionPressedFirst = true;
  bool actionPressedFirst2 = true;

  [HideInInspector]
  public bool firedTorpedo = false;

  float timerTemp = 0f;
  bool firstRun = true;
  float timeOnConnect = 0f;
  [HideInInspector]
  public bool hasTimedOut = false;

  void Start()
  {
    Available = true;
    Acceleration = new Vector3();
    Rotation = new Quaternion();
    rotationDummy = new GameObject("VerticalRotationDummy");
  }

  void Update()
  {
    if (!Available)
    {
      if (firstRun)
      {
        timeOnConnect = Time.time;
        firstRun = false;
      }
    }

    // before even considering checking timeouts wait at least x seconds
    if (timeOnConnect != 0f)
    {
      if (Time.time - timeOnConnect > 10f)
      {
        if (Time.time - answer > 10f)
        {
          Debug.Log("Player 2 timeout");
          hasTimedOut = true;
          Available = true;
          firstRun = true;
          timeOnConnect = 0f;
        }
      }
    }

    if (!Available)
    {
      if (submarine.GetComponent<SubmarineController>().inCave)
      {
        transform.GetComponent<GyroscopeController>().UpdateGyroscope(Rotation, ref capturePhoneStraight, ref captureFlashlightStraight);
        return;
      }
      else
        transform.GetComponent<GyroscopeController>().DisableLight();

      Vector3 dir = Vector3.zero;

      dir.x = -Acceleration.z;

      if (initialVal > dir.x + threshold || initialVal < dir.x - threshold)
      {
        if (dir.sqrMagnitude > 1)
          dir.Normalize();

        rotationDummy.transform.position = submarine.transform.position;
        rotationDummy.transform.rotation = submarine.transform.rotation;
        rotationDummy.transform.forward = submarine.transform.forward;

        rotationDummy.transform.Rotate(dir * verticalSpeed * Time.deltaTime);

        if (Vector3.Angle(rotationDummy.transform.forward, CollisionsWithoutImpact.forward) < clampAngle)
          submarine.transform.Rotate(dir * verticalSpeed * Time.deltaTime);
      }

      if (actionPressed)
      {
        if (actionPressedFirst2)
        {
          timerTemp = Time.time;
          actionPressedFirst2 = false;
        }
      }
      else
      {
        if (Time.time - timeSinceLastFire > coolDown)
        {
          actionPressedFirst2 = true;
        }
      }

      if (actionPressed && player1.GetComponent<PlayerController1>().reloadedTorpedo)
      {
        if (actionPressedFirst)
        {
          timeSinceLastFire = Time.time;

          if (player1.GetComponent<PlayerController1>().timeSinceLastReload < timerTemp)
          {
            Debug.Log("Phone 2: Fired Torpedo @" + timeSinceLastFire);
            firedTorpedo = true;
          }
          actionPressedFirst = false;
        }
      }
      else
      {
        if (Time.time - timeSinceLastFire > coolDown)
        {
          actionPressedFirst = true;
        }

        firedTorpedo = false;
      }
    }
  }
}
