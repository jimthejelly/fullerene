using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawOrgo : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject bondLine;
    [SerializeField] GameObject element;
    GameObject canvas;
    GameObject UI;
    GameObject ElementSelector;
    Ray ray;
    string Function = "Place";

    private int difficulty = 0; // 0 is easy, 1 is medium, 2 is hard
    private string[] basePrefixes = {"benz", "meth", "eth", "prop", "but", "pent", "hex", "hept", "oct", "non", "dec", "undec", "duodec"}; // 0 is benzene
    private string[] countPrefixes = {"di", "tri", "tetra", "penta", "hexa"}; // I'm not sure how many of these I'll need but they're here
    private string[] saturationTypes = { "an", "en", "yn" }; // for single/double/triple bonds respectively
    private string[] suffixes = {"oic acid", "oate", "amide", "enitrile", "al", "one", "ol", "amine", "ether", "e"}; // Suffixes for different groups ("e" is for alkane/en/ynes)
    private string[] functionalPrefixes = {"halides", "alkanes", "hydroxy", "oxy"};
    private string[] halides = {"fluoro", "chloro", "bromo", "iodo"}; // Prefixes for halides
    private string[] alkanePrefixes = {"phenyl", "methyl", "ethyl", "propyl", "butyl", "pentyl", "hexyl", "heptyl", "octyl", "nonyl", "decyl", "undecyl", "duodecyl"};
    public string currentMolecule = "UNDEFINED";
    public string molecularformula;

    public int selectedElement = 8;

    // Start is called before the first frame update
    void Start()
    {
        generateRandomMolecule();
        string apiCall = "https://pubchem.ncbi.nlm.nih.gov/rest/pug/compound/name/"        //make api call to figure out how to organize the chemical formula for the molecule.
            + currentMolecule + "/property/MolecularFormula/CSV";

        StartCoroutine(GetRequest(apiCall, "formula"));

        canvas = GameObject.Find("Canvas");
        UI = canvas.transform.GetChild(1).gameObject;
        ElementSelector = canvas.transform.GetChild(2).gameObject;


    }

    private IEnumerator GetRequest(string uri, string purpose)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string split = webRequest.downloadHandler.text;
            Debug.Log(split);
            if (purpose == "formula")
            {
                molecularformula = split;
            }
        }
    }

            // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) { //based on the current function, start interacting with the scree. Place spawns elements, Add will add bonds, Manipulate will move elements/camera
           ray = cam.ScreenPointToRay(Input.mousePosition);
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    // Process the hit UI element (e.g., Button, Image)
                    Debug.Log("Hit UI Element: " + result.gameObject.name);
                    // Access the specific component on the hit UI element
                    // e.g., if (result.gameObject.GetComponent<Button>() != null) { ... }

                    // You can also trigger specific events on the hit element using EventTrigger component
                    // or by implementing IPointerClickHandler, IPointerEnterHandler, etc.
                }
            }
            else
            {
                // No UI element was hit, you can perform other raycasts here
                // For example, to interact with 3D objects behind the UI
                GameObject e = Instantiate(element);
                e.transform.position = eventDataCurrentPosition.position;
            }
        }


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

    public void SelectNewElement(int index)
    {
        selectedElement = index;
        ElementSelector.SetActive(false);
        UI.SetActive(true);
    }

    public void SelectElement()
    {
        ElementSelector.SetActive(true);
        UI.SetActive(false);
    }

    public void drawBond(GameObject element1, GameObject element2)
    {

        GameObject line = Instantiate(bondLine);

        line.GetComponent<BondLineBehavoir>().setElements(element1, element2);
    }


}
