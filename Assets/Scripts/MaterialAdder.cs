using UnityEngine;

public class MaterialAdder : MonoBehaviour
{
    private SkinnedMeshRenderer[] skinnedMeshRenderer;

    public Material blueMaterial;
    public Material redMaterial;

    private void Awake()
    {
        skinnedMeshRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        AddMaterial(gameObject.layer == LayerMask.NameToLayer("Red"));
    }

    public void AddMaterial(bool isRed)
    {
        Material teamMaterial = isRed ? redMaterial : blueMaterial;

        if (teamMaterial == null)
        {
            return;
        }

        foreach (SkinnedMeshRenderer skin in skinnedMeshRenderer)
        {
            skin.material = teamMaterial;
        }
    }
}
