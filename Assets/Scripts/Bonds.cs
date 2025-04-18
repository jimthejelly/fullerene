using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bonds : MonoBehaviour
{
    public int bondOrder; // 1, 2, or 3 depending on single double or triple bond

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown() {
        //CycleBondOrder();
    }

    public void CycleBondOrder(int num) {
        int newOrder = bondOrder + 1;
        if(bondOrder == 3) {
            newOrder = 1;
        }
        // checking if upgrade possible
        Elements p;
        Elements c;

        if(transform.parent.tag.Equals("Element")) { // if we're currently the parent bond
            p = transform.parent.gameObject.GetComponent(typeof(Elements)) as Elements;
            c = transform.GetChild(0).GetChild(0).gameObject.GetComponent(typeof(Elements)) as Elements;
        }
        else { // if we're currently the child bond
            p = transform.parent.parent.gameObject.GetComponent(typeof(Elements)) as Elements;
            c = transform.parent.GetChild(0).GetChild(0).gameObject.GetComponent(typeof(Elements)) as Elements;
        }
        // checking if parent can make more bonds
        int bondCount = 0;
        foreach(Transform child in p.transform) {
            if(child.tag.Equals("Bond")) {
                foreach(Transform ch in child.transform) {
                    if(ch.tag.Equals("Bond")) {
                        bondCount++;
                    }
                }
            }
        }
        if(p.transform.parent != null && p.transform.parent.tag.Equals("Bond")) {
            bondCount++;
        }
        // checking if the element can make more bonds
        if((!p.expandedOctet && bondCount == p.bondingElectrons) || (p.expandedOctet && bondCount == p.bondingElectrons + 2 * p.lonePairs)) {
            Debug.Log("parent can't make more " + bondCount);
            newOrder = 1;
        }
        else {
            Debug.Log("parent CAN make more " + bondCount);
        }
        // checking if child can make more bonds
        bondCount = bondOrder;
        foreach(Transform child in c.transform) {
            if(child.tag.Equals("Bond")) {
                foreach(Transform ch in child.transform) {
                    if(ch.tag.Equals("Bond")) {
                        bondCount++;
                    }
                }
            }
        }

        // checking if the element can make more bonds
        if((!c.expandedOctet && bondCount == c.bondingElectrons) || (c.expandedOctet && bondCount == c.bondingElectrons + 2 * c.lonePairs)) {
            Debug.Log("child can't make more " + bondCount);
            newOrder = 1;
        }
        else {
            Debug.Log("child CAN make more " + bondCount);
        }
        Debug.Log("new order " + newOrder);
        GameObject obj = null;
        if(newOrder == 1) {
            obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
        }
        else if(newOrder == 2) {
            obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/DoubleBond.prefab", typeof(GameObject)) as GameObject;
        }
        else {
            obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/TripleBond.prefab", typeof(GameObject)) as GameObject;
        }
        GameObject newBond;
        if(transform.parent.tag.Equals("Element")) { // if we're currently the parent bond
            newBond = Instantiate(obj, transform.position, Quaternion.identity, p.transform);
            newBond.transform.localScale = transform.localScale;
            newBond.transform.localEulerAngles = transform.localEulerAngles;
        }
        else { // if we're currently the child bond
            newBond = Instantiate(obj, transform.parent.position, Quaternion.identity, p.transform);
            newBond.transform.localScale = transform.parent.localScale;
            newBond.transform.localEulerAngles = transform.parent.localEulerAngles;
        }
        newBond.name = newBond.name + " " + num;
        c.transform.SetParent(newBond.transform.GetChild(0).transform);
        if(transform.parent.tag.Equals("Element")) { // if we're currently the parent bond
            Destroy(gameObject);
        }
        else { // if we're currently the child bond
            Destroy(transform.parent.gameObject);
        }
;    }
}
