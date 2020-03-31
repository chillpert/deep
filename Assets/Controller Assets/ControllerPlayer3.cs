using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ControllerPlayer3 : MonoBehaviour, IPlayer
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
  [HideInInspector]
  public bool actionPressed;

  [SerializeField]
  GameObject submarine;

  [HideInInspector]
  public GameObject lampDynamic;

  bool firstRun = true;
  float timeOnConnect = 0f;
  [HideInInspector]
  public bool hasTimedOut = false;



  void Start()
  {
    Available = true;
    Acceleration = new Vector3();
    Rotation = new Quaternion();
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
          Debug.Log("Player 3 timeout");
          hasTimedOut = true;
          Available = true;
          firstRun = true;
          timeOnConnect = 0f;
        }
      }
    }

    if (!Available)
    {
      transform.GetComponent<GyroscopeController>().gyroController(Rotation, ref capturePhoneStraight, ref captureFlashlightStraight);
    }
  }
}
