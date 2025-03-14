using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Represents all the information associated with one chemical in the PubChem database. */
public class ChemicalData
{

    private int property_CID;
    public int GetCID()
    {
        return property_CID;
    }

    private string property_MolecularFormula;
    public string GetMolecularFormula()
    {
        return property_MolecularFormula;
    }

    private double property_MolecularWeight;
    public double GetMolecularWeight()
    {
        return property_MolecularWeight;
    }

    private string property_Title;
    public string GetTitle()
    {
        return property_Title;
    }

    private int property_Charge;
    public int GetCharge()
    {
        return property_Charge;
    }


    public void SetEverything(string dataCSV)
    {
        string[] data = new string[20];
        int i = 0;
        // simple csv parser
        string word = "";
        dataCSV += ",";
        bool quoteState = false;
        foreach (char c in dataCSV)
        {
            if ((c == ',' || c == '\n') && !quoteState)
            {
                data[i] = word;
                i++;
                word = "";
            }
            else if (c == '"')
            {
                quoteState = !quoteState;
            }
            else
            {
                word += c;
            }
        }

        string cid = data[5];
        string molecularFormula = data[6];
        string molecularWeight = data[7];
        string title = data[8];
        string charge = data[9];

        this.property_CID = Int32.Parse(cid);
        this.property_MolecularFormula = molecularFormula;
        this.property_MolecularWeight = Double.Parse(molecularWeight);
        this.property_Title = title;
        this.property_Charge = Int32.Parse(charge);

    }


    /** Prints this ChemicalData to the console. */
    public void print()
    {
        Debug.Log("Chemical Data[" + 
            "cid: " + this.property_CID + ", " +
            "molecular formula: " + this.property_MolecularFormula + ", " +
            "molecular weight: " + this.property_MolecularWeight + ", " +
            "title: " + this.property_Title + ", " +
            "charge: " + this.property_Charge + "]"
        );
    }


    /** Opens a single chemical from the database.
     * @param id The id of the chemical. to open */
    public static ChemicalData LoadFromPUGREST(int id)
    {
        ChemicalData chemicalData = new ChemicalData();

        string property = "Title";
        //chemicalData.SetTitle(PubChemAPIManager.GetProperty(id, property));

        return chemicalData;
    }
    
}
