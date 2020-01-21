using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDebugMode : MonoBehaviour
{
  [SerializeField]
  bool debugMode = false;
  [SerializeField]
  bool renderTunnelMesh = true;
  [SerializeField]
  bool enableTunnelMeshCollider = true;

  void Awake()
  {
    for (int i = 0; i < transform.childCount; ++i)
    {
      if (transform.GetChild(i).GetComponent<MeshGenerator>() != null)
      {
        transform.GetChild(i).GetComponent<MeshGenerator>().renderBridge = debugMode;
        transform.GetChild(i).GetComponent<MeshGenerator>().renderTunnelMesh = renderTunnelMesh;
        transform.GetChild(i).GetComponent<MeshGenerator>().enableTunnelMeshCollider = enableTunnelMeshCollider;
      }
    }
  }

  void Start()
  {
    
  }
}
