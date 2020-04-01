using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRendererOnStart : MonoBehaviour
{
  void Start()
  {
    transform.GetComponent<MeshRenderer>().enabled = false;
  }
}
