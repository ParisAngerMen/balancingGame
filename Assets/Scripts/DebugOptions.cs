using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class DebugOptions : MonoBehaviour
{
    public SpawnerScript spawner;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void EnableSpawning(bool spanwerOn)
    {
        spawner.canSpawn = spanwerOn;
    }

    public void OnPause()
    {
        
    }

}
