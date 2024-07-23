using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class creationUser : MonoBehaviour
{
    GameObject molecule;
    GameObject select;
    Transform focus;
    Color focusMaterial;
    public float turnSpeed; // Look speed modifier
    string checkd;
    float zoom = 12;
    float click = 0;
    float clickdelay = 0.2f;
    float clicktime = 0;
    Ray ray;
	RaycastHit hit;
    void Start()
    {
        select = GameObject.Find("Main Camera");
        molecule = GameObject.Find("moleculeBody");
        focus = GameObject.Find("moleculeBody").transform;
        

        transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y+1,molecule.transform.position.z-4);
        transform.eulerAngles = new Vector3(0,0,0);
    }
    // Update is called once per frame
    void Update()
    { 
        if (Input.GetMouseButton(1)) {
            Turning();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Debug.Log("PLEasE");
            transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y,molecule.transform.position.z-zoom);
            transform.eulerAngles = new Vector3(0,0,0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Debug.Log("KILL");
            transform.position = new Vector3(molecule.transform.position.x-zoom,molecule.transform.position.y,molecule.transform.position.z);
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Debug.Log("HeLp");
            transform.position = new Vector3(molecule.transform.position.x+zoom,molecule.transform.position.y,molecule.transform.position.z);
            transform.eulerAngles = new Vector3(0, -90, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            Debug.Log("AHHGHHAh");
            transform.position = new Vector3(molecule.transform.position.x,molecule.transform.position.y,molecule.transform.position.z+zoom);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        Hovering();
        
        zoom -= (Input.mouseScrollDelta.y * 1);
        zoom = Mathf.Clamp(zoom, 2, 20);
        transform.position = focus.position - (transform.forward * zoom);

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
        if (Physics.Raycast(ray, out hit)) {
            if (checkd != hit.collider.gameObject.name) {
                Debug.Log("New Object Hit");
                Debug.Log("return");
                select.GetComponent<Renderer>().material.color = focusMaterial;
                select = GameObject.Find(hit.collider.gameObject.name);
                focusMaterial = select.GetComponent<Renderer>().material.color;
                select.GetComponent<Renderer>().material.color = new Color(1.75f, 1.75f, 1.75f, 0f);
                if (Input.GetMouseButtonUp(0)) {
                    Clicking();
                }
                checkd = hit.collider.gameObject.name;
            } else {
                if (Input.GetMouseButtonUp(0)) {
                    Clicking();
                }
            }
            
        } else {
            if (select.name != "Main Camera") {
                Debug.Log("return");
                select.GetComponent<Renderer>().material.color = focusMaterial;
            }
            select = GameObject.Find("Main Camera");
            checkd = "";
        }
    }

    void Clicking() {
        click++;
        if (click == 1) {
            clicktime = Time.time;
            Interacting(1);
        } 

        if (click > 1 && Time.time - clicktime < clickdelay)
        {
            click = 0;
            clicktime = 0;
            Interacting(2);
        } else if (click > 2 || Time.time - clicktime > clickdelay) {
            click = 1;
            clicktime = Time.time;
            Interacting(1);
        }
    }

    void Interacting(int clicknumber) {
        ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Debug.Log("Inter");
        if (clicknumber == 1) {
            Debug.Log("Creating");
        } else if (clicknumber == 2) {
            Debug.Log("Inteasr");
            if (Physics.Raycast(ray, out hit)) {
                Debug.Log("Doubke + " + hit.collider.gameObject.name);
                molecule = GameObject.Find(hit.collider.gameObject.name);
                focus = GameObject.Find(hit.collider.gameObject.name).transform;
            }
        }
    }
}