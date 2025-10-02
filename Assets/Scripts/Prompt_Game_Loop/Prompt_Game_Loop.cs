using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Prompt_Game_Loop : MonoBehaviour
{
    //*-----------------------------------------------------------------------------------------//
    #region Inspector Tab

    [Header("References -------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Gemini_Api_Handler Gemini_Api_Handler;
    [SerializeField] private Prompt_List Prompt_List;
    [Space]
    [Header("Debug / Private --------------------------------------------------------------------")]
    [Space]
    [SerializeField] private string Prompt_To_String;

    #endregion
    //*-----------------------------------------------------------------------------------------//
    #region Unity Lifecycle

    void Awake()
    {
        if (Gemini_Api_Handler != null && Prompt_List != null && Prompt_List.Prompts.Count > 0)
        {
            Prompt_To_String = Prompt_List.Prompts[0].text;
            Gemini_Api_Handler.Send_Prompt(Prompt_To_String);
        }
        else
        {
            Debug.LogWarning("Prompt listesi boş veya referans atanmamış!");
        }
    }

    void Update()
    {
        //** Her Prompt_List'den prompt api call tamamlandığında Prompt_To_String Temizle
        if (Gemini_Api_Handler.Is_Response_Received && !Gemini_Api_Handler.Is_Request_In_Progress)
        {
            Prompt_To_String = "";
        }
    }

    #endregion
    //*-----------------------------------------------------------------------------------------//
}
