using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Collections;
using System.Text;

public class Gemini_Api_Handler : MonoBehaviour
{
    //*-----------------------------------------------------------------------------------------//

    #region Inspector Tab ------------------------------------------------------------------------

    [Header("Gemini Ayarları -----------------------------------------------------------")]
    [Space]
    [SerializeField] private string model = "gemini-2.5-flash";
    [SerializeField] private string apiKey = "YOUR_GEMINI_API_KEY_HERE";

    [Header("Yanıt Alındığında")]
    [Space]
    [Tooltip("API'den yanıt geldiğinde tetiklenir ve yanıt metnini string olarak iletir.")]
    public UnityEvent<string> On_Response_Received;

    #endregion
   
    //*-----------------------------------------------------------------------------------------//

    #region Send_Prompt Public Func -------------------------------------------------------------

    public void Send_Prompt(string prompt)
    {
        // API anahtarının ayarlanıp ayarlanmadığını kontrol edin
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
        {
            string error = "Hata: Lütfen Gemini API Anahtarınızı atayın!";
            Debug.LogError(error);
            On_Response_Received?.Invoke(error);
            return;
        }

        StartCoroutine(Send_Prompt_To_Gemini(prompt));
    }

    #endregion
   
    //*-----------------------------------------------------------------------------------------//

    #region Private Funcs ------------------------------------------------------------------------

    private IEnumerator Send_Prompt_To_Gemini(string prompt)
    {
        // API URL'si
        string url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

        // Gönderilecek JSON gövdesi (EscapeJson kullanılarak prompt güvenli hale getirildi)
        string jsonBody = "{\"contents\": [{\"parts\": [{\"text\": \"" + EscapeJson(prompt) + "\"}]}]}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log($"Gemini'ye istek gönderiliyor ({model})...");

        // İsteği gönder ve yanıtı bekle
        yield return request.SendWebRequest();

        string result;

        if (request.result != UnityWebRequest.Result.Success)
        {
            // Hata durumunda
            result = $"API Hatası: {request.error}";
            Debug.LogError("Gemini API Hatası: " + request.error + "\nYanıt: " + request.downloadHandler.text);
        }
        else
        {
            // Başarılı yanıt durumunda
            string json = request.downloadHandler.text;
            result = ExtractTextFromGeminiResponse(json);
            Debug.Log("Gemini Yanıtı:\n" + result);
        }

        // Sonucu abone olan tüm yöntemlere ilet
        On_Response_Received?.Invoke(result);
    }

    //*-----------------------------------------------------------------------------------------//

    private string EscapeJson(string s)
    {
        // Ters eğik çizgi, çift tırnak, yeni satır ve satır başı karakterlerini kaçır
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");
    }

    //*-----------------------------------------------------------------------------------------//

    private string ExtractTextFromGeminiResponse(string json)
    {
        // En basit yöntemle JSON'dan "text" alanını bulmaya çalışır.
        // Daha güvenilir bir çözüm için bir JSON ayrıştırıcı kütüphanesi (Newtonsoft.Json gibi) kullanılmalıdır.
        const string marker = "\"text\": \"";
        int start = json.IndexOf(marker);

        if (start == -1)
        {
            // API'dan yanıt alınmış ancak metin içeriği yok. Muhtemelen güvenlik ayarları engelledi.
            return $"Yanıt çözülemedi veya filtrelendi. Tam JSON: {json}";
        }

        start += marker.Length;

        // Metin içeriğinin bitimindeki tırnak işaretini bul
        int end = json.IndexOf("\"", start);

        if (end == -1) return "Yanıt çözülemedi.";

        string result = json.Substring(start, end - start);

        // Kaçırılmış \n karakterlerini gerçek yeni satır karakterlerine çevir
        return result.Replace("\\n", "\n");
    }
   
    #endregion
    //*-----------------------------------------------------------------------------------------//
}