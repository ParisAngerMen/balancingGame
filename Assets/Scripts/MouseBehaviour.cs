using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class MouseBehaviour : MonoBehaviour
{
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
    }

    public void OnClick() 
    {
        Debug.Log("OnClick");
        
        var Ray = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.collider != null)
        {
            Debug.Log("Click Name: " + hit.collider.name);
        }
    }
        

    // Update is called once per frame
    void Update()
    {
        
    }
}
