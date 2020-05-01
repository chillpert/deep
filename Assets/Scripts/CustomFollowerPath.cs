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

  public static bool Slower = true;
  private bool faster = false;

  private float period = 5f;
  public static float TimeOnStop = 0f;
  public static float TimeOnContinue = 0f;
  public static bool FullyStopped = false;

  public static bool FoundDuringDeceleration = false;

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
            if (FoundDuringDeceleration)
            {
              TimeOnContinue = Time.time;
              faster = true;
              FullyStopped = false;
            }
            return;
          }
          
          if (Slower)
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
              Slower = false;
              faster = true;
              FullyStopped = true;

              Debug.Log("fully decelerated");
            }
          }
          else if (faster)
          {
            if (Time.time - TimeOnContinue < period)
            {
              // accelerate
              float distCovered = (Time.time - TimeOnContinue) * 1f;
              float fractionOfJourney = distCovered / period;

              speed = Mathf.Lerp(0f, constSpeed, fractionOfJourney);
            }
            else
            {
              faster = false;
              Stop = false;

              Debug.Log("fully accelerated");
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
