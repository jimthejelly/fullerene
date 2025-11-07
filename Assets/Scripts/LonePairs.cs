using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LonePairs : MonoBehaviour
{
    private Vector3 forceVector = Vector3.zero;
    private Vector3 oldForceVector = Vector3.zero;
    public Elements parent;

    public float sigma = 0.5f;
    public float epsilon = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void CalculateForceVector() {
        forceVector = Vector3.zero;
        int numVectors = 0;
        foreach(Transform element in transform.parent.transform) { // loops through all elements/bonds/lone pairs
            if(element.Equals(transform) || element.CompareTag("Bond")) { // if element is this lone pair or a bond, we don't need to look at interacting forces
                continue;
            }
            if(element.CompareTag("Element") && !element.Equals(parent.transform)) { // if element is an element and not the parent element
                float r = Vector3.Distance(transform.position, element.transform.position);
                float eps = Mathf.Sqrt(epsilon * (element.gameObject.GetComponent<Elements>() as Elements).epsilon);
                float sig = (sigma + (element.gameObject.GetComponent<Elements>() as Elements).sigma) / 2;
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
            else if(element.CompareTag("Lone Pair") && !element.Equals(transform)) { // if element is a lone pair and not this lone pair
                float r = Vector3.Distance(transform.position, element.transform.position);
                float eps = Mathf.Sqrt(epsilon * (element.gameObject.GetComponent<LonePairs>() as LonePairs).epsilon);
                float sig = (sigma + (element.gameObject.GetComponent<LonePairs>() as LonePairs).sigma) / 2;
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
        }
        // average the total force vectors by the number of vectors
        if(numVectors > 0) {
            forceVector /= numVectors;
        }

        Vector3 temp = forceVector;
        temp.Normalize();
        Debug.DrawRay(transform.position, temp, Color.red);
    }

    public void UpdatePosition() {
        Vector3 averageVector = (forceVector + oldForceVector) / 2;
        transform.position = Vector3.MoveTowards(transform.position, transform.position - averageVector, averageVector.magnitude * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, parent.transform.position,
            (Vector3.Distance(transform.position, parent.transform.position) + parent.transform.localScale.x + 0.1f) * Time.deltaTime);
        oldForceVector = forceVector;
    }
}
