using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Elements : MonoBehaviour
{
    private List<Tuple<GameObject,GameObject>> neighbors = new List<Tuple<GameObject,GameObject>>();
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
        //SpawnElement();
    }

    // returns (a.x*b.x, a.y*b.y, a.z*b.z)
    Vector3 indivMult(Vector3 a, Vector3 b) {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    // Spawns the element prefab contained in the global variable element
    /*
    public void SpawnElement(int num) {
        int bondCount = 0;
        int bondOrders = 0;
        foreach(Transform child in transform) {
            if(child.tag.Equals("Bond")) {
                bondCount++;
                foreach(Transform ch in child.transform) {
                    if(ch.tag.Equals("Bond")) {
                        bondOrders++;
                    }
                }
            }
        }
        int start = 0;
        if(transform.parent != null && transform.parent.tag.Equals("Bond")) {
            bondCount++;
            start = 1;
            foreach(Transform ch in transform.parent.parent.transform) {
                if(ch.tag.Equals("Bond")) {
                    bondOrders++;
                }
            }
        }
        // checking if the element can make more bonds
        if((!expandedOctet && bondOrders == bondingElectrons) || (expandedOctet && bondOrders == bondingElectrons + 2 * lonePairs) || (expandedOctet && bondOrders == 6)) {
            return;
        }
        // making new bond
        float radius = 3f;
        GameObject cyl = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
        GameObject cylClone = Instantiate(cyl, Vector3.zero, Quaternion.identity);
        cylClone.transform.localScale = new Vector3(0.3f, radius / 2, 0.3f);
        cylClone.transform.SetParent(transform, true);
        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/ElementsElements/" + selectElement.element + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity);
        clone.transform.SetParent(cylClone.transform.GetChild(0), true);
        clone.name = clone.name + " " + num;
        cylClone.name = cylClone.name + " " + num;
        Debug.Log(bondCount);
        bondCount++;

        resetChildPositions(radius);

        clone.transform.localEulerAngles = cylClone.transform.localEulerAngles + new Vector3(180, 0, 0);
        clone.transform.localPosition = Vector3.up * -1;
        
        moveChildren(bondCount, start);
    }
    */
    
    public void SpawnElement(int num) {
        int bondCount = 0;
        int bondOrders = 0;
        
        foreach(Tuple<GameObject,GameObject> child in neighbors) {
            if(child.Item2.tag.Equals("Bond")) {
                bondCount++;
                foreach(Tuple<GameObject, GameObject> ch in child.Item2.GetComponent<Elements>().neighbors) {
                    if (ch.Item2.tag.Equals("Bond") && ch.Item2 != this) {
                        bondOrders++;
                    }
                }
            }
        }
        
        
        int start = 0;
        /*
        if(transform.parent != null && transform.parent.tag.Equals("Bond")) {
            bondCount++;
            start = 1;
            foreach(Transform ch in transform.parent.parent.transform) {
                if(ch.tag.Equals("Bond")) {
                    bondOrders++;
                }
            }
        }
        */
        // checking if the element can make more bonds
        if((!expandedOctet && bondOrders == bondingElectrons) || (expandedOctet && bondOrders == bondingElectrons + 2 * lonePairs) || (expandedOctet && bondOrders == 6)) {
            return;
        }
        // making new bond
        float radius = 3f;
        GameObject cyl = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
        GameObject cylClone = Instantiate(cyl, Vector3.zero, Quaternion.identity);
        cylClone.transform.localScale = new Vector3(0.3f, radius / 2, 0.3f);
        cylClone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);
        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + selectElement.element + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity);
        clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);
        clone.name = clone.name + " " + num;
        cylClone.name = cylClone.name + " " + num;
        Debug.Log(bondCount);
        bondCount++;

        resetChildPositions(radius);

        clone.transform.localEulerAngles = cylClone.transform.localEulerAngles + new Vector3(180, 0, 0);
        clone.transform.localPosition = Vector3.up * -1;
        
        moveChildren(bondCount, start);
        if (!cylClone)
        {
            Debug.Log("bond breok");
        }

        if (!clone)
        {
            Debug.Log("clone breok");
        }
        neighbors.Add(new Tuple<GameObject, GameObject>(cylClone, clone));

    }

    public void resetChildPositions(float radius) {
        foreach(Tuple<GameObject,GameObject> child in neighbors) {
            child.Item2.transform.localPosition = Vector3.zero;
            child.Item2.transform.localEulerAngles = new Vector3(180, 0, 0);
            child.Item2.transform.Translate(0, -1 * (radius / 2), 0);
        }
    }
    
    public void moveChildren(int bondCount, int start) {
        if(bondCount == 2) {
            neighbors[1-start].Item2.transform.RotateAround(transform.position, transform.forward, 180);
        }
        else if(bondCount == 3) {
            neighbors[1-start].Item2.transform.RotateAround(transform.position, transform.forward, 120);
            neighbors[2-start].Item2.transform.RotateAround(transform.position, transform.forward, 240);
        }
        else if(bondCount == 4) {
            for(int i = 1; i < 4; i++) {
                neighbors[i-start].Item2.transform.RotateAround(transform.position, transform.forward, 109);
            }
            neighbors[2-start].Item2.transform.RotateAround(transform.position, transform.forward, 109);
            neighbors[3-start].Item2.transform.RotateAround(transform.position, transform.forward, -109);
        }
        else if(bondCount == 5) {
            for(int i = 2; i < 5; i++) {
                neighbors[i-start].Item2.transform.RotateAround(transform.position, transform.forward, 90);
            }
            neighbors[1-start].Item2.transform.RotateAround(transform.position, transform.forward, 180);
            neighbors[3-start].Item2.transform.RotateAround(transform.position, transform.forward, 120);
            neighbors[4-start].Item2.transform.RotateAround(transform.position, transform.forward, -120);
        }
        else if(bondCount == 6) {
            neighbors[1-start].Item2.transform.RotateAround(transform.position, transform.forward, 180);
            neighbors[2-start].Item2.transform.RotateAround(transform.position, transform.forward, 90);
            neighbors[3-start].Item2.transform.RotateAround(transform.position, transform.forward, -90);
            neighbors[4-start].Item2.transform.RotateAround(transform.position, transform.forward, 90);
            neighbors[5-start].Item2.transform.RotateAround(transform.position, transform.forward, -90);
        }
    }

    public void DeleteElement() {
        if(!transform.parent.tag.Equals("Bond")) { // if root atom
            Destroy(gameObject);
        }
        else { // if not root atom
            // getting parent
            Elements parent = transform.parent.parent.parent.gameObject.GetComponent(typeof(Elements)) as Elements;
            // disconnecting this from parent
            transform.parent.parent.SetParent(null);
            // getting new bond count of parent
            int bondCount = 0;
            foreach(Transform child in parent.transform) {
                if(child.tag.Equals("Bond")) {
                    bondCount++;
                }
            }
            int start = 0;
            if(parent.transform.parent != null && parent.transform.parent.tag.Equals("Bond")) {
                bondCount++;
                start = 1;
            }
            Debug.Log(bondCount + " " + start);
            // moving parent's old atoms into place
            parent.resetChildPositions(3f);
            parent.moveChildren(bondCount, start);
            Destroy(transform.parent.parent.gameObject);
        }
    }
}
