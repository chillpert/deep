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
  float clampAngle;
  [SerializeField]
  GameObject player2;

  [HideInInspector]
  public Vector2 joystick;
  [HideInInspector]
  public bool actionPressed;
  [HideInInspector]
  public bool available;

  readonly float initialVal = 0f;
  [HideInInspector]
  public float timeSinceLastReload = 0f;
  float coolDown = 5f;

  GameObject rotationDummy;
  bool actionPressedFirst = true;

  [HideInInspector]
  public bool reloadedTorpedo = false;
  
  void Start()
  {
    available = true;
    rotationDummy = new GameObject("HorziontalRotationDummy");
  }

  void Update()
  {
    Vector3 dir = Vector3.zero;

    dir.y = acceleration.x;

    //Debug.Log("Player 1: " + dir.y);

    if (initialVal > dir.y + threshold || initialVal < dir.y - threshold)
    {
      if (dir.sqrMagnitude > 1)
        dir.Normalize();

      rotationDummy.transform.position = submarine.transform.position;
      rotationDummy.transform.rotation = submarine.transform.rotation;
      //rotationDummy.transform.forward = submarine.transform.forward;

      rotationDummy.transform.Rotate(dir * horizontalSpeed * Time.deltaTime);      

      if (Vector3.Angle(rotationDummy.transform.forward, CollisionsWithoutImpact.forward) < clampAngle)
        submarine.transform.Rotate(dir * horizontalSpeed * Time.deltaTime);
    }

    Debug.DrawRay(rotationDummy.transform.position, rotationDummy.transform.forward * 5f, Color.green);
    Debug.DrawRay(rotationDummy.transform.position, CollisionsWithoutImpact.forward * 5f, Color.red);

    /*
    Vector3 lightDir = Vector3.zero;
    lightDir.x = -joystick.y;
    lightDir.y = joystick.x;

    headLight.transform.Rotate(lightDir * headLightSpeed * Time.deltaTime);
    */

    if (actionPressed)
    {
      reloadedTorpedo = true;

      if (actionPressedFirst)
      {
        timeSinceLastReload = Time.time;
        actionPressedFirst = false;

        Debug.Log("Phone 1: Loaded Torpedo @" + timeSinceLastReload);
      }
    }
    else
    {
      if (Time.time - timeSinceLastReload > coolDown)
      {
        actionPressedFirst = true; 
        reloadedTorpedo = false;
      }
    }
  }
}
