using System.Collections;
using UnityEngine;

public class GyroscopeController : MonoBehaviour
{
  [SerializeField]
  private GameObject submarine = null;
  [SerializeField]
  private GameObject headlight = null;

  private Quaternion rotation;
  private Quaternion phoneStraight;
  private Quaternion flashlightStraight;
  private Quaternion offset;
  private Quaternion currentInterpolationValue;
  private Quaternion currentInterpolationGoal;

  private float initialYAngle = 0f;
  private float appliedGyroYAngle = 0f;
  private float calibrationYAngle = 0f;

  [SerializeField]
  private float smoothing = 0.1f;
  private float tempSmoothing;

  [SerializeField]
  private bool useInterpolation = true;
  [SerializeField]
  private float capturingPeriod = 0.25f;

  private float timeStepFix = 0f;
  private bool takeFirstValue = true;

  IEnumerator Start()
  {
    Application.targetFrameRate = 60;
    initialYAngle = transform.eulerAngles.y;

    yield return new WaitForSeconds(1);
    StartCoroutine(CalibrateYAngle());
  }

  void CaptureDataHoldingStraight()
  {
    Debug.Log("Gyroscope Controller: Capture holding phone straight");
    phoneStraight = rotation;
  }

  void CaptureDataFlashlightStraight()
  {
    Debug.Log("Gyroscope Controller: Capture holding flashlight straight");
    flashlightStraight = rotation;

    offset.x = phoneStraight.x - flashlightStraight.x;
    offset.y = phoneStraight.y - flashlightStraight.y;
    offset.z = phoneStraight.z - flashlightStraight.z;
    offset.w = phoneStraight.w - flashlightStraight.w;
  }

  private IEnumerator CalibrateYAngle()
  {
    tempSmoothing = smoothing;
    smoothing = 1;
    calibrationYAngle = appliedGyroYAngle - initialYAngle;
    yield return null;
    smoothing = tempSmoothing;
  }

  private void ApplyGyroRotation()
  {
    Quaternion currentValue = new Quaternion(
      rotation.x - offset.x,
      rotation.y - offset.y,
      rotation.z - offset.z,
      rotation.w - offset.w
    );

    if (useInterpolation)
    {
      float distCovered = Time.time * 1f;
      float fractionOfJourney = distCovered / capturingPeriod;

      // only use new value every x seconds
      if (Time.time > timeStepFix)
      {
        timeStepFix += capturingPeriod;

        if (takeFirstValue)
        {
          takeFirstValue = false;
          currentInterpolationValue = currentValue;
        }
        else
        {
          takeFirstValue = true;
          currentInterpolationGoal = currentValue;
        }
      }

      // interpolate
      headlight.transform.rotation = Quaternion.Slerp(currentInterpolationValue, currentInterpolationGoal, fractionOfJourney);
    }
    else
      headlight.transform.rotation = currentValue;

    // this makes sure that the headlight is rotating with the submarine, but its still very buggy
    headlight.transform.Rotate(-submarine.transform.localRotation.eulerAngles);

    //lampDynamic.transform.rotation = Quaternion.Lerp(prevRotation, rotation, Time.time * 0.01f);

    headlight.transform.Rotate(0f, 0f, 180f, Space.Self);
    headlight.transform.Rotate(90f, 180f, 0f, Space.World);

    appliedGyroYAngle = headlight.transform.eulerAngles.y;
  }

  private void ApplyCalibration()
  {
    headlight.transform.Rotate(0f, -calibrationYAngle, 0f, Space.World);
  }

  public void DisableLight()
  {
    headlight.GetComponent<Light>().enabled = false;
  }

  public void UpdateGyroscope(Quaternion rotation, ref bool capturePhoneStraight, ref bool captureFlashlightStraight)
  {
    this.rotation = rotation;

    if (!headlight.GetComponent<Light>().enabled)
      headlight.GetComponent<Light>().enabled = true;

    if (capturePhoneStraight)
    {
      CaptureDataHoldingStraight();
      capturePhoneStraight = false;
    }

    if (captureFlashlightStraight)
    {
      CaptureDataFlashlightStraight();
      captureFlashlightStraight = false;
    }

    ApplyGyroRotation();
    ApplyCalibration();
  }

  public void SetEnabled(bool value)
  {
    enabled = true;
    StartCoroutine(CalibrateYAngle());
  }
}
