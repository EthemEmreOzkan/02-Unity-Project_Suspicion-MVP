using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Text_Seperator : MonoBehaviour
{
    [SerializeField] private Gemini_Api_Handler Gemini_Api_Handler;
    
    [Header("Parsed Data")]
    public List<string> Motive_For_Murder = new List<string>();
    public List<string> Murder_Weapon = new List<string>();
    public List<string> Murder_Location = new List<string>();
    public List<string> Murder_Combination = new List<string>();
    public string Murder_Scenario = "";
    public List<string> Witness_Words = new List<string>();
    public string Witness_Scenario = "";

    private bool hasProcessed = false;

    void Update()
    {
        if (!hasProcessed && 
            Gemini_Api_Handler.Is_Response_Received && 
            !Gemini_Api_Handler.Is_Request_In_Progress)
        {
            ParseResponse(Gemini_Api_Handler.Last_Response);
            hasProcessed = true;
        }
    }

    private void ParseResponse(string response)
    {
        if (string.IsNullOrEmpty(response)) return;

        // Tüm listeleri temizle
        ClearAllLists();

        // Metni satırlara böl ve boş satırları kaldır
        string[] lines = response.Split('\n')
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line))
            .ToArray();

        string currentSection = "";
        List<string> currentList = null;
        System.Text.StringBuilder scenarioBuilder = null;

        foreach (string line in lines)
        {
            // Yeni bölüm başlığı kontrolü
            if (line.StartsWith("#"))
            {
                // Önceki senaryo varsa kaydet
                if (scenarioBuilder != null)
                {
                    SaveScenario(currentSection, scenarioBuilder.ToString().Trim());
                    scenarioBuilder = null;
                }

                currentSection = line;
                currentList = GetListForSection(currentSection);
                
                // Senaryo bölümlerinde StringBuilder kullan
                if (IsScenarioSection(currentSection))
                {
                    scenarioBuilder = new System.Text.StringBuilder();
                }
            }
            else
            {
                // Senaryo bölümü ise StringBuilder'a ekle
                if (scenarioBuilder != null)
                {
                    if (scenarioBuilder.Length > 0)
                        scenarioBuilder.AppendLine();
                    scenarioBuilder.Append(line);
                }
                // Liste bölümü ise listeye ekle
                else if (currentList != null)
                {
                    currentList.Add(line);
                }
            }
        }

        // Son senaryo varsa kaydet
        if (scenarioBuilder != null)
        {
            SaveScenario(currentSection, scenarioBuilder.ToString().Trim());
        }
    }

    private void ClearAllLists()
    {
        Motive_For_Murder.Clear();
        Murder_Weapon.Clear();
        Murder_Location.Clear();
        Murder_Combination.Clear();
        Witness_Words.Clear();
        Murder_Scenario = "";
        Witness_Scenario = "";
    }

    private List<string> GetListForSection(string section)
    {
        switch (section)
        {
            case "#Cinayet_Nedeni":
                return Motive_For_Murder;
            case "#Cinayet_Silahı":
                return Murder_Weapon;
            case "#Cinayet_Mekanı":
                return Murder_Location;
            case "#Cinayet_Kombinasyonu":
                return Murder_Combination;
            case "#Görgü_Tanığı_Kelimeler":
                return Witness_Words;
            default:
                return null;
        }
    }

    private bool IsScenarioSection(string section)
    {
        return section == "#Cinayet_Senaryosu" || section == "#Görgü_Tanığı_Senaryosu";
    }

    private void SaveScenario(string section, string content)
    {
        if (section == "#Cinayet_Senaryosu")
            Murder_Scenario = content;
        else if (section == "#Görgü_Tanığı_Senaryosu")
            Witness_Scenario = content;
    }

    // Debug için - Inspector'da test etmek isterseniz
    [ContextMenu("Debug Print Results")]
    private void DebugPrintResults()
    {
        Debug.Log($"Cinayet Nedenleri: {string.Join(", ", Motive_For_Murder)}");
        Debug.Log($"Cinayet Silahları: {string.Join(", ", Murder_Weapon)}");
        Debug.Log($"Cinayet Mekanları: {string.Join(", ", Murder_Location)}");
        Debug.Log($"Cinayet Kombinasyonu: {string.Join(", ", Murder_Combination)}");
        Debug.Log($"Cinayet Senaryosu: {Murder_Scenario}");
        Debug.Log($"Görgü Tanığı Kelimeler: {string.Join(", ", Witness_Words)}");
        Debug.Log($"Görgü Tanığı Senaryosu: {Witness_Scenario}");
    }
}