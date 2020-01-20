using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
  [SerializeField]
  public GameObject next;
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

    for (int i = 0; i < 4; ++i)
    {
      Vector3 forwardOfWall = Vector3.zero;

      string name = "__UNDEF__";
      switch (i)
      {
        case 0:
          name = "Bottom";
          break;
        case 1:
          name = "Top";

          break;
        case 2:
          name = "Left";
          break;
        case 3:
          name = "Right";
          break;
      }

      // configure forward and any other vector for the prefabs
      transform.GetChild(i).gameObject.AddComponent<VectorContainer>();

      var endPos = transform.GetChild(i).transform.GetChild(2).position - (transform.GetChild(i).transform.GetChild(2).position - transform.GetChild(i).transform.GetChild(3).position) / 2f;
      var startPos = transform.GetChild(i).transform.GetChild(0).position - (transform.GetChild(i).transform.GetChild(0).position - transform.GetChild(i).transform.GetChild(1).position) / 2f;

      transform.GetChild(i).gameObject.GetComponent<VectorContainer>().startPosition = startPos;
      transform.GetChild(i).gameObject.GetComponent<VectorContainer>().endPosition = endPos;
      transform.GetChild(i).gameObject.GetComponent<VectorContainer>().forward = endPos - startPos;

      var straight = transform.GetChild(i).transform.GetChild(0).position - transform.GetChild(i).transform.GetChild(2).position;
      var right = transform.GetChild(i).transform.GetChild(3).position - transform.GetChild(i).transform.GetChild(2).position;

      if (i == 1 || i == 3)
        transform.GetChild(i).gameObject.GetComponent<VectorContainer>().orthogonal = Vector3.Cross(straight, right).normalized;
      else
        transform.GetChild(i).gameObject.GetComponent<VectorContainer>().orthogonal = -Vector3.Cross(straight, right).normalized;

      // start generating bridge meshes
      newVertices = new Vector3[]
      {
        transform.GetChild(i).transform.GetChild(2).position, // secondLeft
        transform.GetChild(i).transform.GetChild(3).position, // secondRight
        next.transform.GetChild(i).transform.GetChild(0).position, // firstLeft
        next.transform.GetChild(i).transform.GetChild(1).position // firstRight
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

      // flip normals and uvs if mesh is top or right
      if (i == 1 || i == 3)
      {
        int[] reversedTriangles = newTriangles;

        int numpolies = reversedTriangles.Length / 3;
        for (int t = 0; t < numpolies; t++)
        {
          int tribuffer = reversedTriangles[t * 3];
          reversedTriangles[t * 3] = reversedTriangles[(t * 3) + 2];
          reversedTriangles[(t * 3) + 2] = tribuffer;
        }

        Vector2[] flippedUV = newUV;
        for (int uvnum = 0; uvnum < flippedUV.Length; uvnum++)
        {
          flippedUV[uvnum] = new Vector2(1 - flippedUV[uvnum].x, flippedUV[uvnum].y);
        }

        Vector3[] flippedNormals = newMesh.normals;
        for (int normalsnum = 0; normalsnum < flippedNormals.Length; normalsnum++)
        {
          flippedNormals[normalsnum] = -flippedNormals[normalsnum];
        }

        newMesh.vertices = newVertices;
        newMesh.uv = flippedUV;
        newMesh.triangles = reversedTriangles;
        newMesh.normals = flippedNormals;
      }
      else
      {
        newMesh.vertices = newVertices;
        newMesh.uv = newUV;
        newMesh.triangles = newTriangles;
      }

      GameObject bridge = new GameObject(name);
      bridge.AddComponent<MeshFilter>();

      if (renderBridge)
        bridge.AddComponent<MeshRenderer>();

      bridge.GetComponent<MeshFilter>().mesh = newMesh;

      bridge.AddComponent<MeshCollider>();
      bridge.GetComponent<MeshCollider>().sharedMesh = newMesh;

      bridge.AddComponent<VectorContainer>();

      startPos = transform.GetChild(i).transform.GetChild(2).position - (transform.GetChild(i).transform.GetChild(2).position - transform.GetChild(i).transform.GetChild(3).position) / 2f;
      endPos = next.transform.GetChild(i).transform.GetChild(0).position - (next.transform.GetChild(i).transform.GetChild(0).position - next.transform.GetChild(i).transform.GetChild(1).position) / 2f;
      
      bridge.GetComponent<VectorContainer>().startPosition = startPos;
      bridge.GetComponent<VectorContainer>().endPosition = endPos;
      bridge.GetComponent<VectorContainer>().forward = endPos - startPos;

      straight = next.transform.GetChild(i).transform.GetChild(0).position - transform.GetChild(i).transform.GetChild(2).position;
      right = transform.GetChild(i).transform.GetChild(3).position - transform.GetChild(i).transform.GetChild(2).position;
      
      if (i == 1 || i == 3)
        bridge.GetComponent<VectorContainer>().orthogonal = -Vector3.Cross(straight, right).normalized;
      else
        bridge.GetComponent<VectorContainer>().orthogonal = Vector3.Cross(straight, right).normalized;

      bridge.tag = "Bridge";

      // add them to a separate node keep hierarchy viewer clean
      container.transform.parent = GameObject.Find("Bridges").transform;
      bridge.transform.parent = container.transform;
    }
  }
}
