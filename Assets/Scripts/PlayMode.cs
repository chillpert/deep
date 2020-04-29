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
    else
    {
      var headlight1 = GameObject.Find("Headlight1").GetComponent<LookAtObject>();
      headlight1.enabled = false;

      var headlight2 = GameObject.Find("Headlight2").GetComponent<LookAtObject>();
      headlight2.enabled = false;
    }

    pressToStart.gameObject.SetActive(false);
  }
}
