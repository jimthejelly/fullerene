using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class creationUser : MonoBehaviour
{
    GameObject molecule; // Current object the camera is rotated around
    GameObject select; // Current object mouse is interacting with
    Transform focus; // select object's transform properties
    Color focusMaterial; // select object's color properties
    List<Component> bondSiblings = new List<Component>();
    public float turnSpeed; // Look speed modifier

    public float PanSpeed = 50f; //panning speed
    Vector3 panOffset = Vector3.zero;  // To store cumulative pan offset

    bool bondReplace = false;
    int elements = 0;
    string check; // Last object hovered over
    float zoom = 12; // Distance from camera to molecule
    float click = 0; // Number of clicks during clickdelay
    float clickdelay = 0.2f; // Time limit for doubleclick to register
    float clicktime = 0; // Current time between two clicks
    Ray ray; // Tracks mouse
	RaycastHit hit; // Object mouse touches

    // GameObject tempHover; // temporary object made by hovering

    void Start()
    {
        // Initializes variables to effectively nothing
        select = GameObject.Find("Main Camera");
        molecule = GameObject.Find("moleculeBody");
        focus = GameObject.Find("moleculeBody").transform;
        
        // Initializes starting camera position
        transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y+1,molecule.transform.position.z-4);
        transform.eulerAngles = new Vector3(0,0,0);
    }
    // Update is called once per frame
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
        
        
    }

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

    void Turning() {
        float xChange = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
        float yChange = Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime;

        transform.position = focus.position - (transform.forward * zoom) + panOffset;
        transform.RotateAround(focus.position, Vector3.up, xChange);
        transform.RotateAround(focus.position, transform.right, -yChange);
    }

    void Panning() {
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

    void HandleZoom() {
        // Changes distance of camera with scroll wheel input
        zoom -= (Input.mouseScrollDelta.y * 1);
        zoom = Mathf.Clamp(zoom, 2, 100);
        transform.position = focus.position - (transform.forward * zoom)+ panOffset;

    }

    void Hovering() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
        if (Physics.Raycast(ray, out hit)) { // Hovering over object
            if (select != null && check != hit.collider.gameObject.name) { // Don't check if already hovering this object

                

                // Swaps current color and highlight
                select.GetComponent<Renderer>().material.color = focusMaterial;
                if (select.tag == "Bond" && bondSiblings != null)   {
                    foreach (Renderer bond in bondSiblings) {
                        bond.material.color = focusMaterial;
                    }

                } else {

                }

            
                // if(!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {
                //     if(hit.transform.tag.Equals("Element")) {
                //         Elements script = hit.collider.gameObject.GetComponent<Elements>();
                //         script.SpawnElement(elements);
                //         elements++;
                //     }
                //     else if(hit.transform.tag.Equals("Bond")) {
                //         Bonds script = hit.collider.gameObject.GetComponent<Bonds>();
                //         script.CycleBondOrder(elements);
                //         elements++;
                //         bondReplace = true;
                //     }
                // }
                // else if(Input.GetKey(KeyCode.LeftShift)){
                //     Elements script = hit.collider.gameObject.GetComponent<Elements>();
                //     script.DeleteElement();
                // }


                select = GameObject.Find(hit.collider.gameObject.name);


                

                bondSiblings.Clear();
                foreach (Transform bond in hit.collider.transform.parent.transform) {
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


                Debug.Log("Check");
            } else { // Only allow click interaction after initial hover
                if (Input.GetMouseButtonUp(0)) {
                    Clicking();
                }
                if (bondReplace) {
                    select = GameObject.Find("Main Camera");
                }
                
                // if(select.tag == "Bond") {
                //     /*
                //     //bond creation script
                //     Bonds script = hit.collider.gameObject.GetComponent<Bonds>();
                //     script.CycleBondOrder(elements);
                //     elements++;
                //     bondReplace = true;
                //     */
                // } else {
                //     // element creation script
                //     Elements script = hit.collider.gameObject.GetComponent<Elements>();
                //     script.TempSpawnElement(elements);
                //     elements++;

                // }
            }
            
            // tempHover = hit.collider.gameObject;

        } else { // No hovering
            if (select != null && select.name != "Main Camera") {
                select.GetComponent<Renderer>().material.color = focusMaterial;
                foreach (Renderer bond in bondSiblings) {
                        bond.material.color = focusMaterial;
                }

                // Elements preview = select.transform.GetChild(select.transform.childCount-1).GetChild(0).GetChild(0).gameObject.GetComponent<Elements>();
                // preview.DeleteElement();

            }
            select = GameObject.Find("Main Camera");
            check = "";
        }
    }

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

    void Interacting(int clicknumber) {
        ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Debug.Log("Inter");
        if (clicknumber == 1) { // 1 click interaction
            Debug.Log("Creating");
            if (Physics.Raycast(ray, out hit)) {
                if(!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftShift)) {
                    if(hit.transform.tag.Equals("Element")) {
                        createElement script = hit.collider.gameObject.GetComponent<createElement>();
                        script.SpawnElement(elements);
                        elements++;
                    }
                    else if(hit.transform.tag.Equals("Bond")) {
                        Bonds script = hit.collider.gameObject.GetComponent<Bonds>();
                        script.CycleBondOrder(elements);
                        elements++;
                        bondReplace = true;
                    }
                }
                else if(Input.GetKey(KeyCode.LeftShift)){
                    createElement script = hit.collider.gameObject.GetComponent<createElement>();
                    script.DeleteElement();
                }
            }
        } else if (clicknumber == 2) { // 2 click interaction
            Debug.Log("Inteasr");
            if (Physics.Raycast(ray, out hit) && Input.GetKey(KeyCode.LeftControl)) {
                Debug.Log("Doubke + " + hit.collider.gameObject.name);
                molecule = GameObject.Find(hit.collider.gameObject.name);
                focus = GameObject.Find(hit.collider.gameObject.name).transform;
                panOffset = Vector3.zero;
            }
        }
    }
}