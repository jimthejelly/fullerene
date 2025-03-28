using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralDataController : MonoBehaviour
{

    public List<ChemicalData> allData = new List<ChemicalData>();

    public ChemicalData GetChemicalWithCID(int cid)
    {
        foreach (ChemicalData data in allData)
        {
            if (data.GetProperty("CID") == cid.ToString())
            {
                return data;
            }
        }
        Debug.Log("CID not found: " + cid.ToString());
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

    /** Gets a crazy number of chemicals from the PubChem database. */
    public void GetAllChemicals()
    {
        ChemicalData[] data = new ChemicalData[0];
        string[] dataTypes = { "Title", "MolecularFormula", "MolecularWeight", "Charge" };
        pubChemAPIManager.MakeAPIRequest(100, 199, dataTypes);
    }

    public void UpdateInternalData(string text)
    {
        // Debug.Log(text);
        string[] textLines = text.Split('\n');
        string[] dataTypes = textLines[0].Split(',');
        for (int dataTypeIndex = 0; dataTypeIndex < dataTypes.GetLength(0); dataTypeIndex++)
        {
            dataTypes[dataTypeIndex] = dataTypes[dataTypeIndex].Substring(1, dataTypes[dataTypeIndex].Length - 2);
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
                    || textIndex == thisLine.Length - 1)
                {
                    if (!inQuotes)
                    {
                        // Debug.Log("!!" + word);
                        thisData[i] = word;
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
                chemicalData.SetProperty(dataTypes[dataTypeIndex], thisData[dataTypeIndex]);
            }
            allData.Add(chemicalData);

        }
    }

}
