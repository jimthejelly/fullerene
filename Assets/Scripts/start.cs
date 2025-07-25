using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using TMPro;
using System; //TODO: we dont need to import ALL OF SYSTEM


public class start : MonoBehaviour
{


    //parts of the start screen that are referenced
    [SerializeField] GameObject LoginMenu;
    [SerializeField] GameObject RegisterMenu;
    [SerializeField] GameObject EmailPanel;
    [SerializeField] GameObject VerifyPanel;
    [SerializeField] TMP_InputField UsernameField;
    [SerializeField] TMP_InputField PasswordField;
    [SerializeField] TMP_InputField VerifyField;
    [SerializeField] TMP_InputField EmailField;
    [SerializeField] TMP_Text User;


    //User Data stored in this object
    public static UserData _userData=new UserData();    

    bool Pause=false;

    ///Starts the screen
    public void Load()
    {      
        SceneManager.LoadScene("CreationScene", LoadSceneMode.Single);
    }

    ///ends the screen
    public void Kill() {        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }



    //TODO - add input sanitization

    ///Open or close the Menu built for logging in
    public void LoginMenuToggle(){

        Pause = !Pause;
        if (Pause)
        {
            LoginMenu.SetActive(true);
        } else
        {
            LoginMenu.SetActive(false);
        }
    }

    ///Open or close the Menu built for registering an account
    public void RegisterMenuToggle(){
         Pause = !Pause;
        if (Pause)
        {
            RegisterMenu.SetActive(true);
        } else
        {
            RegisterMenu.SetActive(false);
        }
    }


    ///Sends a request from the start screen to the user to make a 
    public void Login()
    {                
        
        //has to be in a CoRoutine, as it is a webrequest 
        StartCoroutine(_userData.FetchUserData(UsernameField.text,result => {
            if(result!=null){
                _userData=result;
                Debug.Log(result);
                User.SetText(_userData.username); 
            }
            else{
                Debug.Log("failed login");
            }
            
        }) );   


        //if goes righgt, TODO to be part of the callback
       // Cancel();
    }


    
    /// Closes all Menus
    public void Cancel(){    
        StartCoroutine(_userData.ping());
        Pause=false;
        LoginMenu.SetActive(false);
        RegisterMenu.SetActive(false);
        
        Debug.Log("cancel");
     
    }

    ///Sends a request to the userdata to get the database to create an account
    public void Register(){

        //first validate for safety (or enough safety)
        var st= Time.realtimeSinceStartup;
        
        
        //print("trying with username: "+UsernameField.text);
        //print("and: "+PasswordField.text);
        
        print("and email: "+EmailField.text+"<>");
        /*
        for (int i=0; i<1; i++){
            StartCoroutine(_userData.MakeUserData(""+i,""+i,"-",result => {Debug.Log("result"+result); } ) );    
        }
        */
        StartCoroutine(_userData.MakeUserData(UsernameField.text,PasswordField.text,EmailField.text,result => {Debug.Log("result"+result); } ) );
        var et= Time.realtimeSinceStartup;
        var diff=et-st;
        Debug.Log("time el:"+diff);
        

        //TODO =if goes righgt as callback
        //Cancel();
        VerifyPanel.SetActive(true);
        EmailPanel.SetActive(false);
      
    }

    

    ///what happens when the verify button is pressed, checks if the textfield properly is just numbers, and if so, asks the UserData to make a web request, potentially signing in if correct
    public void Verify(){

        Debug.Log("tryin to verify");
        
        try{
            int result = Int32.Parse(VerifyField.text);
            StartCoroutine(_userData.VerifyUserData(UsernameField.text,result,result => { 
                if(result){
                    Login();
                   
                }//success validated
                else{

                }//not success
            }));  
        }
        catch(FormatException){
            Debug.Log($"Unable to parse '{VerifyField.text}'");
            
            //VerifyPanel.SetActive(true);
            //EmailPanel.SetActive(false);
        }
        
    }


    ///For testing purposes, to see if the user has successfully logged in
    public void Test(){
        Debug.Log(_userData.password);
        Debug.Log(_userData.username);
        Debug.Log(_userData.Stringify());
        
    }


/*
    private void Update(){
    }
*/


    //current concept to make values safe -
    //any special character in JSON language, { } , : " " ; . [ ] are all illegal 

    private string JSONStringTest(string input){
        string wrongs="";
        int count=0;
        string[] bads = {"{", "}", ",", ":", "\"", ";", "[", "]"};

        foreach (string bad in bads)
        {
            if(input.IndexOf(bad)!=-1) {wrongs+=bad ;}    
        }

        return wrongs;
    }

}


