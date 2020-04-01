using UnityEngine;
using UnityEngine.Playables;

public class AudioController : MonoBehaviour
{
  private SubmarineController submarineController;

  [SerializeField]
  private bool muteDamage = false;
  [SerializeField]
  private bool muteAmbient = false;
  [SerializeField]
  private bool muteStory = false;

  private bool damageCoolDown;

  private AudioSource damageA;
  private AudioSource damageB;
  private AudioSource damageC;
  private AudioSource damageD;

  private AudioSource torpedoLaunch;
  private AudioSource torpedoImpact;

  private PlayableDirector foundCave1;
  private PlayableDirector enterCave1;

  private float timeOnHitWall = 0f;

  private void Start()
  {
    submarineController = GameObject.Find("Submarine").GetComponent<SubmarineController>();
    damageCoolDown = false;

    damageA = GameObject.Find("AudioDamageA").GetComponent<AudioSource>();
    damageB = GameObject.Find("AudioDamageB").GetComponent<AudioSource>();
    damageC = GameObject.Find("AudioDamageC").GetComponent<AudioSource>();
    damageD = GameObject.Find("AudioDamageD").GetComponent<AudioSource>();

    torpedoLaunch = GameObject.Find("AudioTorpedoLaunch").GetComponent<AudioSource>();
    torpedoImpact = GameObject.Find("AudioTorpedoImpact").GetComponent<AudioSource>();

    foundCave1 = GameObject.Find("AudioFoundCave1").GetComponent<PlayableDirector>();
    enterCave1 = GameObject.Find("AudioEnterCave1").GetComponent<PlayableDirector>();

    if (muteAmbient)
    {
      var ambient = GameObject.Find("AudioAmbient").GetComponent<AudioSource>();
      ambient.playOnAwake = false;
      ambient.Stop();
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Bridge") || collision.gameObject.CompareTag("Destructables"))
    {
      if (damageCoolDown)
      {
        if (Time.time - timeOnHitWall > 10f)
          damageCoolDown = false;         
      }
      else
      {
        timeOnHitWall = Time.time;
        damageCoolDown = true;

        PlayDamageSound();
      }
    }

    if (collision.gameObject.name.Equals("FoundCave1"))
      PlayFoundCave1();

    if (collision.gameObject.name.Equals("EnterCave1"))
      PlayEnterCave1();
  }

  private void PlayDamageSound()
  {
    if (muteDamage)
      return;

    if (submarineController.Health > 66f)
    {
      damageA.Play();
    }
    else if (submarineController.Health <= 66f && submarineController.Health > 33f)
    {
      if (Random.Range(0.0f, 1.0f) < 0.5f)
        damageB.Play();
      else
        damageA.Play();
    }
    else
    {
      if (Random.Range(0.0f, 1.0f) < 0.5f)
        damageC.Play();
      else
        damageD.Play();
    }
  }

  private void PlayFoundCave1()
  {
    if (!muteStory)
      foundCave1.Play();
  }

  private void PlayEnterCave1()
  {
    if (!muteStory)
      enterCave1.Play();
  }

  public void PlayTorpedoLaunch()
  {
    torpedoLaunch.Play();
  }

  public void PlayTorpedoImpact()
  {
    torpedoImpact.Play();
  }
}
