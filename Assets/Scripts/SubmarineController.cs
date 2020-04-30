using UnityEngine;
using UnityEngine.UI;

public class SubmarineController : MonoBehaviour
{
  public bool SkipIntroduction { get; set; }
  public bool CompletedGame { get; set; }
  public int Level { get; set; }
  public bool InCave { get; set; }
  public TCPHost TcpHost { get; set; }
  public CameraShake Cam { get; set; }
  public GameObject LastCheckpoint { get; set; }

  private AudioController audioController;
  private PlayerController player1;
  private PlayerController player2;
  private PlayerController player3;
  private bool everyoneConnected = false;
  private PlayMode playMode = null;

  [SerializeField]
  private bool printCurrentLevel = false;
  [SerializeField]
  private Button pressSpaceToStart = null;

  #region Torpedo
  [SerializeField]
  private GameObject missile = null;
  private GameObject firmCollider = null;

  private CoolBool reload = new CoolBool();
  private CoolBool fire = new CoolBool();
  private bool printReload = true;
  public bool TorpedoAvailable { get; set; }

  float timeOnFire = 0f;
  bool fireCooldown = false;
  float fireCoolDownPeriod = 5f;
  #endregion

  #region Black Screen
  private Image blackScreen;
  private const float fadeInSpeed = 0.5f;
  private const float fadeOutSpeed = 0.5f;
  private float alpha = 1f;
  private bool died = false;
  private bool spawned = true;
  #endregion

  #region Health and Damage
  public float Health { get; set; }
  public bool IFrames { get; set; }

  [SerializeField]
  private bool invincible = false;

  [SerializeField]
  private int damageTunnelMesh;

  public int DamageTunnelMesh
  {
    get { return damageTunnelMesh; }
    set { damageTunnelMesh = value; }
  }

  [SerializeField]
  private float damageWall = 10f;
  [SerializeField]
  private int damageDestructables;

  public int DamageDestructables
  {
    get { return damageDestructables; }
    set { damageDestructables = value; }
  }

  private const float maxHealth = 100f;
  private float invincibilityTimeOffset = 0.0f;

  [SerializeField]
  private float invincibilityPeriod = 2f;
  #endregion

  #region Controls
  [SerializeField]
  private float speedControls = 20f;
  [SerializeField]
  private float constantVelocity = 5f;
  [SerializeField]
  private float invincibilityTime = 1.5f;
  [SerializeField]
  private float lerpDuration = 2f;
  [SerializeField]
  private float bouncePeriod = 0.5f;

  public bool TurnCamStraight { get; set; }
  private bool start = false;
  private float timeOnCollision;
  private bool startInvincibilityFrames = false;
  private bool startBouncing = false;
  private bool accelerate = true;

  private float timeOnStartDriving = 0f;
  [SerializeField]
  private float accelerationPeriod = 5f;
  #endregion

  #region Damage texture
  [SerializeField]
  private GameObject damageSphere = null;
  [SerializeField]
  private Material damageMaterial0 = null;
  [SerializeField]
  private Material damageMaterial1 = null;
  [SerializeField]
  private Material damageMaterial2 = null;
  [SerializeField]
  private Material damageMaterial3 = null;

  private bool playCrackSound = false;
  private bool health50first = true;
  private bool health25first = true;
  private bool health0first = true;
  #endregion

  #region Transform
  private Vector3 directionToLerpTo;
  private Vector3 directionOnCollision;

  private Rigidbody rb;
  #endregion

  public GameObject TransitionGoal { get; set; }
  public bool Transition { get; set; }

  private void Start()
  {
    // Level = 1;
    TorpedoAvailable = false;
    CompletedGame = false;
    InCave = false;
    IFrames = false;
    TurnCamStraight = false;

    Cam = GameObject.Find("Camera").GetComponent<CameraShake>();
    TcpHost = GameObject.Find("TCPHost").GetComponent<TCPHost>();
    audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
    player1 = GameObject.Find("Player1").GetComponent<PlayerController>();
    player2 = GameObject.Find("Player2").GetComponent<PlayerController>();
    player3 = GameObject.Find("Player3").GetComponent<PlayerController>();
    playMode = GameObject.Find("Players").GetComponent<PlayMode>();

    firmCollider = GameObject.Find("FirmCollider");
    LastCheckpoint = GameObject.Find("Checkpoint1");
    blackScreen = GameObject.Find("BlackScreen").GetComponent<Image>();

    blackScreen.color = new Color(0f, 0f, 0f, alpha);
    blackScreen.enabled = true;

    TransitionGoal = null;
    Transition = false;

    Health = maxHealth;
    
    rb = GetComponent<Rigidbody>();

    // disable the entire level geometry, it will be disabled depending on the selected spawn point in debug spawner script
    LevelGeometry.SetAll(false);
  }

  public void FadeIn(Color col)
  {
    if (blackScreen.color.a <= 1f)
    {
      alpha += Time.deltaTime * fadeInSpeed;

      blackScreen.color = new Color(col.r, col.g, col.b, alpha);
    }
    else
    {
      died = false;

      if (!CompletedGame)
        ResetSubmarine();
    }
  }

  public void FadeOut(Color col)
  {
    if (blackScreen.color.a >= 0f)
    {
      alpha -= Time.deltaTime * fadeOutSpeed;
 
      blackScreen.color = new Color(col.r, col.g, col.b, alpha);
    }
    else
      spawned = false;
  }

  private void OnDeath()
  {
    died = true;
    audioController.PlayCrack2();
    audioController.PlayCollisionLong();
    audioController.PlayCollisionShort();
  }

  private bool HandleFadeAnimation()
  {
    if (died)
    {
      FadeIn(Color.black);
      return true;
    }

    if (spawned)
    {
      accelerate = true;
      timeOnStartDriving = Time.time;

      FadeOut(Color.black);
      return true;
    }

    return false;
  }

  private void HandleTorpedo()
  {
    switch (Level)
    {
      case 2: case 5: case 8:
        reload = player3.OnAction;
        fire = player1.OnAction;
        break;

      case 3: case 6: case 9:
        reload = player2.OnAction;
        fire = player3.OnAction;
        break;

      case 4: case 7: case 10:
        reload = player1.OnAction;
        fire = player2.OnAction;
        break;
    }

    if (!TorpedoAvailable)
      return;

    if (fire.Cool)
    {
      Debug.Log("Submarine Controller: Fire registered");
    }

    if (reload.Cool)
    {
      if (printReload)
      {
        Debug.Log("Submarine Controller: Reload registered");
        printReload = false;
      }

      if (Time.time - reload.TimeOnTrue > 5f)
      {
        reload.Cool = false;
        printReload = true;
        Debug.Log("Submarine Controller: Reload time window closed");
      }
      else
      {
        if (fire.Cool)
        {
          Debug.Log("Submarine Controller: Fire Missile");
          fire.Cool = false;

          Instantiate(missile, transform.position + transform.forward + new Vector3(0, -2, 0), transform.rotation);
          audioController.PlayTorpedoLaunch();
        }
      }
    }
    else
    {
      fire.Cool = false;
    }
  }

  private void Update()
  {
    if (CustomFollowerPath.Stop)
      return;
    
    if (printCurrentLevel)
      Debug.Log("SubmarineController: Current level: " + Level.ToString());

    // fade to white
    if (CompletedGame)
    {
      FadeIn(Color.white);
      return;
    }

    if (HandleFadeAnimation())
      return;

    HandleTorpedo();

    if (TcpHost.GetComponent<TCPHost>().ConnectedClients.Count == 3)
    {
      everyoneConnected = true;
    }

    if (SkipIntroduction || Time.time > 27f)
    {
      if (!start && playMode.SinglePlayer)
        pressSpaceToStart.gameObject.SetActive(true);

      if (Input.GetKeyDown(KeyCode.Space) || everyoneConnected)
      {
        pressSpaceToStart.gameObject.SetActive(false);
        start = !start;
      }
    } 

    //if (!start)
      //return;

    if (InCave)
      invincible = true;
    else
      invincible = false;

    if (invincible)
      Health = maxHealth;

    if (Transition)
    {
      float distCovered = (Time.time - FirmCollider.TimeOnTransitionEnter) * constantVelocity;
      float fractionOfJourney = distCovered / FirmCollider.JourneyLength;
      transform.position = Vector3.Lerp(FirmCollider.PositionOnTransitionEnter, TransitionGoal.transform.position, fractionOfJourney);
      transform.rotation = Quaternion.Slerp(transform.rotation, TransitionGoal.transform.rotation, Time.deltaTime * 2.5f);
      return;
    }

    if (Time.time > invincibilityTimeOffset)
    {
      invincibilityTimeOffset += invincibilityTime;
      IFrames = false;
    }

    if (Health <= 0f)
    {
      OnDeath();
      Health = maxHealth;
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
    
    // turn camera straight when wall is hit
    if (TurnCamStraight)
    {
      float fracComplete = (Time.time - timeOnCollision) / lerpDuration;

      transform.forward = Vector3.Lerp(directionOnCollision, directionToLerpTo, fracComplete);

      if (Vector3.Angle(transform.forward, directionToLerpTo) <= 5f)
        TurnCamStraight = false;
    }

    if (start)
    {
      if (accelerate)
      {
        if (Time.time - timeOnStartDriving < accelerationPeriod)
        {
          float distCovered = (Time.time - timeOnStartDriving) * 1f;
          float fractionOfJourney = distCovered / accelerationPeriod;

          float res = Mathf.Lerp(0f, constantVelocity, fractionOfJourney);
          transform.Translate(0f, 0f, res * Time.deltaTime);

          //Debug.Log("Accerlerate");
        }
        else
        {
          accelerate = false;
        }
      }
      else
        transform.Translate(0f, 0f, constantVelocity * Time.deltaTime);
    }

    // debug keyboard controller
    if (Input.GetKey("up") || Input.GetKey("w"))
      transform.Rotate(-speedControls * Time.deltaTime, 0f, 0f);

    if (Input.GetKey("down") || Input.GetKey("s"))
      transform.Rotate(speedControls * Time.deltaTime, 0f, 0f);

    if (Input.GetKey("left") || Input.GetKey("a"))
      transform.Rotate(0f, -speedControls * Time.deltaTime, 0f);

    if (Input.GetKey("right") || Input.GetKey("d"))
      transform.Rotate(0f, speedControls * Time.deltaTime, 0f);

    if (Input.GetKeyDown("r"))
    {
      Health = 0f;
    }

    if (fireCooldown && Time.time - timeOnFire > fireCoolDownPeriod)
      fireCooldown = false;

    // Fire the missile
    if (!fireCooldown && TorpedoAvailable && Input.GetKeyDown("f"))
    {
      timeOnFire = Time.time;
      fireCooldown = true;

      Instantiate(missile, transform.position + transform.forward + new Vector3(0, -2, 0), transform.rotation);
      audioController.PlayTorpedoLaunch();
    }


    // lock z-axis
    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
  }

  private void OnCollisionStay(Collision collision)
  {
    StartCoroutine(Cam.GetComponent<CameraShake>().Shake());

    if (collision.gameObject.CompareTag("Bridge") || collision.gameObject.CompareTag("Wall"))
    {
      timeOnCollision = Time.time;

      startInvincibilityFrames = true;
      startBouncing = true;

      if (!IFrames)
        Health -= damageWall;

      UpdateDamageTexture();

      rb.AddForce(collision.gameObject.GetComponent<VectorContainer>().orthogonal, ForceMode.Impulse);

      directionOnCollision = transform.forward;
      directionToLerpTo = collision.gameObject.GetComponent<VectorContainer>().forward;

      TurnCamStraight = true;
    }

    IFrames = true;
  }

  private void ResetSubmarine()
  {
    Debug.Log("Submarine Controller: Reset to " + LastCheckpoint.name);
    startInvincibilityFrames = false;
    startBouncing = false;
    TurnCamStraight = false;
    IFrames = false;
    spawned = true;
    MissileController.HitCounter = 0;

    if (Level != 1)
      --Level;

    // needs to be changed to respawn position
    transform.position = LastCheckpoint.transform.position;
    transform.rotation = LastCheckpoint.transform.rotation;
    transform.forward = LastCheckpoint.transform.forward;

    Destroy(gameObject.GetComponent<SphereCollider>());
    gameObject.AddComponent<SphereCollider>();

    Destroy(firmCollider.GetComponent<SphereCollider>());
    firmCollider.AddComponent<SphereCollider>();

    Health = maxHealth;
    UpdateDamageTexture();

    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
  }

  public void UpdateDamageTexture()
  {
	  if (Health > 75f)
    {
		  damageSphere.GetComponent<Renderer>().material = damageMaterial0;
    }
	  else if (Health <= 75f && Health > 50f)
    {
		  damageSphere.GetComponent<Renderer>().material = damageMaterial1;

      if (health50first)
      {
        playCrackSound = true;
        health50first = false;
      }
    }
	  else if (Health <= 50f && Health > 25f)
    {
		  damageSphere.GetComponent<Renderer>().material = damageMaterial2;

      if (health25first)
      {
        playCrackSound = true;
        health25first = false;
      }
    }
    else
    {
		  damageSphere.GetComponent<Renderer>().material = damageMaterial3;

      if (health0first)
      {
        playCrackSound = true;
        health0first = false;
      }
    }

    if (playCrackSound)
    {
      audioController.PlayCrack1();
      playCrackSound = false;
    }
  }  
}
