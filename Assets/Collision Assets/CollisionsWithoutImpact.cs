using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/*
 * This script works as a fix for the weird twitching issue 
 * that occurs when the submarine collider hits one of the
 * lerp stop planes in the tunnel prefabs. Therefore I used
 * a new object that is constantly being set to the submarine's
 * position. This way the camera keeps still, but collisions
 * are still detected.
 */

public class CollisionsWithoutImpact : MonoBehaviour
{
  [SerializeField]
  GameObject audioCave1Found;
  [SerializeField]
  GameObject audioCave1Enter;
  [SerializeField]
  GameObject audioCave2Found;
  [SerializeField]
  GameObject audioCave2Enter;

  [SerializeField]
  float caveSpeed = 5f;
  [SerializeField]
  GameObject submarine;
  [SerializeField]
  GameObject dynamicLamp;
  [HideInInspector]
  public static Vector3 forward = new Vector3(0f, 0f, 1f);

  Vector3 position;

  void enterCave(Collision collision, int followingLevel)
  {
    submarine.GetComponent<SubmarineController>().inCave = true;

    /*
    script.forwardOnCaveEnter = transform.forward;
    script.lookAtDestination = goal.transform.forward;

    script.pushForward = false;
    script.caveFinish = goal.transform.localPosition; //goal.transform.position;
    */

    submarine.GetComponent<SubmarineController>().updateLevel();

    if (submarine.GetComponent<CustomFollowerPath>() == null)
    {
      submarine.AddComponent<CustomFollowerPath>();
      var temp = submarine.GetComponent<CustomFollowerPath>();
      temp.endOfPathInstruction = PathCreation.EndOfPathInstruction.Stop;
      temp.speed = caveSpeed;
    }

    // set waypoints
    GameObject paths = GameObject.FindGameObjectWithTag("CavePath");
    // create path
    if (paths.GetComponent<CustomPathCreator>() == null)
    {
      paths.AddComponent<CustomPathCreator>();
      paths.GetComponent<PathCreation.PathCreator>().bezierPath.IsClosed = false;
      submarine.GetComponent<CustomFollowerPath>().pathCreator = paths.GetComponent<PathCreation.PathCreator>();
    }

    if (paths == null)
      Debug.Log("Could not find object with tag CavePath");

    // set start object
    var wayPointScript = paths.GetComponent<CustomPathCreator>();
    wayPointScript.start = collision.contacts[0].point;

    // get predefined path objects
    if (followingLevel == 2)
    {
      GameObject wayPointsCave1 = GameObject.Find("Cave1WayPoints");

      if (wayPointsCave1 == null)
        Debug.Log("Could not find object with name Cave1WayPoints");

      List<Transform> newWayPoints = new List<Transform>(wayPointsCave1.transform.childCount);
      for (int i = 0; i < wayPointsCave1.transform.childCount; ++i)
      {
        newWayPoints.Add(wayPointsCave1.transform.GetChild(i));
      }

      wayPointScript.waypoints = newWayPoints;
    }
    else if (followingLevel == 3)
    {
      GameObject wayPointsCave2 = GameObject.Find("Cave2WayPoints");

      if (wayPointsCave2 == null)
        Debug.Log("Could not find object with name Cave2WayPoints");

      List<Transform> newWayPoints = new List<Transform>(wayPointsCave2.transform.childCount);
      for (int i = 0; i < wayPointsCave2.transform.childCount; ++i)
      {
        newWayPoints.Add(wayPointsCave2.transform.GetChild(i));
      }

      wayPointScript.waypoints = newWayPoints;
    }

    wayPointScript.updateWaypoints();

    Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
  }

  void enterLevel(Collision collision, int level)
  {
    submarine.GetComponent<SubmarineController>().inCave = false;

    // delete custom path creator from paths game object
    GameObject paths = GameObject.FindGameObjectWithTag("CavePath");

    if (paths.GetComponent<CustomPathCreator>() != null)
    {
      Destroy(paths.GetComponent<CustomPathCreator>());
      Destroy(paths.GetComponent<PathCreation.PathCreator>());
    }

    if (submarine.GetComponent<CustomFollowerPath>() != null)
    {
      Destroy(submarine.GetComponent<CustomFollowerPath>());
    }


    /*
    script.pushForward = true;
    */
    SubmarineController.currentLevel = level;
    submarine.GetComponent<SubmarineController>().updateLevel();
    
    Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
  }

  void OnCollisionEnter(Collision collision)
  {
    //Debug.Log(collision.gameObject.name);

    if (collision.gameObject.tag == "EnterCave1")
    {
      Debug.Log("Entering Cave 1");
      enterCave(collision, 2);
    }
    else if (collision.gameObject.tag == "EnterLevel2")
    {
      Debug.Log("Entering Level 2");
      enterLevel(collision, 2);
    }
    else if (collision.gameObject.tag == "EnterCave2")
    {
      Debug.Log("Entering Cave 2");
      enterCave(collision, 3);
    }
    else if (collision.gameObject.tag == "EnterLevel3")
    {
      Debug.Log("Entering Level 3");
      enterLevel(collision, 3);
    }
    else if (collision.gameObject.tag == "TunnelMesh")
    {
      StartCoroutine(submarine.GetComponent<SubmarineController>().camera.GetComponent<CameraShake>().Shake());

      if (!submarine.GetComponent<SubmarineController>().isInvincible)
        submarine.GetComponent<SubmarineController>().currentHealth -= submarine.GetComponent<SubmarineController>().damageTunnelMesh;

      submarine.GetComponent<SubmarineController>().updateDamageTexture();

      Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
    }
    
    if (collision.gameObject.tag == "LerpStopIn")
    {
      //Debug.Log("LERP IN");
      forward = collision.gameObject.transform.parent.transform.forward;
      position = collision.gameObject.transform.position;

      submarine.GetComponent<SubmarineController>().turnCamStraight = false;
      Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
    }
    else if (collision.gameObject.tag == "LerpStopOut")
    {
      //Debug.Log("LERP OUT");
      if (collision.gameObject.transform.parent.GetComponent<MeshGenerator>() != null)
      {
        forward = collision.gameObject.transform.parent.GetComponent<MeshGenerator>().next.gameObject.transform.forward;
        position = collision.gameObject.transform.position;
      }

      submarine.GetComponent<SubmarineController>().turnCamStraight = false;
      Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
    }
    
    if (collision.gameObject.tag == "Destructables")
    {
      StartCoroutine(submarine.GetComponent<SubmarineController>().camera.GetComponent<CameraShake>().Shake());

      if (!submarine.GetComponent<SubmarineController>().isInvincible)
        submarine.GetComponent<SubmarineController>().currentHealth -= submarine.GetComponent<SubmarineController>().damageDestuctables;

      Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());

      Object.Destroy(collision.gameObject); // or play destruction animation or similar effects
    }

    if (collision.gameObject.tag == "AudioCave1Found")
    {
      audioCave1Found.GetComponent<PlayableDirector>().Play();
    }

    if (collision.gameObject.tag == "AudioCave1Enter")
    {
      audioCave1Enter.GetComponent<PlayableDirector>().Play();
    }

    if (collision.gameObject.tag == "AudioCave2Found")
    {
      audioCave2Found.GetComponent<PlayableDirector>().Play();
    }

    if (collision.gameObject.tag == "AudioCave2Enter")
    {
      audioCave2Enter.GetComponent<PlayableDirector>().Play();
    }
  }

  void Update()
  {
    Debug.DrawRay(position, forward * 25f, Color.yellow);

    transform.position = submarine.transform.position;
    transform.forward = submarine.transform.forward;
  }
}
