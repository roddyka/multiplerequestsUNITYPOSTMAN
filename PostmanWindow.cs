using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class PostmanWindow : EditorWindow
{
    string url = "";
    int selectedMethodIndex = 0;
    string[] httpMethods = { "GET", "POST", "PUT", "DELETE" };
    string headers = "";

    [MenuItem("Window/MultipleRequests")]
    public static void ShowWindow()
    {
        GetWindow<PostmanWindow>("MultipleRequests");
    }

    void OnGUI()
    {
        GUILayout.Label("MultipleRequests", EditorStyles.boldLabel);

        url = EditorGUILayout.TextField("URL", url);
        selectedMethodIndex = EditorGUILayout.Popup("Method", selectedMethodIndex, httpMethods);
        headers = EditorGUILayout.TextField("Headers", headers);

        if (GUILayout.Button("Send Request"))
        {
            SendRequest();
        }
    }

    async void SendRequest()
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("URL is required");
            return;
        }

        string method = httpMethods[selectedMethodIndex];

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.method = method;

            if (!string.IsNullOrEmpty(headers))
            {
                string[] headerLines = headers.Split('\n');
                foreach (var line in headerLines)
                {
                    string[] header = line.Trim().Split(':');
                    if (header.Length == 2)
                    {
                        request.SetRequestHeader(header[0], header[1]);
                    }
                    else
                    {
                        Debug.LogWarning("Invalid header format: " + line);
                    }
                }
            }

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (!request.isNetworkError && !request.isHttpError)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }
    }
}
