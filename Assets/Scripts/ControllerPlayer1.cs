using System.Collections;
using System.Collections.Generic;
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
  float horizontalSpeed;

  void Start()
  {
        
  }


  void Update()
  {
    Vector3 dir = Vector3.zero;
    
    dir.x = acceleration.y;

    if (dir.sqrMagnitude > 1)
      dir.Normalize();

    submarine.transform.Translate(dir * horizontalSpeed * Time.deltaTime);
  }
}
