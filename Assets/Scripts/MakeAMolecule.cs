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
    private Scene scene;
    // Start is called before the first frame update
    void Start()
    {
        myText.color = Color.black;
        String fullLine = grabMolecule("Assets/Resources/MakeAMolecule.txt"); 
        int index = fullLine.IndexOf(':');
        String displayLine = fullLine.Substring(0, index);
        String formula = fullLine.Substring(index, fullLine.Length - index);
        myText.text = displayLine;
    }

    // Update is called once per frame
    void Update()
    {
        scene = SceneManager.GetActiveScene();
        if(scene.name == "MakeAMolecule") {
            myText.color = Color.white;
        } else {
            myText.color = Color.black;
        }

        GameObject molecule = GameObject.Find("moleculeBody");
        if(creationMenu.molMini == true) {
            myText.color = Color.white;
        } else {
            //myText.color = Color.black;
        }
    }

    private String grabMolecule(String filename) {
        System.Random gen = new System.Random();
        int num = gen.Next(0, 100);
        String line = "";
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
