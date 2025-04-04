using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DrawOrgo : MonoBehaviour
{
    private string[] basePrefixes = {"benz", "meth", "eth", "prop", "but", "pent", "hex", "hept", "oct", "non", "dec", "undec", "duodec"}; // 0 is benzene
    private string[] countPrefixes = {"di", "tri", "tetra", "penta", "hexa"}; // I'm not sure how many of these I'll need but they're here
    private string[] suffixes = {"oic acid", "oic anhydride", "oate", "amide", "enitrile", "al", "one", "ol", "amine", "e"}; // Suffixes for different groups ("e" is for alkane/en/ynes)
    private string[] functionalPrefixes = {"halides", "alkanes", "hydroxy", "oxy"};
    private string[] halides = {"fluoro", "chloro", "bromo", "iodo"}; // Prefixes for halides
    private string[] alkanePrefixes = {"phenyl", "methyl", "ethyl", "propyl", "butyl", "pentyl", "hexyl", "heptyl", "octyl", "nonyl", "decyl", "undecyl", "duodecyl"};
    public string currentMolecule;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void generateRandomMolecule() {
        // generate base
        System.Random rand = System.Random();
        int base = rand.Next(13);

        // generate functional groups
        int temp = base;
        if(temp == 0) {
            temp = 6;
        }
        int[][] functionalGroups = new int[temp][];

    }
}
