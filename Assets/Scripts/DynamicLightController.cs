using UnityEngine;

public class DynamicLightController : MonoBehaviour
{
  [SerializeField]
  GameObject submarine = null;

  void Start()
  {
    transform.position = submarine.transform.position;
  }

  void Update()
  {
    transform.position = submarine.transform.position;
  }
}
