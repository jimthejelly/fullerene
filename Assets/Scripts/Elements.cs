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

    int bondCount = 0;
    int bondOrders = 0;
    int start = 0;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// Helper function for finding the construction order of Elements, its main use is finding the parent of an Element
    /// </summary>
    /// <returns>The number to the right of the name of the element, or -1 if it's the root</returns>
    int getID() {
        if(name[name.Length - 1] == ')') {
            return -1;
        }
        return Int32.Parse(name.Substring(name.LastIndexOf(' ')));
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

        bondCount = 0;
        bondOrders = 0;

        for(int i = 0; i < neighbors.Count(); i++) {
            bondCount++;
            bondOrders++;
        }

        start = 0;

        if(neighbors.Any()) {
            start = 1;
        }

        // checking if the element can make more bonds
        if((!expandedOctet && bondOrders == bondingElectrons) || (expandedOctet && bondOrders == bondingElectrons + 2 * lonePairs) || (expandedOctet && bondOrders == 6)) {
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
        neighbors.Add(new Tuple<GameObject, GameObject>(cylClone, clone));
        clone.GetComponent<Elements>().neighbors.Add(new Tuple<GameObject, GameObject>(cylClone, gameObject));

        resetChildPositions(radius);

        clone.transform.localEulerAngles = cylClone.transform.localEulerAngles; //+ this.transform.localEulerAngles;

        clone.transform.localPosition = cylClone.transform.localPosition;
        clone.transform.Translate(0, -radius / 2, 0);

        moveChildren(bondCount, start);


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
    /// </summary>
    /// <param name="bondCount">The number of bonds the current Element has (does nothing with bonds < 2 or bonds > 6</param>
    /// <param name="start">An offset variable that ensures moveChildren() will never move the "parent" Element</param>
    public void moveChildren(int bondCount, int start) {
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
                if((bond.Item2.GetComponent(typeof(Elements)) as Elements).getID() == -1
                    || (bond.Item2.GetComponent(typeof(Elements)) as Elements).getID() < this.getID()) {
                    parent = bond.Item2;
                }
                foreach(Tuple<GameObject, GameObject> t in (bond.Item2.GetComponent(typeof(Elements)) as Elements).neighbors) {
                    if(t.Item2.Equals(gameObject)) {
                        (bond.Item2.GetComponent(typeof(Elements)) as Elements).neighbors.Remove(t);
                        //Debug.Log("removing " + t.Item1.name);
                        break;
                    }
                }
            }
            foreach(Tuple<GameObject, GameObject> t in (parent.gameObject.GetComponent(typeof(Elements)) as Elements).neighbors) {
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
                else if(t.tag.Equals("Element") && (t.gameObject.GetComponent(typeof(Elements)) as Elements).neighbors.Count() > 1) {
                    (t.gameObject.GetComponent(typeof(Elements)) as Elements).resetChildPositions(3f);
                    (t.gameObject.GetComponent(typeof(Elements)) as Elements).moveChildren(
                        (t.gameObject.GetComponent(typeof(Elements)) as Elements).neighbors.Count(), (t.gameObject.GetComponent(typeof(Elements)) as Elements).start);
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
        foreach(Tuple<GameObject, GameObject> bond in (current.GetComponent(typeof(Elements)) as Elements).neighbors) {
            if(!found.Contains(bond.Item1)) { // if this bond has not been travelled already
                Debug.Log("added " + bond.Item1.name);
                found.Add(bond.Item1);
                deletionDFS(bond.Item2, found);
            }
        }
    }
}