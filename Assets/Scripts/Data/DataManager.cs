using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ExcelManager : MonoBehaviour
{
    [Header("Google Sheets CSV URL")]
    [SerializeField] private string csvUrl ;

    [Header("Drag capsules from Hierarchy here")]
    public GameObject melee;
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

            if (fields.Length >= 6)
            {
                string unitName = fields[0].Trim().ToLower();
                float.TryParse(fields[1].Trim(), out float speed);
                float.TryParse(fields[2].Trim(), out float health);
                float.TryParse(fields[3].Trim(), out float damage);
                float.TryParse(fields[4].Trim(), out float range);
                float.TryParse(fields[5].Trim(), out float cooldown);

                AssignToObject(unitName, speed, health, damage, range, cooldown);
        }
    }

    void AssignToObject(string name, float speed, float health, float damage, float range, float cooldown)
    {
        GameObject target = null;

        // Note: These cases must match exactly the text in your CSV (e.g., "soldier", "tank", "archer")
        switch (name)
        {
            case "melee": target = melee; break;
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

            stats.damage = damage;
            stats.cooldown = cooldown;
            stats.speed = speed;
            stats.health = health;
            stats.range = range;

            Debug.Log($"[Success] {target.name} updated -> Speed: {speed}, Health: {health}, Damage: {damage}, Range: {range}, Cooldown: {cooldown}");
        }
    }
}
}