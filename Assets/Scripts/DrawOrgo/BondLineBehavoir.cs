using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondLineBehavoir : MonoBehaviour
{

    private GameObject element1;
    private GameObject element2;
    private LineRenderer lr;
    private bool set = false;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        lr.startWidth = 10f;
        lr.endWidth = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (set && element1 != null && element2 != null) { 
            Vector3[] positions = { element1.transform.position, element2.transform.position };
            lr.SetPositions(positions);
        }
        if (element1 == null || element2 == null)
        {
            Destroy(gameObject);
        }
    }

    public GameObject getElement1() { return element1; }
    public GameObject getElement2() { return element2; }

    public void setElements(GameObject element1, GameObject element2)
    {

        //use colors of each element to create a color gradient for the line

        //lr.SetColors(element1.GetComponent<SpriteRenderer>().color, element2.GetComponent<SpriteRenderer>().color);
        lr.startColor = element1.GetComponent<ElementBehavoir>().GetColor();
        lr.endColor = element2.GetComponent<ElementBehavoir>().GetColor();
        print(lr.startColor);
        print(lr.endColor);
        Color aCheck = lr.startColor;
        aCheck.a = 250;
        lr.startColor = aCheck;
        aCheck = lr.endColor;
        aCheck.a = 250;
        lr.endColor = aCheck;
        this.element1 = element1;
        //lr.colorGradient.
        this.element2 = element2;
        set = true;
    }


}
