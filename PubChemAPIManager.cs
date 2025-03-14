using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/** Handles all interactions with PUG REST, the PubChem API.
 @author Erica Hammersmark */
public class PubChemAPIManager : MonoBehaviour
{

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
        GetProperty(100, data);
    }


    // Pulls any property from the API.
    public async void GetProperty(int id, ChemicalData data)
    {

        // below is the template API call:
        // https://pubchem.ncbi.nlm.nih.gov/rest/pug/
        // <input specification>/<operation specification>/[<output specification>][?<operation_options>]
        // header / input / operation / output

        // do... something with this.

        string apiHeader = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/";
        string apiDetails = "compound/cid/444/property/MolecularFormula,MolecularWeight,Title,Charge/CSV";
        string apiCall = apiHeader + apiDetails;

        StartCoroutine(GetRequest(apiCall, data));

    }


    // copied from https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Networking.UnityWebRequest.Get.html
    // I'm not totally sure what it does...

    // I fixed it with heresy... do not call this in parallel!
    private IEnumerator GetRequest(string uri, ChemicalData data)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string text = webRequest.downloadHandler.text;
                    data.SetEverything(text);
                    later(data);
                    break;
            }
        }
    }


    private void later(ChemicalData data)
    {

        //data.print();

        Debug.Log("success!");

    }

}
