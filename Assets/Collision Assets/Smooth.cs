using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smooth : MonoBehaviour
{
  [SerializeField]
  GameObject next;

  [SerializeField]
  GameObject firstEdge;

  [SerializeField]
  GameObject secondEdge;

  public void newScale(GameObject theGameObject, float newSize)
  {

    float size = theGameObject.GetComponent<Renderer>().bounds.size.z;

    Vector3 rescale = theGameObject.transform.localScale;

    rescale.z = newSize * rescale.z / size;

    theGameObject.transform.localScale = rescale;
  }

  void Start()
  {
    /*
    GameObject bridge = GameObject.CreatePrimitive(PrimitiveType.Plane);

    //bridge.transform.parent = transform;
    bridge.transform.position = transform.position + ((next.transform.position - transform.position) / 2f);

    float distance = (secondEdge.transform.position - firstEdge.transform.position).magnitude;

    Vector3 direction = secondEdge.transform.position - firstEdge.transform.position;
    bridge.transform.rotation = Quaternion.LookRotation(direction);
    //bridge.transform.localScale = new Vector3(1, 1, direction.magnitude);
    newScale(bridge, distance);
    */
  }

  void Update()
  {
    
  }
}
