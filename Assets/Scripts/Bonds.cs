using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bonds : MonoBehaviour
{
    public int bondOrder; // 1, 2, or 3 depending on single double or triple bond
    public Elements parent; // parent element of this bond
    public Elements child; // child element of this bond

    /// <summary>
    /// Start is called before the first frame update and initializes the materials of the bond
    /// </summary>
    void Start()
    {
        Material mat = Resources.Load<Material>("BondColor");
        Material invis = Resources.Load<Material>("Transparent");
        transform.GetComponent<MeshRenderer>().material = invis;
        foreach (Transform child in transform)
        {
            child.transform.GetComponent<Renderer>().material = mat; // This needs to loop for all children
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sets the parent and child Elements this Bond connects
    /// </summary>
    /// <param name="p">The parent Element of this Bond</param>
    /// <param name="c">The child Element of this Bond</param>
    public void SetElements(Elements p, Elements c) {
        Debug.Log("parent: " + p.name + "   child: " + c.name);
        parent = p;
        child = c;
    }

    /// <summary>
    /// Cycles the bond order of this Bond (i.e. 1 goes to 2, 2 goes to 3, and 3 goes back to 1)
    /// <br></br>
    /// If either the parent or child Element cannot support an increase in bond order, this method cycles back to 1
    /// </summary>
    /// <param name="num">The "construction ID" or number next to the name in the Hierarchy View</param>
    public void CycleBondOrder(int num) {
        int newOrder = bondOrder + 1;
        if(bondOrder == 3) {
            newOrder = 1;
        }

        // checking if upgrade possible
        if(!parent.CanBondMore() || !child.CanBondMore()) {
            if(bondOrder == 1) { // if current bond order is 1, there's nothing to cycle to
                return;
            }
            newOrder = 1;
        }

        // loading new bond asset
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
        GameObject newBond = newBond = Instantiate(obj, transform.position, Quaternion.identity, transform.parent);
        newBond.transform.localScale = transform.localScale;
        newBond.transform.localEulerAngles = transform.localEulerAngles;
        (newBond.GetComponent<Bonds>() as Bonds).SetElements(parent, child);
        newBond.name = newBond.name + " " + num;

        // updating parent and child neighbor lists
        parent.UpdateBond(newBond, child.gameObject);
        child.UpdateBond(newBond, parent.gameObject);

        // updating parent and child bond orders
        parent.bondOrders += (newOrder - bondOrder);
        child.bondOrders += (newOrder - bondOrder);

        // updating parent and child electron counts
        parent.UpdateElectrons(newOrder - bondOrder);
        child.UpdateElectrons(newOrder - bondOrder);

        // deleting this
        Destroy(gameObject);
    }
}
