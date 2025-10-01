using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Google Gemini API'den kullanılabilir modellerin listesini çeker.
/// </summary>
public class Gemini_Model_List : MonoBehaviour
{
    [Tooltip("Google AI Studio'dan aldığınız Gemini API Key'inizi buraya girin.")]
    public string Gemini_API_Key = "YOUR_GEMINI_API_KEY_HERE";

    [Header("Test Ayarları")]
    [Tooltip("Liste işlemine başlamak için hangi tuşa basılacağını belirtir.")]
    public KeyCode triggerKey = KeyCode.X;

    void Update()
    {
        // Belirtilen tuşa basıldığında listelemeyi başlat
        if (Input.GetKeyDown(triggerKey))
        {
            Debug.Log($"'{triggerKey}' tuşuna basıldı. Gemini API modelleri listeleniyor...");
            StartCoroutine(ListGeminiModels());
        }
    }

// --------------------------------------------------------------------------------------
    
    IEnumerator ListGeminiModels()
    {
        // 1. Anahtar Kontrolü
        if (string.IsNullOrEmpty(Gemini_API_Key) || Gemini_API_Key == "YOUR_GEMINI_API_KEY_HERE")
        {
            Debug.LogError("Hata: Lütfen 'Gemini_API_Key' alanına geçerli bir anahtar girin.");
            yield break; // Coroutine'i sonlandır
        }

        string apiUrl = $"https://generativelanguage.googleapis.com/v1/models?key={Gemini_API_Key}";

        // UnityWebRequest, varsayılan olarak GET isteği için uygun şekilde başlatılır, 
        // ancak açıkça belirtmek daha temizdir.
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                // Hata durumunda, hem genel hatayı hem de yanıt gövdesini logla
                Debug.LogError("Gemini Model Listeleme Hatası: " + request.error);
                Debug.LogError("Hata Detayı (JSON Yanıtı): " + request.downloadHandler.text);
            }
            else
            {
                // Başarılı yanıt
                string modelJson = request.downloadHandler.text;
                Debug.Log("Gemini API'dan Gelen Model Listesi:\n" + modelJson);
                
                // İpucu: Bu JSON'ı ayrıştırarak sadece model adlarını (name) veya
                // desteklenen metotları (supportedGenerationMethods) listeleyebilirsiniz.
            }
        } // 'using' bloğu ile UnityWebRequest nesnesi otomatik olarak temizlenir.
    }
}