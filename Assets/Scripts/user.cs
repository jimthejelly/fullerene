using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class user : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    public float moveSpeed = 2;
    public float turnSpeed = 50;
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
        float y = Input.GetAxis("Mouse X") * moveSpeed * Time.timeScale;
        float x = Input.GetAxis("Mouse Y") * moveSpeed * Time.timeScale;
        x = Mathf.Clamp(x, -90, 90);
        _camera.transform.eulerAngles = new Vector3(transform.eulerAngles.x - x, transform.eulerAngles.y + y, 0);
    }
}
