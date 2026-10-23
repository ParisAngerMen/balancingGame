using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static UnityEditor.Handles;

public class MouseBehaviour : MonoBehaviour
{
    private Camera camera;
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackTimer;
    public TextMeshProUGUI enemySeen;
    public TextMeshProUGUI currentState;
    public TextMeshProUGUI teamName;

    private void Awake()
    {
        camera = Camera.main;
    }

    public void OnClick() 
    {
        Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                enemySeen.enabled = true;
                currentState.enabled = true;
                
                StartCoroutine(EnemyStatNames(hit));
            }

            if (hit.collider.CompareTag("RedBase") || hit.collider.CompareTag("BlueBase"))
            {
                enemySeen.enabled = false;
                currentState.enabled = false;
                
                StartCoroutine(BaseStatNames(hit));
            }
            
            Selection.activeGameObject = hit.collider.gameObject;

        }
    }
        

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerator EnemyStatNames(RaycastHit hit)
    {
        while (true)
        {
            if (hit.collider != null)
            {
                var enemy = hit.collider.gameObject;
            
                nameText.text = enemy.name;
                healthText.text = "Health: " + enemy.GetComponent<IDamageable>().currentHealth;
                attackTimer.text = "Attack timer: " + enemy.GetComponent<EnemyController>().attackTimer;
                teamName.text = "Layer: " + enemy.layer;
                currentState.text = "Current State: " + enemy.GetComponent<EnemyController>().StateMachine.CurrentEnemyState;

                if (enemy.GetComponent<EnemyController>().enemyHit != null)
                {
                    enemySeen.text = "Going to: " + enemy.GetComponent<EnemyController>().enemyHit.name;
                }

                else
                {
                    enemySeen.text = "Going to: " + "None";
                }
            }

            else
            {
                yield break;
            }
            
            yield return new WaitForEndOfFrame();

        }
        
    }
    private IEnumerator BaseStatNames(RaycastHit hit)
    {
        while (true)
        {
            if (hit.collider != null)
            {
                var enemyBase = hit.collider.gameObject;
            
                nameText.text = enemyBase.name;
                healthText.text = "Health: " + enemyBase.GetComponent<IDamageable>().currentHealth;
                teamName.text = "Layer: " + enemyBase.layer;
                
            }

            else
            {
                yield break;
            }
            
            yield return new WaitForEndOfFrame();

        }
        
        
        
    }


}
