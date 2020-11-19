using System.Collections;
using UnityEngine;

public class GyroscopeController : MonoBehaviour
{
  [SerializeField]
  private GameObject submarine = null;

  private Quaternion rotation;
  private Quaternion offset;

  private void Start()
  {
    rotation = new Quaternion();
    offset = new Quaternion();
    offset = Quaternion.identity;
  }

  private void Calibrate()
  {
    Debug.Log("Gyroscope Controller: Calibrating phone");
    offset = Quaternion.identity * Quaternion.Inverse(rotation);
  }

  public void DisableLight()
  {
   // headlight.GetComponent<Light>().enabled = false;
  }

  public void UpdateGyroscope(GameObject headlight, Quaternion rot, ref bool calibrate)
  {
    rotation = GyroToUnity(rot);

    if (calibrate)
    {
      Calibrate();
      calibrate = false;
    }

    rotation = submarine.transform.rotation * offset * rotation;
    headlight.transform.rotation = rotation;
  }

  private static Quaternion GyroToUnity(Quaternion q)
  {
    return new Quaternion(q.x, q.y, -q.z, -q.w);
  }
}
