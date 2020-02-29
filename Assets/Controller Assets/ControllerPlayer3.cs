using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayer3 : MonoBehaviour
{
  [HideInInspector]
  public Vector3 rotation = new Vector3();
  [HideInInspector]
  public Vector3 acceleration = new Vector2();
  [HideInInspector]
  public Vector2 joystick;
  [HideInInspector]
  public bool available;
  [HideInInspector]
  public bool actionPressed;

  [SerializeField]
  GameObject lampDynamic;
  [SerializeField]
  float headlightSpeed = 20f;

  float initialRotationX;
  float initialRotationY;
  float initialRotationZ;

  void Start()
  {
    available = true;

    initialRotationX = rotation.x;
    initialRotationY = rotation.y;
    initialRotationZ = rotation.z;

    Debug.Log(initialRotationX + ", " + initialRotationY + ", " + initialRotationZ);
  }

  void resetHeadlight()
  {
    // this should be depended on the forward vector of the current tunnel segement, but since it is a debug thing .... who cares
    lampDynamic.transform.forward = new Vector3(0f, 0f, 1f);
  }

  void Update()
  {
    lampDynamic.transform.Rotate(
      -(initialRotationX - rotation.x) * Time.deltaTime * headlightSpeed,
      -(initialRotationY - rotation.y) * Time.deltaTime * headlightSpeed,
      0f); // initialRotationZ - rotation.z * Time.deltaTime * headlightSpeed);

    if (Input.GetKeyDown("r"))
      resetHeadlight();
  }
}
