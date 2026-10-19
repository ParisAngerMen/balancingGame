using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager instance;

    public float bluePoints = 200f;
    public float redPoints = 200f;
    
    public float automaticPoints = 5f;
    public TextMeshProUGUI redPointText;
    public TextMeshProUGUI bluePointText;

    public bool pointsEnabled = true;
    
    public float timeTillNextPoint = 1f;
    private float pointTimer = 0f;
        

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }

        else
        {
            instance = this;
        }
        
        redPointText.text = "Points: " + redPoints;
        bluePointText.text = "Points: " + bluePoints;
    }

    private void Update()
    {
        pointTimer += Time.deltaTime;

        if (pointTimer >= timeTillNextPoint && pointsEnabled)
        {
            AddPoints(automaticPoints, true);
            AddPoints(automaticPoints, false);
            pointTimer = 0f;
        }
    }

    public void AddPoints(float points, bool isRed)
    {
        if (pointsEnabled)
        {
            if (isRed)
            {
                redPoints += points;
                redPointText.text = "Points: " + redPoints;
            }

            else
            {
                bluePoints += points;
                bluePointText.text = "Points: " + bluePoints;
            }
        }

    }

    public void SubtractPoints(float points, bool isRed)
    {
        if (isRed)
        {
            redPoints -= points;
            redPointText.text = "Points: " + redPoints;
        }

        else
        {
            bluePoints -= points;
            bluePointText.text = "Points: " + bluePoints;
        }
    }
    

}
