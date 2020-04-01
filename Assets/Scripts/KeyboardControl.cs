using UnityEngine;

public class KeyboardControl : MonoBehaviour
{
  void Start()
  {
        
  }

  void Update()
  {
		if (gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
		{
			if (Input.GetKey ("up"))
			{
				//~ transform.position += transform.forward * Time.deltaTime * speed;
				//~ x += Time.deltaTime * speed;
				transform.Translate(0.0f, 0.1f, 0.0f);
			}
			if (Input.GetKey ("down"))
			{
				//~ transform.position -= transform.forward * Time.deltaTime * speed;
				//~ x -= Time.deltaTime * speed;
				transform.Translate(0.0f, -0.1f, 0.0f);
			}
			//~ if (Input.GetKey ("left") && transform.position.x > -1.0f) {
			if (Input.GetKey ("left"))
			{
				transform.Translate(-0.1f, 0.0f, 0.0f);
				//~ rot_y -= speed * Time.deltaTime;
			}
			//~ if (Input.GetKey ("right") && transform.position.x < 1.0f) {
			if (Input.GetKey ("right"))
			{
				transform.Translate(0.1f, 0.0f, 0.0f);
				//~ rot_y += speed * Time.deltaTime;
			}
		}
  }
}
