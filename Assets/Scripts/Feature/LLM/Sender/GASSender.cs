using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GASSender : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private string deployId;
    [SerializeField, TextArea] private string startingPromptText;

    [Header("Output")]
    [SerializeField] private TMP_Text responseText;

    public IEnumerator CallGoogleAppsScript(IRequestResponse prompter, string prompt)
    {
        string url = $"https://script.google.com/macros/s/{deployId}/exec?prompt=" + UnityWebRequest.EscapeURL(startingPromptText + '\n' + prompt);
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            if(responseText) responseText.text = "ERROR: " + request.downloadHandler.text;
            Debug.LogError(request.downloadHandler.text);
            prompter.Receive(null);
        }
        else
        {
            Dictionary<string, string> response = null;
            try
            {
                response = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);

                //Debug.Log("Response: " + response["answer"]);

                if (responseText) responseText.text = response["answer"];

                prompter.Receive(response["answer"]);
            }
            catch(System.Exception ex)
            {
                Debug.LogWarning("ERROR: Failed to receive Response (" + ex.Message + ")");
                prompter.Receive(null);
            }
        }
    }

}
