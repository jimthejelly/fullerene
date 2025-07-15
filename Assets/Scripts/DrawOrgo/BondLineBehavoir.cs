using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondLineBehavoir : MonoBehaviour
{

    private GameObject element1;
    private GameObject element2;
    private LineRenderer lr;
    private bool set = false;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (set) { 
            Vector3[] positions = { element1.transform.position, element2.transform.position };
            lr.SetPositions(positions);
        }
    }

    public void setElements(GameObject element1, GameObject element2)
    {
        this.element1 = element1;
        this.element2 = element2;
        set = true;
    }


}
