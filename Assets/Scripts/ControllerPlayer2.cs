using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPlayer2 : MonoBehaviour
{
  [HideInInspector]
  public Quaternion rotation = new Quaternion();
  [HideInInspector]
  public Vector3 acceleration = new Vector2();

  [SerializeField]
  GameObject submarine;
  [SerializeField]
  float verticalSpeed;

  void Start()
  {
        
  }


  void Update()
  {
    Vector3 dir = Vector3.zero;

    dir.z = -acceleration.z;

    if (dir.sqrMagnitude > 1)
      dir.Normalize();

    submarine.transform.Translate(dir * verticalSpeed * Time.deltaTime);
  }
}
