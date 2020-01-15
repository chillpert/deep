using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* http://wiki.unity3d.com/index.php?title=Xbox360Controller */

public class ControllerPlayer3 : MonoBehaviour
{
  [SerializeField]
  GameObject submarine;

  [SerializeField]
  float missileSpeed;

  [SerializeField]
  float controlSpeed;

  bool buttonA;

  [HideInInspector]
  public bool fired;

  bool positionRefresh;

  void resetButtons()
  {
    buttonA = false;
  }

  void Start()
  {
    positionRefresh = false;
    fired = false;
    resetButtons();
    updatePosition();
  }

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.tag == "EndOfTunnelSegment")
    {
      Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
      return;
    }

    Debug.Log(collision.gameObject.name);

    Debug.Log("BOOOOOM");
    fired = false;
    transform.GetComponent<Renderer>().enabled = false;
    updatePosition();
  }

  void updatePosition()
  {
    transform.position = new Vector3(submarine.transform.position.x + 0.5f, -1f, submarine.transform.position.z);
    transform.forward = submarine.transform.forward;
    transform.Rotate(90f, 0f, 0f, Space.Self);
  }

  float updateSpeed(float x)
  {
    return x * x * 0.5f * Time.time;
  }

  void Update()
  {
    buttonA = Input.GetKeyDown("joystick button 0");

    if (buttonA)
    {
      Debug.Log("FIRREEE");

      fired = true;
      positionRefresh = true;
      transform.GetComponent<Renderer>().enabled = true;
    }

    if (fired)
    {
      if (positionRefresh)
      {
        positionRefresh = false;
        updatePosition();
      }

      transform.Translate(0f, updateSpeed(missileSpeed), 0f);      

      float x = Input.GetAxis("Horizontal");
      float y = Input.GetAxis("Vertical");

      transform.Translate(x * controlSpeed * Time.deltaTime, 0, -y * controlSpeed * Time.deltaTime);
    }
    else
    {
      transform.GetComponent<Renderer>().enabled = false;
    }

    resetButtons();
  }
}
