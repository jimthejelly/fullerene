using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBranchButton : MonoBehaviour
{

    Vector3 mousepos = new Vector3 (0, 0, 0);
    Vector3 MouseFromButton = new Vector3(1, 1, 0) * 3;

    GameObject parent;
    ElementBehavoir parentScript;
    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent.gameObject;
        parentScript = parent.GetComponent<ElementBehavoir>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            parentScript.forgeBond(transform.localPosition);
        }
    }
}
