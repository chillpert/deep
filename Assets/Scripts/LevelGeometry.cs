using UnityEngine;

public class LevelGeometry : MonoBehaviour
{
  public static GameObject Level1 = null;
  public static GameObject Level2 = null;
  public static GameObject Level3 = null;
  public static GameObject Level4 = null;

  public static GameObject Cave1 = null;
  public static GameObject Cave2 = null;
  public static GameObject Cave3 = null;

  private void Start()
  {
    Debug.Log("LevelGeomtry: Setting up geometry");

    Level1 = GameObject.Find("Level1");
    Level2 = GameObject.Find("Level2");
    Level3 = GameObject.Find("Level3");
    Level4 = GameObject.Find("Level4");

    Cave1 = GameObject.Find("Cave1");
    Cave2 = GameObject.Find("Cave2");
    Cave3 = GameObject.Find("Cave3");
  }

  public static void SetAll(bool visibility)
  {
    Level1.SetActive(visibility);
    Level2.SetActive(visibility);
    Level3.SetActive(visibility);
    Level4.SetActive(visibility);

    Cave1.SetActive(visibility);
    Cave2.SetActive(visibility);
    Cave3.SetActive(visibility);
  }
}
