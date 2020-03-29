using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopeController : MonoBehaviour
{
  [HideInInspector]
  public Quaternion rotation = new Quaternion();
  [SerializeField]
  GameObject submarine;
  [HideInInspector]
  public bool capturePhoneStraight = false;
  [HideInInspector]
  public bool captureFlashlightStraight = false;

  [SerializeField]
  GameObject lampDynamic;

  float initialYAngle = 0f;
  float appliedGyroYAngle = 0f;
  float calibrationYAngle = 0f;
  float tempSmoothing;

  float phoneStraightRotX = 0f;
  float phoneStraightRotY = 0f;
  float phoneStraightRotZ = 0f;
  float phoneStraightRotW = 0f;

  float flashlightStraightRotX = 0f;
  float flashlightStraightRotY = 0f;
  float flashlightStraightRotZ = 0f;
  float flashlightStraightRotW = 0f;

  float actualOffsetX;
  float actualOffsetY;
  float actualOffsetZ;
  float actualOffsetW;

  [SerializeField]
  private float smoothing = 0.1f;
  [SerializeField]
  bool useInterpolation = true;
  [SerializeField]
  float capturingPeriod = 0.25f;
  float timeStepFix = 0f;
  Quaternion currentInterpolationValue;
  Quaternion currentInterpolationGoal;
  bool takeFirstValue = true;

  IEnumerator Start()
  {
    Application.targetFrameRate = 60;
    initialYAngle = transform.eulerAngles.y;

    yield return new WaitForSeconds(1);
    StartCoroutine(CalibrateYAngle());
  }

  void captureDataHoldingStraight()
  {
    Debug.Log("Capture holding phone straight");
    phoneStraightRotX = rotation.x;
    phoneStraightRotY = rotation.y;
    phoneStraightRotZ = rotation.z;
    phoneStraightRotW = rotation.w;

    capturePhoneStraight = false;
  }

  void captureDataFlashlightStraight()
  {
    Debug.Log("Capture holding flashlight straight");
    flashlightStraightRotX = rotation.x;
    flashlightStraightRotY = rotation.y;
    flashlightStraightRotZ = rotation.z;
    flashlightStraightRotW = rotation.w;

    actualOffsetX = phoneStraightRotX - flashlightStraightRotX;
    actualOffsetY = phoneStraightRotY - flashlightStraightRotY;
    actualOffsetZ = phoneStraightRotZ - flashlightStraightRotZ;
    actualOffsetW = phoneStraightRotW - flashlightStraightRotW;

    captureFlashlightStraight = false;
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
    float x = rotation.x - actualOffsetX;
    float y = rotation.y - actualOffsetY;
    float z = rotation.z - actualOffsetZ;
    float w = rotation.w - actualOffsetW;
    Quaternion currentValue = new Quaternion(x, y, z, w);

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
      lampDynamic.transform.rotation = Quaternion.Slerp(currentInterpolationValue, currentInterpolationGoal, fractionOfJourney);
    }
    else
      lampDynamic.transform.rotation = currentValue;

    // this makes sure that the headlight is rotating with the submarine, but its still very buggy
    lampDynamic.transform.Rotate(-submarine.transform.localRotation.eulerAngles);

    //lampDynamic.transform.rotation = Quaternion.Lerp(prevRotation, rotation, Time.time * 0.01f);

    lampDynamic.transform.Rotate(0f, 0f, 180f, Space.Self);
    lampDynamic.transform.Rotate(90f, 180f, 0f, Space.World);

    appliedGyroYAngle = lampDynamic.transform.eulerAngles.y;
  }

  private void ApplyCalibration()
  {
    lampDynamic.transform.Rotate(0f, -calibrationYAngle, 0f, Space.World);
  }

  public void DisableLight()
  {
    lampDynamic.GetComponent<Light>().enabled = false;
  }

  public void gyroController()
  {
    if (!lampDynamic.GetComponent<Light>().enabled)
      lampDynamic.GetComponent<Light>().enabled = true;

    if (capturePhoneStraight)
      captureDataHoldingStraight();

    if (captureFlashlightStraight)
      captureDataFlashlightStraight();

    ApplyGyroRotation();
    ApplyCalibration();
  }

  public void SetEnabled(bool value)
  {
    enabled = true;
    StartCoroutine(CalibrateYAngle());
  }
}
