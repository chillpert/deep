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
    meshRenderer.material.SetFloat("_Outline", outlineWidth);
    meshRenderer.material.SetColor("_OutlineColor", outlineColor);
  }

  public void HideOutline()
  {
    meshRenderer.material.SetFloat("_Outline", 0f);
  }
}
