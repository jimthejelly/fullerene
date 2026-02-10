using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ChemWordle
{
    
    /** Handles all interactions with PUG REST, the PubChem API. */
    public class PubChemAPIManager : MonoBehaviour
    {
        
        /** The longest tolerable delay on a web request before it just times out. */
        private const int MAX_REQUEST_DELAY_MS = 1500;
        /** How often to ask the request if it's done. */
        private const int REQUEST_CHECK_INTERVAL_MS = 50;

		/** Goes at the beginning of every API request. */
        private const string API_CALL_HEADER = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/";

		/** Finds the CIDs of a number of chemicals with the specified property.
		 * Can return less chemicals than requested if not enough match, or null if an exception occurs. */
        public static async Task<List<int>> requestCIDsWithProperty(
            string propertyName,
            string propertyValue,
            int max
        ) {

            // put together a request to get the CIDs of all chemicals with the specified property
            var apiCall = API_CALL_HEADER + "compound/" +
                          propertyName + "/" + propertyValue + // e.g. name/Water
                          "/cids/TXT?MaxRecords=" + max;
            
            // make the request!
            var result = await MakeAPIRequest(apiCall);
            if (result == null) return null;
            
            // process the result into a list of numbers and return it
            var resultArray = result.Split("\n");
            List<int> cidsList = new();
            for (var i = 0; i < resultArray.Length - 1; i++)
            {
                cidsList.Add(int.Parse(resultArray[i]));
            }
            return cidsList;
            
        }

        /** Retrieves chemical data objects for the given CIDs. */
        public static async Task<List<ChemicalData>> RequestChemicals(
            List<int> cids,
            List<string> dataTypes
        ) {
            // put the cids and datatypes together into one big request
            var callResult = await MakeAPIRequest(
                API_CALL_HEADER +
                "compound/cid/" + string.Join(",", cids) +
                "/property/" + string.Join(",", dataTypes) +
                "/CSV"
            );
            return ParseStringData(callResult);
        }

        /** Performs a PubChem API request.
         * 'apiCall' is the specific request to send.
         * Returns the result of the request, or null if something went wrong. */
        private static async Task<string> MakeAPIRequest(string apiCall)
        {
            
            // create and send the web request
            var webRequest = UnityWebRequest.Get(apiCall);
            webRequest.SendWebRequest();
            
            // wait for it to finish (or time out if it takes too long)
            for (var delayI = 0; delayI < MAX_REQUEST_DELAY_MS / REQUEST_CHECK_INTERVAL_MS; delayI++)
            {
                if (webRequest.isDone) break;
                await Task.Delay(REQUEST_CHECK_INTERVAL_MS);
            }

            // time out: close the request, report to the console, and return null
            if (!webRequest.isDone)
            {
                Debug.LogError("Request timed out: " + apiCall);
                webRequest.Abort();
                return null;
            }
            
            // success: return the result!
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                return webRequest.downloadHandler.text;
            }
            
            // failure: complain to the console and return null
            Debug.LogError(
                apiCall.Split('/')[^1] + ": " +
                "Web Request Error: " + webRequest.error + " " +
                "(for request " + apiCall + ")"
            );
            return null;
            
        }

        /** Parses the given string and returns the resultant chemical data. */
        private static List<ChemicalData> ParseStringData(string text)
        {
            
            // split the entire thing into lines.
            // each line after the first represents one chemical.
            var textLines = text.Split('\n');
            
            // extract the data types given in this text
            // by parsing the first line.
            // example text:
            // "CID","Title","MolecularFormula","MolecularWeight","Charge"
            var dataTypesReceived = textLines[0].Split(',');
            for (var dataTypeIndex = 0;
                 dataTypeIndex < dataTypesReceived.GetLength(0);
                 dataTypeIndex++)
            {
                // strip the quotes off each side of each word
                dataTypesReceived[dataTypeIndex] =
                    dataTypesReceived[dataTypeIndex]
                        .Substring(1, dataTypesReceived[dataTypeIndex].Length - 2);
            }

            List<ChemicalData> dataToReturn = new();
            
            // parse the rest of the file, which has a very regular pattern
            for (var lineIndex = 1; lineIndex < textLines.GetLength(0); lineIndex++)
            {
                var thisLine = textLines[lineIndex];
                
                // example text:
                // 962,"Water","H2O",18.015,0
                //
                // note that we CANNOT just split the string by ',' like above,
                // since the chemical name could potentially have random ,s in it.
                
                // create the chemical data object that will hold all this data
                ChemicalData chemicalData = new();
                
                // temporarily store the data in this array to avoid headaches in a bit
                var thisData = new string[dataTypesReceived.GetLength(0)];
                
                var word = "";          // the current word being read. will just be text
                var inQuotes = false;   // tracks whether the parser is inside a string
                var dataIndex = 0;      // tracks which data type the parser is currently reading
                
                // iterate through every character in the line
                for (var textIndex = 0; textIndex < thisLine.Length; textIndex++)
                {
                    
                    // if the parser hits a " symbol,
                    // flip the current quote status.
                    // HOPEFULLY theres no random " in any
                    // chemical name in the database.
                    if (thisLine[textIndex] == '"') inQuotes = !inQuotes;
                    
                    // if the parser hits a , symbol (OUTSIDE OF QUOTES!),
                    // or if it hits the end of the line,
                    // then the current word is finished.
                    if ((!inQuotes && thisLine[textIndex] == ',')
                        || textIndex == thisLine.Length - 1)
                    {
                        // if we're not in quotes and at the very end of the line,
                        // then we do need to include this last character as well.
                        if (!inQuotes && textIndex == thisLine.Length - 1) word += thisLine[textIndex];
                        
                        // put the word in our list and then clear the word variable.
                        thisData[dataIndex++] = word;
                        word = "";
                        
                    }
                    
                    // otherwise,
                    // unless the parsed character is ",
                    // it should be made part of the current word.
                    else if (thisLine[textIndex] != '"')
                        word += thisLine[textIndex];
                    
                }
                
                // now, iterate through the data types we just parsed, and assign them to the chemical data.
                for (var typeIndex = 0; typeIndex < dataTypesReceived.GetLength(0); typeIndex++)
                {
                    var dataType = dataTypesReceived[typeIndex];
                    var dataValue = thisData[typeIndex];
                    
                    // assert that the default value of "charge" is 0...
                    if (dataType == "Charge" && dataValue == "")
                        chemicalData.SetProperty(dataType, "0");
                    
                    // ...but otherwise just set the value
                    else chemicalData.SetProperty(dataType, dataValue);
                    
                }
                
                // sanity check. not sure why this would happen but i think it does.
                if (chemicalData.GetProperty("CID") == null) continue;
                
                // add the chemical data!
                // TODO: you can totally create duplicates of the chemical data. that sucks
                dataToReturn.Add(chemicalData);

            }
            
            return dataToReturn;
            
        }
        
    }
    
}
