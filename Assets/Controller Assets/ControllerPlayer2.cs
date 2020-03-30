using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ControllerPlayer2 : MonoBehaviour, IPlayer
{
  [HideInInspector]
  public Quaternion rotation = new Quaternion();
  [HideInInspector]
  public Vector3 acceleration = new Vector2();
  [HideInInspector]
  public bool available;
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

  public string Id { get; set; }

  void Start()
  {
    available = true;
    rotationDummy = new GameObject("VerticalRotationDummy");
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
          Debug.Log("Player 2 timeout");
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

      dir.x = -acceleration.z;

      //Debug.Log("Player 2: " + dir.x);

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

      /*
      Vector3 lightDir = Vector3.zero;
      lightDir.x = -joystick.y;
      lightDir.y = joystick.x;

      headLight.transform.Rotate(lightDir * headLightSpeed * Time.deltaTime);
      */

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

      if (actionPressed && player1.GetComponent<ControllerPlayer1>().reloadedTorpedo)
      {
        if (actionPressedFirst)
        {
          timeSinceLastFire = Time.time;

          if (player1.GetComponent<ControllerPlayer1>().timeSinceLastReload < timerTemp)
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
