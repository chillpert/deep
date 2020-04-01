using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnStart : MonoBehaviour
{
  [SerializeField]
  bool active = false;

  void Start()
  {
    transform.gameObject.SetActive(active);
  }
}
