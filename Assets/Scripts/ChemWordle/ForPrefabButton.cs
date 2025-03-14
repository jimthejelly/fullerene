using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForPrefabButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoSomethingCool()
    {
        GameObject thing = GameObject.Find("WordleManager");
        Debug.Log(thing);
    }

}
