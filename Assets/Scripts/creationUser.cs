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
    public float turnSpeed; // Look speed modifier
    string check; // Last object hovered over
    float zoom = 12; // Distance from camera to molecule
    float click = 0; // Number of clicks during clickdelay
    float clickdelay = 0.2f; // Time limit for doubleclick to register
    float clicktime = 0; // Current time between two clicks
    Ray ray; // Tracks mouse
	RaycastHit hit; // Object mouse touches
    void Start()
    {
        // Initializes variables to effectively nothing
        select = GameObject.Find("Main Camera");
        molecule = GameObject.Find("4-Beryllium");
        focus = GameObject.Find("4-Beryllium").transform;
        
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
        
        // Uses number bar to reset camera position
        ResetCamera();

        // Manages mouse click and hover interaction
        Hovering();
        
        // Changes distance of camera with scroll wheel input
        zoom -= (Input.mouseScrollDelta.y * 1);
        zoom = Mathf.Clamp(zoom, 2, 20);
        transform.position = focus.position - (transform.forward * zoom);

    }

    void ResetCamera() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y,molecule.transform.position.z-zoom);
            transform.eulerAngles = new Vector3(0,0,0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            transform.position = new Vector3(molecule.transform.position.x-zoom,molecule.transform.position.y,molecule.transform.position.z);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            transform.position = new Vector3(molecule.transform.position.x+zoom,molecule.transform.position.y,molecule.transform.position.z);
            transform.eulerAngles = new Vector3(0, -90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y,molecule.transform.position.z+zoom);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    void Turning() {
        float xChange = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
        float yChange = Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime;

        transform.position = focus.position - (transform.forward * zoom);
        transform.RotateAround(focus.position, Vector3.up, xChange);
        transform.RotateAround(focus.position, transform.right, -yChange);
    }

    void Hovering() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);	
        if (Physics.Raycast(ray, out hit)) { // Hovering over object
            if (check != hit.collider.gameObject.name) { // Don't check if already hovering this object

                // Swaps current color and highlight
                select.GetComponent<Renderer>().material.color = focusMaterial;
                select = GameObject.Find(hit.collider.gameObject.name);
                focusMaterial = select.GetComponent<Renderer>().material.color;
                select.GetComponent<Renderer>().material.color = new Color(1.75f, 1.75f, 1.75f, 0f);

                // If left clicks object
                if (Input.GetMouseButtonUp(0)) {
                    Clicking();
                }
                check = hit.collider.gameObject.name;

            } else { // Only allow click interaction after initial hover
                if (Input.GetMouseButtonUp(0)) {
                    Clicking();
                }
            }
            
        } else { // No hovering
            if (select.name != "Main Camera") {
                select.GetComponent<Renderer>().material.color = focusMaterial;
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
                //Elements script = molecule.GetComponent<Elements>();
                //script.SpawnElement();
            }
        } else if (clicknumber == 2) { // 2 click interaction
            Debug.Log("Inteasr");
            if (Physics.Raycast(ray, out hit)) {
                Debug.Log("Doubke + " + hit.collider.gameObject.name);
                molecule = GameObject.Find(hit.collider.gameObject.name);
                focus = GameObject.Find(hit.collider.gameObject.name).transform;
            }
        }
    }
}