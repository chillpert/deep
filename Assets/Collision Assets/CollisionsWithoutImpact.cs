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

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.tag == "EndOfTunnelSegment")
    {
      submarine.GetComponent<SubmarineController>().turnCamStraight = false;
      Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
      return;
    }
  }

  void Update()
  {
    transform.position = submarine.transform.position;
    transform.forward = submarine.transform.forward;
  }
}
