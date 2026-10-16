using UnityEditor;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    public SpawnerScript spawner;

    public float meleeCost;
    public float tankCost;

    public bool isRed;

    public GameObject melee;
    public GameObject tank;

    private void BuyUnit(float cost, GameObject unit)
    {
        if (isRed)
        {
            if (PointManager.instance.redPoints >= cost)
            {
                spawner.SetTeam(unit, true);
                PointManager.instance.SubtractPoints(cost, true);
            }
        }

        else
        {
            if (PointManager.instance.bluePoints >= cost)
            {
                spawner.SetTeam(unit, false);
                PointManager.instance.SubtractPoints(cost, false);
            }
        }

    }
    
    public void BuyMelee()
    {
        BuyUnit(meleeCost, melee);
    }    
    
    public void BuyTank()
    {
        BuyUnit(tankCost, tank);
    }
    
    
}
