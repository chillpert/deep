﻿using UnityEngine;

public class ControllerPlayer2 : MonoBehaviour
{
  [HideInInspector]
  public Quaternion rotation = new Quaternion();
  [HideInInspector]
  public Vector3 acceleration = new Vector2();

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

  [HideInInspector]
  public Vector2 joystick;
  [HideInInspector]
  public bool actionPressed;
  [HideInInspector]
  public bool available;

  float prevX;
  
  void Start()
  {
    available = true;
  }

  void Update()
  {
    Vector3 dir = Vector3.zero;

    dir.x = -acceleration.z;
    
    if (prevX > dir.y + threshold || prevX < dir.y - threshold)
    {
      if (dir.sqrMagnitude > 1)
        dir.Normalize();

      submarine.transform.Rotate(dir * verticalSpeed * Time.deltaTime);
    }

    Vector3 lightDir = Vector3.zero;
    lightDir.x = -joystick.y;
    lightDir.y = joystick.x;

    headLight.transform.Rotate(lightDir * headLightSpeed * Time.deltaTime);
  }
}
