using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;
    void Start()
    {
        cam = Camera.main; 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.forward = cam.transform.forward;
    }
}
