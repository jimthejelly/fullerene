using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{
    public void Load()
    {
        SceneManager.LoadScene("CreationScene", LoadSceneMode.Single);
    }
}


