using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserCollision : MonoBehaviour
{

    [SerializeField] GameObject es;
    bool paused = false;
    public GameObject target;

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
        if (transform.position.y < -5 || transform.position.y > 7 || transform.position.x < -14 || transform.position.x > 14)
        {
            Destroy(gameObject);
        }

    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }
        //float x = target.transform.position.x - transform.position.x;
        //float y = target.transform.position.y - transform.position.y;
        //transform.Rotate(0f,0f,Mathf.Atan2(y,x)*Mathf.Rad2Deg);
        transform.Translate(transform.up*0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Rock")
        {
            print("hit");
            //es.GetComponent<GameObjectsManager>().RemoveObject(collision.collider.gameObject);
            Destroy(collision.collider.gameObject);
            Destroy(gameObject);

        }
    }
}
