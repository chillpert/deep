using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
  GameObject submarine;
  [SerializeField]
  GameObject dynamicLamp;
  [HideInInspector]
  public static Vector3 forward = new Vector3(0f, 0f, 1f);

  Vector3 position;

  void enterCave(Collision collision, GameObject goal)
  {
    var script = submarine.GetComponent<SubmarineController>();

    script.forwardOnCaveEnter = transform.forward;
    script.lookAtDestination = goal.transform.forward;

    script.inCave = true;
    script.pushForward = false;
    script.caveFinish = goal.transform.position;
    submarine.GetComponent<SubmarineController>().updateLevel();
    Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
  }

  void enterLevel(Collision collision, int level)
  {
    var script = submarine.GetComponent<SubmarineController>();

    script.inCave = false;
    script.pushForward = true;
    SubmarineController.currentLevel = level;
    submarine.GetComponent<SubmarineController>().updateLevel();
    Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
  }

  void OnCollisionEnter(Collision collision)
  {
    //Debug.Log(collision.gameObject.name);

    if (collision.gameObject.tag == "EnterCave1")
    {
      enterCave(collision, submarine.GetComponent<SubmarineController>().enterLevel2);
    }
    else if (collision.gameObject.tag == "EnterLevel2")
    {
      enterLevel(collision, 2);
    }
    else if (collision.gameObject.tag == "EnterCave2")
    {
      enterCave(collision, submarine.GetComponent<SubmarineController>().enterLevel3);
    }
    else if (collision.gameObject.tag == "EnterLevel3")
    {
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
  }

  void Update()
  {
    Debug.DrawRay(position, forward * 25f, Color.yellow);

    transform.position = submarine.transform.position;
    transform.forward = submarine.transform.forward;
  }
}
