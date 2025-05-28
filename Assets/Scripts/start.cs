using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;


public class start : MonoBehaviour
{
    public void Load()
    {      
        SceneManager.LoadScene("CreationScene", LoadSceneMode.Single);
    }

    public void Kill() {        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public static UserData _userData=new UserData();    

    public void Login()
    {
                
        //has to be in a CoRoutine, dont ask why, no call gets made otherwise
        
        
        StartCoroutine(_userData.FetchUserData("sebastien",result => {_userData=result; }));   
        
    }

    public void Test(){
        Debug.Log(_userData.password);
        Debug.Log(_userData.username);
        Debug.Log(_userData.Stringify());
        
    }
    

}


