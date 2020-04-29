using UnityEngine;

public class BlinkingEffect : MonoBehaviour
{
  [SerializeField]
  private float blinkingPeriod = 3f;

  private bool makeDarker = true;
  private bool makeBrighter = false;

  private float difference = 0f;
  private bool useFirstValue = true;

  private float startTime = 0f;
  private float speed = 1f;

  private void Update()
  {
    if (Time.time - difference > blinkingPeriod)
    {
      difference += blinkingPeriod;

      startTime = Time.time;

      if (useFirstValue)
      {
        makeDarker = true;
        makeBrighter = false;
        useFirstValue = false;
      }
      else
      {
        makeDarker = false;
        makeBrighter = true;
        useFirstValue = true;
      }
    }

    float distCovered = (Time.time - startTime) * speed;
    float fracJourney = distCovered / blinkingPeriod;

    if (makeDarker)
    {
      float res = Mathf.Lerp(1f, 0.5f, fracJourney);
      GetComponent<Light>().intensity = res;
    }
    else if (makeBrighter)
    {
      float res = Mathf.Lerp(0.5f, 1f, fracJourney);
      GetComponent<Light>().intensity = res;
    }
  }
}
