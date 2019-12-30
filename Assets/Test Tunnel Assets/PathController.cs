using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Animator>().speed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
		if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			Debug.Log("Story mode");
		}
    }
}
