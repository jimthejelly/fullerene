using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class creationUser : MonoBehaviour
{
    /// <summary> First element added </summary>
    public static GameObject head;
    /// <summary> Current object the camera is rotated around </summary>
    GameObject molecule;
    /// <summary> Current object mouse is interacting with </summary>
    GameObject select;
    /// <summary> <see cref="select"/>'s transform properties </summary>
    Transform focus;
    /// <summary> <see cref="select"/>'s color properties </summary>
    Color focusMaterial;
    /// <summary> Helper list to change color of all <c> Bond </c> sub-objects </summary>
    List<Component> bondSiblings = new List<Component>();

    /// <summary> Look speed modifier </summary>
    public float turnSpeed;
    /// <summary> Panning speed </summary>
    public float PanSpeed = 50f;
    /// <summary> Cumulative pan offset </summary>
    Vector3 panOffset = Vector3.zero;

    /// <summary> Whether or not a <c> Bond </c> has been updated this frame </summary>
    bool bondReplace = false;
    /// <summary> The number of times that <c> Elements </c> or <c> Bonds </c> have been spawned in the molecule </summary>
    int spawnCount = 0;

    /// <summary> Last object hovered over </summary>
    string check;
    /// <summary> Distance from camera to molecule </summary>
    float zoom = 12;
    /// <summary> Number of clicks during <see cref="clickdelay"/> </summary>
    float click = 0;
    /// <summary> Time limit for a double click to register </summary>
    float clickdelay = 0.2f;
    /// <summary> Current time between two clicks </summary>
    float clicktime = 0;
    /// <summary> Tracks mouse </summary>
    Ray ray;
    /// <summary> Object mouse touches </summary>
	RaycastHit hit;

    /// <summary> Whether or not lone pairs are currently visible </summary>
    public static bool lonePairsVisible = false; // Whether or not lone pairs are currently visible

    /// <summary> Whether or not the molecule has been updated recently </summary>
    /// <remarks>Used to keep the lone pairs visible even while editing the molecule </remarks>
    private bool moleculeUpdated = false;
    /// <summary> How many frames have passed since the last time the molecule was updated
    /// <remarks>Used to keep the lone pairs visible even while editing the molecule </remarks>
    private int framesSinceMoleculeUpdated = 0; // used to keep the lone pairs visible even while editing the molecule

    /// <summary> Dictionary for converting <see cref="Elements"/> to ids for saving and loading </summary>
    private Dictionary<Elements, string> atomIDs = new Dictionary<Elements, string>();
    /// <summary> Dictionary for converting ids to <see cref="Elements"/> for saving and loading </summary>
    private Dictionary<string, Elements> IDToAtom = new Dictionary<string, Elements>();

    /// <summary> The first <c> Element </c> to be connected with a new <c> Bond </c> </summary>
    private Elements bondParent;

    /// <summary>
    /// Initializes molecule.cml, <see cref="select"/>, and camera position
    /// </summary>
    void Start()
    {
        // clears the molecule.cml file
        FileStream stream = File.Open("./Assets/Resources/molecule.cml", FileMode.OpenOrCreate);
        stream.SetLength(0);
        stream.Close();

        // Initializes variables to effectively nothing
        select = GameObject.Find("Main Camera");
        molecule = GameObject.Find("moleculeBody");
        focus = GameObject.Find("moleculeBody").transform;
        
        // Initializes starting camera position
        transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y+1,molecule.transform.position.z-4);
        transform.eulerAngles = new Vector3(0,0,0);
    }

    /// <summary>
    /// Restarts <see cref="creationUser"/> and re-initializes all variables initialized in <see cref="Start"/>
    /// </summary>
    public void Restart()
    {
        // clears the molecule.cml file
        FileStream stream = File.Open("./Assets/Resources/molecule.cml", FileMode.OpenOrCreate);
        stream.SetLength(0);
        stream.Close();

        // Initializes variables to effectively nothing
        select = GameObject.Find("Main Camera");
        molecule = GameObject.Find("moleculeBody");
        focus = GameObject.Find("moleculeBody").transform;
        
        // Initializes starting camera position
        transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y+1,molecule.transform.position.z-4);
        transform.eulerAngles = new Vector3(0,0,0);
    }

    /// <summary>
    /// Manages user input and molecule force interactions each frame update
    /// </summary>
    /// <remarks>
    /// Force interactions should probably be moved elsewhere, it's bad code design. Works for now though!
    /// </remarks>
    void Update()
    { 
        // Camera movement
        if (Input.GetMouseButton(1)) {
            Turning();
        }
        if (Input.GetMouseButton(2)) {

            float xChange = Input.GetAxis("Mouse X");
            float yChange = Input.GetAxis("Mouse Y");

            Debug.Log("Mouse X: " + xChange + ", Mouse Y: " + yChange);
            
            Panning();
            Debug.Log("PANNING");
        }
        
        HandleZoom();

        // Uses number bar to reset camera position
        ResetCamera();

        // Manages mouse click and hover interaction
        Hovering();

        // NOTE: All of these keybinds can be subject to change, they are mostly here to test functionality
        // If "s" key is pressed, save molecule
        if(Input.GetKeyDown("s")) {
            Debug.Log("Saving Molecule");
            SaveMolecule();
        }

        // If "l" key is pressed, load molecule
        if(Input.GetKeyDown("l")) {
            Debug.Log("Loading Molecule");
            LoadMolecule();
            if(lonePairsVisible) {
                HideLonePairs();
                moleculeUpdated = true;
            }
        }

        // if "p" key is pressed, toggle whether or not lone pairs are shown
        if(Input.GetKeyDown("p")) {
            if(lonePairsVisible) {
                Debug.Log("hiding");
                HideLonePairs();
            }
            else {
                Debug.Log("un-hiding");
                ShowLonePairs();
            }
        }

        // if molecule was updated recently, wait a bit to give everything time to update and then respawn lone pairs
        if(moleculeUpdated) {
            framesSinceMoleculeUpdated++;
            if(framesSinceMoleculeUpdated > molecule.transform.childCount) {
                Debug.Log("up");
                DeleteLonePairs();
                if(lonePairsVisible) {
                    SpawnLonePairs();
                }
                else {
                    SpawnLonePairs();
                    HideLonePairs();
                }
                framesSinceMoleculeUpdated = 0;
                moleculeUpdated = false;
            }
        }
        
        // exert forces on all the atoms starting at the root and moving out
        if(head != null) {
            foreach(Transform element in molecule.transform) {
                if(element.CompareTag("Element")) {
                    element.GetComponent<Elements>().CalculateForceVector();
                }
                else if(element.CompareTag("Lone Pair")) {
                    element.GetComponent<LonePairs>().CalculateForceVector();
                }
                
            }
            head.GetComponent<Elements>().UpdatePosition();
            foreach(Transform element in molecule.transform) {
                if(element.CompareTag("Lone Pair")) {
                    element.GetComponent<LonePairs>().UpdatePosition();
                }
                else if(element.CompareTag("Bond")) {
                    element.GetComponent<Bonds>().UpdatePosition();
                }
            }
        }
    }

    /// <summary>
    /// Makes all <see cref="LonePairs"/> in the molecule visible
    /// </summary>
    void ShowLonePairs() {
        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Lone Pair")) {
                item.gameObject.SetActive(true);
            }
        }
        lonePairsVisible = true;
    }

    /// <summary>
    /// Hides all <see cref="LonePairs"/> in the molecule
    /// </summary>
    void HideLonePairs() {
        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Lone Pair")) {
                item.gameObject.SetActive(false);
            }
        }
        lonePairsVisible = false;
    }

    /// <summary>
    /// Spawns <see cref="LonePairs"/> around each <see cref="Elements"/>
    /// </summary>
    void SpawnLonePairs() {
        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Element")) {
                item.gameObject.GetComponent<Elements>().ShowLonePairs();
            }
        }
        lonePairsVisible = true;
    }

    /// <summary>
    /// Deletes all <see cref="LonePairs"/> in the molecule
    /// </summary>
    void DeleteLonePairs() {
        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Lone Pair")) {
                Destroy(item.gameObject);
            }
        }

    }

    /// <summary>
    /// Saves the molecule into <c>./Assets/Resources/molecule.cml</c>
    /// </summary>
    void SaveMolecule() {
        if(molecule.transform.childCount == 0) {
            Debug.Log("No molecule to save!");
            return;
        }
        XmlTextWriter writer = new XmlTextWriter("./Assets/Resources/molecule.cml", null);
        writer.WriteStartDocument();

        writer.Formatting = Formatting.Indented;
        writer.Indentation = 4;

        writer.WriteStartElement("molecule");

        writer.WriteStartElement("atomArray");

        atomIDs = new Dictionary<Elements, string>();
        IDToAtom = new Dictionary<string, Elements>();
        int count = 1;
        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Element")) {
                atomIDs.Add(item.gameObject.GetComponent<Elements>(), "a" + count);
                IDToAtom.Add("a" + count++, item.gameObject.GetComponent<Elements>());
                writer.WriteStartElement("atom");
                writer.WriteAttributeString("id", atomIDs[item.gameObject.GetComponent<Elements>()]);
                writer.WriteAttributeString("elementType", ((ElementSymbols)(item.gameObject.GetComponent<Elements>().protons)).ToString("F"));
                writer.WriteAttributeString("x3", item.position.x.ToString());
                writer.WriteAttributeString("y3", item.position.y.ToString());
                writer.WriteAttributeString("z3", item.position.z.ToString());
                writer.WriteEndElement(); // end of atom
            }
        }

        writer.WriteEndElement(); // end of atomArray

        writer.WriteStartElement("bondArray");

        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Bond")) {
                writer.WriteStartElement("bond");
                writer.WriteAttributeString("atomRefs2", atomIDs[item.gameObject.GetComponent<Bonds>().parent] + " " + atomIDs[item.gameObject.GetComponent<Bonds>().child]);
                writer.WriteAttributeString("order", (item.gameObject.GetComponent<Bonds>().bondOrder).ToString());
                writer.WriteEndElement(); // end of bond
            }
        }
        
        writer.WriteEndElement(); // end of bondArray

        writer.WriteEndElement(); // end of molecule

        writer.WriteEndDocument();
        writer.Close();
    }

    /// <summary>
    /// Loads a molecule from <c>./Assets/Resources/molecule.cml</c>
    /// </summary>
    /// <remarks>
    /// Only works if the molecule hasn't changed since loading into the file as all it does is move the <see cref="Elements"/> around
    /// </remarks>
    void LoadMolecule() {
        if(atomIDs.Count == 0) {
            Debug.Log("no IDs stored");
            return;
        }
        // move atoms into place
        XmlTextReader reader = new XmlTextReader("./Assets/Resources/molecule.cml");
        while(reader.Read()) {
            if(reader.NodeType == XmlNodeType.Element) {
                if(reader.Name.Equals("atom")) {
                    IDToAtom[reader.GetAttribute("id")].transform.position = new Vector3(
                        float.Parse(reader.GetAttribute("x3")), float.Parse(reader.GetAttribute("y3")), float.Parse(reader.GetAttribute("z3")));
                }
            }
        }
        reader.Close();

        // move bonds
        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Bond")) {
                // setting bond position
                Vector3 parentPos = item.gameObject.GetComponent<Bonds>().parent.transform.position;
                Vector3 childPos = item.gameObject.GetComponent<Bonds>().child.transform.position;
                Debug.Log(parentPos + " " + childPos);
                item.position = new Vector3((parentPos.x + childPos.x) / 2, (parentPos.y + childPos.y) / 2, (parentPos.z + childPos.z) / 2);

                // setting bond rotation
                item.LookAt(parentPos);
                item.Rotate(90, 0, 0);

                // setting bond length
                item.localScale = new Vector3(0.15f, Mathf.Sqrt(
                    Mathf.Pow(parentPos.x - childPos.x, 2) + Mathf.Pow(parentPos.y - childPos.y, 2) + Mathf.Pow(parentPos.z - childPos.z, 2)) / 2, 0.15f);
            }
        }
    }
    /// <summary>
    /// Uses the number bar to reset the camera position
    /// </summary>
    void ResetCamera() {

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            panOffset = Vector3.zero;  // To store cumulative pan offset
            transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y,molecule.transform.position.z-zoom);
            transform.eulerAngles = new Vector3(0,0,0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            panOffset = Vector3.zero;  // To store cumulative pan offset
            transform.position = new Vector3(molecule.transform.position.x-zoom,molecule.transform.position.y,molecule.transform.position.z);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            panOffset = Vector3.zero;  // To store cumulative pan offset
            transform.position = new Vector3(molecule.transform.position.x+zoom,molecule.transform.position.y,molecule.transform.position.z);
            transform.eulerAngles = new Vector3(0, -90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            panOffset = Vector3.zero;  // To store cumulative pan offset
            transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y,molecule.transform.position.z+zoom);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    /// <summary>
    /// Rotates the camera around the center point
    /// </summary>
    void Turning() {
        Debug.Log("turn");
        float xChange = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
        float yChange = Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime;

        transform.position = focus.position - (transform.forward * zoom) + panOffset;
        transform.RotateAround(focus.position, Vector3.up, xChange);
        transform.RotateAround(focus.position, transform.right, -yChange);
    }

    /// <summary>
    /// Pans the camera
    /// </summary>
    void Panning() {
        Debug.Log("pan");
        // Get the mouse movement deltas in screen space
        float xChange = Input.GetAxis("Mouse X") * PanSpeed * Time.deltaTime;
        float yChange = Input.GetAxis("Mouse Y") * PanSpeed * Time.deltaTime;

        // Move the camera based on the right and up vectors, which correspond to horizontal and vertical panning
        Vector3 move = (transform.right * -xChange) + (transform.up * -yChange);

        // Apply the movement to the camera's position
        panOffset += move;

        // Update the camera's position with the new pan offset
        transform.position = focus.position - (transform.forward * zoom) + panOffset;

        Debug.Log("Camera Position: " + transform.position);  // Debug to track camera position changes
    }

    /// <summary>
    /// Zooms in and out of the focus point
    /// </summary>
    void HandleZoom() {
        // Changes distance of camera with scroll wheel input
        zoom -= (Input.mouseScrollDelta.y * 1);
        zoom = Mathf.Clamp(zoom, 2, 100);
        transform.position = focus.position - (transform.forward * zoom)+ panOffset;

    }

    /// <summary>
    /// Master function for handling what happens when the mouse is hovering over a <see cref="GameObject"/>
    /// </summary>
    void Hovering() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
        if (Physics.Raycast(ray, out hit)) { // Hovering over object 
            // If "b" key is pressed, try and make a new bond
            if(Input.GetKeyDown("b")) {
                Debug.Log("b");
                // If hovered object is an Element, keep going
                if(select.CompareTag("Element")) {
                    // If bondParent is null, set it
                    if(bondParent == null) {
                        bondParent = select.GetComponent<Elements>();
                        Debug.Log("bonding " + bondParent.name);
                    }
                    else if(select.GetComponent<Elements>().Equals(bondParent)) { // If bondParent == select, cancel bonding
                        bondParent = null;
                        Debug.Log("cancelling bond");
                    }
                    else { // else make the bond between bondParent and select
                           // If bondParent and select can bond more, bond them
                        if(select.GetComponent<Elements>().CanBondMore() && bondParent.CanBondMore()) {
                            // If select and bondParent are already bonded, skip bonding
                            bool bonded = false;
                            foreach(Tuple<GameObject, GameObject> neighbor in select.GetComponent<Elements>().GetNeighbors()) {
                                if(neighbor.Item2.GetComponent<Elements>().Equals(bondParent)) {
                                    bonded = true;
                                }
                            }
                            if(!bonded) { // if select and bondParent are not already bonded
                                float radius = bondParent.covalentRadius + select.GetComponent<Elements>().covalentRadius;
                                GameObject cyl = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
                                GameObject cylClone = Instantiate(cyl, Vector3.zero, Quaternion.identity);
                                cylClone.transform.localScale = new Vector3(0.15f, radius / 2, 0.15f);
                                cylClone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);
                                cylClone.name = cylClone.name + " " + spawnCount;
                                spawnCount++;

                                select.GetComponent<Elements>().UpdateElectrons(1);
                                bondParent.UpdateElectrons(1);
                                select.GetComponent<Elements>().bondCount++;
                                select.GetComponent<Elements>() .bondOrders++;
                                bondParent.bondCount++;
                                bondParent.bondOrders++;

                                cylClone.GetComponent<Bonds>().SetElements(bondParent, select.GetComponent<Elements>());
                                cylClone.GetComponent<Bonds>().UpdatePosition();

                                bondParent.GetNeighbors().Add(new Tuple<GameObject, GameObject>(cylClone, select));
                                select.GetComponent<Elements>().GetNeighbors().Add(new Tuple<GameObject, GameObject>(cylClone, bondParent.gameObject));

                                bondParent = null;
                            }
                        }
                    }
                }
            }
            if (select != null && check != hit.collider.gameObject.name) { // Don't check if already hovering this object
                // Swaps current color and highlight
                select.GetComponent<Renderer>().material.color = focusMaterial;
                if (select.tag == "Bond" && bondSiblings != null)   {
                    foreach(Renderer bond in bondSiblings) {
                        bond.material.color = focusMaterial;
                    }
                }

                select = GameObject.Find(hit.collider.gameObject.name);

                bondSiblings.Clear();
                foreach (Transform bond in hit.collider.transform) {
                    if (bond.tag == "Bond") {
                        bondSiblings.Add(bond.gameObject.GetComponent<Renderer>());
                    }
                }

                focusMaterial = select.GetComponent<Renderer>().material.color;
                if (select.tag == "Bond" && bondSiblings != null)   {
                    foreach (Renderer bond in bondSiblings) {
                        bond.material.color = new Color(1.75f, 1.75f, 1.75f, 0f);
                    }
                    
                } else {
                    select.GetComponent<Renderer>().material.color = new Color(1.75f, 1.75f, 1.75f, 0f);
                }
                
                bondReplace = false;

                // If left clicks object
                if (Input.GetMouseButtonUp(0)) {
                    Clicking();
                }
                check = hit.collider.gameObject.name;
                if (bondReplace) {
                    select = GameObject.Find("Main Camera");
                }

            } else { // Only allow click interaction after initial hover
                if (Input.GetMouseButtonUp(0)) {
                    Clicking();
                }
                if (bondReplace) {
                    select = GameObject.Find("Main Camera");
                }
            }
        } else { // No hovering
            // Debug.Log("Not Hovering");
            if (select != null && select.name != "Main Camera") {
                select.GetComponent<Renderer>().material.color = focusMaterial;
                foreach (Renderer bond in bondSiblings) {
                        bond.material.color = focusMaterial;
                }
            }
            select = GameObject.Find("Main Camera");
            check = "";
        }
    }
    /// <summary>
    /// Determines single click vs double click and calls <see cref="Interacting"/> accordingly
    /// </summary>
    void Clicking() {
        click++;
        if (click == 1) { // If 1 clikc run 1 click interaction
            clicktime = Time.time;
            Interacting(1);
        } 

        if (click > 1 && Time.time - clicktime < clickdelay) // If 2 clicks run double click interaction
        {
            click = 0;
            clicktime = 0;
            Interacting(2);
        } else if (click > 2 || Time.time - clicktime > clickdelay) { // If > 2 clicks above clickdelay reset click stats
            click = 1;
            clicktime = Time.time;
            Interacting(1);
        }
    }

    /// <summary>
    /// Master function for handling what happens when a <see cref="GameObject"/> is clicked
    /// </summary>
    /// <param name="clicknumber"> The number of clicks (one or two) </param>
    void Interacting(int clicknumber) {
        ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (clicknumber == 1) { // 1 click interaction
            if (Physics.Raycast(ray, out hit)) {
                if(!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {
                    if(hit.transform.tag.Equals("Element")) {
                        Elements script = hit.collider.gameObject.GetComponent<Elements>();
                        script.SpawnElement(spawnCount);
                        spawnCount++;
                        moleculeUpdated = true;
                    }
                    else if(hit.transform.tag.Equals("Bond")) {
                        Bonds script = hit.collider.gameObject.GetComponent<Bonds>();
                        script.CycleBondOrder(spawnCount);
                        spawnCount++;
                        bondReplace = true;
                        moleculeUpdated = true;
                    }
                    else {
                        Debug.Log("clicked neither a bond nor an element");
                    }
                }
                else if(Input.GetKey(KeyCode.LeftShift)){
                    Elements script = hit.collider.gameObject.GetComponent<Elements>();
                    script.DeleteElement();
                    moleculeUpdated = true;
                }
            }
        } else if (clicknumber == 2) { // 2 click interaction
            if (Physics.Raycast(ray, out hit) && Input.GetKey(KeyCode.LeftControl)) {
                molecule = GameObject.Find(hit.collider.gameObject.name);
                focus = GameObject.Find(hit.collider.gameObject.name).transform;
                panOffset = Vector3.zero;
            }
        }
    }
}