using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;


public class start : MonoBehaviour
{
    public void Load()
    {
        _userData = new UserData();        
        SceneManager.LoadScene("CreationScene", LoadSceneMode.Single);
    }

    public void Kill() {        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    private UserData _userData;

    public void Login()
    {
        _userData=new UserData();
        Debug.Log("Hit Login");  
        StartCoroutine(_userData.FetchUserData("sebastien",result => {Debug.Log(result); }));      
    }

    

}


