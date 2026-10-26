using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public SpawnerScript spawner;

    public GameObject[] enemies;
    public float[] costs;

    public bool isRed = true;
    
    private void BuyUnit(int unit)
    {
        if (isRed)
        {
            if (PointManager.instance.redPoints >= costs[unit])
            {
                spawner.SetTeam(enemies[unit], true);
                PointManager.instance.SubtractPoints(costs[unit], true);
            }
        }

        else
        {
            if (PointManager.instance.bluePoints >= costs[unit])
            {
                spawner.SetTeam(enemies[unit], false);
                PointManager.instance.SubtractPoints(costs[unit], false);
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
