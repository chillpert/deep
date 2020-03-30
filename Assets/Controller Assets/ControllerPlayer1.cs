using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ControllerPlayer1 : MonoBehaviour, IPlayer
{
  [HideInInspector]
  //public Vector3 rotation = new Vector3();
  public Quaternion rotation = new Quaternion();
  [HideInInspector]
  public Vector3 acceleration = new Vector2();
  [HideInInspector]
  public bool available;
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

  public string Id { get; set; }

  void Start()
  {
    available = true;
    rotationDummy = new GameObject("HorziontalRotationDummy");
  }

  void Update()
  {
    if (!available)
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
          available = true;
          firstRun = true;
          timeOnConnect = 0f;
        }
      }
    }

    if (!available)
    {
      if (submarine.GetComponent<SubmarineController>().inCave)
      {
        transform.GetComponent<GyroscopeController>().gyroController();
        return;
      }
      else
        transform.GetComponent<GyroscopeController>().DisableLight();

      Vector3 dir = Vector3.zero;

      dir.y = acceleration.x;

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

      /*
      Vector3 lightDir = Vector3.zero;
      lightDir.x = -joystick.y;
      lightDir.y = joystick.x;

      headLight.transform.Rotate(lightDir * headLightSpeed * Time.deltaTime);
      */

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
