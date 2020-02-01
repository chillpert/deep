using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLightController : MonoBehaviour
{
  [SerializeField]
  GameObject submarine;

  void Start()
  {
    transform.position = submarine.transform.position;
    transform.forward = submarine.transform.forward;
  }

  void Update()
  {
    transform.position = submarine.transform.position;
  }
}
