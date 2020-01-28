using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineController : MonoBehaviour
{
  [SerializeField]
  GameObject player1;
  [SerializeField]
  GameObject player2;
  [SerializeField]
  GameObject udpParser;
  [HideInInspector]
  public static int currentLevel = 1;
  [SerializeField]
  float speedControls;
  [SerializeField]
  float constantVelocity;
  [SerializeField]
  public GameObject camera;
  [SerializeField]
  float maxHealth;
  [SerializeField]
  public float currentHealth;
  [SerializeField]
  public float damageTunnelMesh;
  [SerializeField]
  public float damageTunnelWall;
  [SerializeField]
  public float damageDestuctables;
  [SerializeField]
  bool canDie;
  [SerializeField]
  float invincibilityTime;
  [SerializeField]
  GameObject missile;
  [SerializeField]
  float lerpDuration;

  [HideInInspector]
  public bool turnCamStraight = false;
  [HideInInspector]
  public bool isInvincible = false;
  float invincibilityTimeOffset = 0.0f;

  [HideInInspector]
  public Vector3 respawnPosition;
  [HideInInspector]
  public Quaternion respawnOrientation;

  float lockPos = 0f;
  bool start = false;
  Vector3 directionToLerpTo;
  Vector3 directionOnCollision;

  [SerializeField]
  float invincibilityPeriod;
  [SerializeField]
  float bouncePeriod;

  float timeOnCollision;
  bool startInvincibilityFrames = false;
  bool startBouncing = false;

  Rigidbody rb;

  void Start()
  {
    currentHealth = maxHealth;

    lockPos = 0f;
    respawnPosition = transform.position;
    rb = GetComponent<Rigidbody>();
  }

  // sends message to both phones containing the number of the current level
  void updateLevel()
  {
    var ips = udpParser.GetComponent<UDPParser>().localIPs;

    if (ips.Count == 1)
    {
      Debug.Log(ips[0]);
      udpParser.GetComponent<UDPParser>().Send("{L(" + currentLevel + ")}", ips[0]);
    }
    else if (ips.Count == 2)
    {
      udpParser.GetComponent<UDPParser>().Send("{L(" + currentLevel + ")}", ips[0]);
      udpParser.GetComponent<UDPParser>().Send("{L(" + currentLevel + ")}", ips[1]);
    }
  }

  void OnCollisionStay(Collision collision)
  {
    StartCoroutine(camera.GetComponent<CameraShake>().Shake());
    
    if (collision.gameObject.tag == "Destructables")
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
      timeOnCollision = Time.time;

      startInvincibilityFrames = true;
      startBouncing = true;

      if (!isInvincible)
        currentHealth -= damageTunnelWall;

      //transform.Translate(collision.gameObject.GetComponent<VectorContainer>().orthogonal);
      rb.AddForce(collision.gameObject.GetComponent<VectorContainer>().orthogonal, ForceMode.Impulse);

      directionOnCollision = transform.forward;
      directionToLerpTo = collision.gameObject.GetComponent<VectorContainer>().forward;
      //transform.forward = collision.gameObject.GetComponent<VectorContainer>().forward;

      turnCamStraight = true;
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
    // Debug.DrawRay(transform.position, transform.forward * 100f);

    // demo code for swapping control schemes
    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      currentLevel = 1;
      updateLevel();
    }
    else if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      currentLevel = 2;
      updateLevel();
    }

    //Debug.Log("R: " + player1.GetComponent<ControllerPlayer1>().reloadedTorpedo + " | F: " + player2.GetComponent<ControllerPlayer2>().firedTorpedo);

    if (player1.GetComponent<ControllerPlayer1>().reloadedTorpedo && player2.GetComponent<ControllerPlayer2>().firedTorpedo)
    {
      Instantiate(missile, transform.position + transform.forward + new Vector3(0, -2, 0), transform.rotation);
      GetComponent<AudioSource>().Play();

      player1.GetComponent<ControllerPlayer1>().reloadedTorpedo = false;
      player2.GetComponent<ControllerPlayer2>().firedTorpedo = false;
    }

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

    // on wall collision make submarine invincible for a certain amount of time
    if (startInvincibilityFrames)
    {
      if (Time.time - timeOnCollision > invincibilityPeriod)
        startInvincibilityFrames = false;
    }

    // reset bounce of rb every period of time
    if (startBouncing)
    {
      if (Time.time - timeOnCollision > bouncePeriod)
      {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        startBouncing = false;
      }
    }
    
    if (turnCamStraight)
    {
      float fracComplete = (Time.time - timeOnCollision) / lerpDuration;

      transform.forward = Vector3.Lerp(directionOnCollision, directionToLerpTo, fracComplete);

      if (Vector3.Angle(transform.forward, directionToLerpTo) <= 5f)
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
		  GetComponent<AudioSource>().Play();
	  }
  }
}
