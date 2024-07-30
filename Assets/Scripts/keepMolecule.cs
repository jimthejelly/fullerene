using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keepMolecule : MonoBehaviour
{
    public GameObject moleculeBody;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(moleculeBody);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
