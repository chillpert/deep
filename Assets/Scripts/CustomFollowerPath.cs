using UnityEngine;
using PathCreation;

public class CustomFollowerPath : MonoBehaviour
{
  public PathCreator PathCreator { get; set; }
  public EndOfPathInstruction EndOfPathInstruction { get; set; }

  private const float constSpeed = 5f;
  private float speed = 5f;

  public static bool Stop = false;

  private float distanceTravelled = 0f;

  private bool slower = true;
  private bool faster = false;

  private float period = 5f;
  public static float TimeOnStop = 0f;
  public static float TimeOnContinue = 0f;
  public static bool FullyStopped = false;

  private void Update()
  {
    if (PathCreator != null)
    {
      // only activate animation if you are actually in a cave
      if (GetComponent<SubmarineController>().InCave)
      {
        if (Stop)
        {
          if (FullyStopped)
          {
            return;
          }

          if (slower)
          {
            if (Time.time - TimeOnStop < period)
            {
              // decelerate
              float distCovered = (Time.time - TimeOnStop) * 1f;
              float fractionOfJourney = distCovered / period;

              speed = Mathf.Lerp(constSpeed, 0f, fractionOfJourney);
            }
            else
            {
              slower = false;
              faster = true;
              FullyStopped = true;
            }
          }
          else if (faster)
          {
            if (Time.time - TimeOnContinue < period)
            {
              Debug.Log("faster god damn it");
              // accelerate
              float distCovered = (Time.time - TimeOnContinue) * 1f;
              float fractionOfJourney = distCovered / period;

              speed = Mathf.Lerp(0f, constSpeed, fractionOfJourney);
            }
            else
            {
              faster = false;
              Stop = false;
            }
          }
        }
        
        if (!FullyStopped)
        {
          distanceTravelled += speed * Time.deltaTime;
          transform.position = PathCreator.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction);
          transform.rotation = PathCreator.path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction);
        }
      }
    }
  }
}
