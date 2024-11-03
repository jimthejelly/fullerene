using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class creationMenu : MonoBehaviour
{
    public GameObject pause_menu;
    public GameObject table_menu;
    public GameObject preset_menu;
    public GameObject general_info;

    public GameObject preset_dropdown;
    public bool isPaused;
    // Start is called before the first frame update
    
    public bool simplified = false;    //simplified version

    public static bool molMini = false; //toggle for makeAMolecule, will probably end up making a separate menu for different mini games

    public bool orthographic = false;
    public GameObject ortho;
    public GameObject persp;

    void Start()
    {
        // Pauses time and brings up menu
        Time.timeScale = 1;
        isPaused = false;
        pause_menu.SetActive(false);
        table_menu.SetActive(false);
        preset_menu.SetActive(false);

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

    public void PressSimplified()
    {
        GameObject molecule = GameObject.Find("moleculeBody");
        if(!simplified) {    // now simplified
            simplified = false;

        } else {            // now not simplified

            simplified = true;
        }
    }

    public void makeAMolecule() {
        GameObject molecule = GameObject.Find("moleculeBody");
        if(!molMini) {
            molMini = false;
        } else {
            molMini = true;
        }
    }

    public void LoadPauseMenu()
    {
        if (!isPaused) {
            Time.timeScale = 0;
            pause_menu.SetActive(true);

            ortho.SetActive(orthographic);
            persp.SetActive(!orthographic);

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
            preset_menu.SetActive(true);
            general_info.SetActive(false);
        } else {
            table_menu.SetActive(false);
            preset_menu.SetActive(false);
            general_info.SetActive(true);
        }
        
    }
    public void LoadDropdownMenu()
    {
         if (!preset_dropdown.activeSelf) {
            preset_dropdown.SetActive(true);
        } else {
            preset_dropdown.SetActive(false);
        }
    }

    public void LoadExplorationMenu()
    {
        SceneManager.LoadScene("ExplorationScene", LoadSceneMode.Single);
    }

    public void CameraToggleText() {
        orthographic = !orthographic;

        ortho.SetActive(orthographic);
        persp.SetActive(!orthographic);
    }

    public void Return()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
