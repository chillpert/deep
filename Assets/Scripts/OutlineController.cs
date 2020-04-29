using UnityEngine;

public class OutlineController : MonoBehaviour
{
  private MeshRenderer meshRenderer = null;

  [SerializeField]
  private float outlineWidth = 2.5f;
  [SerializeField]
  private Color outlineColor = Color.yellow;

  private void Start()
  {
    meshRenderer = GetComponent<MeshRenderer>();
  }

  public void ShowOutline()
  {
    foreach (var mat in meshRenderer.materials)
    {
      Debug.Log(mat.name);
      mat.SetFloat("_Outline", outlineWidth);
      mat.SetColor("_OutlineColor", outlineColor);
    }
  }

  public void HideOutline()
  {
    foreach (var mat in meshRenderer.materials)
    {
      mat.SetFloat("_Outline", 0f);
    }
  }
}
