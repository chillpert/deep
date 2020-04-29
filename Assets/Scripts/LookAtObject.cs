using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
  [SerializeField]
  private float viewingDistance = 30f;

  private SubmarineController submarineController;
  private GameObject lastObjectHit;
  private AudioController audioController;

  public static bool FoundObject = false;

  private bool firstRun = false;
  private bool lookingAt = false;
  private float timeOnStartedLookingAt = 0f;

  [SerializeField]
  private float timeToLookAt = 1.5f;

  private void Start()
  {
    submarineController = GameObject.Find("Submarine").GetComponent<SubmarineController>();
    audioController = GameObject.Find("AudioController").GetComponent<AudioController>();

    //Debug.LogWarning("Ray casting is currently enabled in tunnels. This needs to be disabled later on");
  }

  private void Update()
  {
    // only apply ray casting when inside a cave
    if (!submarineController.InCave)
      return;

    // only use layer "LookAtInteractive" on position 10
    int layerMask = 1 << 10;

    RaycastHit hit;

    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, viewingDistance, layerMask))
    {
      //Debug.Log("LookAtObject: " + hit.collider.gameObject.name);

      if (firstRun)
      {
        firstRun = false;
        timeOnStartedLookingAt = Time.time;
        lookingAt = true;
      }

      if (lookingAt)
      {
        if (Time.time - timeOnStartedLookingAt > timeToLookAt)
        {
          Debug.Log("FirmCollider: Discovered " + hit.collider.gameObject.name);
          FoundObject = true;
          CustomFollowerPath.Stop = false;
          hit.collider.gameObject.layer = 0;

          switch (hit.collider.gameObject.name)
          {
            case "Buoy1":
              audioController.PlayBouyCave1();
              break;

            case "Buoy2":
              audioController.PlayBouyCave2();
              break;

            case "CrashedSubmarine":
              audioController.PlayBouyCave3();
              break;
          }
        }
      }

      lastObjectHit = hit.collider.gameObject;
      var outlineController = lastObjectHit.GetComponent<OutlineController>();
      
      if (outlineController == null)
        return;

      outlineController.ShowOutline();
    }
    else
    {
      lookingAt = false;
      firstRun = true;

      if (lastObjectHit != null)
      {
        var temp = lastObjectHit.GetComponent<OutlineController>();
        if (temp != null)
        {
          temp.HideOutline();
          lastObjectHit = null;
        }
      }
    }
  }
}
