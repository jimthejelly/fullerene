using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class creationMenu : MonoBehaviour
{
    public GameObject pause_menu;
    public GameObject table_menu;
    public bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        // Pauses time and brings up menu
        Time.timeScale = 1;
        isPaused = false;
        pause_menu.SetActive(false);
        table_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !table_menu.activeSelf) {
            LoadPauseMenu();
        } else if (Input.GetKeyDown(KeyCode.Tab) && !pause_menu.activeSelf) {
           LoadTableMenu();
        }
    }

    public void LoadPauseMenu()
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

    public void LoadTableMenu()
    {
        if (!table_menu.activeSelf) {
            table_menu.SetActive(true);
        } else {
            table_menu.SetActive(false);
        }
        
    }

    public void LoadExplorationMenu()
    {
        SceneManager.LoadScene("ExplorationScene", LoadSceneMode.Single);
    }

    public void Return()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
