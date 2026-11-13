using System.Collections;
using TMPro;
using UnityEngine;

public class MouseBehaviour : MonoBehaviour
{
    private Camera cam;

    public EnemyController baseEnemy;

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

    // "Number of Units" column of the Sheet: the debug menu inspects a single unit
    private const float NumberOfUnits = 1f;

    // Only one object is inspected at a time
    private Coroutine statsRoutine;

    private void Awake()
    {
        cam = Camera.main;
    }

    private static float AttacksPerSecond(EnemyController enemyController)
    {
        return enemyController.cooldown > 0f ? 1f / enemyController.cooldown : 0f;
    }

    // Sheet: Time to Fire = MAX(0, Range - base Range) / (Speed + base Speed)
    private float TimeToFire(EnemyController enemyController)
    {
        if (baseEnemy == null)
        {
            return 0f;
        }

        float combinedSpeed = enemyController.Speed + baseEnemy.Speed;

        return combinedSpeed > 0f
            ? Mathf.Max(0f, enemyController.range - baseEnemy.range) / combinedSpeed
            : 0f;
    }

    // Sheet: P = MaxHP * DPS * Number of Units^2 * (1 + Time to Fire * Cooldown)
    private float CombatPower(EnemyController enemyController)
    {
        float unitDps = enemyController.damage * AttacksPerSecond(enemyController);

        return enemyController.maxHealth * unitDps * NumberOfUnits * NumberOfUnits *
               (1f + TimeToFire(enemyController) * enemyController.cooldown);
    }

    public void OnClick()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null || !Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            return;
        }

        GameObject clicked = hit.collider.gameObject;

        if (hit.collider.CompareTag("Enemy"))
        {
            var enemyController = clicked.GetComponentInParent<EnemyController>();

            if (enemyController != null)
            {
                enemySeen.enabled = true;
                currentState.enabled = true;

                GetPassiveValues(enemyController);
                ShowStats(GetActiveValues(enemyController));
            }
        }

        else if (hit.collider.CompareTag("RedBase") || hit.collider.CompareTag("BlueBase"))
        {
            enemySeen.enabled = false;
            currentState.enabled = false;

            ShowStats(BaseStatNames(clicked));
        }

#if UNITY_EDITOR
        // Editor only: this API does not exist in a built game
        UnityEditor.Selection.activeGameObject = clicked;
#endif
    }

    private void ShowStats(IEnumerator routine)
    {
        // Stop following the previously selected object before following a new one
        if (statsRoutine != null)
        {
            StopCoroutine(statsRoutine);
        }

        statsRoutine = StartCoroutine(routine);
    }

    void GetPassiveValues(EnemyController enemyController)
    {
        maxhealth.text = "Max Health: " + enemyController.maxHealth;
        damage.text = "Damage: " + enemyController.damage;
        range.text = "Range: " + enemyController.range;
        cooldown.text = "Cooldown: " + enemyController.cooldown;
        lootPoints.text = "Loot Points: " + enemyController.lootPoints;
        cost.text = "Cost: " + enemyController.cost;

        attackpersec = AttacksPerSecond(enemyController);
        // Damage per second = damage per hit x hits per second (rounded to one decimal)
        dps = Mathf.Round(enemyController.damage * attackpersec * 10.0f) / 10.0f;
        ehp = baseEnemy != null && baseEnemy.damage > 0f ? enemyController.maxHealth / baseEnemy.damage : 0f;
        combatpower = CombatPower(enemyController);
        // Calculated on every click: the base unit's stats can change when the Sheet is loaded
        float baseEnemyCP = baseEnemy != null ? CombatPower(baseEnemy) : 0f;
        relativepower = baseEnemyCP > 0f ? (combatpower / baseEnemyCP) * 100 : 0f;

        attackpersectext.text = "Attack Per Sec: " + attackpersec;
        dpstext.text = "DPS: " + dps;
        ehptext.text = "EHP: " + ehp;
        combatpowertext.text = "Combat Power: " + combatpower;
        relativepowertext.text = "Relative Power: " + relativepower + "%";
    }

    private IEnumerator GetActiveValues(EnemyController enemyController)
    {
        // Runs until the unit is destroyed (or another object is selected)
        while (enemyController != null)
        {
            GameObject enemy = enemyController.gameObject;

            nameText.text = enemy.name;
            healthText.text = "Current Health: " + enemyController.currentHealth;
            attackTimer.text = "Attack Timer: " + enemyController.attackTimer;
            teamName.text = "Layer: " + enemy.layer;
            currentState.text = "Current State: " + enemyController.StateMachine?.CurrentEnemyState;

            if (enemyController.enemyHit != null)
            {
                enemySeen.text = "Going to: " + enemyController.enemyHit.name;
            }

            else
            {
                enemySeen.text = "Going to: " + "None";
            }

            yield return null;
        }

        statsRoutine = null;
    }

    private IEnumerator BaseStatNames(GameObject enemyBase)
    {
        // Runs until the base is destroyed (or another object is selected)
        while (enemyBase != null)
        {
            IDamageable damageable = enemyBase.GetComponent<IDamageable>();

            nameText.text = enemyBase.name;
            healthText.text = "Health: " + (damageable != null ? damageable.currentHealth.ToString() : "-");
            teamName.text = "Layer: " + enemyBase.layer;

            yield return null;
        }

        statsRoutine = null;
    }
}
