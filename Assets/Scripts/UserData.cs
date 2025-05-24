using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

using System.Text;
using UnityEngine.Networking;


public class UserData
{
    public string username;
    public string password;
	
    public string Stringify()
    {
        return JsonUtility.ToJson(this);
    }

    public static UserData Parse(string json)
    {
        return JsonUtility.FromJson<UserData>(json);
    }


    public IEnumerator FetchUserData(string id, System.Action<UserData> callback = null)
    {
        Debug.Log("Made it");
        
        using (UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/fullerene/"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result ==UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                if (callback != null)
                {
                    callback.Invoke(null);
                }
            }
            else
            {
                Debug.Log("got info back");

                //UnityEngine.Networking.UnityWebRequest is the thing i need a JSON from.
                Debug.Log(request.downloadHandler.text);
                Debug.Log(request.downloadHandler.text.GetType());
                
                

                if (callback != null)
                {
                    //callback.Invoke(Debug.Log(request.result));
                    callback.Invoke(UserData.Parse(request.downloadHandler.text));
                }
            }
        }


        /*

        using (UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/fullerene/" + id))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result ==UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                if (callback != null)
                {
                    callback.Invoke(null);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback.Invoke(UserData.Parse(request.downloadHandler.text));
                }
            }
        }
        */
    }

    public IEnumerator SaveUserData(string profile, System.Action<bool> callback = null)
    {
        
        using (UnityWebRequest request = new UnityWebRequest("http://localhost:3000/fullerene", "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(profile);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            }
            else
            {
                if (callback != null)
                {
                    callback.Invoke(request.downloadHandler.text != "{}");
                }
            }
        }
        
    }

}
