using UnityEngine;

public class SubmarineController : MonoBehaviour
{
  public int Level { get; set; }
  public bool InCave { get; set; }
  public TCPHost TcpHost { get; set; }
  public CameraShake Cam { get; set; }

  private AudioController audioController;
  private PlayerController player1;
  private PlayerController player2;

  [SerializeField]
  private GameObject missile = null;
  private GameObject firmCollider = null;

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
  #endregion

  #region Damage texture
  [SerializeField]
  GameObject damageSphere = null;
  [SerializeField]
  Material damageMaterial0 = null;
  [SerializeField]
  Material damageMaterial1 = null;
  [SerializeField]
  Material damageMaterial2 = null;
  [SerializeField]
  Material damageMaterial3 = null;
  #endregion

  #region Transform
  private Vector3 respawnPosition;
  private Vector3 respawnForward;
  private Quaternion respawnRotation;

  private Vector3 directionToLerpTo;
  private Vector3 directionOnCollision;

  private Rigidbody rb;
  #endregion

  void Start()
  {
    Level = 1;
    InCave = false;
    IFrames = false;
    TurnCamStraight = false;

    Cam = GameObject.Find("Camera").GetComponent<CameraShake>();
    TcpHost = GameObject.Find("TCPHost").GetComponent<TCPHost>();
    audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
    player1 = GameObject.Find("Player1").GetComponent<PlayerController>();
    player2 = GameObject.Find("Player2").GetComponent<PlayerController>();

    firmCollider = GameObject.Find("FirmCollider");

    Health = maxHealth;
    
    respawnPosition = transform.position;
    respawnForward = transform.forward;
    respawnRotation = transform.rotation;

    rb = GetComponent<Rigidbody>();
  }

  void OnCollisionStay(Collision collision)
  {    
    StartCoroutine(Cam.GetComponent<CameraShake>().Shake());
    
    if (collision.gameObject.CompareTag("Finish"))
      ResetSubmarine();

    if (collision.gameObject.CompareTag("Bridge") || collision.gameObject.CompareTag("Wall"))
    {
      timeOnCollision = Time.time;

      startInvincibilityFrames = true;
      startBouncing = true;

      if (!IFrames)
        Health -= damageWall;
      
      updateDamageTexture();

      rb.AddForce(collision.gameObject.GetComponent<VectorContainer>().orthogonal, ForceMode.Impulse);

      directionOnCollision = transform.forward;
      directionToLerpTo = collision.gameObject.GetComponent<VectorContainer>().forward;

      TurnCamStraight = true;
    }

    IFrames = true;
  }

  private void ResetSubmarine()
  {
    startInvincibilityFrames = false;
    startBouncing = false;
    TurnCamStraight = false;
    IFrames = false;

    // needs to be changed to respawn position
    transform.position = respawnPosition;
    transform.forward = respawnForward;
    transform.rotation = respawnRotation;

    Destroy(gameObject.GetComponent<SphereCollider>());
    gameObject.AddComponent<SphereCollider>();

    Destroy(firmCollider.GetComponent<SphereCollider>());
    firmCollider.AddComponent<SphereCollider>();

    Health = maxHealth;
    updateDamageTexture();

    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
  }

  void Update()
  {
    // fire torpedo
    if (player1.OnAction && player2.OnAction)
    {
      Instantiate(missile, transform.position + transform.forward + new Vector3(0, -2, 0), transform.rotation);
      audioController.PlayTorpedoLaunch();

      player1.OnAction = false;
      player2.OnAction = false;
    }

    if (Input.GetKeyDown(KeyCode.Space) || TcpHost.GetComponent<TCPHost>().ConnectedClients.Count == 3)
      start = !start;

    if (!start)
      return;

    if (!invincible)
      Health = maxHealth;

    if (Time.time > invincibilityTimeOffset)
    {
      invincibilityTimeOffset += invincibilityTime;
      IFrames = false;
    }

    if (Health <= 0f)
    {
      ResetSubmarine();
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

    transform.Translate(0f, 0f, constantVelocity * Time.deltaTime);

    // debug keyboard controller
    if (Input.GetKey("up") || Input.GetKey("w"))
      transform.Rotate(-speedControls * Time.deltaTime, 0f, 0f);

    if (Input.GetKey("down") || Input.GetKey("s"))
      transform.Rotate(speedControls * Time.deltaTime, 0f, 0f);

    if (Input.GetKey("left") || Input.GetKey("a"))
      transform.Rotate(0f, -speedControls * Time.deltaTime, 0f);

    if (Input.GetKey("right") || Input.GetKey("d"))
      transform.Rotate(0f, speedControls * Time.deltaTime, 0f);

    // Fire the missile
    if (Input.GetKeyDown("f"))
    {
      Instantiate(missile, transform.position + transform.forward + new Vector3(0, -2, 0), transform.rotation);
      GetComponent<AudioSource>().Play();
    }

    // lock z-axis
    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
  }
  
  public void updateDamageTexture()
  {
	  if (Health > 75f)
		  damageSphere.GetComponent<Renderer>().material = damageMaterial0;

	  else if (Health <= 75f && Health > 50f)
		  damageSphere.GetComponent<Renderer>().material = damageMaterial1;

	  else if (Health <= 50f && Health > 25f)
		  damageSphere.GetComponent<Renderer>().material = damageMaterial2;
	  
    else
		  damageSphere.GetComponent<Renderer>().material = damageMaterial3;
  }  
}
