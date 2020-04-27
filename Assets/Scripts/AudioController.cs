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

  private PlayableDirector introduction;
  private PlayableDirector victory;

  private PlayableDirector startLevel1;
  private PlayableDirector startLevel2;
  private PlayableDirector startLevel3;
  private PlayableDirector startLevel4;

  private PlayableDirector cave1Bouy;
  private PlayableDirector cave2Bouy;
  private PlayableDirector cave3Bouy;

  private PlayableDirector cave1Enter;
  private PlayableDirector cave2Enter;
  private PlayableDirector cave3Enter;

  private float timeOnHitWall = 0f;
  private float timeOnStoryAudio = 0f;
  private double muteDamageDuration = 0d;

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

    introduction = GameObject.Find("AudioIntroduction").GetComponent<PlayableDirector>();
    victory = GameObject.Find("AudioVictory").GetComponent<PlayableDirector>();
    Debug.Log("setting");

    startLevel1 = GameObject.Find("AudioStart_Level1").GetComponent<PlayableDirector>();
    startLevel2 = GameObject.Find("AudioStart_Level2").GetComponent<PlayableDirector>();
    startLevel3 = GameObject.Find("AudioStart_Level3").GetComponent<PlayableDirector>();
    startLevel4 = GameObject.Find("AudioStart_Level4").GetComponent<PlayableDirector>();

    cave1Bouy = GameObject.Find("AudioCave1_Bouy").GetComponent<PlayableDirector>();
    cave2Bouy = GameObject.Find("AudioCave2_Bouy").GetComponent<PlayableDirector>();
    cave3Bouy = GameObject.Find("AudioCave3_Bouy").GetComponent<PlayableDirector>();

    cave1Enter = GameObject.Find("AudioCave1_Enter").GetComponent<PlayableDirector>();
    cave2Enter = GameObject.Find("AudioCave2_Enter").GetComponent<PlayableDirector>();
    cave3Enter = GameObject.Find("AudioCave3_Enter").GetComponent<PlayableDirector>();

    if (muteAmbient)
    {
      var ambient = GameObject.Find("AudioAmbient").GetComponent<AudioSource>();
      ambient.playOnAwake = false;
      ambient.Stop();
    }
  }

  private void Update()
  {
    if (muteDamage)
    {
      if (Time.time - timeOnStoryAudio > muteDamageDuration)
        muteDamage = false;
    }
  }

  public void PlayDamageVoice()
  {
    if (muteDamage)
      return;

    if (damageCoolDown)
    {
      if (Time.time - timeOnHitWall > 10f)
        damageCoolDown = false;
    }
    else
    {
      timeOnHitWall = Time.time;
      damageCoolDown = true;

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
  }

  private void MuteDamageVoice(double duration)
  {
    muteDamageDuration = duration;
    timeOnStoryAudio = Time.time;
    muteDamage = true;
  }

  public void PlayBouyCave1()
  {
    if (muteStory)
      return;

    cave1Bouy.Play();
    MuteDamageVoice(cave1Bouy.duration);
  }

  public void PlayBouyCave2()
  {
    if (muteStory)
      return;

    cave2Bouy.Play();
    MuteDamageVoice(cave2Bouy.duration);
  }

  public void PlayBouyCave3()
  {
    if (muteStory)
      return;

    cave3Bouy.Play();
    MuteDamageVoice(cave3Bouy.duration);
  }

  public void PlayEnterCave1()
  {
    if (muteStory)
      return;

    cave1Enter.Play();
    MuteDamageVoice(cave1Enter.duration);
  }

  public void PlayEnterCave2()
  {
    if (muteStory)
      return;

    cave2Enter.Play();
    MuteDamageVoice(cave2Enter.duration);
  }

  public void PlayEnterCave3()
  {
    if (muteStory)
      return;

    cave3Enter.Play();
    MuteDamageVoice(cave3Enter.duration);
  }

  public void PlayEnterLevel1()
  {
    if (muteStory)
      return;

    startLevel1.Play();
    MuteDamageVoice(startLevel1.duration);
  }

  public void PlayEnterLevel2()
  {
    if (muteStory)
      return;

    startLevel2.Play();
    MuteDamageVoice(startLevel2.duration);
  }

  public void PlayEnterLevel3()
  {
    if (muteStory)
      return;

    startLevel3.Play();
    MuteDamageVoice(startLevel3.duration);
  }

  public void PlayEnterLevel4()
  {
    if (muteStory)
      return;

    startLevel4.Play();
    MuteDamageVoice(startLevel4.duration);
  }

  public void PlayIntroduction()
  {
    if (muteStory)
      return;

    introduction.Play();
    MuteDamageVoice(introduction.duration);
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
