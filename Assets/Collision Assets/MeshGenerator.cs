using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
  [SerializeField]
  GameObject next;
  [SerializeField]
  bool renderBridge;

  Vector3[] newVertices;
  int[] newTriangles;
  Vector2[] newUV;

  static int index = 0;

  void Start()
  {
    GameObject container = new GameObject("Bridge_" + index.ToString());
    ++index;

    // calculate forward direction of bridge
    Vector3 newForward = next.transform.GetChild(0).transform.GetChild(0).position - transform.GetChild(0).transform.GetChild(2).position;

    container.transform.forward = newForward;

    for (int i = 0; i < 4; ++i)
    {
      newVertices = new Vector3[]
      {
        transform.GetChild(i).transform.GetChild(2).position,
        transform.GetChild(i).transform.GetChild(3).position,
        next.transform.GetChild(i).transform.GetChild(0).position,
        next.transform.GetChild(i).transform.GetChild(1).position
      };

      newTriangles = new int[]
      {
        1, 2, 3,
        1, 0, 2,
      };

      newUV = new Vector2[]
      {
        new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)
      };

      Mesh newMesh = new Mesh();
      newMesh.vertices = newVertices;
      newMesh.uv = newUV;
      newMesh.triangles = newTriangles;

      string name = "__UNDEF__";
      switch (i)
      {
        case 0:
          name = "Bottom";
          break;
        case 1:
          name = "Left";
          break;
        case 2:
          name = "Top";
          break;
        case 3:
          name = "Right";
          break;
      }

      GameObject bridge = new GameObject(name);
      bridge.AddComponent<MeshFilter>();

      if (renderBridge)
        bridge.AddComponent<MeshRenderer>();

      bridge.GetComponent<MeshFilter>().mesh = newMesh;

      bridge.AddComponent<MeshCollider>();
      bridge.GetComponent<MeshCollider>().sharedMesh = newMesh;

      // add them to a separate node keep hierarchy viewer clean
      container.transform.parent = GameObject.Find("Bridges").transform;
      bridge.transform.parent = container.transform;
    }
  }
}
