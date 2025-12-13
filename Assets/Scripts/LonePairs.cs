using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LonePairs : MonoBehaviour
{
    /// <summary> The force vector this <c> Lone Pair </c> will feel in the current frame </summary>
    private Vector3 forceVector = Vector3.zero;
    /// <summary> The force vector this <c> Lone Pair </c> felt in the previous frame </summary>
    /// <remarks> Used to lessen oscillation when approaching equilibrium (doesn't really work yet) </remarks>
    private Vector3 oldForceVector = Vector3.zero;
    /// <summary> The <see cref="Elements"/> this <c> Lone Pair </c> belongs to</summary>
    public Elements parent;

    /// <summary> The epsilon value of this <c> Lone Pair </c> </summary>
    /// <remarks> Used for the Lennard Jones potential equation to approximate force interactions of atoms in the molecule
    /// <br></br> The Lennard Jones potential equation is not used for lone pairs, but until we find an equation that is correct this will do</remarks>
    public float sigma = 0.25f;
    /// <summary> The sigma value of this <c> Lone Pair </c> </summary>
    /// <remarks> Used for the Lennard Jones potential equation to approximate force interactions of atoms in the molecule
    /// <br></br> The Lennard Jones potential equation is not used for lone pairs, but until we find an equation that is correct this will do</remarks>
    public float epsilon = 2f;
    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {}

    /// <summary>
    /// Sets <see cref="forceVector"/> to the force this <c> Lone Pair </c> will experience this frame
    /// </summary>
    public void CalculateForceVector() {
        forceVector = Vector3.zero;
        int numVectors = 0;
        foreach(Transform element in transform.parent.transform) { // loops through all elements/bonds/lone pairs
            if(element.Equals(transform) || element.CompareTag("Bond")) { // if element is this lone pair or a bond, we don't need to look at interacting forces
                continue;
            }
            if(element.CompareTag("Element") && !element.Equals(parent.transform)) { // if element is an element and not the parent element
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
            else if(element.CompareTag("Lone Pair") && !element.Equals(transform)) { // if element is a lone pair and not this lone pair
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
    }

    /// <summary>
    /// Updates the position of this <c> Lone Pair </c> based on its <see cref="forceVector"/> for this frame
    /// </summary>
    public void UpdatePosition() {
        Vector3 averageVector = (forceVector + oldForceVector) / 2;
        transform.position = Vector3.MoveTowards(transform.position, transform.position - averageVector, averageVector.magnitude * Time.deltaTime);
        if(Vector3.Distance(transform.position, parent.transform.position) > parent.transform.localScale.x - 0.1f) {
            //Debug.Log(Vector3.Distance(transform.position, parent.transform.position) + " " + parent.transform.localScale.x);
            Debug.Log(Vector3.Distance(transform.position, parent.transform.position) + " " + parent.transform.localScale.x + " " + 
                Vector3.MoveTowards(transform.position, parent.transform.position,
                (Vector3.Distance(transform.position, parent.transform.position) + parent.transform.localScale.x) * Time.deltaTime));
            transform.position = Vector3.MoveTowards(transform.position, parent.transform.position,
                (Vector3.Distance(transform.position, parent.transform.position) + parent.transform.localScale.x) * Time.deltaTime);
        }
            oldForceVector = forceVector;

        // making lone pair face atom
        transform.LookAt(parent.transform);
        transform.Rotate(0, 90, 0);
    }
}
