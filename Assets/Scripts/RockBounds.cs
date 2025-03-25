using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBounds : MonoBehaviour
{

    bool paused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5)
        {
            Destroy(gameObject);
        }

        if (Input.GetKeyUp(KeyCode.Escape) && paused)
        {
            paused = false;
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -.02f, 0);
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && !paused)
        {
            paused = true;
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }
}
