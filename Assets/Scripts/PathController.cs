using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
  void Start()
  {
      gameObject.GetComponent<Animator>().speed = 0.5f;
  }

  void Update()
  {
		if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			Debug.Log("Story mode");
		}
  }
}
