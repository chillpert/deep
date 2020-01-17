using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0.0f, -1.0f, 0.0f); // This speed should be coherent with the exhaust particle speed
        
    }
    
	// TODO Check collision with wall (delete self), obstacle (delete obstacle)
}
