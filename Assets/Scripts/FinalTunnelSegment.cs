using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalTunnelSegment : MonoBehaviour
{
  void Start()
  {
    transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;
    transform.GetChild(1).gameObject.GetComponent<Renderer>().enabled = false;
    transform.GetChild(2).gameObject.GetComponent<Renderer>().enabled = false;
    transform.GetChild(3).gameObject.GetComponent<Renderer>().enabled = false;
  }
}
