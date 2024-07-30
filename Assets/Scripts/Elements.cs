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
        //SpawnElement();
    }

    // returns (a.x*b.x, a.y*b.y, a.z*b.z)
    Vector3 indivMult(Vector3 a, Vector3 b) {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    // Spawns the element prefab contained in the global variable element
<<<<<<< HEAD
    public void SpawnElement() {
=======
    public void SpawnElement(int num) {
        GameObject cyl = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
        GameObject cylClone = Instantiate(cyl, Vector3.zero, Quaternion.identity, transform);
        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + selectElement.element + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity);
        clone.name = clone.name + "-" + num;
        cylClone.name = cylClone.name + "-" + num;
        clone.transform.SetParent(cylClone.transform, true);
        float radius = 3f;
>>>>>>> david
        int bondCount = 0;
        foreach(Transform child in transform) {
            if(child.tag.Equals("Bond")) {
                bondCount++;
            }
        }
        int start = 0;
        if(transform.parent != null && transform.parent.tag.Equals("Bond")) {
            bondCount++;
            start = 1;
        }
        // checking if the element can make more bonds
        if((!expandedOctet && bondCount == bondingElectrons) || (expandedOctet && bondCount == bondingElectrons + 2 * lonePairs)) {
            return;
        }
        // making new bond
        float radius = 5f;
        GameObject cyl = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
        GameObject cylClone = Instantiate(cyl, Vector3.zero, Quaternion.identity, transform);
        cylClone.transform.localScale = new Vector3(0.3f, radius / 2, 0.3f);
        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + selectElement.element + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity);
        clone.transform.SetParent(cylClone.transform, true);
        bondCount++;
        
        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).localPosition = Vector3.up * radius / 2;
            transform.GetChild(i).localEulerAngles = new Vector3(180, 0, 0);
        }
        clone.transform.localEulerAngles = cylClone.transform.localEulerAngles + new Vector3(180, 0, 0);
        clone.transform.localPosition = Vector3.up * -1;
        
        if(bondCount == 2) {
            transform.GetChild(1 - start).RotateAround(transform.position, transform.forward, 180);
        }
        else if(bondCount == 3) {
            transform.GetChild(1 - start).RotateAround(transform.position, transform.forward, 120);
            transform.GetChild(2 - start).RotateAround(transform.position, transform.forward, 240);
        }
        else if(bondCount == 4) {
            for(int i = 1; i < 4; i++) {
                transform.GetChild(i - start).RotateAround(transform.position, transform.right, 109);
            }
            transform.GetChild(2 - start).RotateAround(transform.position, transform.up, 109);
            transform.GetChild(3 - start).RotateAround(transform.position, transform.up, -109);
        }
        else if(bondCount == 5) {
            for(int i = 2; i < 5; i++) {
                transform.GetChild(i - start).RotateAround(transform.position, transform.right, 90);
            }
            transform.GetChild(1 - start).RotateAround(transform.position, transform.right, 180);
            transform.GetChild(3 - start).RotateAround(transform.position, transform.up, 120);
            transform.GetChild(4 - start).RotateAround(transform.position, transform.up, -120);
        }
        else if(bondCount == 6) {
            transform.GetChild(1 - start).RotateAround(transform.position, transform.right, 180);
            transform.GetChild(2 - start).RotateAround(transform.position, transform.right, 90);
            transform.GetChild(3 - start).RotateAround(transform.position, transform.right, -90);
            transform.GetChild(4 - start).RotateAround(transform.position, transform.forward, 90);
            transform.GetChild(5 - start).RotateAround(transform.position, transform.forward, -90);
        }
    }
}
