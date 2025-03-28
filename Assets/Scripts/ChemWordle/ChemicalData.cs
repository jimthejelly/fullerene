using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Represents all the information associated with one chemical in the PubChem database. */
public class ChemicalData
{

    private Dictionary<string, string> properties = new Dictionary<string, string>();
    public string GetProperty(string propertyName)
    {
        if (properties.ContainsKey(propertyName))
        {
            return properties[propertyName];
        }
        else
        {
            return "<NULL>";
        }
    }
    public void SetProperty(string propertyName, string propertyValue)
    {
        properties[propertyName] = propertyValue;
    }


    /** Prints this ChemicalData to the console. */
    public void print()
    {
        Debug.Log("Chemical Data[" + 
            "cid: " + this.GetProperty("CID") + ", " +
            "molecular formula: " + this.GetProperty("MolecularFormula") + ", " +
            "molecular weight: " + this.GetProperty("MolecularWeight") + ", " +
            "title: " + this.GetProperty("Title") + ", " +
            "charge: " + this.GetProperty("Charge") + "]"
        );
    }


    /** Opens a single chemical from the database.
     * @param id The id of the chemical. to open */
    public static ChemicalData LoadFromPUGREST(PubChemAPIManager pubChemAPIManager, int id)
    {
        ChemicalData chemicalData = new ChemicalData();

        string property = "Title";
        // Debug.Log(pubChemAPIManager);
        // pubChemAPIManager.GetProperty(id, chemicalData);

        return chemicalData;
    }
    
}
