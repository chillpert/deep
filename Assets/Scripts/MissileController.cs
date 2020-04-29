using UnityEngine;

public class MissileController : MonoBehaviour
{
	[SerializeField]
	public GameObject trailFX = null;
	[SerializeField]
	public GameObject explosionFX = null;
	[SerializeField]
	float controlSpeed = 1.5f;
	private AudioController audioController = null;
	private SubmarineController submarineController = null;

	public static int HitCounter = 0;
	
  void Start()
  {
		audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
		submarineController = GameObject.Find("Submarine").GetComponent<SubmarineController>();
		Destroy(transform.root.gameObject, 10);
	}

  void Update()
  {
		transform.Rotate(-Input.GetAxis("Controller Vertical") * Time.deltaTime * controlSpeed, Input.GetAxis("Controller Horizontal") * Time.deltaTime * controlSpeed, 0f, Space.World);
		transform.Translate(-10.0f * Time.deltaTime, 0f, 0f); // This speed should be coherent with the exhaust particle speed
  }
    
	void OnCollisionStay(Collision collision)
	{
		Debug.Log(collision.gameObject.name);

		if (collision.gameObject.CompareTag("Destructables"))
		{
			Object.Destroy(collision.gameObject);
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
		}

		if (collision.gameObject.CompareTag("Finish"))
		{
			++HitCounter;
			Debug.Log("MissileController: Hit, " + HitCounter);
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());

			if (HitCounter >= 3)
			{
				// won game
				submarineController.CompletedGame = true;
				audioController.PlayVictory();
			}
		}

		/*
		if(collision.gameObject.name.Contains("CavePart"))
			Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
		else
			explode();
		*/

		explode();
	}
	
	void explode()
	{
		audioController.PlayTorpedoImpact();
		explosionFX.SetActive(true);
		trailFX.SetActive(false);
		transform.localScale = new Vector3(0, 0, 0);
		Destroy(transform.root.gameObject, 2);
	}
}
