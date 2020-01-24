using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
	float startTime;
	
    void Start()
    {
		Destroy(transform.root.gameObject, 10);
		
		startTime = Time.timeSinceLevelLoad;
    }

    void Update()
    {
        transform.Translate(0.0f, -10.0f * Time.deltaTime, 0.0f); // This speed should be coherent with the exhaust particle speed
    }
    
	void OnCollisionEnter(Collision collision)
	{
		if((collision.gameObject.name.Contains("Bottom") || collision.gameObject.name.Contains("Top")
				|| collision.gameObject.name.Contains("Left") || collision.gameObject.name.Contains("Right"))
				&& (Time.timeSinceLevelLoad - startTime) > 5.0f)
		{
			// TODO some smoke
			Object.Destroy(this.gameObject);
		}
		if(collision.gameObject.tag == "Destructables")
		{
			// TODO explosion
			Object.Destroy(collision.gameObject);
			Object.Destroy(this.gameObject);
		}
	}
}
