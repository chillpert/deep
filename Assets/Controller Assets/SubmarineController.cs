using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmarineController : MonoBehaviour
{
  [SerializeField]
  GameObject player1;
  [SerializeField]
  GameObject player2;
  [SerializeField]
  GameObject player3;
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
  [SerializeField]
  GameObject submarineColliderHelper;
  [SerializeField]
  float voiceOnDamagePeriod = 5.0f;
  bool playvoiceOnDamage = false;

  [SerializeField]
  public GameObject enterCave1;
  [SerializeField]
  public GameObject enterLevel2;
  [SerializeField]
  public GameObject enterCave2;
  [SerializeField]
  public GameObject enterLevel3;

  [SerializeField]
  GameObject damageSphere;
  [SerializeField]
  Material damageMaterial0;
  [SerializeField]
  Material damageMaterial1;
  [SerializeField]
  Material damageMaterial2;
  [SerializeField]
  Material damageMaterial3;
  
  [SerializeField]
  AudioSource fireSound;
  [SerializeField]
  AudioSource damageSound1A;
  [SerializeField]
  AudioSource damageSound1B;
  [SerializeField]
  AudioSource damageSound2A;
  [SerializeField]
  AudioSource damageSound2B;
  [SerializeField]
  AudioSource damageSound3A;
  [SerializeField]
  AudioSource damageSound3B;

  [HideInInspector]
  public bool turnCamStraight = false;
  [HideInInspector]
  public bool isInvincible = false;
  float invincibilityTimeOffset = 0.0f;

  [HideInInspector]
  public Vector3 respawnPosition;
  [HideInInspector]
  public Quaternion respawnOrientation;

  Quaternion respawnRotation;
  Vector3 respawnForward;

  [HideInInspector]
  public float timeSinceLastTorepdo = 0f;

  public bool pushForward = true;
  public bool inCave = false;

  public Vector3 caveFinish;
  public Vector3 lookAtDestination;
  public Vector3 forwardOnCaveEnter;

  float lockPos = 0f;
  bool start = false;
  Vector3 directionToLerpTo;
  Vector3 directionOnCollision;

  [SerializeField]
  float invincibilityPeriod;
  [SerializeField]
  float bouncePeriod;

  float timeOnCollision;
  float prevTimeOnCollision;
  bool startInvincibilityFrames = false;
  bool startBouncing = false;

  Rigidbody rb;

  void Start()
  {
    currentHealth = maxHealth;

    lockPos = 0f;
    
    respawnPosition = transform.position;
    respawnForward = transform.forward;
    respawnRotation = transform.rotation;

    rb = GetComponent<Rigidbody>();
  }

  // sends message to both phones containing the number of the current level
  public void updateLevel()
  {
    string message = "{L(" + currentLevel + ")}";
    if (inCave)
      message = "{" + message + "{C(1)}}";

    //var ips = udpParser.GetComponent<UDPParser>().localIPs;

    /*
    if (ips.Count == 1)
    {
      udpParser.GetComponent<UDPParser>().Send(message, ips[0]);
    }
    else if (ips.Count == 2)
    {
      udpParser.GetComponent<UDPParser>().Send(message, ips[0]);
      udpParser.GetComponent<UDPParser>().Send(message, ips[1]);
    }
    else if (ips.Count == 3)
    {
      udpParser.GetComponent<UDPParser>().Send(message, ips[0]);
      udpParser.GetComponent<UDPParser>().Send(message, ips[1]);
      udpParser.GetComponent<UDPParser>().Send(message, ips[2]);
    }
    */
  }

  void OnCollisionStay(Collision collision)
  {    
    StartCoroutine(camera.GetComponent<CameraShake>().Shake());
    
    if (collision.gameObject.tag == "Finish")
    {
      resetSubmarine();
    }
    /*
    else if (collision.gameObject.tag == "EnterCave1")
    {
      forwardOnCaveEnter = transform.forward;
      lookAtDestination = enterLevel2.transform.forward;

      inCave = true;
      pushForward = false;
      caveFinish = enterLevel2.transform.position;
      Destroy(enterCave1);
      updateLevel();
      Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
    }
    else if (collision.gameObject.tag == "EnterLevel2")
    {
      inCave = false;
      pushForward = true;
      currentLevel = 2;
      Destroy(enterLevel2);
      updateLevel();
      Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
    }
    */
    else if (collision.gameObject.tag == "Bridge" || collision.gameObject.tag == "Wall")
    {
      timeOnCollision = Time.time;

      startInvincibilityFrames = true;
      startBouncing = true;

      if (!isInvincible)
        currentHealth -= damageTunnelWall;
      
      updateDamageTexture();

      if (timeOnCollision - prevTimeOnCollision > voiceOnDamagePeriod)
        playDamageSound();

      prevTimeOnCollision = timeOnCollision;

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
    inCave = false;
    startInvincibilityFrames = false;
    startBouncing = false;
    turnCamStraight = false;
    isInvincible = false;
    pushForward = true;

    //CollisionsWithoutImpact.forward = new Vector3(0f, 0f, 1f);

    // needs to be changed to respawn position
    transform.position = respawnPosition;
    transform.forward = respawnForward;
    transform.rotation = respawnRotation;

    submarineColliderHelper.transform.position = respawnPosition;
    submarineColliderHelper.transform.forward = respawnForward;
    submarineColliderHelper.transform.rotation = respawnRotation;

    //transform.rotation = Quaternion.Euler(lockPos, lockPos, lockPos);

    //Destroy(gameObject.GetComponent<Rigidbody>());
    Destroy(gameObject.GetComponent<BoxCollider>());

    //gameObject.AddComponent<Rigidbody>();
    gameObject.AddComponent<BoxCollider>();

    //Destroy(submarineColliderHelper.GetComponent<Rigidbody>());
    Destroy(submarineColliderHelper.GetComponent<BoxCollider>());

    //submarineColliderHelper.AddComponent<Rigidbody>();
    submarineColliderHelper.AddComponent<BoxCollider>();

    currentHealth = maxHealth;
    updateDamageTexture();

    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
  }

  void Update()
  {
    // if a player has timedout then pause game
    if (player1.GetComponent<ControllerPlayer1>().hasTimedOut)
       return;

    if (player2.GetComponent<ControllerPlayer2>().hasTimedOut)
      return;

    if (player3.GetComponent<ControllerPlayer3>().hasTimedOut)
      return;

    //Debug.DrawRay(transform.position, transform.forward * 100f);

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
    else if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      currentLevel = 3;
      updateLevel();
    }
    else if (Input.GetKeyDown(KeyCode.Alpha4))
    {
      currentLevel = 4;
      updateLevel();
    }
    else if (Input.GetKeyDown(KeyCode.Alpha5))
    {
      currentLevel = 5;
      updateLevel();
    }

    //Debug.Log("R: " + player1.GetComponent<ControllerPlayer1>().reloadedTorpedo + " | F: " + player2.GetComponent<ControllerPlayer2>().firedTorpedo);

    if (player1.GetComponent<ControllerPlayer1>().reloadedTorpedo && player2.GetComponent<ControllerPlayer2>().firedTorpedo)
    {
      Instantiate(missile, transform.position + transform.forward + new Vector3(0, -2, 0), transform.rotation);
      fireSound.Play();

      player1.GetComponent<ControllerPlayer1>().reloadedTorpedo = false;
      player2.GetComponent<ControllerPlayer2>().firedTorpedo = false;

      timeSinceLastTorepdo = Time.time;
    }

    if (Input.GetKeyDown(KeyCode.Space))// || udpParser.GetComponent<UDPParser>().localIPs.Count == 3)
    {
      start = !start;
    }

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

    if (playvoiceOnDamage)
    {
      playDamageSound();
      playvoiceOnDamage = false;
    }

    if (inCave)
    {
      //Debug.Log(Vector3.Lerp(forwardOnCaveEnter, lookAtDestination, Time.deltaTime * 2.0f) + " to " + lookAtDestination);
      //transform.LookAt(caveFinish);

      //transform.position = Vector3.MoveTowards(transform.position, caveFinish, constantVelocity * Time.deltaTime);
      //transform.forward = Vector3.Lerp(forwardOnCaveEnter, lookAtDestination, Time.deltaTime * 2.0f);

      //inCave = false;
      //transform.position = caveFinish;
      //transform.forward = lookAtDestination;
    }

    if (pushForward)
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
  
  public void updateDamageTexture()
  {
	  if(currentHealth > 75)
	  {
		  damageSphere.GetComponent<Renderer>().material = damageMaterial0;
	  }
	  else if(currentHealth <= 75 && currentHealth > 50)
	  {
		  damageSphere.GetComponent<Renderer>().material = damageMaterial1;
	  }
	  else if(currentHealth <= 50 && currentHealth > 25)
	  {
		  damageSphere.GetComponent<Renderer>().material = damageMaterial2;
	  }
	  else
	  {
		  damageSphere.GetComponent<Renderer>().material = damageMaterial3;
	  }
  }
  
  void playDamageSound()
  {
    if (currentHealth > 66)
    {
      if (Random.Range(0.0f, 1.0f) < 0.5f)
        damageSound1A.Play();
      else
        damageSound1B.Play();
    }
    else if (currentHealth <= 66 && currentHealth > 33)
    {
      if (Random.Range(0.0f, 1.0f) < 0.5f)
        damageSound2A.Play();
      else
        damageSound2B.Play();
    }
    else
    {
      if (Random.Range(0.0f, 1.0f) < 0.5f)
        damageSound3A.Play();
      else
        damageSound3B.Play();
    }
  }
}
