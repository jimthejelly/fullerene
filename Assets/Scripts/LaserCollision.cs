using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserCollision : MonoBehaviour
{

    [SerializeField] GameObject es;
    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("Submit") && !paused )
        {
            paused = true;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        } else if (Input.GetButtonDown("Submit") && paused)
        {
            paused = false;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

        }

    }

    private void FixedUpdate()
    {
        transform.Translate(transform.up);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Rock")
        {
            print("hit");
            es.GetComponent<GameObjectsManager>().RemoveObject(collision.collider.gameObject);
            Destroy(collision.collider.gameObject);
            Destroy(gameObject);

        }
    }
}
