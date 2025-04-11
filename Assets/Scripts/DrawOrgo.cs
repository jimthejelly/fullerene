using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DrawOrgo : MonoBehaviour
{
    private int difficulty = 0; // 0 is easy, 1 is medium, 2 is hard
    private string[] basePrefixes = {"benz", "meth", "eth", "prop", "but", "pent", "hex", "hept", "oct", "non", "dec", "undec", "duodec"}; // 0 is benzene
    private string[] countPrefixes = {"di", "tri", "tetra", "penta", "hexa"}; // I'm not sure how many of these I'll need but they're here
    private string[] saturationTypes = { "an", "en", "yn" }; // for single/double/triple bonds respectively
    private string[] suffixes = {"oic acid", "oate", "amide", "enitrile", "al", "one", "ol", "amine", "ether", "e"}; // Suffixes for different groups ("e" is for alkane/en/ynes)
    private string[] functionalPrefixes = {"halides", "alkanes", "hydroxy", "oxy"};
    private string[] halides = {"fluoro", "chloro", "bromo", "iodo"}; // Prefixes for halides
    private string[] alkanePrefixes = {"phenyl", "methyl", "ethyl", "propyl", "butyl", "pentyl", "hexyl", "heptyl", "octyl", "nonyl", "decyl", "undecyl", "duodecyl"};
    public string currentMolecule = "UNDEFINED";

    // Start is called before the first frame update
    void Start()
    {
        generateRandomMolecule();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generateRandomMolecule() {
        System.Random rand = new System.Random();
        if(difficulty == 0) {
            // generate base number (to be used to determine methane/ethane...)
            int baseNumber = rand.Next(1, 13);
            int functionalGroupLocation = rand.Next(baseNumber) + 1;
            int functionalGroupType = rand.Next(10);
            int saturationType = 0;
            if(functionalGroupType == 9) {
                saturationType = rand.Next(3);
                // alkanes don't need a location
                if(saturationType == 0) {
                    currentMolecule = basePrefixes[baseNumber] + saturationTypes[saturationType] + suffixes[functionalGroupType];
                    Debug.Log("generated: " + currentMolecule);
                    return;
                }
            }
            // carboxylic acids don't need a location
            if(functionalGroupType == 0) {
                currentMolecule = basePrefixes[baseNumber] + saturationTypes[saturationType] + suffixes[functionalGroupType];
                Debug.Log("generated: " + currentMolecule);
                return;
            }
            currentMolecule = functionalGroupLocation + "-" + basePrefixes[baseNumber] + saturationTypes[saturationType] + suffixes[functionalGroupType];
        }
        else {
            // generate base number and normalize the length of benzene
            int baseNumber = rand.Next(13);
            int length = baseNumber;
            if(length == 0) {
                length = 6;
            }
            int[][] functionalGroups = new int[length][];
        }

        Debug.Log("generated: " + currentMolecule);
    }
}
