using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    void Start()
    {
		Destroy(transform.root.gameObject, 10);
    }

    void Update()
    {
        transform.Translate(0.0f, -2.0f, 0.0f); // This speed should be coherent with the exhaust particle speed
    }
    
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Missile collide: " + collision.gameObject.name);
		if(collision.gameObject.name.Contains("Tube"))
		{
			// TODO some smoke
			Debug.Log("Deleted self");
			Object.Destroy(this.gameObject);
		}
		if(collision.gameObject.name.Contains("Obstacle"))
		{
			// TODO explosion
			Object.Destroy(collision.gameObject);
		}
	}
}
