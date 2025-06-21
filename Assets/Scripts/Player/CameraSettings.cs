using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // Create a RenderTexture with custom resolution
        RenderTexture rt = new RenderTexture(1280, 720, 0);
        cam.targetTexture = rt;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
