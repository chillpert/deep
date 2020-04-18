using UnityEngine;

public class LookAtObject : MonoBehaviour
{
  private void Update()
  {
    int layerMask = 1 << 10;

    RaycastHit hit;

    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
    {
      // hit
    }
  }
}
