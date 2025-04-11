using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameSelectionButtons : MonoBehaviour
{
    public GameObject previousMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // NOTE: This currently breaks Andrew's code a little, isPaused remains on when exiting the menu so it takes an extra click to do stuff
    public void BackButton() {
        previousMenu.SetActive(true);  
        gameObject.SetActive(false);
    }

    public void LoadMakeAMolecule() {
        SceneManager.LoadScene("MakeAMolecule", LoadSceneMode.Single);
    }

    public void LoadShootAMolecule()
    {
        SceneManager.LoadScene("ShootAMolecule", LoadSceneMode.Single);
    }
}
