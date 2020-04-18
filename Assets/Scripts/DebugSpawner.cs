using UnityEngine;

public class DebugSpawner : MonoBehaviour
{
  private enum Spawn
  {
    Level1, Level2, Level3, Level4,
    Cave1, Cave2, Cave3,
    Free
  }

  [SerializeField]
  private Spawn spawn = Spawn.Level1;

  private void Start()
  {
    GameObject submarine = GameObject.Find("Submarine");
    SubmarineController controller = submarine.GetComponent<SubmarineController>();

    switch (spawn)
    {
      case Spawn.Level1:
        SetTransform(GameObject.Find("Level1Position"));
        controller.Level = 1;
        break;

      case Spawn.Level2:
        SetTransform(GameObject.Find("Level2Position"));
        controller.Level = 2;
        break;

      case Spawn.Level3:
        SetTransform(GameObject.Find("Level3Position"));
        controller.Level = 3;
        break;

      case Spawn.Level4:
        SetTransform(GameObject.Find("Level4Position"));
        controller.Level = 4;
        break;

      case Spawn.Cave1:
        SetTransform(GameObject.Find("Cave1Position"));
        controller.Level = 1;
        break;

      case Spawn.Cave2:
        SetTransform(GameObject.Find("Cave2Position"));
        controller.Level = 2;
        break;

      case Spawn.Cave3:
        SetTransform(GameObject.Find("Cave3Position"));
        controller.Level = 3;
        break;

      case Spawn.Free:
        controller.Level = 1;
        break;
    }
  }

  private void Update()
  {
    GameObject submarine = GameObject.Find("Submarine");
    SubmarineController controller = submarine.GetComponent<SubmarineController>();
  }

  private void SetTransform(GameObject gameobject)
  {
    transform.position = gameobject.transform.position;
    transform.rotation = gameobject.transform.rotation;
    transform.forward = gameobject.transform.forward;
  }
}
