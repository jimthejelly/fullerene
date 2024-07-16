using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Elements : MonoBehaviour
{
    public int lonePairs;
    public int bondingElectrons;
    public bool expandedOctet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown() {
        if(Input.GetKeyDown("left shift")) {
            SpawnElement();
        }
    }
    // Spawns the element prefab contained in the global variable element
    void SpawnElement() {
        GameObject tempObj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" /*+ selectElement.element */+ ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(tempObj, Vector3.zero, Quaternion.identity, transform);
        float radius;
        int bondCount = 0;
        foreach(Transform child in transform) {
            if(child.tag.Equals("Element")) {
                bondCount++;
            }
        }
        int start = 0;
        if(transform.parent != null) {
            bondCount++;
            start = 1;
        }
        Debug.Log(bondCount);
        if(bondCount == 1) {
            radius = 3f;
            transform.GetChild(0).localPosition = Vector3.up * radius;
            transform.GetChild(0).localEulerAngles = new Vector3(180, 0, 0);
        }
        else if(bondCount == 2) {
            for(int i = start; i < 2; i++) {
                radius = 3f;
                transform.GetChild(i).localPosition = Vector3.up * radius;
                transform.GetChild(i).localEulerAngles = new Vector3(180, 0, 0);
            }
            transform.GetChild(1).RotateAround(transform.position, transform.forward, 180);
        }
        else if(bondCount == 3) {
           for(int i = start; i < 3; i++) {
                radius = 3f;
                transform.GetChild(i).localPosition = Vector3.up * radius;
                transform.GetChild(i).localEulerAngles = new Vector3(180, 0, 0);
            }
            transform.GetChild(1).RotateAround(transform.position, transform.forward, 120);
            transform.GetChild(2).RotateAround(transform.position, transform.forward, 240);
        }
        else if(bondCount == 4) {
           for(int i = start; i < 4; i++) {
                radius = 3f;
                transform.GetChild(i).localPosition = Vector3.up * radius;
                transform.GetChild(i).localEulerAngles = new Vector3(180, 0, 0);
                if(i > 0) {
                    transform.GetChild(i).RotateAround(transform.position, transform.right, 109);
                }
            }
            transform.GetChild(2).RotateAround(transform.position, transform.up, 109);
            transform.GetChild(3).RotateAround(transform.position, transform.up, -109);
        }
    }
}
