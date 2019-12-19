using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : MonoBehaviour
{
	//~ float speed = 3.0f;
	float x = 0.0f;
	float y = 0.0f;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKey ("up") && transform.position.y < 1.0f) {
			//~ transform.position += transform.forward * Time.deltaTime * speed;
			//~ x += Time.deltaTime * speed;
			transform.Translate(0.0f, 0.1f, 0.0f);
		}
		if (Input.GetKey ("down")  && transform.position.y > -1.0f) {
			//~ transform.position -= transform.forward * Time.deltaTime * speed;
			//~ x -= Time.deltaTime * speed;
			transform.Translate(0.0f, -0.1f, 0.0f);
		}
		//~ if (Input.GetKey ("left") && transform.position.x > -1.0f) {
		if (Input.GetKey ("left")) {
			transform.Translate(-0.1f, 0.0f, 0.0f);
			//~ rot_y -= speed * Time.deltaTime;
		}
		//~ if (Input.GetKey ("right") && transform.position.x < 1.0f) {
		if (Input.GetKey ("right")) {
			transform.Translate(0.1f, 0.0f, 0.0f);
			//~ rot_y += speed * Time.deltaTime;
		}
    }
}
