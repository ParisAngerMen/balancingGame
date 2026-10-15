using System;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class MouseBehaviour : MonoBehaviour
{
    private Camera camera;
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI velocity;
    public TextMeshProUGUI enemySeen;
    public TextMeshProUGUI attackCooldown;
    public TextMeshProUGUI teamName;

    private void Awake()
    {
        camera = Camera.main;
    }

    public void OnClick() 
    {
        Debug.Log("OnClick");
        
        Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                StartCoroutine(EnemyStatNames(hit));
            }

            if (hit.collider.CompareTag("RedBase") || hit.collider.CompareTag("BlueBase"))
            {
                StartCoroutine(BaseStatNames(hit));
            }
            
            Debug.Log("Click Name: " + hit.collider.name);
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
                velocity.text = "Speed: " + enemy.GetComponent<NavMeshAgent>().speed;
                attackCooldown.text = "Time until next attack: " + enemy.GetComponent<EnemyController>().attackTimer;
                teamName.text = "Layer: " + enemy.layer;

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
                
            }

            else
            {
                yield break;
            }
            
            yield return new WaitForEndOfFrame();

        }
        
        
        
    }
}
