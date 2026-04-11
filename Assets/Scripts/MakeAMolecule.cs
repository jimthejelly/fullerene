using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

public class makeAMolecule : MonoBehaviour
{
    public TMP_Text myText;
    private Scene scene;

    private String formula;

    private int correctMols;

    private int currMol;

    private GameObject currentMol;

    private Elements currMolRep;

    // private int[] usedMols = new int[100]; meant to store which molecules have been prompted before, but maybe unnecessary?

    // Start is called before the first frame update
    void Start()
    {
        myText.color = Color.black;
        String fullLine = GrabMolecule("Assets/Resources/MakeAMolecule.txt");
        int index = fullLine.IndexOf(':');
        String displayLine = fullLine.Substring(0, index);
        formula = fullLine.Substring(index, fullLine.Length - index);
        correctMols = 0;

        myText.text = displayLine;
        CreateCheckButton();
        NewMol();
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
        currentMol = molecule;
        if(creationMenu.molMini == true) {
            myText.color = Color.white;
        } else {
            // myText.color = Color.black;
        }
    }
    
    private String GrabMolecule(String filename) {
        System.Random gen = new System.Random();
        currMol = gen.Next(0, 100);
        String line = "";
        var openFile = new System.IO.StreamReader(filename);
        for (int i = 0; i < currMol; i++)
        {
            line = openFile.ReadLine();
            if (line == null)
                break; // there are less than 15 lines in the file
        }
        return line;
    }

    private void NewMol()
    {
        // String fullLine = GrabMolecule("Assets/Resources/MakeAMolecule.txt");
        // int index = fullLine.IndexOf(':');
        // String displayLine = fullLine.Substring(0, index);
        // formula = fullLine.Substring(index, fullLine.Length - index);
        // currMolRep = BuildMol(formula);

        // myText.text = displayLine;
        
        FileInfo file = new("./Assets/Resources/MoleculeTemplates/Carbon Dioxide.cml");

        GameObject molecule = MoleculeFileIO.LoadMolecule(file);
        
        FileInfo file2 = new("./Assets/TEST.cml");
        MoleculeFileIO.SaveMolecule(file2, molecule);


        currMolRep = molecule.GetComponent<Elements>();

    }

    void OnGUI()
    {
        Handles.BeginGUI();
        if (GUILayout.Button("Check"))
        {
            if (MolIsCorrect(currentMol)) // cannot currently return true;
            {
                //ShowCorrect();
                NewMol();
                correctMols += 1;
            }
        }
        Handles.EndGUI();
    }

    // creates a button to check if the molecule is correct(called at Start())
    private void CreateCheckButton()
    {
        
    }

    // Changes the screen to show that what was created was correct
    private void ShowCorrect(){
        // Todo
        
    }

    // Helper function that tries all starting points
    private bool MolIsCorrect(GameObject molecule){
        // Todo
        Debug.Log("entered method!");
        Elements[] elements = molecule.GetComponents<Elements>();
        HashSet<Elements> empty = new HashSet<Elements>();
        foreach (Elements i in elements)
        {
            if (ChildrenAreCorrect(i, currMolRep,empty))
            {
                Debug.Log("true");
                return true;
            }
        }
        Debug.Log("false");
        return false;
        
    }
    // Loops through the children from the root
    private bool ChildrenAreCorrect(Elements root0, Elements root1, HashSet<Elements> alreadyChecked){

        ElementsComparer EC = new ElementsComparer();
        if (!EC.Equals(root0, root1) || root0.GetComponents<Elements>().Length != root1.GetComponents<Elements>().Length) return false;

        HashSet<Elements> bAlreadyChecked = new HashSet<Elements>();
        foreach (Elements root in root0.GetComponents<Elements>())
        {
            bool couldMatch = false;
            foreach (Elements root2 in root0.GetComponents<Elements>())
            {
                if (!ElementSetContains(bAlreadyChecked, root2)) continue;
                alreadyChecked.Add(root);
                if (ChildrenAreCorrect(root, root2, alreadyChecked))
                {
                    bAlreadyChecked.Add(root2);
                    couldMatch = true;
                    break;
                }
            }
            if (!couldMatch) return false;
        }
        return true;
    }

    private bool ElementSetContains(HashSet<Elements> set, Elements element)
    {
        ElementsComparer EC = new ElementsComparer();
        bool allEqual = false;
        foreach (Elements x in set)
        { 
            if (EC.Equals(x, element) == true) {
                Debug.Log("any true");
                allEqual = true;
            }
            
        } 
        return allEqual;
    }
}
