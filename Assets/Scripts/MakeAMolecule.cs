using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class makeAMolecule : MonoBehaviour
{
    public TMP_Text myText;
    // Start is called before the first frame update
    void Start()
    {
        String line = grabMolecule("Assets/Resources/MakeAMolecule.txt"); 
        myText.text = line;
        // myText.text = "Hello, World!";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private String grabMolecule(String filename) {
        System.Random gen = new System.Random();
        int num = gen.Next(0, 100);
        String line = "test";
        var openFile = new System.IO.StreamReader(filename);
        for (int i = 0; i < num; i++)
        {
            line = openFile.ReadLine();
            if (line == null) 
                break; // there are less than 15 lines in the file
        }
        return line;
    }
}
