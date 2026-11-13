using TMPro;
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
            return;
        }

        instance = this;
        UpdateTexts();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        pointTimer += Time.deltaTime;

        if (pointTimer >= timeTillNextPoint && pointsEnabled)
        {
            AddPoints(automaticPoints, true);
            AddPoints(automaticPoints, false);

            // Keep the leftover time so the income rate does not depend on the frame rate
            pointTimer = timeTillNextPoint > 0f ? pointTimer % timeTillNextPoint : 0f;
        }
    }

    public void AddPoints(float points, bool isRed)
    {
        if (!pointsEnabled)
        {
            return;
        }

        if (isRed)
        {
            redPoints += points;
        }

        else
        {
            bluePoints += points;
        }

        UpdateTexts();
    }

    public void SubtractPoints(float points, bool isRed)
    {
        if (isRed)
        {
            redPoints -= points;
        }

        else
        {
            bluePoints -= points;
        }

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        if (redPointText != null)
        {
            redPointText.text = "Points: " + redPoints;
        }

        if (bluePointText != null)
        {
            bluePointText.text = "Points: " + bluePoints;
        }
    }
}
