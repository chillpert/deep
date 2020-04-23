using UnityEngine;

public class Printer : MonoBehaviour
{
  [SerializeField]
  private string message;

  void Start()
  {
    Debug.Log(message);
  }
}
