using UnityEngine;

public class ShopScript : MonoBehaviour
{
    public SpawnerScript spawner;

    public GameObject[] enemies;
    public float[] costs;

    public bool isRed = true;

    private void Start()
    {
        // Both arrays are filled in the Inspector and may not have the same length
        int count = Mathf.Min(enemies.Length, costs.Length);

        for (int i = 0; i < count; i++)
        {
            if (enemies[i] != null && enemies[i].TryGetComponent(out EnemyController enemyController))
            {
                enemyController.cost = costs[i];
            }
        }
    }

    /// <summary>
    /// Changes the price of one of the units sold by the shop.
    /// </summary>
    public void SetUnitCost(GameObject unit, float cost)
    {
        int index = System.Array.IndexOf(enemies, unit);

        if (index >= 0 && index < costs.Length)
        {
            costs[index] = cost;
        }
    }

    private void BuyUnit(int unit)
    {
        if (unit < 0 || unit >= enemies.Length || unit >= costs.Length)
        {
            Debug.LogWarning("Shop has no unit or cost configured at index " + unit);
            return;
        }

        PointManager pointManager = PointManager.instance;

        if (pointManager == null || spawner == null)
        {
            return;
        }

        float availablePoints = isRed ? pointManager.redPoints : pointManager.bluePoints;

        if (availablePoints >= costs[unit])
        {
            spawner.SetTeam(enemies[unit], isRed);

            if (pointManager.pointsEnabled)
            {
                pointManager.SubtractPoints(costs[unit], isRed);
            }
        }
    }

    public void SetBlue()
    {
        isRed = false;
    }

    public void SetRed()
    {
        isRed = true;
    }

    public void BuyMelee()
    {
        BuyUnit(0);
    }

    public void BuyTank()
    {
        BuyUnit(1);
    }

    public void BuyArcher()
    {
        BuyUnit(2);
    }

    public void BuyWizard()
    {
        BuyUnit(3);
    }
}
