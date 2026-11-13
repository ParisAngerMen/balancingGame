using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Downloads the unit table of the Google Sheet when the scene starts and applies
/// its values (MaxHP, Damage, Range, Cooldown, Loot Points, Cost, Speed) to the unit
/// prefabs, the shop prices and the price labels of the buy buttons.
/// The Sheet must be shared as "Anyone with the link can view".
/// </summary>
public class ExcelManager : MonoBehaviour
{
    [Header("Google Sheets CSV URL")]
    [SerializeField] private string csvUrl ;

    [Header("Drag capsules from Hierarchy here")]
    public GameObject melee;
    public GameObject tankObj;
    public GameObject archerObj;

    [Header("Shop (optional, found on this object if empty)")]
    [SerializeField] private ShopScript shop;

    private const int TimeoutSeconds = 10;

    // Columns of the unit table, identified by the start of their header text
    private static readonly string[] columnHeaders =
        { "maxhp", "damage", "range", "cooldown", "loot", "cost", "speed" };

    void Start()
    {
        if (string.IsNullOrWhiteSpace(csvUrl))
        {
            Debug.LogWarning("Sheet sync: the CSV URL is empty, unit stats will not be downloaded");
            return;
        }

        if (shop == null)
        {
            shop = GetComponent<ShopScript>();
        }

        StartCoroutine(DownloadAndAssignCSV());
    }

    IEnumerator DownloadAndAssignCSV()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(BuildCsvUrl(csvUrl)))
        {
            www.timeout = TimeoutSeconds;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Sheet sync: error downloading the Sheet, the values saved in the prefabs are used. " + www.error);
            }
            else
            {
                ParseAndAssignValues(www.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Accepts the normal link of the Sheet (".../edit?gid=123#gid=123") and turns it
    /// into the link that returns that tab as CSV. Other links are used unchanged.
    /// </summary>
    static string BuildCsvUrl(string url)
    {
        url = url.Trim();

        if (!url.Contains("docs.google.com/spreadsheets") || url.Contains("/export") || url.Contains("output=csv"))
        {
            return url;
        }

        Match id = Regex.Match(url, @"/spreadsheets/d/([a-zA-Z0-9_-]+)");

        if (!id.Success)
        {
            return url;
        }

        Match gid = Regex.Match(url, @"[?&#]gid=(\d+)");
        string export = "https://docs.google.com/spreadsheets/d/" + id.Groups[1].Value + "/export?format=csv";

        return gid.Success ? export + "&gid=" + gid.Groups[1].Value : export;
    }

    void ParseAndAssignValues(string csvData)
    {
        List<List<string>> rows = ParseCsv(csvData);

        // The unit table is the one whose header has both "Unit" and "DPS"
        // (the base unit and the multiplier tables do not have a DPS column)
        int headerRow = -1;
        int nameColumn = -1;

        for (int i = 0; i < rows.Count && headerRow < 0; i++)
        {
            int unitIndex = FindColumn(rows[i], "unit");

            if (unitIndex >= 0 && FindColumn(rows[i], "dps") >= 0)
            {
                headerRow = i;
                nameColumn = unitIndex;
            }
        }

        if (headerRow < 0)
        {
            Debug.LogError("Sheet sync: the unit table (a header row with 'Unit' and 'DPS') was not found. " +
                           "Check that the link points to the right tab and that the Sheet is shared as 'Anyone with the link can view'.");
            return;
        }

        int[] columns = new int[columnHeaders.Length];

        for (int c = 0; c < columnHeaders.Length; c++)
        {
            columns[c] = FindColumn(rows[headerRow], columnHeaders[c]);

            if (columns[c] < 0)
            {
                Debug.LogError("Sheet sync: the unit table has no column starting with '" + columnHeaders[c] + "'");
                return;
            }
        }

        List<GameObject> units = CollectUnits();
        TMP_Text[] labels = Resources.FindObjectsOfTypeAll<TMP_Text>();
        int updated = 0;

        // The table ends at the first row without a unit name
        for (int i = headerRow + 1; i < rows.Count; i++)
        {
            List<string> row = rows[i];
            string unitName = nameColumn < row.Count ? row[nameColumn].Trim() : "";

            if (unitName.Length == 0)
            {
                break;
            }

            float[] values = new float[columns.Length];
            bool valid = true;

            for (int c = 0; c < columns.Length && valid; c++)
            {
                valid = columns[c] < row.Count && TryParseNumber(row[columns[c]], out values[c]);
            }

            if (!valid)
            {
                Debug.LogWarning("Sheet sync: skipping '" + unitName + "', one of its values is not a number");
                continue;
            }

            if (AssignToObject(units, labels, unitName, values))
            {
                updated++;
            }
        }

        Debug.Log("Sheet sync: " + updated + " unit(s) updated from the Sheet");
    }

    // Unit prefabs known to the shop plus the ones assigned on this component
    List<GameObject> CollectUnits()
    {
        var units = new List<GameObject>();

        if (shop != null && shop.enemies != null)
        {
            units.AddRange(shop.enemies);
        }

        units.Add(melee);
        units.Add(tankObj);
        units.Add(archerObj);
        units.RemoveAll(unit => unit == null);

        return units;
    }

    // values: MaxHP, Damage, Range, Cooldown, Loot Points, Cost, Speed (same order as columnHeaders)
    bool AssignToObject(List<GameObject> units, TMP_Text[] labels, string unitName, float[] values)
    {
        // The name in the Sheet must match the name of the prefab (e.g. "Melee", "Tank", "Archer", "Wizard")
        GameObject target = units.Find(unit => string.Equals(unit.name, unitName, StringComparison.OrdinalIgnoreCase));

        if (target == null || !target.TryGetComponent(out EnemyController controller))
        {
            Debug.LogWarning("Sheet sync: no unit prefab called '" + unitName + "' was found");
            return false;
        }

        float health = values[0], damage = values[1], range = values[2], cooldown = values[3];
        float lootPoints = values[4], cost = values[5], speed = values[6];

        controller.ApplyStats(health, damage, range, cooldown, lootPoints, cost, speed);

        if (target.TryGetComponent(out UnitStats stats))
        {
            stats.damage = damage;
            stats.cooldown = cooldown;
            stats.speed = speed;
            stats.health = health;
            stats.range = range;
        }

        if (shop != null)
        {
            shop.SetUnitCost(target, cost);
        }

        UpdatePriceLabels(labels, unitName, cost);

#if UNITY_EDITOR
        // Keep the new values in the prefab so the project always reflects the Sheet
        UnityEditor.EditorUtility.SetDirty(target);
#endif

        Debug.Log($"[Success] {target.name} updated -> Health: {health}, Damage: {damage}, Range: {range}, " +
                  $"Cooldown: {cooldown}, Loot Points: {lootPoints}, Cost: {cost}, Speed: {speed}");
        return true;
    }

    // Buy buttons show texts like "ARCHER: 100"
    static void UpdatePriceLabels(TMP_Text[] labels, string unitName, float cost)
    {
        string prefix = unitName.ToUpperInvariant() + ":";

        foreach (TMP_Text label in labels)
        {
            // Only texts that are in the scene (not inside prefab assets)
            if (label == null || !label.gameObject.scene.IsValid() || label.text == null)
            {
                continue;
            }

            if (label.text.Trim().StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                label.text = prefix + " " + cost.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    static int FindColumn(List<string> row, string headerStart)
    {
        for (int i = 0; i < row.Count; i++)
        {
            if (row[i].Trim().StartsWith(headerStart, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }

    // The sheet uses '.' as decimal separator, whatever the language of the computer is
    static bool TryParseNumber(string text, out float value)
    {
        return float.TryParse(text.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    // Splits CSV text into rows and cells, supporting quoted cells ("a, b", "say ""hi""")
    static List<List<string>> ParseCsv(string csv)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var cell = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < csv.Length; i++)
        {
            char ch = csv[i];

            if (inQuotes)
            {
                if (ch != '"')
                {
                    cell.Append(ch);
                }
                else if (i + 1 < csv.Length && csv[i + 1] == '"')
                {
                    cell.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = false;
                }
            }
            else if (ch == '"')
            {
                inQuotes = true;
            }
            else if (ch == ',')
            {
                row.Add(cell.ToString());
                cell.Clear();
            }
            else if (ch == '\n')
            {
                row.Add(cell.ToString());
                cell.Clear();
                rows.Add(row);
                row = new List<string>();
            }
            else if (ch != '\r')
            {
                cell.Append(ch);
            }
        }

        row.Add(cell.ToString());
        rows.Add(row);

        return rows;
    }
}
