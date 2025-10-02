using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Run_Time_Tabs : MonoBehaviour
{
    [Header("References -------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Text_Seperator Text_Seperator;
    [SerializeField] private TextMeshPro Witness_Scenario;
    [SerializeField] private List<TextMeshPro> Motives_For_Murder;
    [SerializeField] private List<TextMeshPro> Murder_Weapons;
    [SerializeField] private List<TextMeshPro> Murder_Locations;


    void Update()
    {
        if (Text_Seperator.hasProcessed)
        {
            TMP_Assignment();
        }
    }

    private void TMP_Assignment()
    {
        //! FİX
        Witness_Scenario.text = Text_Seperator.Witness_Scenario;

        for (int i = 0; i < 3; i++)
        {
            Motives_For_Murder[i].text = Text_Seperator.Motives_For_Murder[i];
            Murder_Weapons[i].text = Text_Seperator.Murder_Weapons[i];
            Murder_Locations[i].text = Text_Seperator.Murder_Locations[i];
        }   
    }
}
