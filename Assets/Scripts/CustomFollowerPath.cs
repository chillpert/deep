using UnityEngine;
using PathCreation;

public class CustomFollowerPath : MonoBehaviour
{
  public PathCreator PathCreator { get; set; }
  public EndOfPathInstruction EndOfPathInstruction { get; set; }
  public float Speed { get; set; }

  public static bool Stop = false;

  private float distanceTravelled = 0f;

  private void Update()
  {
    if (PathCreator != null)
    {
      // only activate animation if you are actually in a cave
      if (GetComponent<SubmarineController>().InCave)
      {
        if (Stop) return;

        distanceTravelled += Speed * Time.deltaTime;
        transform.position = PathCreator.path.GetPointAtDistance(distanceTravelled, EndOfPathInstruction);
        transform.rotation = PathCreator.path.GetRotationAtDistance(distanceTravelled, EndOfPathInstruction);
      }
    }
  }
}
