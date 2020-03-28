using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[RequireComponent(typeof(PathCreator))]
public class CustomPathCreator : MonoBehaviour
{
  public bool closedLoop = false;
  public Vector3 start;

  public List<Transform> waypoints;

  public void updateWaypoints()
  {
    if (waypoints.Count > 0)
    {
      List<Transform> actualWaypoints = new List<Transform>();

      GameObject temp = new GameObject();
      temp.transform.position = start;

      actualWaypoints.Add(temp.transform);

      foreach (Transform transform in waypoints)
        actualWaypoints.Add(transform);

      // Create a new bezier path from the waypoints.
      BezierPath bezierPath = new BezierPath(actualWaypoints.ToArray(), closedLoop, PathSpace.xyz);
      bezierPath.GlobalNormalsAngle = 90f;
      GetComponent<PathCreator>().bezierPath = bezierPath;
    }
    else
      Debug.Log("Missing waypoints");
  }
}

