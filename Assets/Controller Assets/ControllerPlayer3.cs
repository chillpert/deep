using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ControllerPlayer3 : MonoBehaviour, IPlayer
{
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

  [HideInInspector]
  public bool capturePhoneStraight = false;
  [HideInInspector]
  public bool captureFlashlightStraight = false;

  [HideInInspector]
  public GameObject lampDynamic;

  bool firstRun = true;
  float timeOnConnect = 0f;
  [HideInInspector]
  public bool hasTimedOut = false;

  public string Id { get; set; }

  void Start()
  {
    available = true;
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
          Debug.Log("Player 3 timeout");
          hasTimedOut = true;
          available = true;
          firstRun = true;
          timeOnConnect = 0f;
        }
      }
    }

    if (!available)
    {
      transform.GetComponent<GyroscopeController>().gyroController();
    }
  }
}
