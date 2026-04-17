using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bonds : MonoBehaviour
{
    /// <summary> 1, 2, or 3 depending on if this <c> Bond </c> is a single, double, or triple bond respectively</summary>
    public int bondOrder;
    /// <summary> The parent <see cref="Elements"/> of this <c> Bond </c></summary>
    public Elements parent;
    /// <summary> The child <see cref="Elements"/> of this <c> Bond </c></summary>
    public Elements child;

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
    {}

    /// <summary>
    /// Sets the parent and child <see cref="Elements"/> this Bond connects
    /// </summary>
    /// <param name="p">The parent <see cref="Elements"/> of this <c> Bond </c></param>
    /// <param name="c">The child <see cref="Elements"/> of this <c> Bond </c></param>
    public void SetElements(Elements p, Elements c) {
        Debug.Log("parent: " + p.name + "   child: " + c.name);
        parent = p;
        child = c;
    }

    /// <summary>
    /// Cycles the bond order of this <c> Bond </c> (i.e. 1 goes to 2, 2 goes to 3, and 3 goes back to 1)
    /// <br></br>
    /// If either the parent or child <c> Element </c> cannot support an increase in bond order, this method cycles back to 1
    /// </summary>
    /// <param name="num">The "construction ID" or number next to the name in the Hierarchy View</param>
    public void CycleBondOrder(int num) => SetBondOrder((bondOrder + 1) % 3, num);
    
    public void SetBondOrder(int newOrder, int num) {

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
        newBond.GetComponent<Bonds>().SetElements(parent, child);
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

    /// <summary>
    /// Updates the position and length of this <c> Bond </c> based on the locations of <see cref="parent"/> and <see cref="child"/> this frame
    /// </summary>
    public void UpdatePosition() {
        // setting bond position
        Vector3 parentPos = parent.transform.position;
        Vector3 childPos = child.transform.position;
        transform.position = new Vector3((parentPos.x + childPos.x) / 2, (parentPos.y + childPos.y) / 2, (parentPos.z + childPos.z) / 2);

        // setting bond rotation
        transform.LookAt(parentPos);
        transform.Rotate(90, 0, 0);

        // setting bond length
        transform.localScale = new Vector3(0.15f, Mathf.Sqrt(
            Mathf.Pow(parentPos.x - childPos.x, 2) + Mathf.Pow(parentPos.y - childPos.y, 2) + Mathf.Pow(parentPos.z - childPos.z, 2)) / 2, 0.15f);
    }
}
