using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLight : MonoBehaviour
{
  void Start()
  {
    transform.GetComponent<Light>().intensity = 0.01f;  
  }
}
