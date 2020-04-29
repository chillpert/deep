using System.Collections.Generic;
using UnityEngine;

public class FirmCollider : MonoBehaviour
{
  #region Transition Data
  public static float TimeOnTransitionEnter = 0f;
  public static Vector3 PositionOnTransitionEnter = Vector3.zero;
  public static Quaternion RotationOnTransitionEnter = Quaternion.identity;
  public static float JourneyLength = 0f;
  #endregion

  public Vector3 Forward { get; set; }

  [SerializeField]
  private float caveAnimationSpeed = 5f;
  
  private SubmarineController submarineController;
  private AudioController audioController;
  private GameObject submarine;

  private void Start()
  {
    Forward = new Vector3(0f, 0f, 1f);
    submarine = GameObject.Find("Submarine");
    audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
    submarineController = submarine.GetComponent<SubmarineController>();
  }

  private void Update()
  {
    transform.position = submarine.transform.position;
    transform.rotation = submarine.transform.rotation;
    transform.forward = submarine.transform.forward;

    if (Input.GetKeyDown(KeyCode.Alpha1))
    {
      submarineController.Level = 1;
      UpdateLevel();
    }
    else if (Input.GetKeyDown(KeyCode.Alpha2))
    {
      submarineController.Level = 2;
      UpdateLevel();
    }
    else if (Input.GetKeyDown(KeyCode.Alpha3))
    {
      submarineController.Level = 3;
      UpdateLevel();
    }
    else if (Input.GetKeyDown(KeyCode.Alpha4))
    {
      submarineController.Level = 4;
      UpdateLevel();
    }
    else if (Input.GetKeyDown(KeyCode.Alpha5))
    {
      submarineController.Level = 5;
      UpdateLevel();
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.name.Equals("EnterCave1Transition"))
      EnterTransition("WayPointStartCave1");

    if (collision.gameObject.name.Equals("EnterCave2Transition"))
      EnterTransition("WayPointStartCave2");

    if (collision.gameObject.name.Equals("EnterCave3Transition"))
      EnterTransition("WayPointStartCave3");

    if (collision.gameObject.CompareTag("EnterCave"))
      EnterCave(collision);

    if (collision.gameObject.CompareTag("EnterLevel"))
      EnterLevel(++submarineController.Level);

    if (collision.gameObject.CompareTag("TunnelMesh"))
    {
      StartCoroutine(submarineController.Cam.Shake());

      if (!submarineController.IFrames)
        submarineController.Health -= submarineController.DamageTunnelMesh;

      submarineController.UpdateDamageTexture();
    }

    if (collision.gameObject.CompareTag("LerpStopIn"))
    {
      submarineController.TurnCamStraight = false;
      Forward = collision.gameObject.transform.parent.transform.forward;
    }

    if (collision.gameObject.CompareTag("LerpStopOut"))
    {
      submarineController.TurnCamStraight = false;

      if (collision.gameObject.transform.parent.GetComponent<MeshGenerator>() != null)
        Forward = collision.gameObject.transform.parent.GetComponent<MeshGenerator>().next.gameObject.transform.forward;
    }

    if (collision.gameObject.CompareTag("Destructables"))
    {
      StartCoroutine(submarineController.Cam.Shake());

      if (!submarineController.IFrames)
        submarineController.Health -= submarineController.DamageDestructables;

      Destroy(collision.gameObject); // or play destruction animation or similar effects
    }

    if (collision.gameObject.CompareTag("Finish"))
      submarineController.CompletedGame = true;

    if (!LookAtObject.FoundObject && collision.gameObject.CompareTag("PausePathAnimation"))
    {
      var cfp = submarine.GetComponent<CustomFollowerPath>();

      if (cfp != null)
      {
        CustomFollowerPath.Stop = true;
        Debug.Log("FirmCollider: Pausing until players found object of interest");
      }
      else
        Debug.Log("MASAKA!");
    }

    // audio
    if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Bridge") || collision.gameObject.CompareTag("Destructables"))
      audioController.PlayDamageVoice();

    if (collision.gameObject.name.Equals("EnterCave1"))
      audioController.PlayEnterCave1();

    if (collision.gameObject.name.Equals("EnterCave2"))
      audioController.PlayEnterCave2();

    if (collision.gameObject.name.Equals("EnterCave3"))
      audioController.PlayEnterCave3();

    /*
    if (collision.gameObject.name.Equals("BouyCave1"))
      audioController.PlayBouyCave1();

    if (collision.gameObject.name.Equals("BouyCave2"))
      audioController.PlayBouyCave2();

    if (collision.gameObject.name.Equals("BouyCave3"))
      audioController.PlayBouyCave3();
    */

    if (collision.gameObject.name.Equals("EnterLevel1"))
      audioController.PlayEnterLevel1();

    if (collision.gameObject.name.Equals("EnterLevel2"))
      audioController.PlayEnterLevel2();

    if (collision.gameObject.name.Equals("EnterLevel3"))
      audioController.PlayEnterLevel3();

    if (collision.gameObject.name.Equals("EnterLevel4"))
      audioController.PlayEnterLevel4();

    Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
  }

  private void EnterTransition(string wayPointStart)
  {
    submarineController.TransitionGoal = GameObject.Find(wayPointStart);
    submarineController.Transition = true;

    TimeOnTransitionEnter = Time.time;
    PositionOnTransitionEnter = submarine.transform.position;
    JourneyLength = Vector3.Distance(PositionOnTransitionEnter, submarineController.TransitionGoal.transform.position);
  }

  private void EnterCave(Collision collision)
  {
    LookAtObject.FoundObject = false;
    submarineController.TorpedoAvailable = false;

    submarineController.InCave = true;

    UpdateLevel();

    if (submarine.GetComponent<CustomFollowerPath>() == null)
    {
      submarine.AddComponent<CustomFollowerPath>();
      var temp = submarine.GetComponent<CustomFollowerPath>();
      temp.EndOfPathInstruction = PathCreation.EndOfPathInstruction.Stop;
      temp.Speed = caveAnimationSpeed;
    }

    // set waypoints
    GameObject path = GameObject.Find("Path");
    // create path
    if (path.GetComponent<CustomPathCreator>() == null)
    {
      path.AddComponent<CustomPathCreator>();
      path.GetComponent<PathCreation.PathCreator>().bezierPath.IsClosed = false;
      submarine.GetComponent<CustomFollowerPath>().PathCreator = path.GetComponent<PathCreation.PathCreator>();
    }

    // set start object
    var customPath = path.GetComponent<CustomPathCreator>();
    customPath.start = submarineController.TransitionGoal.transform.position;
    //customPath.start = collision.contacts[0].point;
    submarineController.Transition = false;

    string name = "PathCave" + submarineController.Level.ToString();
    Debug.Log("FirmCollider: Using way points: " + name);
    GameObject newPath = GameObject.Find(name);

    List<Transform> newWayPoints = new List<Transform>(newPath.transform.childCount);

    for (int i = 0; i < newPath.transform.childCount; ++i)
      newWayPoints.Add(newPath.transform.GetChild(i));

    Debug.Log("waypoints: " + newWayPoints.ToString());

    customPath.waypoints = newWayPoints;
    customPath.updateWaypoints();

    // enable next level and cave geometry
    switch (submarineController.Level)
    {
      case 1:
        LevelGeometry.Level2.SetActive(true);
        break;

      case 2:
        LevelGeometry.Level3.SetActive(true);
        break;

      case 3:
        LevelGeometry.Level4.SetActive(true);
        break;
    }
  }

  private void EnterLevel(int level)
  {
    LookAtObject.FoundObject = false;

    string name = "Checkpoint" + level.ToString();
    Debug.Log("FirmCollider: Reached Checkpoint: " + name);
    submarineController.LastCheckpoint = GameObject.Find(name);

    switch (level)
    {
      case 2:
        // delete Level1
        GameObject.Destroy(LevelGeometry.Level1);

        LevelGeometry.Cave2.SetActive(true);
        submarineController.TorpedoAvailable = true;
        break;

      case 3:
        // delete Level1 if not already done
        if (LevelGeometry.Level1 != null)
          GameObject.Destroy(LevelGeometry.Level1);

        // delete Cave1
        GameObject.Destroy(LevelGeometry.Cave1);

        // delete Level2
        GameObject.Destroy(LevelGeometry.Level2);

        LevelGeometry.Cave3.SetActive(true);
        submarineController.TorpedoAvailable = true;
        break;

      case 4:
        // delete Level1 if not already done
        if (LevelGeometry.Level1 != null)
          GameObject.Destroy(LevelGeometry.Level1);

        // delete Cave1 if not already done
        if (LevelGeometry.Cave1 != null)
          GameObject.Destroy(LevelGeometry.Cave1);

        // delete Level2 if not already done
        if (LevelGeometry.Level2 != null)
          GameObject.Destroy(LevelGeometry.Level2);

        // delete Cave2
        GameObject.Destroy(LevelGeometry.Cave2);

        // delete Level3
        GameObject.Destroy(LevelGeometry.Level3);
        submarineController.TorpedoAvailable = true;
        break;
    }

    submarineController.InCave = false;

    // delete custom path creator
    GameObject path = GameObject.Find("Path");

    if (path.GetComponent<CustomPathCreator>() != null)
    {
      Destroy(path.GetComponent<CustomPathCreator>());
      Destroy(path.GetComponent<PathCreation.PathCreator>());
    }

    if (submarine.GetComponent<CustomFollowerPath>() != null)
      Destroy(submarine.GetComponent<CustomFollowerPath>());

    submarineController.Level = level;
    UpdateLevel();
  }

  public void UpdateLevel()
  {
    if (GameObject.Find("Players").GetComponent<PlayMode>().SinglePlayer)
      return;

    Package levelUpdate = new Package(PackageType.Level, null);

    if (submarineController.InCave)
      levelUpdate.data.Add(0);
    else
      levelUpdate.data.Add(submarineController.Level);

    submarineController.TcpHost.GetComponent<TCPHost>().Send(levelUpdate);
  }
}
