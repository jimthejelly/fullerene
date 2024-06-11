using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class end : MonoBehaviour
{
    public GameObject pause_menu;
    public bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        isPaused = false;
        pause_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Load();
        }
    }

    public void Load()
    {
        if (!isPaused) {
            Time.timeScale = 0;
            pause_menu.SetActive(true);
            isPaused = true;
        } else {
            Time.timeScale = 1;
            pause_menu.SetActive(false);
            isPaused = false;
        }
        
    }

    public void Return()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
