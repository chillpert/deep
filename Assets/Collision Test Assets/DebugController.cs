using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
  [SerializeField]
  float speedControls;
  [SerializeField]
  float constantVelocity;
  [SerializeField]
  float bounceIntensity;
  [SerializeField]
  float turnSpeed;
  [SerializeField]
  GameObject camera;

  float bounceLeft = 0f;
  float bounceRight = 0f;
  float bounceTop = 0f;
  float bounceBottom = 0f;

  bool turnLeft = false;
  bool turnRight = false;
  bool turnDown = false;
  bool turnUp = false;

  Vector3 initialPosition;
  float lockPos = 0f;

  Rigidbody rb;

  void Start()
  {
    lockPos = 0f;
    initialPosition = transform.position;
    rb = GetComponent<Rigidbody>();
  }

  void OnCollisionStay(Collision collision)
  {
    StartCoroutine(camera.GetComponent<CameraShake>().Shake());

    if (collision.gameObject.tag == "TunnelMesh")
    {
      Debug.Log("ok");
      Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
      return;
    }

    if (collision.gameObject.tag == "TunnelTop")
    {
      rb.AddForce(-transform.up * bounceIntensity, ForceMode.Impulse);
      turnDown = true;
    }
    else if (collision.gameObject.tag == "TunnelBottom")
    {
      rb.AddForce(transform.up * bounceIntensity, ForceMode.Impulse);
      turnUp = true;
    }
    else if (collision.gameObject.tag == "TunnelLeft")
    {
      Debug.Log("left wall collision");
      rb.AddForce(transform.right * bounceIntensity, ForceMode.Impulse);
      turnRight = true;
    }
    else if (collision.gameObject.tag == "TunnelRight")
    {
      rb.AddForce(-transform.right * bounceIntensity, ForceMode.Impulse);
      turnLeft = true;
    }

    if (collision.gameObject.tag == "Destructables")
      Destroy(collision.gameObject);

    if (collision.gameObject.tag == "Finish")
    {
      transform.position = initialPosition;
      transform.rotation = Quaternion.Euler(lockPos, lockPos, lockPos);
    }
  }

  void Update()
  {
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    if (turnLeft)
    {
      if (transform.rotation.eulerAngles.y >= 0f && transform.rotation.eulerAngles.y <= 180f)
        transform.Rotate(0f, -turnSpeed * Time.deltaTime, 0f);
      else
        turnLeft = false;
    }

    if (turnRight)
    {
      if (transform.rotation.eulerAngles.y >= 270f && transform.rotation.eulerAngles.y <= 360f)
        transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f);
      else
        turnRight = false;
    }

    if (turnDown)
    {
      if (transform.rotation.eulerAngles.x >= 0f && transform.rotation.eulerAngles.x <= 180f)
        turnDown = false;
      else
        transform.Rotate(turnSpeed * Time.deltaTime, 0f, 0f);
    }

    if (turnUp)
    {
      if (transform.rotation.eulerAngles.x >= 270f && transform.rotation.eulerAngles.x <= 360f)
        turnUp = false;
      else
        transform.Rotate(-turnSpeed * Time.deltaTime, 0f, 0f);
    }

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
