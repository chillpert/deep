using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineController : MonoBehaviour
{
  [SerializeField]
  float speedControls;
  [SerializeField]
  float constantVelocity;
  [SerializeField]
  GameObject camera;
  [SerializeField]
  float maxHealth;
  [SerializeField]
  float currentHealth;
  [SerializeField]
  float damageTunnelMesh;
  [SerializeField]
  float damageTunnelWall;
  [SerializeField]
  float damageDestuctables;
  [SerializeField]
  bool canDie;
  [SerializeField]
  float lerpSpeed;
  [SerializeField]
  float invincibilityTime;
  [SerializeField]
  GameObject missile;

  [HideInInspector]
  public bool turnCamStraight = false;
  bool isInvincible = false;
  float invincibilityTimeOffset = 0.0f;

  [HideInInspector]
  public Vector3 respawnPosition;
  [HideInInspector]
  public Quaternion respawnOrientation;

  float lockPos = 0f;
  bool start = false;
  Vector3 directionToLerpTo;
  Vector3 directionOnCollision;

  Rigidbody rb;

  void Start()
  {
    currentHealth = maxHealth;

    lockPos = 0f;
    respawnPosition = transform.position;
    rb = GetComponent<Rigidbody>();
  }

  void OnCollisionStay(Collision collision)
  {
    StartCoroutine(camera.GetComponent<CameraShake>().Shake());
    
    if (collision.gameObject.tag == "TunnelMesh")
    {
      if (!isInvincible) currentHealth -= damageTunnelMesh;
      Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
    }
    else if (collision.gameObject.tag == "Destructables")
    {
      if (!isInvincible) currentHealth -= damageDestuctables;
      Destroy(collision.gameObject);
    }
    else if (collision.gameObject.tag == "Finish")
    {
      resetSubmarine();
    }
    else if (collision.gameObject.tag == "Bridge" || collision.gameObject.tag == "Wall")
    {
      if (!isInvincible)
        currentHealth -= damageTunnelWall;

      directionOnCollision = transform.forward;

      transform.Translate(collision.gameObject.GetComponent<VectorContainer>().orthogonal);
      //directionToLerpTo = collision.gameObject.GetComponent<VectorContainer>().forward;

      transform.forward = collision.gameObject.GetComponent<VectorContainer>().forward;
      //turnCamStraight = true;
    }

    isInvincible = true;
  }

  void resetSubmarine()
  {
    // needs to be changed to respawn position
    transform.position = respawnPosition;
    transform.rotation = Quaternion.Euler(lockPos, lockPos, lockPos);
    currentHealth = maxHealth;
  }

  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
      start = !start;

    if (!start)
      return;

    if (!canDie)
      currentHealth = maxHealth;

    if (Time.time > invincibilityTimeOffset)
    {
      invincibilityTimeOffset += invincibilityTime;
      isInvincible = false;
    }

    if (currentHealth <= 0f)
    {
      resetSubmarine();
      currentHealth = maxHealth;
    }

    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    if (turnCamStraight)
    {
      transform.forward = Vector3.Lerp(directionOnCollision, directionToLerpTo, lerpSpeed * Time.deltaTime);

      if (Vector3.Distance(transform.forward, directionToLerpTo) <= 0.1f)
        turnCamStraight = false;
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
    
    if (Input.GetKeyDown("f")) // Fire the missile
    {
		  Instantiate(missile, transform.position + transform.forward + new Vector3(0, -2, 0), transform.rotation);
	  }
  }
}
