using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MaterialAdder : MonoBehaviour
{
    private SkinnedMeshRenderer[] skinnedMeshRenderer;
    private GameObject[] bodyParts;

    public Material blueMaterial;
    public Material redMaterial;

    private void Awake()
    {
        skinnedMeshRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        if (gameObject.layer == 6)
        {
            AddMaterial(true);
        }
        
        else
        {
            AddMaterial(false);
        }
    }

    public void AddMaterial(bool isRed)
    {
        foreach (SkinnedMeshRenderer skin in skinnedMeshRenderer)
        {
            if (isRed)
            {
                Debug.Log("Adding red skin");
                skin.material = redMaterial;
            }

            else
            {
                skin.material = blueMaterial;
            }
        }
    }
}
