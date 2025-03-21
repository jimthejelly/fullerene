using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Xml.Linq;

public class makeAMolecule : MonoBehaviour
{
    public TMP_Text myText;
    private Scene scene;

    private String formula;

    private int correctMols;

    private int currMol;

    private Elements currMolRep;

    // private int[] usedMols = new int[100];

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
            // myText.color = Color.black;
        }

        //if(MolIsCorrect(molecule)){
        //    ShowCorrect();
        //    NewMol();
        //    correctMols += 1;
        //}
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
        String fullLine = GrabMolecule("Assets/Resources/MakeAMolecule.txt");
        int index = fullLine.IndexOf(':');
        String displayLine = fullLine.Substring(0, index);
        formula = fullLine.Substring(index, fullLine.Length - index);
        currMolRep = BuildMol(formula);

        myText.text = displayLine;
    }

    // Changes the screen to show that what was created was correct
    private void ShowCorrect(){
        // Todo
        
    }

    private Elements BuildMol(String molName)
    {
        Elements root = new Elements();

        return root;
    }

    // Helper function that tries all starting points
    private bool MolIsCorrect(GameObject molecule){
        // Todo
        Elements[] elements = molecule.GetComponents<Elements>();
        HashSet<Elements> empty = new HashSet<Elements>();
        foreach (Elements i in elements)
        {
            if (ChildrenAreCorrect(i, currMolRep,empty))
            {
                return true;
            }
        }
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
                allEqual = true;
            }
            
        } 
        return allEqual;
    }
}
