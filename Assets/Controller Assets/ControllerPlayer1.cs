using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ControllerPlayer1 : MonoBehaviour, IPlayer
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
  public float answer = 0f;
  float prevAnswer = 0f;
  [HideInInspector]
  public bool actionPressed;

  [SerializeField]
  GameObject submarine;
  [SerializeField]
  GameObject headLight;
  [SerializeField]
  float horizontalSpeed;
  [SerializeField]
  float headLightSpeed;
  [SerializeField]
  float threshold = 0.1f;
  [SerializeField]
  float clampAngle;
  [SerializeField]
  GameObject player2;

  readonly float initialVal = 0f;
  [HideInInspector]
  public float timeSinceLastReload = 0f;
  float coolDown = 5f;

  GameObject rotationDummy;
  bool actionPressedFirst = true;

  [HideInInspector]
  public bool reloadedTorpedo = false;

  [SerializeField]
  GameObject lamp;
  bool firstRun = true;
  float timeOnConnect = 0f;
  [HideInInspector]
  public bool hasTimedOut = false;

  void Start()
  {
    Available = true;
    Acceleration = new Vector3();
    Rotation = new Quaternion();
    rotationDummy = new GameObject("HorziontalRotationDummy");
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
          Debug.Log("Player 1 timeout");
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
        transform.GetComponent<GyroscopeController>().gyroController(Rotation, ref capturePhoneStraight, ref captureFlashlightStraight);
        return;
      }
      else
        transform.GetComponent<GyroscopeController>().DisableLight();

      Vector3 dir = Vector3.zero;

      dir.y = Acceleration.x;

      if (initialVal > dir.y + threshold || initialVal < dir.y - threshold)
      {
        if (dir.sqrMagnitude > 1)
          dir.Normalize();

        rotationDummy.transform.position = submarine.transform.position;
        rotationDummy.transform.rotation = submarine.transform.rotation;
        //rotationDummy.transform.forward = submarine.transform.forward;

        rotationDummy.transform.Rotate(dir * horizontalSpeed * Time.deltaTime);

        if (Vector3.Angle(rotationDummy.transform.forward, CollisionsWithoutImpact.forward) < clampAngle)
          submarine.transform.Rotate(dir * horizontalSpeed * Time.deltaTime);
      }

      Debug.DrawRay(rotationDummy.transform.position, rotationDummy.transform.forward * 5f, Color.green);
      Debug.DrawRay(rotationDummy.transform.position, CollisionsWithoutImpact.forward * 5f, Color.red);

      if (actionPressed)
      {
        reloadedTorpedo = true;

        if (actionPressedFirst)
        {
          timeSinceLastReload = Time.time;
          actionPressedFirst = false;

          Debug.Log("Phone 1: Loaded Torpedo @" + timeSinceLastReload);
        }
      }
      else
      {
        if (Time.time - timeSinceLastReload > coolDown)
        {
          actionPressedFirst = true;
          reloadedTorpedo = false;
        }
      }
    }
  }
}
