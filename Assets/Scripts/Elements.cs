using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.Services.Analytics;

/// <summary>
/// Class for representing the internal data of an atom in the main molecule
/// </summary>
public class Elements : MonoBehaviour
{
    /// <summary> The number of electrons in this <c> Element </c> </summary>
    public int electrons;
    /// <summary> The number of protons in this <c> Element </c> </summary>
    public int protons;
    /// <summary> The number of neutrons in this <c> Element </c> </summary>
    /// <remarks> Will be useful when isotopes come into the mix, currently unused </remarks>
    public int neutrons;
    /// <summary> A <c> List </c> of <c> Tuple</c>s of <see cref="Bonds"/> and <see cref="Elements"/> storing which atoms are bonded by which bonds to this <c> Element </c></summary>
    private List<Tuple<GameObject, GameObject>> neighbors = new List<Tuple<GameObject, GameObject>>();
    /// <summary> The number of <see cref="LonePairs"/> this <c> Element </c> started with </summary>
    public int defaultLonePairs;
    /// <summary> The number of <see cref="LonePairs"/> currently in this <c> Element </c> </summary>
    public int lonePairs;
    /// <summary> The number of bonding electrons in this <c> Element </c> </summary>
    public int bondingElectrons;
    /// <summary> Whether or not this <c> Element </c> can expand its octet </summary>
    public bool expandedOctet;

    /// <summary> The number of <see cref="Bonds"/> bonded to this <c> Element </c> </summary>
    public int bondCount = 0;
    /// <summary> The total bond order of <see cref="Bonds"/> bonded to this <c> Element </c> </summary>
    public int bondOrders = 0;

    /// <summary> Whether or not this <c> Element </c> has moved yet this frame </summary>
    public bool hasMoved = false;

    /// <summary> The covalent radius of this <c> Element </c> </summary>
    public float covalentRadius;
    /// <summary> The epsilon value of this <c> Element </c> </summary>
    /// <remarks> Used for the Lennard Jones potential equation to approximate force interactions of atoms in the molecule </remarks>
    public float epsilon;
    /// <summary> The sigma value of this <c> Element </c> </summary>
    /// <remarks> Used for the Lennard Jones potential equation to approximate force interactions of atoms in the molecule </remarks>
    public float sigma;

    /// <summary> The force vector this <c> Element </c> will feel in the current frame </summary>
    public Vector3 forceVector = Vector3.zero;
    /// <summary> The force vector this <c> Element </c> felt in the previous frame </summary>
    /// <remarks> Used to lessen oscillation when approaching equilibrium (doesn't really work yet) </remarks>
    private Vector3 oldForceVector = Vector3.zero;

    /// <summary> An offset variable that ensures this <c> Element </c> will never attempt to move its parent </summary>
    int offset = 0;

    /// <summary>
    /// Start is called before the first frame update and initializes the material of the Element
    /// </summary>
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
    void Update()
    {}

    /// <summary>
    /// Finds the construction order of <c> Element </c>s, its main use is finding the parent of an <c> Element </c>
    /// </summary>
    /// <returns>The number to the right of the name of the element, or -1 if it's the root</returns>
    public int GetID()
    {
        if (name[name.Length - 1] == ')')
        {
            return -1;
        }
        return Int32.Parse(name.Substring(name.LastIndexOf(' ')));
    }

    /// <summary>
    /// Determines whether or not this <c> Element </c> can make more bonds
    /// </summary>
    /// <returns>True if this <c> Element </c> can make more bonds, or False if it can't</returns>
    public bool CanBondMore()
    {
        if (expandedOctet)
        {
            return bondingElectrons + 2 * lonePairs > 0;
        }
        return bondingElectrons > 0;
    }

    /// <summary>
    /// Subtracts <c> change </c> from <see cref="bondingElectrons"/>, converting lone pairs to bonding electrons if necessary
    /// </summary>
    /// <param name="change">The change in bonding electrons</param>
    public void UpdateElectrons(int change)
    {
        if (change > 0)
        {
            while (bondingElectrons < change)
            {
                lonePairs--;
                bondingElectrons += 2;
            }
        }
        bondingElectrons -= change;
    }

    /// <summary>
    /// Accessor method for <see cref="neighbors"/>
    /// </summary>
    /// <returns><see cref="neighbors"/></returns>
    public List<Tuple<GameObject, GameObject>> GetNeighbors()
    {
        return neighbors;
    }

    /// <summary>
    /// Spawns in a new <see cref="Elements"/> instance
    /// </summary>
    /// <param name="num">The "construction ID" or number next to the name in the Hierarchy View</param>
    public void SpawnElement(int num)
    {
        Debug.Log("neighbor num: " + neighbors.Count);

        offset = 0;

        if (neighbors.Any())
        {
            offset = 1;
        }

        // checking if the element can make more bonds
        if (!CanBondMore())
        {
            Debug.Log("Not enough space");
            return;
        }
        // making new bond
        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + selectElement.element + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity);
        // making sure the newly created atom can bond
        if (!clone.GetComponent<Elements>().CanBondMore())
        {
            Debug.Log(selectElement.element + " is inert!");
            return;
        }
        // setting initial (theoretical) length of bond
        float radius = covalentRadius + clone.GetComponent<Elements>().covalentRadius;
        // creating bond and element objects
        GameObject cyl = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
        GameObject cylClone = Instantiate(cyl, Vector3.zero, Quaternion.identity);
        // setting bond and element parameters
        cylClone.transform.localScale = new Vector3(0.15f, radius / 2, 0.15f);
        cylClone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);
        clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);
        clone.name = clone.name + " " + num;
        cylClone.name = cylClone.name + " " + num;
        bondCount++;
        bondOrders++;
        UpdateElectrons(1);

        clone.GetComponent<Elements>().bondCount = 1;
        clone.GetComponent<Elements>().bondOrders = 1;
        clone.GetComponent<Elements>().UpdateElectrons(1);

        cylClone.GetComponent<Bonds>().SetElements(this, clone.GetComponent<Elements>());

        neighbors.Add(new Tuple<GameObject, GameObject>(cylClone, clone));
        clone.GetComponent<Elements>().neighbors.Add(new Tuple<GameObject, GameObject>(cylClone, gameObject));
        ResetChildPositions();
        // setting element and bond positions and angles
        clone.transform.localEulerAngles = cylClone.transform.localEulerAngles;

        clone.transform.localPosition = cylClone.transform.localPosition;
        clone.transform.Translate(0, -radius / 2, 0);

        MoveChildren(offset);

        if (!cylClone)
        {
            Debug.Log("bond breok");
        }

        if (!clone)
        {
            Debug.Log("clone breok");
        }

    }

    /// <summary>
    /// Resets the positions of each "child" of this <c> Element </c> to be moved with <see cref="MoveChildren(int)"/> later
    /// </summary>
    public void ResetChildPositions()
    {

        foreach (Tuple<GameObject, GameObject> child in neighbors)
        {
            float radius = covalentRadius + child.Item2.GetComponent<Elements>().covalentRadius;
            
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

                child.Item2.transform.localPosition = transform.position;
                child.Item2.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z + 180);
                child.Item2.transform.Translate(0, -1 * (radius), 0);


            }
            else {

            }

        }
    }

    /// <summary>
    /// Moves the "children" of this <c> Element </c> to their proper VSEPR geometrical positions
    /// </summary>
    /// <remarks>
    /// NOTE: This does not currently work with cyclic molecules
    /// </remarks>
    /// <param name="offset">Ensures this <c> Element </c> will never try to move its parent</param>
    public void MoveChildren(int offset)
    {
        if (bondCount < 2)
        {
            return;
        }
        int bonds = bondCount + lonePairs;
        Debug.Log("bonds: " + bonds);
        if (bonds == 2)
        {
            neighbors[neighbors.Count() - 1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
            neighbors[neighbors.Count() - 1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
      
            Debug.Log("index: " + (1 - offset) + "   name: " + neighbors[1 - offset].Item2.name);
        }
        else if (bonds == 3)
        {
            if (gameObject == creationUser.head)
            {
                for (int i = 1; i < 3 - lonePairs; i++)
                {
                    neighbors[i - offset].Item1.transform.RotateAround(transform.position, transform.forward, 120 * i);
                    neighbors[i - offset].Item2.transform.RotateAround(transform.position, transform.forward, 120 * i);
                }
            }
            else
            {
                for (int i = 2; i < 4 - lonePairs; i++)
                {
                    neighbors[i - offset].Item1.transform.RotateAround(transform.position, transform.forward, 120 * (i - 1));
                    neighbors[i - offset].Item2.transform.RotateAround(transform.position, transform.forward, 120 * (i - 1));
                }
            }

        }
        else if (bonds == 4)
        {
            if (gameObject == creationUser.head)
            {
                for (int i = 1; i < 4 - lonePairs; i++)
                {
                    neighbors[i - offset].Item1.transform.RotateAround(transform.position, transform.forward, 120);
                    neighbors[i - offset].Item2.transform.RotateAround(transform.position, transform.forward, 120);

                    neighbors[i - offset].Item1.transform.RotateAround(transform.position, transform.up, 120 * i);
                    neighbors[i - offset].Item2.transform.RotateAround(transform.position, transform.up, 120 * i);

                }
            }
            else
            {
                for (int i = 1; i < 4 - lonePairs; i++)
                {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 120);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 120);

                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 120 * i);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 120 * i);

                }
            }
        }
        else if (bonds == 5)
        {
            if (gameObject == creationUser.head)
            {
                neighbors[1 - offset].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1 - offset].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                for (int i = 2; i < 5 - lonePairs; i++)
                {
                    neighbors[i - offset].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                    neighbors[i - offset].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                    neighbors[i - offset].Item1.transform.RotateAround(transform.position, transform.up, 120 * i);
                    neighbors[i - offset].Item2.transform.RotateAround(transform.position, transform.up, 120 * i);

                }
            }
            else
            {
                neighbors[1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                for (int i = 2; i < 5 - lonePairs; i++)
                {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 120 * i);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 120 * i);

                }
            }
        }
        else if (bonds == 6)
        {
            if (gameObject == creationUser.head)
            {
                neighbors[1 - offset].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1 - offset].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                for (int i = 2; i < 6 - lonePairs; i++)
                {
                    neighbors[i - offset].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                    neighbors[i - offset].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                    neighbors[i - offset].Item1.transform.RotateAround(transform.position, transform.up, 90 * i);
                    neighbors[i - offset].Item2.transform.RotateAround(transform.position, transform.up, 90 * i);
                }
                if (lonePairs == 2)
                { // fixing square planar geometry
                    neighbors[2].Item1.transform.RotateAround(transform.position, transform.up, 90);
                    neighbors[2].Item2.transform.RotateAround(transform.position, transform.up, 90);
                }
            }
            else
            {
                neighbors[1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                for (int i = 2; i < 6 - lonePairs; i++)
                {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 90 * i);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 90 * i);
                }
                if (lonePairs == 2)
                { // fixing square planar geometry
                    neighbors[3].Item1.transform.RotateAround(transform.position, transform.up, 90);
                    neighbors[3].Item2.transform.RotateAround(transform.position, transform.up, 90);
                }
            }
        }
        else if (bonds == 7)
        {
            if (gameObject == creationUser.head)
            {
                for (int i = 0; i < 6 - lonePairs; i++)
                {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 72 * i);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 72 * i);
                }
                if (lonePairs < 2)
                {
                    neighbors[5].Item1.transform.RotateAround(transform.position, transform.right, 90);
                    neighbors[5].Item2.transform.RotateAround(transform.position, transform.right, 90);
                    if (lonePairs < 1)
                    {
                        neighbors[6].Item1.transform.RotateAround(transform.position, transform.right, -90);
                        neighbors[6].Item2.transform.RotateAround(transform.position, transform.right, -90);
                    }
                }
            }
            else
            {
                for (int i = 1; i < 6 - lonePairs; i++)
                {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 72 * i);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 72 * i);
                }
                if (lonePairs < 2)
                {
                    neighbors[5].Item1.transform.RotateAround(transform.position, transform.right, 90);
                    neighbors[5].Item2.transform.RotateAround(transform.position, transform.right, 90);
                    if (lonePairs < 1)
                    {
                        neighbors[6].Item1.transform.RotateAround(transform.position, transform.right, -90);
                        neighbors[6].Item2.transform.RotateAround(transform.position, transform.right, -90);
                    }
                }
            }
        }
        else if (bonds == 8)
        {
            // TODO: should probably fix this, it's eyeballed without proper mathed-out angles (and also is only square antiprismal)
            // getting an axis for rotating things at a 60 degree offset from transform.right
            Vector3 newAxis = Vector3.RotateTowards(transform.right, transform.up, Mathf.PI / 6, 0);
            for (int i = 4; i < 8; i++)
            {
                neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 60);
                neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 60);

                neighbors[i].Item1.transform.RotateAround(transform.position, newAxis, 45);
                neighbors[i].Item2.transform.RotateAround(transform.position, newAxis, 45);
            }
            for (int i = 0; i < 8; i++)
            {
                neighbors[i].Item1.transform.RotateAround(transform.position, newAxis, 90 * i);
                neighbors[i].Item2.transform.RotateAround(transform.position, newAxis, 90 * i);
            }
        }
        else
        {
            // TODO: implement coordination numbers up to 16.
            // This is way unimportant right now given coordination numbers above 8 aren't possible with just valence electrons/lone pairs
        }
        // recursively move all grandchildren
        foreach (Tuple<GameObject, GameObject> bond in neighbors)
        {
            if (gameObject != creationUser.head && Equals(bond, neighbors[0]))
            {
                continue;
            }
            Elements childElement = bond.Item2.GetComponent<Elements>();
            childElement.ResetChildPositions();
            childElement.MoveChildren(1);
        }
    }

    /// <summary>
    /// Deletes the <c> Element </c> selected in creationUser
    /// </summary>
    /// <remarks>
    /// NOTE: Doesn't work with cyclic molecules
    /// </remarks>
    public void DeleteElement()
    {
        // If molecule head, just delete everything
        if (GetID() == -1)
        {
            foreach (Transform item in transform.parent)
            {
                Destroy(item.gameObject);
            }
        }
        else
        {
            // finding the parent and deleting neighbors' bonds
            GameObject parent = null;
            foreach (Tuple<GameObject, GameObject> bond in neighbors)
            {
                if (bond.Item2.GetComponent<Elements>().GetID() == -1
                    || bond.Item2.GetComponent<Elements>().GetID() < this.GetID())
                {
                    parent = bond.Item2;
                }
                foreach (Tuple<GameObject, GameObject> t in bond.Item2.GetComponent<Elements>().neighbors)
                {
                    if (t.Item2.Equals(gameObject))
                    {
                        bond.Item2.GetComponent<Elements>().neighbors.Remove(t);
                        bond.Item2.GetComponent<Elements>().bondCount--;
                        bond.Item2.GetComponent<Elements>().bondOrders -= bond.Item1.GetComponent<Bonds>().bondOrder;
                        bond.Item2.GetComponent<Elements>().bondingElectrons += bond.Item1.GetComponent<Bonds>().bondOrder;
                        if(bond.Item2.GetComponent<Elements>().expandedOctet) {
                            for(int i = 0; i < bond.Item2.GetComponent<Elements>().bondingElectrons / 2; i++) {
                                if(bond.Item2.GetComponent<Elements>().lonePairs >= bond.Item2.GetComponent<Elements>().defaultLonePairs) {
                                    break;
                                }
                                bond.Item2.GetComponent<Elements>().bondingElectrons -= 2;
                                bond.Item2.GetComponent<Elements>().lonePairs++;
                            }
                        }
                        ;
                        //Debug.Log("removing " + t.Item1.name);
                        break;
                    }
                }
            }
            foreach (Tuple<GameObject, GameObject> t in parent.gameObject.GetComponent<Elements>().neighbors)
            {
                Debug.Log("still got " + t.Item1.name);
            }
            // pathfinding from parent and adding every object pathed through to found
            if (parent == null)
            { // if parent is still null, that means the parent couldn't be found (an error occurred somewhere)
                Debug.Log("couldn't find parent");
                return;
            }
            HashSet<GameObject> found = new HashSet<GameObject>();
            DeletionDFS(parent, found);
            // looping through all objects and deleting any that haven't been found
            foreach (Transform t in transform.parent)
            {
                if (!found.Contains(t.gameObject))
                {
                    Destroy(t.gameObject);
                }
                else if (t.tag.Equals("Element") && t.gameObject.GetComponent<Elements>().neighbors.Count() > 1)
                {
                    t.gameObject.GetComponent<Elements>().ResetChildPositions();
                    t.gameObject.GetComponent<Elements>().MoveChildren(t.gameObject.GetComponent<Elements>().offset);
                }
            }


            Destroy(gameObject);
        }
    }

    /// <summary>
    /// DFS algorithm for marking GameObjects for deletion
    /// </summary>
    /// <param name="current">The current <see cref="GameObject"/> (<see cref="Elements"/>) being checked by the algorithm</param>
    /// <param name="found">The list of all <see cref="GameObject"/> that have been found by the algorithm</param>
    private void DeletionDFS(GameObject current, HashSet<GameObject> found)
    {
        if (found.Contains(current))
        { // if this element has been visited already, return to the last one
            return;
        }
        Debug.Log("added " + current.name);
        found.Add(current);
        foreach (Tuple<GameObject, GameObject> bond in current.GetComponent<Elements>().neighbors)
        {
            if (!found.Contains(bond.Item1))
            { // if this bond has not been travelled already
                Debug.Log("added " + bond.Item1.name);
                found.Add(bond.Item1);
                DeletionDFS(bond.Item2, found);
            }
        }
    }

    /// <summary>
    /// Updates a bond <see cref="GameObject"/> being pointed to in neighbors whenever its bond order is cycled
    /// </summary>
    /// <param name="newBond">The new bond <see cref="GameObject"/> to be referenced in neighbors</param>
    /// <param name="otherElement">The other element the bond connects to - used to find which <c> Tuple </c> to replace</param>
    public void UpdateBond(GameObject newBond, GameObject otherElement)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (Equals(neighbors[i].Item2, otherElement))
            {
                neighbors.Insert(i, new Tuple<GameObject, GameObject>(newBond, otherElement));
                neighbors.RemoveAt(i + 1);
                return;
            }
        }

    }

    /// <summary>
    /// Calculates the positions of the <see cref="LonePairs"/> of this atom and spawns them
    /// </summary>
    public void ShowLonePairs() {
        if(lonePairs == 0) {
            return;
        }
        // finding the position on the atom furthest from any bonds
        int count = 0;
        Vector3 lonePairCenter = Vector3.zero;
        Vector3 long1 = Vector3.zero;
        Vector3 long2 = Vector3.zero;
        Vector3 short1 = Vector3.positiveInfinity;
        Vector3 short2 = Vector3.negativeInfinity;
        foreach(Tuple<GameObject, GameObject> item in neighbors) {
            lonePairCenter += item.Item2.transform.position;
            count++;
            foreach(Tuple<GameObject, GameObject> other in neighbors) {
                if(!Equals(item, other)) {
                    if(Vector3.Distance(item.Item2.transform.position, other.Item2.transform.position) > Vector3.Distance(long1, long2)) {
                        long1 = item.Item2.transform.position;
                        long2 = other.Item2.transform.position;
                    }
                    if(Vector3.Distance(item.Item2.transform.position, other.Item2.transform.position) < Vector3.Distance(short1, short2)) {
                        short1 = item.Item2.transform.position;
                        short2 = other.Item2.transform.position;
                    }
                }
            }
        }
        if(count > 0) {
            lonePairCenter /= count;
        }
        lonePairCenter = (transform.position - (lonePairCenter - transform.position));
        lonePairCenter = Vector3.Lerp(transform.position, lonePairCenter, (transform.localScale.x / 2 + 0.1f) / Vector3.Distance(transform.position, lonePairCenter));

        // finding which axis has the greatest distance between atoms
        Vector3 rotationAxis = long1 - long2;
        float lonePairAngle = Vector3.Angle(short1 - transform.position, short2 - transform.position);

        // if atom is solo, set an arbitrary axis for the lone pairs to rotate around
        if(neighbors.Count == 0) {
            rotationAxis = Vector3.up;
        }

        // if atom only has 1 bond, set the rotation axis to be in line with the bond
        if(neighbors.Count == 1) {
            rotationAxis = transform.position - lonePairCenter;
        }

        bool fullRadial = false; // determines if the lone pairs should spread evenly around the axis or if they will be constrained by the other atoms
        // setting the center point if lonePairCenter = center of atom
        // this currently only works if the molecular geometry is linear or planar, I don't know enough chemistry to know if that's okay
        if(Mathf.Abs(lonePairCenter.x) < 0.01f && Mathf.Abs(lonePairCenter.y) < 0.01f && Mathf.Abs(lonePairCenter.z) < 0.01f) {
            fullRadial = true;
            // setting an arbitrarily different value from the center if neighbors = 0
            Vector3 otherPosition = transform.position;
            otherPosition.x += 1;
            // setting an arbitrarily different value from a neighbor if neighbors = 2
            if(neighbors.Count == 2) {
                otherPosition = neighbors[0].Item2.transform.position;
                if(Mathf.Abs(otherPosition.x) < 0.01f) {
                    otherPosition.x += 1;
                }
                else if(Mathf.Abs(otherPosition.y) < 0.01f) {
                    otherPosition.y += 1;
                }
                else {
                    otherPosition.z += 1;
                }
            }
            else if(neighbors.Count > 2) {
                // if there are more than 2 neighbors there's guaranteed to be an atom not on the rotation axis
                // finding an atom not on the rotation axis
                foreach(Tuple<GameObject, GameObject> item in neighbors) {
                    if(item.Item2.transform.position != long1 && item.Item2.transform.position != long2) {
                        otherPosition = item.Item2.transform.position;
                    }
                }
            }
            otherPosition = Vector3.Cross(rotationAxis, transform.position - otherPosition);
            lonePairCenter = Vector3.Lerp(transform.position, otherPosition, (transform.localScale.x / 2 + 0.1f) / Vector3.Distance(transform.position, otherPosition));
        }

        if(neighbors.Count == 1) {
            // currently this is hard-coded, I'm not aware of a way to do this mathematically
            // also it only works for 3 or less lone pairs, I don't think atoms can have more and still bond but I might be wrong there
            if(lonePairs == 1) {
                // spawning a lone pair
                GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as GameObject;
                GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                // making lone pair face the atom
                clone.transform.LookAt(transform);
                clone.transform.Rotate(0, 90, 0);

                // setting the lone pair's parent element to this element
                clone.GetComponent<LonePairs>().parent = this;

                // hiding the lone pair if lone pairs are hidden
                if(!creationUser.lonePairsVisible) {
                    clone.SetActive(false);
                }
            }
            else if(lonePairs == 2) {
                // getting the actual axis of rotation
                Vector3 otherPosition = neighbors[0].Item2.transform.position;
                if(Mathf.Abs(otherPosition.x) < 0.01f) {
                    otherPosition.x += 1;
                }
                else if(Mathf.Abs(otherPosition.y) < 0.01f) {
                    otherPosition.y += 1;
                }
                else {
                    otherPosition.z += 1;
                }
                rotationAxis = Vector3.Cross(rotationAxis, transform.position - otherPosition);

                // spawning a lone pair
                GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as GameObject;
                GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                // making lone pair face the atom
                clone.transform.LookAt(transform);
                clone.transform.Rotate(0, 90, 0);
                
                // setting the lone pair's parent element to this element
                clone.GetComponent<LonePairs>().parent = this;

                // moving the lone pair
                clone.transform.RotateAround(transform.position, rotationAxis, 60);

                // hiding the lone pair if lone pairs are hidden
                if(!creationUser.lonePairsVisible) {
                    clone.SetActive(false);
                }

                // spawning the second lone pair
                obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as GameObject;
                clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                // making lone pair face the atom
                clone.transform.LookAt(transform);
                clone.transform.Rotate(0, 90, 0);

                // setting the lone pair's parent element to this element
                clone.GetComponent<LonePairs>().parent = this;

                // moving the lone pair
                clone.transform.RotateAround(transform.position, rotationAxis, -60);

                // hiding the lone pair if lone pairs are hidden
                if(!creationUser.lonePairsVisible) {
                    clone.SetActive(false);
                }
            }
            else if(lonePairs == 3) {
                // getting the secondary axis of rotation
                Vector3 otherAxis = neighbors[0].Item2.transform.position;
                if(Mathf.Abs(otherAxis.x) < 0.01f) {
                    otherAxis.x += 1;
                }
                else if(Mathf.Abs(otherAxis.y) < 0.01f) {
                    otherAxis.y += 1;
                }
                else {
                    otherAxis.z += 1;
                }
                otherAxis = Vector3.Cross(rotationAxis, transform.position - otherAxis);

                for(int i = 0; i < 3; i++) {
                    // spawning a lone pair
                    GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as GameObject;
                    GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                    clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                    // making lone pair face the atom
                    clone.transform.LookAt(transform);
                    clone.transform.Rotate(0, 90, 0);

                    // setting the lone pair's parent element to this element
                    clone.GetComponent<LonePairs>().parent = this;

                    // moving the lone pair
                    clone.transform.RotateAround(transform.position, otherAxis, 71);
                    clone.transform.RotateAround(transform.position, rotationAxis, 120 * i);

                    // hiding the lone pair if lone pairs are hidden
                    if(!creationUser.lonePairsVisible) {
                        clone.SetActive(false);
                    }
                }
            }
            else {
                Debug.Log("More than 3 lone pairs!");
            }
        }
        else if(fullRadial) {
            for(int i = 0; i < lonePairs; i++) {
                // spawning a lone pair
                GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as GameObject;
                GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                // making lone pair face the atom
                clone.transform.LookAt(transform);
                clone.transform.Rotate(0, 90, 0);

                // setting the lone pair's parent element to this element
                clone.GetComponent<LonePairs>().parent = this;

                // moving the lone pair into place
                clone.transform.RotateAround(transform.position, rotationAxis, i * (360 / lonePairs));

                // hiding the lone pair if lone pairs are hidden
                if(!creationUser.lonePairsVisible) {
                    clone.SetActive(false);
                }
            }
        }
        else {
            if(lonePairs % 2 == 0) { // this wouldn't work for even lone pair counts other than 2, but I don't believe those cases exist
                // spawning a lone pair
                GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as GameObject;
                GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                // making lone pair face the atom
                clone.transform.LookAt(transform);
                clone.transform.Rotate(0, 90, 0);

                // setting the lone pair's parent element to this element
                clone.GetComponent<LonePairs>().parent = this;

                // moving the lone pair
                clone.transform.RotateAround(transform.position, rotationAxis, lonePairAngle / 2);

                // hiding the lone pair if lone pairs are hidden
                if(!creationUser.lonePairsVisible) {
                    clone.SetActive(false);
                }

                // spawning the second lone pair
                obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as GameObject;
                clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                // making lone pair face the atom
                clone.transform.LookAt(transform);
                clone.transform.Rotate(0, 90, 0);

                // setting the lone pair's parent element to this element
                clone.GetComponent<LonePairs>().parent = this;

                // moving the lone pair
                clone.transform.RotateAround(transform.position, rotationAxis, lonePairAngle / -2);

                // hiding the lone pair if lone pairs are hidden
                if(!creationUser.lonePairsVisible) {
                    clone.SetActive(false);
                }
            }
            else {
                for(int i = 0; i < lonePairs; i++) {
                    // spawning a lone pair
                    GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as GameObject;
                    GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                    clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                    // making lone pair face the atom
                    clone.transform.LookAt(transform);
                    clone.transform.Rotate(0, 90, 0);

                    // setting the lone pair's parent element to this element
                    clone.GetComponent<LonePairs>().parent = this;

                    // moving the lone pair
                    if(i > lonePairs / 2) {
                        clone.transform.RotateAround(transform.position, rotationAxis, (i / 2) * lonePairAngle);
                    }
                    else {
                        clone.transform.RotateAround(transform.position, rotationAxis, i * lonePairAngle);
                    }

                    // hiding the lone pair if lone pairs are hidden
                    if(!creationUser.lonePairsVisible) {
                        clone.SetActive(false);
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// Sets <see cref="forceVector"/> to the force this <c> Element </c> will experience this frame
    /// </summary>
    public void CalculateForceVector() {
        forceVector = Vector3.zero;
        int numVectors = 0;
        foreach(Transform element in transform.parent.transform) { // loops through all elements/bonds/lone pairs
            if(element.Equals(transform) || element.CompareTag("Bond")) { // if element is this Element or a bond, we don't need to look at interacting forces
                continue;
            }
            bool bonded = false;
            int bondOrder = 1;
            foreach(Tuple<GameObject, GameObject> neighbor in neighbors) {
                if(element.Equals(neighbor.Item2.transform)) {
                    bonded = true;
                    bondOrder = neighbor.Item1.GetComponent<Bonds>().bondOrder;
                    break;
                }
            }
            if(bonded) { // if element is bonded to this Element, we attract instead of repel
                float r = Vector3.Distance(transform.position, element.transform.position);
                float eps = Mathf.Sqrt(epsilon * element.gameObject.GetComponent<Elements>().epsilon);
                float sig = (sigma + element.gameObject.GetComponent<Elements>().sigma) / 2;
                float force = 24 * eps * (2 * Mathf.Pow(sig / r, 12) - Mathf.Pow(sig / r, 6)) * (1 / r);

                // increasing the pulling force if double or triple bonded to element
                if(bondOrder == 2) {
                    force = 24 * eps * (0.867f * Mathf.Pow(sig / r, 12) - Mathf.Pow(sig / r, 6)) * (1 / r);
                }
                else if(bondOrder == 3) {
                    force = 24 * eps * (0.45f * Mathf.Pow(sig / r, 12) - Mathf.Pow(sig / r, 6)) * (1 / r);
                }
                // capping the force so the elements don't explode out
                // this wouldn't be as necessary if I were better at math, but I'm not so here we are
                if(force > 0) {
                    force = 0.02f;
                }
                Vector3 forceDirection = element.transform.position - transform.position;
                forceDirection.Normalize();
                forceVector += (forceDirection * force);
                numVectors++;
            }
            else if(element.CompareTag("Element")) {
                float r = Vector3.Distance(transform.position, element.transform.position);
                float eps = Mathf.Sqrt(epsilon * element.gameObject.GetComponent<Elements>().epsilon);
                float sig = (sigma + element.gameObject.GetComponent<Elements>().sigma) / 2;
                float force = 24 * eps * (2 * Mathf.Pow(sig / r, 12) - Mathf.Pow(sig / r, 6)) * (1 / r);

                // capping force so molecules don't explode out as much
                if(force > 2f) {
                    force = 2f;
                }
                else if(force < -2f) {
                    force = -2f;
                }

                Vector3 forceDirection = transform.position - element.transform.position;
                forceDirection.Normalize();
                forceVector += (forceDirection * force);
                numVectors++;
            }
            else { // if element is a lone pair
                if(element.GetComponent<LonePairs>().parent.Equals(this)) {
                    // if lone pair belongs to this element, there is no force interaction
                    continue;
                }
                float r = Vector3.Distance(transform.position, element.transform.position);
                float eps = Mathf.Sqrt(epsilon * element.gameObject.GetComponent<LonePairs>().epsilon);
                float sig = (sigma + element.gameObject.GetComponent<LonePairs>().sigma) / 2;
                float force = 12 * eps * (2 * Mathf.Pow(sig / r, 12) - Mathf.Pow(sig / r, 6)) * (1 / r);

                // capping force so molecules don't explode out as much
                if(force > 2f) {
                    force = 2f;
                }
                else if(force < -2f) {
                    force = -2f;
                }

                Vector3 forceDirection = transform.position - element.transform.position;
                forceDirection.Normalize();
                forceVector += (forceDirection * force);
                numVectors++;
            }
        }
        // average the total force vectors by the number of vectors
        if(numVectors > 0) {
            forceVector /= numVectors;
        }

        Vector3 temp = forceVector;
        temp.Normalize();
        Debug.DrawRay(transform.position, temp, Color.red);
        //Debug.Log(forceVector.magnitude);
    }

    /// <summary>
    /// Updates the position of this <c> Element </c> based on its <see cref="forceVector"/> for this frame
    /// </summary>
    public void UpdatePosition() {
        if(GetID() != -1) {
            foreach(Tuple<GameObject, GameObject> neighbor in neighbors) { // if any bonds are too long, shorten them
                if(Vector3.Distance(transform.position, neighbor.Item2.transform.position) > 1.5 * (covalentRadius + neighbor.Item2.GetComponent<Elements>().covalentRadius)) {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position - neighbor.Item2.transform.position,
                        -0.1f * Vector3.Distance(transform.position, neighbor.Item2.transform.position) * Time.deltaTime);
                    break;
                }
            }
            if(forceVector.magnitude >= 0.01f) { // if magnitude of force is not very small, move along forceVector
                Vector3 averageVector = (forceVector + oldForceVector) / 2;
                transform.position = Vector3.MoveTowards(transform.position, transform.position - averageVector, forceVector.magnitude * Time.deltaTime);
                oldForceVector = forceVector;
            }
        }
        hasMoved = true;
        foreach(Tuple<GameObject, GameObject> neighbor in neighbors) {
            if(!neighbor.Item2.GetComponent<Elements>().hasMoved) {
                neighbor.Item2.GetComponent<Elements>().UpdatePosition();
            }
        }
    }
}

/// <summary>
/// A comparer for <see cref="Elements"/> that only checks certain variables of the <c> Element </c> and its children
/// </summary>
public class ElementsComparer : IEqualityComparer<Elements>
{
    /// <summary>
    /// Equals override for <see cref="Elements"/>
    /// </summary>
    /// <param name="x"> The first <c> Element </c> to compare </param>
    /// <param name="y"> The second <c> Element </c> to compare </param>
    /// <returns> True if <see cref="Elements.electrons"/>, <see cref="Elements.protons"/>, <see cref="Elements.neutrons"/>, and <see cref="Elements.bondOrders"/>
    /// are equal for <c> x</c>, <c> y</c>, and all <see cref="Elements"/> connected to them.</returns>
    /// <remarks>
    /// NOTE: Does not work with cyclic molecules, a check must be implemented to ensure molecules are only checked once
    /// </remarks>
    public bool Equals(Elements x, Elements y)
    {
        if (x.electrons != y.electrons) return false;
        if (x.protons != y.protons) return false;
        if (x.neutrons != y.neutrons) return false;
        if (x.bondOrders != y.bondOrders) return false;

        foreach (Tuple<GameObject, GameObject> neighbor_x in x.GetNeighbors())
        {
            bool works = false;
            foreach (Tuple<GameObject, GameObject> neighbor_y in y.GetNeighbors())
            {
                if (neighbor_x.Item1.GetComponent<Bonds>().bondOrder != neighbor_y.Item1.GetComponent<Bonds>().bondOrder && 
                    neighbor_x.Item2.GetComponent<Elements>().protons != neighbor_y.Item2.GetComponent<Elements>().protons)
                {
                    works = true;
                }
            }
            if (works == false) return false;
        }

        return true;
    }

    /// <summary>
    /// GetHashCode override for <see cref="Elements"/>
    /// </summary>
    /// <param name="obj"> the <see cref="Elements"/> to get the Hash Code of</param>
    /// <returns></returns>
    public int GetHashCode(Elements obj)
    {
        return obj.GetHashCode();
    }
}

/// <summary>
/// An enum that converts atomic number to atomic symbol (i.e. 1 = H, 2 = He, etc.)
/// </summary>
public enum ElementSymbols {
    H=1, He,
    Li, Be, B, C, N, O, F, Ne,
    Na, Mg, Al, Si, P, S, Cl, Ar,
    K, Ca, Sc, Ti, V, Cr, Mn, Fe, Co, Ni, Cu, Zn, Ga, Ge, As, Se, Br, Kr,
    Rb, Sr, Y, Zr, Nb, Mo, Tc, Ru, Rh, Pd, Ag, Cd, Id, Sn, Sb, Te, I, Xe,
    Cs, Ba, La, Ce, Pr, Nd, Pm, Sm, Eu, Gd, Tb, Dy, Ho, Er, Tm, Yb, Lu, Hf, Ta, W, Re, Os, Ir, Pt, Au, Hg, Tl, Pb, Bi, Po, At, Rn,
    Fr, Ra, Ac, Th, Pa, U, Np, Pu, Am, Cm, Bk, Cf, Es, Fm, Md, No, Lr, Rf, Db, Sg, Bh, Hs, Mt, Ds, Rg, Cn, Nh, Fl, Mc, Lv, Ts, Og
}