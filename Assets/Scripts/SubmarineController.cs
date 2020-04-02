using UnityEngine;
using UnityEngine.UI;

public class SubmarineController : MonoBehaviour
{
  public int Level { get; set; }
  public bool InCave { get; set; }
  public TCPHost TcpHost { get; set; }
  public CameraShake Cam { get; set; }
  public GameObject LastCheckpoint { get; set; }

  private AudioController audioController;
  private PlayerController player1;
  private PlayerController player2;

  [SerializeField]
  private GameObject missile = null;
  private GameObject firmCollider = null;

  [SerializeField]
  private Image blackScreen;
  private const float fadeInSpeed = 0.5f;
  private const float fadeOutSpeed = 0.5f;
  private float alpha = 1f;
  private bool died = false;
  private bool spawned = true;

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
  private Vector3 directionToLerpTo;
  private Vector3 directionOnCollision;

  private Rigidbody rb;
  #endregion

  private void Start()
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
    LastCheckpoint = GameObject.Find("Checkpoint1");
    blackScreen = GameObject.Find("BlackScreen").GetComponent<Image>();
      
    blackScreen.color = new Color(0f, 0f, 0f, alpha);
    blackScreen.enabled = true;

    Health = maxHealth;
    
    rb = GetComponent<Rigidbody>();
  }

  private void FadeIn()
  {
    if (blackScreen.color.a <= 1f)
    {
      alpha += Time.deltaTime * fadeInSpeed;
      Color color = blackScreen.color;
      blackScreen.color = new Color(color.r, color.g, color.b, alpha);
    }
    else
    {
      died = false;
      ResetSubmarine();
    }
  }

  private void FadeOut()
  {
    if (blackScreen.color.a >= 0f)
    {
      alpha -= Time.deltaTime * fadeOutSpeed;
      Color color = blackScreen.color;
      blackScreen.color = new Color(color.r, color.g, color.b, alpha);
    }
    else
      spawned = false;
  }

  private void OnDeath()
  {
    died = true;
  }

  private bool HandleFadeAnimation()
  {
    if (died)
    {
      FadeIn();
      return true;
    }

    if (spawned)
    {
      FadeOut();
      return true;
    }

    return false;
  }

  private void Update()
  {
    if (HandleFadeAnimation())
      return;

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

    if (invincible)
      Health = maxHealth;

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

    if (Input.GetKeyDown("r"))
    {
      Health = 0f;
    }

    // Fire the missile
    if (Input.GetKeyDown("f"))
    {
      Instantiate(missile, transform.position + transform.forward + new Vector3(0, -2, 0), transform.rotation);
      GetComponent<AudioSource>().Play();
    }

    // lock z-axis
    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
  }

  private void OnCollisionStay(Collision collision)
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
    Debug.Log("Submarine Controller: Reset");
    startInvincibilityFrames = false;
    startBouncing = false;
    TurnCamStraight = false;
    IFrames = false;
    spawned = true;

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
		  damageSphere.GetComponent<Renderer>().material = damageMaterial0;

	  else if (Health <= 75f && Health > 50f)
		  damageSphere.GetComponent<Renderer>().material = damageMaterial1;

	  else if (Health <= 50f && Health > 25f)
		  damageSphere.GetComponent<Renderer>().material = damageMaterial2;
	  
    else
		  damageSphere.GetComponent<Renderer>().material = damageMaterial3;
  }  
}
