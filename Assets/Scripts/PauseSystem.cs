using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    bool Paused;
    // Start is called before the first frame update
    void Start()
    {
        Paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Paused = !Paused;
        }
    }
}
