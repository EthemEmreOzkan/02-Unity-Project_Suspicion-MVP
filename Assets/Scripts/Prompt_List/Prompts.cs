using UnityEngine;

// Sadece veri sınıfı, MonoBehaviour DEĞİL
[System.Serializable]
public class Prompts
{
    [TextArea(3, 10)]
    public string text;
}
