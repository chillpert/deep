﻿using UnityEngine;

public class ControllerPlayer2 : MonoBehaviour
{
  [HideInInspector]
  //public Vector3 rotation = new Vector3();
  public Quaternion rotation = new Quaternion();
  [HideInInspector]
  public Vector3 acceleration = new Vector2();
  [HideInInspector]
  public Vector2 joystick;
  [HideInInspector]
  public bool available;
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

  void Start()
  {
    available = true;
    rotationDummy = new GameObject("VerticalRotationDummy");
  }

  void Update()
  {
    //Debug.Log("Available: " + available);

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
