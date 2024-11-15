using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Elements : MonoBehaviour
{

    public int electrons;
    public int protons;
    public int neutrons;
    private List<Tuple<GameObject,GameObject>> neighbors = new List<Tuple<GameObject,GameObject>>();
    public int lonePairs;
    public int bondingElectrons;
    public bool expandedOctet;
    public bool physicsOn = false;

    int bondCount = 0;
    int bondOrders = 0;
    int start = 0;
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

        // checking if the element follows physics:
        if (physicsOn && transform.parent != null && !validBond(transform.parent.parent.gameObject, selectElement.element)) {
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
        // int bondCount = 0;
        // int bondOrders = 0;
        Debug.Log("neighbor num: " + neighbors.Count);
        protons = 16;
        electrons = 16;
        neutrons = 0;
        
        bondCount = 0;
        bondOrders = 0;

        for (int i = 0; i < neighbors.Count(); i++)
        {
            bondCount++;
            bondOrders++;
        }
        
        
        // int start = 0;
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
        start = 0;

        if (neighbors.Any()) {
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
        clone.transform.Translate(0, -radius/2, 0);
        
        // creationMenu sn = gameObject.GetComponent<creationMenu>();

        if (creationMenu.simplified) { clone.transform.localScale = new Vector3(1,1,1); }

        moveChildren(bondCount, start);

        if (!cylClone)
        {
            Debug.Log("bond breok");
        }

        if (!clone)
        {
            Debug.Log("clone breok");
        }
       
    }

    public void resetChildPositions(float radius) {

        foreach(Tuple<GameObject,GameObject> child in neighbors) {
            
            // child.Item1.transform.localPosition = transform.position;
            // child.Item1.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
            //     this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
            // child.Item1.transform.Translate(0, -1 * (radius / 2), 0);

            // child.Item2.transform.localPosition = transform.position;
            // child.Item2.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
            //     this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
            // child.Item2.transform.Translate(0, -1 * (radius), 0);
            
            
            if (gameObject == creationUser.head)
            {
                child.Item1.transform.localPosition = transform.position;
                child.Item1.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
                child.Item1.transform.Translate(0, -1 * (radius / 2), 0);

                child.Item2.transform.localPosition = transform.position;
                child.Item2.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
                child.Item2.transform.Translate(0, -1 * (radius), 0);
            } else if (!Equals(child, neighbors[0])) {

                child.Item1.transform.localPosition = transform.position;
                child.Item1.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
                child.Item1.transform.Translate(0, -1 * (radius / 2), 0);

                // // child.Item1.transform.RotateAround(transform.position, transform.forward, 180);
                // // child.Item1.transform.RotateAround(transform.position, transform.up, 180);
                // // child.Item1.transform.RotateAround(transform.position, transform.right, 180);

                child.Item2.transform.localPosition = transform.position;
                child.Item2.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x,
                    this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
                child.Item2.transform.Translate(0, -1 * (radius), 0);

                // child.Item2.transform.RotateAround(transform.position, transform.forward, 180);
                // child.Item2.transform.RotateAround(transform.position, transform.up, 180);
                // child.Item2.transform.RotateAround(transform.position, transform.right, 180);

            } else {

            }
            
        }
    }
    
    public void moveChildren(int bondCount, int start) {
        if(bondCount == 2) {
            if (gameObject != creationUser.head && neighbors.Count()-1 != 0) {
                neighbors[neighbors.Count()-1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[neighbors.Count()-1].Item2.transform.RotateAround(transform.position, transform.forward, 180);

            }
 
            neighbors[neighbors.Count()-1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
            neighbors[neighbors.Count()-1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
            
            Debug.Log("index: " + (1) + "   name: " + neighbors[1].Item2.name);

            // List<GameObject> branch = GetBranch(gameObject);
            // foreach (var obj in branch)
            // {
            //     // Print the name of each connected GameObject
            //     Debug.Log("Connected Element: " + obj.name);
            // }

        }
        else if(bondCount == 3) {
            
            for(int i = 0; i < 3; i++) {
                if (gameObject != creationUser.head && i != 0) {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 180);

                }

                neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 120*i);
                neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 120*i);

                //moveInnerChildren(neighbors[i-start], neighbors[i-start].Item2.GetComponent<Elements>().bondCount, neighbors[i-start].Item2.GetComponent<Elements>().start, 0, 0, 120*i);
            }
            
        }
        else if(bondCount == 4) {
            for(int i = 1; i < 4; i++) {
                if (gameObject != creationUser.head && i != 0) {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                }

                neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 120);
                neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 120);

                neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 120*i);
                neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 120*i);
            }
        }
        else if(bondCount == 5) {
            neighbors[1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
            neighbors[1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
            if (gameObject != creationUser.head) {
                neighbors[1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
            }

            for(int i = 2; i < 5; i++) {
                if (gameObject != creationUser.head && i != 0) {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                }

                neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 120*i);
                neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 120*i);
            }
        }
        else if(bondCount == 6) {
            neighbors[1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
            neighbors[1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
            if (gameObject != creationUser.head) {
                neighbors[1].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[1].Item2.transform.RotateAround(transform.position, transform.forward, 180);
            }

            for(int i = 2; i < 6; i++) {
                if (gameObject != creationUser.head && i != 0) {
                    neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 180);
                    neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 180);
                }

                neighbors[i].Item1.transform.RotateAround(transform.position, transform.forward, 90);
                neighbors[i].Item2.transform.RotateAround(transform.position, transform.forward, 90);

                neighbors[i].Item1.transform.RotateAround(transform.position, transform.up, 90*i);
                neighbors[i].Item2.transform.RotateAround(transform.position, transform.up, 90*i);
            }
        }

    }

    // perform dfs in order to find all the elements/bonds on a branch of the molecule. this will be used for moving inner children.
    // public List<GameObject> GetBranch(GameObject parent)
    // {
    //     if (parent == null)
    //     {
    //         Debug.LogError("Parent GameObject is null!");
    //         return new List<GameObject>();  // Return an empty list to avoid further issues
    //     }

    //     List<GameObject> branch = new List<GameObject>();
    //     HashSet<GameObject> visited = new HashSet<GameObject>();

    //     // Call the recursive method to populate the branch list
    //     GetBranchRecursive(parent, branch, visited);

    //     return branch;
    // }

    // private void GetBranchRecursive(GameObject current, List<GameObject> branch, HashSet<GameObject> visited)
    // {
    //     if (current == null)
    //     {
    //         Debug.LogError("Current GameObject is null in GetBranchRecursive!");
    //         return;
    //     }

    //     // Add the current element to the branch list if it's not already visited
    //     if (!visited.Contains(current))
    //     {
    //         branch.Add(current);
    //         visited.Add(current);

    //         // Get the neighbors of the current element (connected atoms)
    //         Elements currentElement = current.GetComponent<Elements>();

    //         // Loop through all neighbors and recursively visit them
    //         foreach (var neighbor in currentElement.neighbors)
    //         {
    //             GameObject neighbor1 = neighbor.Item1;
    //             GameObject neighbor2 = neighbor.Item2;

    //             // Check both items in the tuple (they represent the bond and the connected element)
    //             if (!visited.Contains(neighbor1))
    //             {
    //                 GetBranchRecursive(neighbor1, branch, visited);
    //             }

    //             if (!visited.Contains(neighbor2))
    //             {
    //                 GetBranchRecursive(neighbor2, branch, visited);
    //             }
    //         }
    //     }
    // }




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

    public bool validBond(GameObject parent, string child) {
        
        return true;
    }

    public void togglePhysics(){
        physicsOn = !physicsOn;
    }

    // Method to change the material's rendering mode to "Fade"
    void SetMaterialToFadeMode(Material material)
    {
        material.SetFloat("_Mode", 2); // Set rendering mode to "Fade"
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000; // Use the transparent render queue


        Color color = material.color;
        color.a = 0.5f;  // Set transparency value
    }

    // temporarily spawns the element prefab contained in the global variable element when selected
    public void TempSpawnElement(int num) {
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

        // checking if the element follows physics:
        if (physicsOn && transform.parent != null && !validBond(transform.parent.parent.gameObject, selectElement.element)) {
            return;
        }

        // making new bond
        float radius = 3f;
        GameObject cyl = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
        GameObject cylClone = Instantiate(cyl, Vector3.zero, Quaternion.identity);
        cylClone.transform.localScale = new Vector3(0.3f, radius / 2, 0.3f);
        cylClone.transform.SetParent(transform, true);


        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + selectElement.element + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity);
        clone.transform.SetParent(cylClone.transform.GetChild(0), true);
        clone.name = clone.name + " " + num;
        cylClone.name = cylClone.name + " " + num;
        bondCount++;

        resetChildPositions(radius);

        clone.transform.localEulerAngles = cylClone.transform.localEulerAngles + new Vector3(180, 0, 0);
        clone.transform.localPosition = Vector3.up * -1;
        
        moveChildren(bondCount, start);

        // obj.GetComponent<Renderer>().material.SetFloat("_Mode", 3);
        
        // obj.layer = 2;
        // SetMaterialToFadeMode(obj.GetComponent<Renderer>().material);
        // // ChangeAlpha(obj.GetComponent<Renderer>().material, 0.5f);
        
        clone.layer = 2;
        
        SetMaterialToFadeMode(clone.GetComponent<Renderer>().material);
        // ChangeAlpha(clone.GetComponent<Renderer>().material, 0f);

        // cyl.layer = 2;
        // SetMaterialToFadeMode(cyl.GetComponent<Renderer>().material);

        cylClone.layer = 2;
        SetMaterialToFadeMode(cylClone.GetComponent<Renderer>().material);
        // ChangeAlpha(cylClone.GetComponent<Renderer>().material, 0.5f);

        cylClone.transform.GetChild(0).gameObject.layer = 2;

        SetMaterialToFadeMode(cylClone.transform.GetChild(0).gameObject.GetComponent<Renderer>().material);
        // ChangeAlpha(cylClone.transform.GetChild(0).gameObject.GetComponent<Renderer>().material, 0f);

    }
}

