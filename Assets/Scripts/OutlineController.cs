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
      if (mat.shader.name == "Outlined/Silhouetted Diffuse")
      {
        mat.SetFloat("_Outline", outlineWidth);
        mat.SetColor("_OutlineColor", outlineColor);
      }
    }
  }

  public void HideOutline()
  {
    foreach (var mat in meshRenderer.materials)
    {
      if (mat.shader.name == "Outlined/Silhouetted Diffuse")
      {
        mat.SetFloat("_Outline", 0f);
        mat.SetColor("_OutlineColor", new Color(0f, 0f, 0f, 0f));
      }
    }
  }
}
