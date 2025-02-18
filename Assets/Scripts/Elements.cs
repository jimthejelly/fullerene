using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Class for representing the internal data of an atom in the main molecule
/// </summary>
public class Elements : MonoBehaviour {

    public int electrons;
    public int protons;
    public int neutrons;
    private List<Tuple<GameObject, GameObject>> neighbors = new List<Tuple<GameObject, GameObject>>();
    public int lonePairs;
    public int bondingElectrons;
    public bool expandedOctet;
    public bool physicsOn = false;

    public int bondCount = 0;
    public int bondOrders = 0;
    int start = 0;
    // Start is called before the first frame update
    void Start()
    {
        string name = transform.name;
        string[] split = name.Split('-');
        string nameNumber = split[0].Trim();
        Material mat = null;
        switch (nameNumber) // Needs cases for every possible color gonna be yikes moment for me 
        {
            
            case "1":
                mat = Resources.Load<Material>("Materials/Hydrogen");
                break;
            case "6":
                mat = Resources.Load<Material>("Materials/Carbon");
                break;
            case "7":
                mat = Resources.Load<Material>("Materials/Nitrogen");
                break;
            case "8":
                mat = Resources.Load<Material>("Materials/Oxygen");
                break;
            case "15":
                mat = Resources.Load<Material>("Materials/Phosphorus");
                break;
            case "16":
                mat = Resources.Load<Material>("Materials/Sulfur");
                break;
            case "9" or "17":
                mat = Resources.Load<Material>("Materials/Fluorine_Chlorine");
                break;
            case "22":
                mat = Resources.Load<Material>("Materials/Titanium");
                break;
            case "26":
                mat = Resources.Load<Material>("Materials/Iron");
                break;
            case "35":
                mat = Resources.Load<Material>("Materials/Bromine");
                break;
            case "53":
                mat = Resources.Load<Material>("Materials/Iodine");
                break;
            case "2" or "10" or "18" or "36" or "54" or "86":
                mat = Resources.Load<Material>("Materials/Noble_Gases");
                break;
            case "3" or "11" or "19" or "37" or "55" or "87":
                mat = Resources.Load<Material>("Materials/Alkali_Metals");
                break;
            case "4" or "12" or "20" or "38" or "56" or "88":
                mat = Resources.Load<Material>("Materials/Alkaline_Earth_Metals");
                break;
            case "5" or "21" or "22" or "23" or "24" or "25" or "26" or "27" or "28" or "29" or "30" or "39" or "40" or "41" or "42" or "44" or "45" or "46" or "47" or "48" or "72" or "73" or "74" or "75" or "76" or "77" or "78" or "79" or "80":
                // Yeah imma figure out how to make these if statements rn I want all the #s down
                mat = Resources.Load<Material>("Materials/Boron_Transition_Metals");
                break;
            default:
                mat = Resources.Load<Material>("Materials/Other"); // Probably should make a error material
                break;
        }
        transform.GetComponent<Renderer>().material = mat;
    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// Finds the construction order of Elements, its main use is finding the parent of an Element
    /// </summary>
    /// <returns>The number to the right of the name of the element, or -1 if it's the root</returns>
    public int getID() {
        if(name[name.Length - 1] == ')') {
            return -1;
        }
        return Int32.Parse(name.Substring(name.LastIndexOf(' ')));
    }

    /// <summary>
    /// Determines whether or not this Element can make more bonds
    /// </summary>
    /// <returns>True if this Element can make more bonds, or False if it can't</returns>
    public bool canBondMore() {
        if(bondingElectrons + 2 * lonePairs < 6) {
            return (!expandedOctet && bondOrders < bondingElectrons) || (expandedOctet && bondOrders < bondingElectrons + 2 * lonePairs);
        }
        return (!expandedOctet && bondOrders < bondingElectrons) || (expandedOctet && bondOrders < 6);
    }

    /// <summary>
    /// Spawns in a new Element instance
    /// </summary>
    /// <param name="num">The "construction ID" or number next to the name in the Hierarchy View</param>
    public void SpawnElement(int num) {
        // int bondCount = 0;
        // int bondOrders = 0;
        Debug.Log("neighbor num: " + neighbors.Count);
        protons = 16;
        electrons = 16;
        neutrons = 0;

        start = 0;

        if(neighbors.Any()) {
            start = 1;
        }

        // checking if the element can make more bonds
        if(!canBondMore()) {
            Debug.Log("Not enough space");
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
        bondCount++;
        bondOrders++;

        (clone.GetComponent<Elements>() as Elements).bondCount = 1;
        (clone.GetComponent<Elements>() as Elements).bondOrders = 1;

        (cylClone.GetComponent<Bonds>() as Bonds).setElements(this, clone.GetComponent<Elements>() as Elements);

        neighbors.Add(new Tuple<GameObject, GameObject>(cylClone, clone));
        clone.GetComponent<Elements>().neighbors.Add(new Tuple<GameObject, GameObject>(cylClone, gameObject));
        resetChildPositions(radius);

        clone.transform.localEulerAngles = cylClone.transform.localEulerAngles; //+ this.transform.localEulerAngles;

        clone.transform.localPosition = cylClone.transform.localPosition;
        clone.transform.Translate(0, -radius / 2, 0);

        moveChildren(start);

        if(!cylClone) {
            Debug.Log("bond breok");
        }

        if(!clone) {
            Debug.Log("clone breok");
        }

    }

    /// <summary>
    /// Resets the positions of each "child" of this Element to be moved with moveChildren() later
    /// </summary>
    /// <param name="radius">The radius of the bonds between Elements</param>
    public void resetChildPositions(float radius) {

        foreach(Tuple<GameObject, GameObject> child in neighbors) {
            if(gameObject == creationUser.head) {
                child.Item1.transform.localPosition = transform.position;
                child.Item1.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
                child.Item1.transform.Translate(0, -1 * (radius / 2), 0);

                child.Item2.transform.localPosition = transform.position;
                child.Item2.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
                child.Item2.transform.Translate(0, -1 * (radius), 0);
            }
            else if(!Equals(child, neighbors[0])) {

                child.Item1.transform.localPosition = transform.position;
                child.Item1.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z + 180);
                child.Item1.transform.Translate(0, -1 * (radius / 2), 0);

                // // child.Item1.transform.RotateAround(transform.position, transform.forward, 180);
                // // child.Item1.transform.RotateAround(transform.position, transform.up, 180);
                // // child.Item1.transform.RotateAround(transform.position, transform.right, 180);

                child.Item2.transform.localPosition = transform.position;
                child.Item2.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z + 180);
                child.Item2.transform.Translate(0, -1 * (radius), 0);

                // child.Item2.transform.RotateAround(transform.position, transform.forward, 180);
                // child.Item2.transform.RotateAround(transform.position, transform.up, 180);
                // child.Item2.transform.RotateAround(transform.position, transform.right, 180);

            }
            else {

            }

        }
    }

    /// <summary>
    /// Moves the "children" of this Element to their proper VSEPR geometrical positions (Does not currently account for lone pairs)
    /// <br></br>
    /// NOTE: This does not currently work with cyclic molecules
    /// </summary>
    /// <param name="bondCount">The number of bonds the current Element has (does nothing with bonds less than 2 or bonds greater than 6</param>
    /// <param name="start">An offset variable that ensures moveChildren() will never move the "parent" Element</param>
    public void moveChildren(int start) {
        if(bondCount == 2) {
            // if (gameObject == creationUser.head)
            // {
            neighbors[neighbors.Count() - 1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
            neighbors[neighbors.Count() - 1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
            // }

            Debug.Log("index: " + (1 - start) + "   name: " + neighbors[1 - start].Item2.name);
        }
        else if(bondCount == 3) {
            if(gameObject == creationUser.head) {
                for(int i = 1; i < 3; i++) {
                    neighbors[i - start].Item1.transform.RotateAround(transform.position, transform.forward, 120 * i);
                    neighbors[i - start].Item2.transform.RotateAround(transform.position, transform.forward, 120 * i);
                }
            }
            else {
                for(int i = 2; i < 4; i++) {
                    neighbors[i - start].Item1.transform.RotateAround(transform.position, transform.forward, 120 * (i - 1));
                    neighbors[i - start].Item2.transform.RotateAround(transform.position, transform.forward, 120 * (i - 1));
                }
            }

        }
        else if(bondCount == 4) {
            if(gameObject == creationUser.head) {
                for(int i = 1; i < 4; i++) {
                    neighbors[i - start].Item1.transform.RotateAround(transform.position, transform.forward, 120);
                    neighbors[i - start].Item2.transform.RotateAround(transform.position, transform.forward, 120);

                    neighbors[i - start].Item1.transform.RotateAround(transform.position, transform.up, 120 * i);
                    neighbors[i - start].Item2.transform.RotateAround(transform.position, transform.up, 120 * i);

                }
            }
            else {
                for(int i = 1; i < 4; i++) {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 120);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 120);

                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 120 * i);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 120 * i);

                }
            }
        }
        else if(bondCount == 5) {
            if(gameObject == creationUser.head) {
                neighbors[1 - start].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1 - start].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                for(int i = 2; i < 5; i++) {
                    neighbors[i - start].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                    neighbors[i - start].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                    neighbors[i - start].Item1.transform.RotateAround(transform.position, transform.up, 120 * i);
                    neighbors[i - start].Item2.transform.RotateAround(transform.position, transform.up, 120 * i);

                }
            }
            else {
                neighbors[1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                for(int i = 2; i < 5; i++) {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 120 * i);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 120 * i);

                }
            }
        }
        else if(bondCount == 6) {
            if(gameObject == creationUser.head) {
                neighbors[1 - start].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1 - start].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                for(int i = 2; i < 6; i++) {
                    neighbors[i - start].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                    neighbors[i - start].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                    neighbors[i - start].Item1.transform.RotateAround(transform.position, transform.up, 90 * i);
                    neighbors[i - start].Item2.transform.RotateAround(transform.position, transform.up, 90 * i);

                }
            }
            else {
                neighbors[1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                for(int i = 2; i < 6; i++) {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 90 * i);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 90 * i);

                }
            }
        }
        // recursively move all grandchildren
        foreach(Tuple<GameObject, GameObject> bond in neighbors) {
            if(gameObject != creationUser.head && Equals(bond, neighbors[0])) {
                continue;
            }
            Elements childElement = bond.Item2.GetComponent<Elements>() as Elements;
            childElement.resetChildPositions(3f);
            childElement.moveChildren(1);
        }
    }

    /// <summary>
    /// Deletes the Element selected in creationUser
    /// </summary>
    public void DeleteElement() {
        if(getID() == -1) {
            foreach(Transform item in transform.parent) {
                Destroy(item.gameObject);
            }
        }
        else {
            // finding the parent and deleting neighbors' bonds
            GameObject parent = null;
            foreach(Tuple<GameObject, GameObject> bond in neighbors) {
                if((bond.Item2.GetComponent<Elements>() as Elements).getID() == -1
                    || (bond.Item2.GetComponent<Elements>() as Elements).getID() < this.getID()) {
                    parent = bond.Item2;
                }
                foreach(Tuple<GameObject, GameObject> t in (bond.Item2.GetComponent<Elements>() as Elements).neighbors) {
                    if(t.Item2.Equals(gameObject)) {
                        (bond.Item2.GetComponent<Elements>() as Elements).neighbors.Remove(t);
                        (bond.Item2.GetComponent<Elements>() as Elements).bondCount--;
                        (bond.Item2.GetComponent<Elements>() as Elements).bondOrders -= (bond.Item1.GetComponent<Bonds>() as Bonds).bondOrder;
                        ;
                        //Debug.Log("removing " + t.Item1.name);
                        break;
                    }
                }
            }
            foreach(Tuple<GameObject, GameObject> t in (parent.gameObject.GetComponent<Elements>() as Elements).neighbors) {
                Debug.Log("still got " + t.Item1.name);
            }
            // pathfinding from parent and adding every object pathed through to found
            if(parent == null) { // if parent is still null, that means the parent couldn't be found (an error occurred somewhere)
                Debug.Log("couldn't find parent");
                return;
            }
            HashSet<GameObject> found = new HashSet<GameObject>();
            deletionDFS(parent, found);
            // looping through all objects and deleting any that haven't been found
            foreach(Transform t in transform.parent) {
                if(!found.Contains(t.gameObject)) {
                    Destroy(t.gameObject);
                }
                else if(t.tag.Equals("Element") && (t.gameObject.GetComponent<Elements>() as Elements).neighbors.Count() > 1) {
                    (t.gameObject.GetComponent<Elements>() as Elements).resetChildPositions(3f);
                    (t.gameObject.GetComponent<Elements>() as Elements).moveChildren((t.gameObject.GetComponent<Elements>() as Elements).start);
                }
            }


            Destroy(gameObject);
        }
    }

    /// <summary>
    /// DFS algorithm for marking GameObjects for deletion
    /// </summary>
    /// <param name="current">The current GameObject (Element) being checked by the algorithm</param>
    /// <param name="found">The list of all GameObjects that have been found by the algorithm</param>
    private void deletionDFS(GameObject current, HashSet<GameObject> found) {
        if(found.Contains(current)) { // if this element has been visited already, return to the last one
            return;
        }
        Debug.Log("added " + current.name);
        found.Add(current);
        foreach(Tuple<GameObject, GameObject> bond in (current.GetComponent<Elements>() as Elements).neighbors) {
            if(!found.Contains(bond.Item1)) { // if this bond has not been travelled already
                Debug.Log("added " + bond.Item1.name);
                found.Add(bond.Item1);
                deletionDFS(bond.Item2, found);
            }
        }
    }

    /// <summary>
    /// Updates a bond GameObject being pointed to in neighbors whenever its bond order is cycled
    /// </summary>
    /// <param name="newBond">The new bond GameObject to be referenced in neighbors</param>
    /// <param name="otherElement">The other element the bond connects to - used to find which Tuple to replace</param>
    public void updateBond(GameObject newBond, GameObject otherElement) {
        for(int i = 0; i < neighbors.Count; i++) {
            if(Equals(neighbors[i].Item2, otherElement)) {
                neighbors.Insert(i, new Tuple<GameObject, GameObject>(newBond, otherElement));
                neighbors.RemoveAt(i + 1);
                return;
            }
        }
        
    }
}