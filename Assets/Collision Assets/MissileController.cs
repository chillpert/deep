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
        transform.Translate(0.0f, -10.0f * Time.deltaTime, 0.0f); // This speed should be coherent with the exhaust particle speed
    }
    
	void OnCollisionEnter(Collision collision)
	{
		//~ Debug.Log("Missile collide: " + collision.gameObject.tag);
		if(collision.gameObject.name.Contains("Bottom") || collision.gameObject.name.Contains("Top")
				|| collision.gameObject.name.Contains("Left") || collision.gameObject.name.Contains("Right"))
		{
			// TODO some smoke
			//~ Debug.Log("Deleted self");
			//~ Object.Destroy(this.gameObject);
		}
		if(collision.gameObject.tag == "Destructables")
		{
			// TODO explosion
			Object.Destroy(collision.gameObject);
			Object.Destroy(this.gameObject);
		}
	}
}
