using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

using System.Text;
using UnityEngine.Networking;
using System.Threading.Tasks;
using  Newtonsoft.Json.Linq;

[System.Serializable]
public class UserData
{
    public string _id ;
    public string username ;
    public string password ;
    public string email;
    public bool verified;
    public int verificationCode;

    private string destination= "192.168.61.213";
    //public DateTime lastLogin;
    //public Datetime firstLogin;
    //TODO: teacher toggle
    //public bool teacher;
	
    ///turns the current object into string form, used for further conversion to byte[] for web requests
    public string Stringify()
    {
        return JsonUtility.ToJson(this);
    }

    ///Returns a UserData Object from what should be a JSON string
    public static UserData Parse(string json)
    {
        //TODO - try catch for safety
        return JsonUtility.FromJson<UserData>(json);
    }

    
    ///turns JSON string into UserData
    public UserData(string json){
        Parse(json);        
    }

    ///Empty Constructor - should never be used. Always use the JSON string parse method
    public UserData(){

    }

    public IEnumerator ping(){

        Debug.Log("pingin0");
        using (UnityWebRequest request = UnityWebRequest.Get($"http://{destination}:3000/t"))
        {
            Debug.Log("pingin");
            var what= request.SendWebRequest();
            
            //i have it idle 
            while(request.result == UnityWebRequest.Result.InProgress){
                //Debug.Log("inp");
            }
            

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result ==UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                
            }
            else
            {

                //UnityEngine.Networking.UnityWebRequest is the thing i need a JSON from.
                Debug.Log(request.downloadHandler.text);                     

                
            }
            
            yield return what;
        }
    }

    ///Logins an account
    public IEnumerator FetchUserData(string id, System.Action<UserData> callback = null)
    {        
        //this is the request right here officer
        //const hostname = "192.168.0.183";
        using (UnityWebRequest request = UnityWebRequest.Get($"http://{destination}:3000/fullerene/"+id))
        {
            Debug.Log("test fetch");
            var what= request.SendWebRequest();
            
            //i have it idle 
            while(request.result == UnityWebRequest.Result.InProgress){
                //Debug.Log("inp");
            }
            

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

                //UnityEngine.Networking.UnityWebRequest is the thing i need a JSON from.
                Debug.Log(request.downloadHandler.text);                     

                if (callback != null)
                {
                    callback.Invoke(UserData.Parse(request.downloadHandler.text));   
                }
            }
            
            yield return what;
        }
    }

    ///Web request to make an account
    public IEnumerator MakeUserData(string user,string pass, string email, System.Action<bool> callback = null)
    {
        
        using (UnityWebRequest request = new UnityWebRequest($"http://{destination}:3000/fullerene", "POST"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            
            
            string jason = "{\"username\":\""+user+"\",\"password\":\""+pass+"\",\"email\":\""+email+"\" }";
            Debug.Log(jason);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jason);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            //download handler is how ill get the data back/
            //but its either byte data or utf8, i think ill take byte and reconstruct?
            request.downloadHandler = new DownloadHandlerBuffer();
            
            var temp = request.SendWebRequest();
            
            //stop until the reuqest is terminated
            while(request.result == UnityWebRequest.Result.InProgress){
                //Debug.Log("inp");
            }

            byte[] inputRaw=request.downloadHandler.data;
            string yourText = System.Text.Encoding.UTF8.GetString(inputRaw);
            JObject sentJson= JObject.Parse(yourText);

            //have for error checking, will be made robust
            //var test= sentJson.Property("reason").Value;


            
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                //Debug.Log("Other type of error");
                //Debug.Log(request.error);
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            }
            else if (request.result == UnityWebRequest.Result.DataProcessingError){
                //Debug.Log("was another person w/ that name");
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            }
            else
            {
                //this is also what gets called in case 2...
                Debug.Log("Seems to have worked out");
                if (callback != null)
                {
                    callback.Invoke(request.downloadHandler.text != "{}");
                }
            }
            yield return temp;
        }
        
    }

    ///Web request to modify an account - only verification
    public IEnumerator VerifyUserData(string user, int code, System.Action<bool> callback=null){
        using (UnityWebRequest request = new UnityWebRequest($"http://{destination}:3000/fullerene", "PUT"))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            
            
            string jason = "{\"username\":\""+user+"\",\"code\":"+code+"}";
            Debug.Log("decoding+"+jason);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jason);
            
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            
            //download handler is how ill get the data back/
            //but its either byte data or utf8, i think ill take byte and reconstruct?
            request.downloadHandler = new DownloadHandlerBuffer();
            
            var temp = request.SendWebRequest();
            
            //stop until the reuqest is terminated
            while(request.result == UnityWebRequest.Result.InProgress){
                //Debug.Log("inp");
            }
            
            byte[] inputRaw=request.downloadHandler.data;
            
            string yourText = System.Text.Encoding.UTF8.GetString(inputRaw);
            
            JObject sentJson= JObject.Parse(yourText);
            
//            var test= sentJson.Property("reason").Value;



            //boilerplate error checking
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Other type of error");
                Debug.Log(request.error);
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            }
            else if (request.result == UnityWebRequest.Result.DataProcessingError){
                Debug.Log("was another person w/ that name");
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            }
            else
            {
                if(sentJson.ContainsKey("success")   &&  sentJson.Property("success").Value.ToObject<bool>() ==false){
                    Debug.Log("validation failed");
                    Debug.Log("err: "+sentJson.Property("reason").Value);
                    callback.Invoke(false);
                }
                else{
                    Debug.Log("successfully validated!");
                    if (callback != null)
                    {
                        callback.Invoke(true);
                    }
                }
                //this is also what gets called in case 2...
                
            }
            yield return temp;



        }
    }
}
