using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/** Handles all interactions with PUG REST, the PubChem API.
 @author Erica Hammersmark */
public class PubChemAPIManager : MonoBehaviour
{

    public GeneralDataController generalDataController;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void TempOnButtonPush()
    {
        ChemicalData data = new ChemicalData();
        // GetProperty(100, data);
    }


    // Performs a PubChem API request.
    // @param minCID - The minimum CID.
    // @param maxCID - The maximum CID.
    // @param dataTypes - which data types to request.
    public void MakeAPIRequest(int minCID, int maxCID, string[] dataTypes)
    {

        // put all the cids in sequence so information is returned for all of them
        // ex. if minCID = 1 and maxCID = 5, this will build 1,2,3,4,5
        string allCIDsRequest = "";
        for (int cid = minCID; cid <= maxCID; cid++)
        {
            allCIDsRequest += cid.ToString();
            if (cid != maxCID) allCIDsRequest += ",";
        }

        // put all the requested data types in sequence for a similar reason
        string allDataTypesRequest = "";
        for (int dataTypeIndex = 0; dataTypeIndex < dataTypes.GetLength(0); dataTypeIndex++)
        {
            allDataTypesRequest += dataTypes[dataTypeIndex];
            if (dataTypeIndex != dataTypes.GetLength(0) - 1) allDataTypesRequest += ",";
        }

        // API calls are of the form:
        // <input specification>/<operation specification>/[<output specification>]
        string apiCall = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/cid/"
            + allCIDsRequest + "/property/" + allDataTypesRequest + "/CSV";

        StartCoroutine(GetRequest(apiCall));

    }

    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // send the data to the controller to be stored
                generalDataController.UpdateInternalData(webRequest.downloadHandler.text);
            }
            else
            {
                string[] pages = uri.Split('/');
                int page = pages.Length - 1;
                Debug.LogError(pages[page] + ": Web Request Error: " + webRequest.error);
            }

        }
    }


    private void later(ChemicalData data)
    {

        // data.print();

        Debug.Log("success!");

    }

}
