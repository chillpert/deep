using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnStart : MonoBehaviour
{
  void Start()
  {
    transform.gameObject.SetActive(false);
  }
}
