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
        //if(Input.GetKeyDown("left shift")) {
            SpawnElement();
        //}
    }

    // returns (a.x*b.x, a.y*b.y, a.z*b.z)
    Vector3 indivMult(Vector3 a, Vector3 b) {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    // Spawns the element prefab contained in the global variable element
    void SpawnElement() {
        GameObject cyl = AssetDatabase.LoadAssetAtPath("Assets/Resources/SingleBond.prefab", typeof(GameObject)) as GameObject;
        GameObject cylClone = Instantiate(cyl, Vector3.zero, Quaternion.identity, transform);
        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + selectElement.element + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity);
        clone.transform.SetParent(cylClone.transform, true);
        float radius = 3f;
        int bondCount = 0;
        foreach(Transform child in transform) {
            if(child.tag.Equals("Bond")) {
                bondCount++;
            }
        }
        int start = 0;
        if(transform.parent != null) {
            bondCount++;
            start = 1;
        }
        for(int i = start; i < bondCount; i++) {
            transform.GetChild(i).localPosition = Vector3.up * radius / 2;
            transform.GetChild(i).localEulerAngles = new Vector3(180, 0, 0);
        }
        transform.GetChild(bondCount - 1).GetChild(0).localEulerAngles = transform.GetChild(bondCount - 1).localEulerAngles + new Vector3(180, 0, 0);
        clone.transform.localPosition = indivMult(cylClone.transform.up * radius / 2, clone.transform.localScale);
        if(bondCount == 1) {
            transform.GetChild(0).localPosition = Vector3.up * radius / 2;
            transform.GetChild(0).localEulerAngles = new Vector3(180, 0, 0);
        }
        else if(bondCount == 2) {
            transform.GetChild(1).RotateAround(transform.position, transform.forward, 180);
        }
        else if(bondCount == 3) {
            transform.GetChild(1).RotateAround(transform.position, transform.forward, 120);
            transform.GetChild(2).RotateAround(transform.position, transform.forward, 240);
        }
        else if(bondCount == 4) {
            for(int i = 1; i < 4; i++) {
                transform.GetChild(i).RotateAround(transform.position, transform.right, 109);
            }
            transform.GetChild(2).RotateAround(transform.position, transform.up, 109);
            transform.GetChild(3).RotateAround(transform.position, transform.up, -109);
        }
    }
}
