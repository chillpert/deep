using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
  [SerializeField]
  float duration = 1.25f;
  [SerializeField]
  float magnitude = 0.1f;

  public IEnumerator Shake()
  {
    Vector3 originalPos = transform.localPosition;

    float elapsed = 0f;

    while (elapsed < duration)
    {
      float x = Random.Range(-1f, 1f) * magnitude;
      float y = Random.Range(-1f, 1f) * magnitude;

      transform.localPosition = new Vector3(x, y, originalPos.z);

      elapsed += Time.deltaTime;

      yield return null;
    }

    transform.localPosition = originalPos;
  }
}
