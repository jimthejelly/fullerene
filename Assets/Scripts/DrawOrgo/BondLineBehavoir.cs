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
        lr.startWidth = 10f;
        lr.endWidth = 10f;
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

        //use colors of each element to create a color gradient for the line
        print(element1.GetComponent<ElementBehavoir>().GetColor());
        print(lr.endColor = element2.GetComponent<ElementBehavoir>().GetColor());

        this.element1 = element1;
        //lr.colorGradient.
        this.element2 = element2;
        set = true;
    }


}
