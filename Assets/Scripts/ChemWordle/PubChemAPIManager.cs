using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

/** Handles all interactions with PUG REST, the PubChem API.
 @author Erica Hammersmark */
public class PubChemAPIManager : MonoBehaviour
{

    public GeneralDataController generalDataController;

    public WordleManager wordleManager;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    int i = 0;

    public void TempOnButtonPush()
    {
        wordleManager.AddGuess(generalDataController.GetChemicalWithProperty("CID", (100 + i++).ToString()));
    }

    public void RequestChemicalWithProperty(string propertyName, string propertyValue)
    {

        string apiCall = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/"
            + propertyName + "/" + propertyValue + "/cids/TXT?MaxRecords=10";

        // Debug.Log(apiCall);

        StartCoroutine(GetRequest(apiCall, "get_cids"));

    }


    // Performs a PubChem API request.
    // @param minCID - The minimum CID.
    // @param maxCID - The maximum CID.
    // @param dataTypes - which data types to request.
    public void MakeAPIRequest(string cids, string[] dataTypes, string purpose = "whatever")
    {

        // put all the requested data types in sequence for a similar reason
        string allDataTypesRequest = "";
        for (int dataTypeIndex = 0; dataTypeIndex < dataTypes.GetLength(0); dataTypeIndex++)
        {
            allDataTypesRequest += dataTypes[dataTypeIndex];
            if (dataTypeIndex != dataTypes.GetLength(0) - 1) allDataTypesRequest += ",";
        }
        //print(allDataTypesRequest);

        // API calls are of the form:
        // <input specification>/<operation specification>/[<output specification>]
        string apiCall = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/"
            + cids + "/property/" + allDataTypesRequest + "/CSV";
        //print(apiCall);
        StartCoroutine(GetRequest(apiCall, purpose, Int32.Parse(cids.Split(",")[0])));

    }

    private IEnumerator GetRequest(string uri, string purpose, int cid = 0)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                if (purpose == "get_cids")
                {
                    string[] split = webRequest.downloadHandler.text.Split("\n");
                    string cidsRequest = "";
                    for (int i = 0; i < split.Length - 1; i++)
                    {
                        cidsRequest += split[i];
                        if (i != split.Length - 2) cidsRequest += ",";
                    }
                     Debug.Log("cids: " + cidsRequest);
                    MakeAPIRequest(cidsRequest, generalDataController.dataTypes, "set_guessed_chemical");
                }
                else if (purpose == "set_guessed_chemical")
                {
                    generalDataController.UpdateInternalData(webRequest.downloadHandler.text);
                    wordleManager.set(generalDataController.GetChemicalWithProperty("CID", cid.ToString()), false);
                }
                else if (purpose == "set_mystery_chemical")
                {
                    generalDataController.UpdateInternalData(webRequest.downloadHandler.text);
                    ChemicalData[] data = new ChemicalData[200];
                    for (int i = 0; i < 200; i++)
                    {
                        data[i] = (generalDataController.GetChemicalWithProperty("CID", (cid + i).ToString()));
                    }
                    wordleManager.SetMysteryChemical(data);
                }
                else
                {
                    // send the data to the controller to be stored
                    generalDataController.UpdateInternalData(webRequest.downloadHandler.text);
                }
            }
            else
            {
                string[] pages = uri.Split('/');
                int page = pages.Length - 1;
                Debug.LogError(pages[page] + ": Web Request Error: " + webRequest.error + "(for request " + uri + ")");
            }

        }
    }


    private void later(ChemicalData data)
    {

        // data.print();

        Debug.Log("success!");

    }

}
