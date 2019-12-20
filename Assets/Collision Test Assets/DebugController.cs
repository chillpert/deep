using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
  [SerializeField]
  float speedControls;
  [SerializeField]
  float constantVelocity;

  Vector3 initialPosition;
  float lockPos = 0f;

  void Start()
  {
    lockPos = 0f;
    initialPosition = transform.position;
  }

  void OnCollisionStay(Collision collision)
  {
    Debug.Log(collision.collider.name);
    transform.position = initialPosition;
    transform.rotation = Quaternion.Euler(lockPos, lockPos, lockPos);
  }

  void Update()
  {
    transform.Translate(0f, 0f, constantVelocity * Time.deltaTime);

    if (Input.GetKey("up") || Input.GetKey("w"))
    {
      transform.Rotate(-speedControls * Time.deltaTime, 0f, 0f);
    }

    if (Input.GetKey("down") || Input.GetKey("s"))
    {
      transform.Rotate(speedControls * Time.deltaTime, 0f, 0f);
    }

    if (Input.GetKey("left") || Input.GetKey("a"))
    {
      transform.Rotate(0f, -speedControls * Time.deltaTime, 0f);
    }

    if (Input.GetKey("right") || Input.GetKey("d"))
    {
      transform.Rotate(0f, speedControls * Time.deltaTime, 0f);
    }

    // lock z-axis
    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, lockPos);

    // reset once end of tunnel has been reached
    if (transform.position.z >= 60f)
    {
      transform.position = initialPosition;
    }
  }
}
