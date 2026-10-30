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

    public EnemyController baseEnemy;
    private float baseEnemyCP;
    
    [Header("Realtime Stats")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackTimer;
    public TextMeshProUGUI enemySeen;
    public TextMeshProUGUI currentState;
    public TextMeshProUGUI teamName;
    
    [Header("BaseStats")]
    public TextMeshProUGUI maxhealth;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI range;
    public TextMeshProUGUI cooldown;
    public TextMeshProUGUI lootPoints;
    public TextMeshProUGUI cost;
    
    [Header("OtherStats")]
    [SerializeField] private TextMeshProUGUI attackpersectext;
    [SerializeField] private TextMeshProUGUI dpstext;
    [SerializeField] private TextMeshProUGUI ehptext;
    [SerializeField] private TextMeshProUGUI combatpowertext;
    [SerializeField] private TextMeshProUGUI relativepowertext;

    private float attackpersec;
    private float dps;
    private float ehp;
    private float combatpower;
    private float relativepower;

    private void Awake()
    {
        camera = Camera.main;
        float baseattackpersec = 1 / baseEnemy.cooldown;
        baseEnemyCP = (baseEnemy.maxHealth * baseattackpersec * (baseEnemy.damage / baseEnemy.maxHealth)) * 1000;
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
                
                var enemy = hit.collider.gameObject;
                var enemyController = hit.collider.gameObject.GetComponent<EnemyController>();
                
                GetPassiveValues(enemyController);
                StartCoroutine(GetActiveValues(hit));
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

    void GetPassiveValues(EnemyController enemyController)
    {
        maxhealth.text = "Max Health: " + enemyController.maxHealth;
        damage.text = "Damage: " + enemyController.damage;
        range.text = "Range: " + enemyController.range;
        cooldown.text = "Cooldown: " + enemyController.cooldown;
        lootPoints.text = "Loot Points: " + enemyController.lootPoints;
        //acost.text = "Cooldown: " + enemyController.cost;

        attackpersec = (1f / enemyController.cooldown);
        dps = Mathf.Round(attackpersec * 10.0f) * 0.1f;
        ehp = enemyController.maxHealth / baseEnemy.damage;
        combatpower = (enemyController.maxHealth * attackpersec * (enemyController.damage / baseEnemy.maxHealth)) * 1000;
        relativepower = ((combatpower / baseEnemyCP) * 100);

        attackpersectext.text = "Attack Per Sec: " + attackpersec;
        dpstext.text = "DPS: " + dps;
        ehptext.text = "EHP: " + ehp;
        combatpowertext.text = "Combat Power: " + combatpower;
        relativepowertext.text = "Relative Power: " + relativepower + "%";
    }
        

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerator GetActiveValues(RaycastHit hit)
    {
        while (true)
        {
            if (hit.collider != null)
            {
                var enemy = hit.collider.gameObject;
                var enemyController = hit.collider.gameObject.GetComponent<EnemyController>();
            
                nameText.text = enemy.name;
                healthText.text = "Current Health: " + enemy.GetComponent<IDamageable>().currentHealth;
                attackTimer.text = "Attack Timer: " + enemyController.attackTimer;
                teamName.text = "Layer: " + enemy.layer;
                currentState.text = "Current State: " + enemyController.StateMachine.CurrentEnemyState;

                if (enemyController.enemyHit != null)
                {
                    enemySeen.text = "Going to: " + enemyController.enemyHit.name;
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
