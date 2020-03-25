﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayer3 : MonoBehaviour
{
  [HideInInspector]
  public Quaternion rotation = new Quaternion();
  Quaternion prevRotation = new Quaternion();
  [HideInInspector]
  public Vector3 acceleration = new Vector2();
  [HideInInspector]
  public Vector2 joystick;
  //[HideInInspector]
  public bool available;
  [HideInInspector]
  public bool actionPressed;
  [SerializeField]
  GameObject submarine;

  [HideInInspector]
  public bool capturePhoneStraight = false;
  [HideInInspector]
  public bool captureFlashlightStraight = false;

  [SerializeField]
  GameObject lampDynamic;
  [SerializeField]
  float headlightSpeed = 20f;

  float initialYAngle = 0f;
  float appliedGyroYAngle = 0f;
  float calibrationYAngle = 0f;
  float tempSmoothing;

  float phoneStraightRotX = 0f;
  float phoneStraightRotY = 0f;
  float phoneStraightRotZ = 0f;
  float phoneStraightRotW = 0f;

  float flashlightStraightRotX = 0f;
  float flashlightStraightRotY = 0f;
  float flashlightStraightRotZ = 0f;
  float flashlightStraightRotW = 0f;

  float actualOffsetX;
  float actualOffsetY;
  float actualOffsetZ;
  float actualOffsetW;

  [SerializeField] 
  private float smoothing = 0.1f;
  bool firstRun = true;

  IEnumerator Start()
  {
    available = true;
    
    //initialRotationX = rotation.x;
    //initialRotationY = rotation.y;
    //initialRotationZ = rotation.z;
    //
    //Debug.Log(initialRotationX + ", " + initialRotationY + ", " + initialRotationZ);

    Application.targetFrameRate = 60;
    initialYAngle = transform.eulerAngles.y;

    yield return new WaitForSeconds(1);
    StartCoroutine(CalibrateYAngle());

    
  }

  void captureDataHoldingStraight()
  {
    phoneStraightRotX = rotation.x;
    phoneStraightRotY = rotation.y;
    phoneStraightRotZ = rotation.z;
    phoneStraightRotW = rotation.w;

    capturePhoneStraight = false;
  }

  void captureDataFlashlightStraight()
  {
    flashlightStraightRotX = rotation.x;
    flashlightStraightRotY = rotation.y;
    flashlightStraightRotZ = rotation.z;
    flashlightStraightRotW = rotation.w;

    actualOffsetX = phoneStraightRotX - flashlightStraightRotX;
    actualOffsetY = phoneStraightRotY - flashlightStraightRotY;
    actualOffsetZ = phoneStraightRotZ - flashlightStraightRotZ;
    actualOffsetW = phoneStraightRotW - flashlightStraightRotW;

    captureFlashlightStraight = false;
  }

  void resetHeadlight()
  {
    // this should be depended on the forward vector of the current tunnel segement, but since it is a debug thing .... who cares
    lampDynamic.transform.forward = new Vector3(0f, 0f, 1f);
    //lampDynamic.transform.LookAt(new Vector3(0f, 0f, 1f));
  }

  private IEnumerator CalibrateYAngle()
  {
    tempSmoothing = smoothing;
    smoothing = 1;
    calibrationYAngle = appliedGyroYAngle - initialYAngle;
    yield return null;
    smoothing = tempSmoothing;
  }

  private void ApplyGyroRotation()
  {
    if (firstRun)
    {
      firstRun = false;
      prevRotation = rotation;
    }

    float x = rotation.x - actualOffsetX;
    float y = rotation.y - actualOffsetY;
    float z = rotation.z - actualOffsetZ;
    float w = rotation.w - actualOffsetW;

    lampDynamic.transform.rotation = new Quaternion(x, y, z, w);
    
    // important
    lampDynamic.transform.Rotate(-submarine.transform.localRotation.eulerAngles);

    //lampDynamic.transform.rotation = Quaternion.Lerp(prevRotation, rotation, Time.time * 0.01f);

    prevRotation = rotation;

    lampDynamic.transform.Rotate(0f, 0f, 180f, Space.Self);
    lampDynamic.transform.Rotate(90f, 180f, 0f, Space.World);

    appliedGyroYAngle = lampDynamic.transform.eulerAngles.y;
  }

  private void ApplyCalibration()
  {
    lampDynamic.transform.Rotate(0f, -calibrationYAngle, 0f, Space.World);
  }

  void Update()
  {
    if (!available)
    {
      if (Input.GetKeyDown("c") || capturePhoneStraight)
        captureDataHoldingStraight();

      if (Input.GetKeyDown("v") || captureFlashlightStraight)
        captureDataFlashlightStraight();

      ApplyGyroRotation();
      ApplyCalibration();
    }   
    
    //lampDynamic.transform.Rotate(
    //  -(initialRotationX - rotation.x) * Time.deltaTime * headlightSpeed,
    //  -(initialRotationY - rotation.y) * Time.deltaTime * headlightSpeed,
    //  0f); // initialRotationZ - rotation.z * Time.deltaTime * headlightSpeed);
    

    if (Input.GetKeyDown("r"))
      resetHeadlight();
  }

  public void SetEnabled(bool value)
  {
    enabled = true;
    StartCoroutine(CalibrateYAngle());
  }
}
