using UnityEngine;
using UnityEngine.UI;

public class PlayMode : MonoBehaviour
{
  [SerializeField]
  public bool SinglePlayer = false;

  [SerializeField]
  private Text instructions = null;
  [SerializeField]
  private Button pressToStart = null;

  private void Start()
  {
    if (!SinglePlayer)
    {
      instructions.gameObject.SetActive(false);
    }

    pressToStart.gameObject.SetActive(false);
  }
}
