using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explorationUser : MonoBehaviour
{
    public float moveSpeed; // Movement speed modifier
    public float turnSpeed; // Look speed modifier
    float xRotation = 0;
    float yRotation = 0;
    public void Start() {
        
    }
    // Update is called once per frame
    void Update()
    {
        Turning();
        Movement();
    }

    void Movement() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Jump");
        float y = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(x,z,y);
        transform.Translate(moveSpeed * movement * Time.deltaTime);
    }

    void Turning() {
        float xChange = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
        float yChange = Input.GetAxis("Mouse Y") * turnSpeed * Time.deltaTime;
        
        yRotation -= yChange;
        xRotation += xChange;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);
        transform.localEulerAngles = (Vector3.right * yRotation) + (Vector3.up * xRotation);
    }
}
