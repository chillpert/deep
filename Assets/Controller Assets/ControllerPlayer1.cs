using UnityEngine;

public class ControllerPlayer1 : MonoBehaviour
{
  [HideInInspector]
  public Quaternion rotation = new Quaternion();
  [HideInInspector]
  public Vector3 acceleration = new Vector2();

  [SerializeField]
  GameObject submarine;
  [SerializeField]
  GameObject headLight;
  [SerializeField]
  float horizontalSpeed;
  [SerializeField]
  float headLightSpeed;
  [SerializeField]
  float threshold = 0.1f;
  [SerializeField]
  float clampAngle = 80f;

  [HideInInspector]
  public Vector2 joystick;
  [HideInInspector]
  public bool actionPressed;
  [HideInInspector]
  public bool available;

  readonly float initialVal = 0f;

  GameObject rotationDummy;
  
  void Start()
  {
    available = true;
    rotationDummy = new GameObject("HorziontalRotationDummy");
  }

  void Update()
  {
    Vector3 dir = Vector3.zero;

    dir.y = acceleration.x;

    if (initialVal > dir.y + threshold || initialVal < dir.y - threshold)
    {
      if (dir.sqrMagnitude > 1)
        dir.Normalize();

      rotationDummy.transform.position = submarine.transform.position;
      rotationDummy.transform.rotation = submarine.transform.rotation;
      rotationDummy.transform.forward = submarine.transform.forward;

      rotationDummy.transform.Rotate(dir * horizontalSpeed * Time.deltaTime);

      if (Vector3.Angle(rotationDummy.transform.forward, CollisionsWithoutImpact.forward) < clampAngle)
        submarine.transform.Rotate(dir * horizontalSpeed * Time.deltaTime);
    }

    Vector3 lightDir = Vector3.zero;
    lightDir.x = -joystick.y;
    lightDir.y = joystick.x;

    headLight.transform.Rotate(lightDir * headLightSpeed * Time.deltaTime);
  }
}
