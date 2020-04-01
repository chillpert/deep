using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * This script is used to simply hold a set of vectors.
 * First is a custom forward vector used for the camera
 * interpolation. Second a vector orthogonal to any wall
 * respectively.
 */

public class VectorContainer : MonoBehaviour
{
  [HideInInspector]
  public Vector3 forward;
  [HideInInspector]
  public Vector3 orthogonal;
  [HideInInspector]
  public Vector3 startPosition;
  [HideInInspector]
  public Vector3 endPosition;
  [HideInInspector]
  public bool debugMode = false;

  void Update()
  {
    if (debugMode)
    {
      Debug.DrawRay(startPosition, forward);
      Debug.DrawRay(startPosition + (endPosition - startPosition) / 2, orthogonal);
    }
  }
}
