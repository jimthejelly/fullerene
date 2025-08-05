using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondLineBehavoir : MonoBehaviour
{

    GameObject mainScript;

    private GameObject element1;
    private GameObject element2;
    private LineRenderer lr;
    private bool set = false;
    double lineWidth = 0.1f;
    private EdgeCollider2D collision;
    private List<Vector2> points;
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        mainScript = GameObject.Find("script");
        lr.startWidth = 10f;
        lr.endWidth = 10f;
        collision = GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (set && element1 != null && element2 != null) { 
            Vector3[] positions = { element1.transform.position, element2.transform.position };
            points.Add(new Vector2(element1.transform.position.x, element1.transform.position.y));
            points.Add(new Vector2(element2.transform.position.x, element2.transform.position.y));
            lr.SetPositions(positions);
            collision.SetPoints(points);
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

    private void OnMouseOver()
    {
        if (mainScript.GetComponent<DrawOrgo>().Function == "Trim" && Input.GetMouseButtonDown(0))
        {
            element1.GetComponent<ElementBehavoir>().decrementBond();
            element2.GetComponent<ElementBehavoir>().decrementBond();
            Destroy(gameObject);
        }
    }


}
