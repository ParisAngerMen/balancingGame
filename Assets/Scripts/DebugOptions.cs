using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class DebugOptions : MonoBehaviour
{
    public SpawnerScript spawner;
    
    public GameObject pauseMenu;
    public GameObject gameplayCanvas;
    public GameObject debugCanvas;
    public GameObject redCanvas;
    private MouseBehaviour mouseBehaviour;

    private void Awake()
    {
        mouseBehaviour = GetComponent<MouseBehaviour>();
    }
    
    public void EnableSpawning()
    {
        spawner.canSpawn = !spawner.canSpawn;
        Debug.Log("Enable spawning: " + spawner.canSpawn);
    }

    public void EnableDebug()
    {
        debugCanvas.SetActive(!debugCanvas.activeSelf);
        mouseBehaviour.enabled = !mouseBehaviour.enabled;
    }

    public void EnableAI()
    {
        spawner.enableAutoSpawn = !spawner.enableAutoSpawn;
        redCanvas.SetActive(!spawner.enableAutoSpawn);
    }

    public void OnPause()
    {
        Debug.Log("Click pause");
        
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        gameplayCanvas.SetActive(!pauseMenu.activeSelf);

        if (pauseMenu.activeSelf)
        {
            Time.timeScale = 0;
            Debug.Log("Pause game");
        }

        else
        {
            Time.timeScale = 1;
            Debug.Log("Unpause game");
        }
    }

}
