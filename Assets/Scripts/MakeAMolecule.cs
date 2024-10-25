using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using System;
public class MakeAMolecule : MonoBehaviour {
    public TextMeshProUGUI moleculeText;

    void start() {
        String line = grabMolecule("Assets/Resources/MakeAMolecule.txt"); 
        moleculeText.text = line;
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