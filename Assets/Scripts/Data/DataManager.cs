using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ExcelManager : MonoBehaviour
{
    [Header("Google Sheets CSV URL")]
    [SerializeField] private string csvUrl ;

    [Header("Drag capsules from Hierarchy here")]
    public GameObject soldierObj;
    public GameObject tankObj;
    public GameObject archerObj;

    void Start()
    {
        StartCoroutine(DownloadAndAssignCSV());
    }

    IEnumerator DownloadAndAssignCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(csvUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading CSV: " + www.error);
        }
        else
        {
            ParseAndAssignValues(www.downloadHandler.text);
        }
    }

    void ParseAndAssignValues(string csvData)
    {
        string[] lines = csvData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Start from 1 to skip the header
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Trim().Split(',');

            if (fields.Length >= 4)
            {
                string unitName = fields[0].Trim().ToLower();
                int.TryParse(fields[1].Trim(), out int attack);
                int.TryParse(fields[2].Trim(), out int defense);
                int.TryParse(fields[3].Trim(), out int speed);

                AssignToObject(unitName, attack, defense, speed);
            }
        }
    }

    void AssignToObject(string name, int attack, int defense, int speed)
    {
        GameObject target = null;

        // Note: These cases must match exactly the text in your CSV (e.g., "soldier", "tank", "archer")
        switch (name)
        {
            case "soldier": target = soldierObj; break;
            case "tank": target = tankObj; break;
            case "archer": target = archerObj; break;
        }

        if (target != null)
        {
            UnitStats stats = target.GetComponent<UnitStats>();
            if (stats == null)
            {
                stats = target.AddComponent<UnitStats>();
            }

            stats.attack = attack;
            stats.defense = defense;
            stats.speed = speed;

            Debug.Log($"[Success] {target.name} updated -> Attack: {attack}, Defense: {defense}, Speed: {speed}");
        }
    }
}