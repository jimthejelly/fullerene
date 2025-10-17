using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneralDataController : MonoBehaviour
{

    public List<ChemicalData> allData = new List<ChemicalData>();

    public ChemicalData GetChemicalWithProperty(string propertyName, string propertyValue)
    {
        foreach (ChemicalData data in allData)
        {
            try {
                if (data != null && data.GetProperty(propertyName).ToLower() == propertyValue.ToLower())
                {
                    return data;
                }
            }
            catch (Exception e)
            {
                // oh well :)
            }
        }
        Debug.Log("Could not find a chemical with value '" + propertyValue + "' (for property '" + propertyName + "')");
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetAllChemicals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public PubChemAPIManager pubChemAPIManager;
    public string[] dataTypes = { "Title", "MolecularFormula", "MolecularWeight", "Charge"};

    /** Gets a crazy number of chemicals from the PubChem database. */
    public void GetAllChemicals()
    {
        // ChemicalData[] data = new ChemicalData[0];
        // // pubChemAPIManager.MakeAPIRequest(1, 1000, dataTypes);
    }

    public void UpdateInternalData(string text)
    {
        // Debug.Log(text);
        string[] textLines = text.Split('\n');
        string[] dataTypes = textLines[0].Split(',');
        for (int dataTypeIndex = 0; dataTypeIndex < dataTypes.GetLength(0); dataTypeIndex++)
        {
            dataTypes[dataTypeIndex] = dataTypes[dataTypeIndex].Substring(1, dataTypes[dataTypeIndex].Length - 2);
            //print(dataTypes[dataTypeIndex]);
        }
        for (int dataIndex = 1; dataIndex < textLines.GetLength(0); dataIndex++)
        {
            ChemicalData chemicalData = new ChemicalData();
            string[] thisData = new string[dataTypes.GetLength(0)];
            string thisLine = textLines[dataIndex];
            string word = "";
            bool inQuotes = false;
            int i = 0;
            for (int textIndex = 0; textIndex < thisLine.Length; textIndex++)
            {
                if (thisLine[textIndex] == '"')
                {
                    inQuotes = !inQuotes;
                }
                if ((thisLine[textIndex] != '"' && thisLine[textIndex] == ',')
                    || textIndex == thisLine.Length-1)
                {
                    if (!inQuotes)
                    {
                        //Debug.Log("!!" + word);

                        if (dataTypes[i] == "Charge")
                        {
                            word += (thisLine[thisLine.Length - 1]);
                        }
                        thisData[i] = word;
                        //Debug.Log(dataTypes[i] + "!!" + word);
                        word = "";
                        i++;
                    }
                }
                else if (thisLine[textIndex] != '"')
                {
                    word += thisLine[textIndex];
                }
            }
            for (int dataTypeIndex = 0; dataTypeIndex < dataTypes.GetLength(0); dataTypeIndex++)
            {
                // Debug.Log(dataTypes[dataTypeIndex] + " = " + thisData[dataTypeIndex]);
                if (dataTypes[dataTypeIndex] == "Charge")
                {
                    if (thisData[dataTypeIndex] == "")
                    {
                        chemicalData.SetProperty(dataTypes[dataTypeIndex], "0");
                    } else
                    {
                        chemicalData.SetProperty(dataTypes[dataTypeIndex], thisData[dataTypeIndex]);
                    }
                }
                else
                {
                    chemicalData.SetProperty(dataTypes[dataTypeIndex], thisData[dataTypeIndex]);
                }
                //print(dataTypes[dataTypeIndex]);
            }
            if (chemicalData != null && chemicalData.GetProperty("CID") != "")
            {
                allData.Add(chemicalData);
                // chemicalData.print();
            }

        }
    }

}
