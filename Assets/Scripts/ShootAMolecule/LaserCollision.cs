using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserCollision : MonoBehaviour
{

    [SerializeField] GameObject Manager;
    GameObjectsManager GOM;
    ShootMoleculeLives lives;
    bool paused = false;
    public GameObject target;

    private void Awake()
    {
        Manager = GameObject.Find("GameObjectManager");
        GOM = Manager.GetComponent<GameObjectsManager>();
        lives = Manager.GetComponent<ShootMoleculeLives>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 difference = target.transform.position - gameObject.transform.position;
        //difference = difference.normalized;
        transform.Rotate(0, 0, 0);
        print("Laser Targeting");
        print(difference);
        print("Laser Shot");
        transform.rotation = Quaternion.AngleAxis(-Mathf.Atan2(difference.x, (difference.y+2f)*2f) * Mathf.Rad2Deg, Vector3.forward);
    }

    // Update is called once per frame
    void Update()
    {

        if (!lives.Alive)
        {
            Destroy(gameObject);
        }
        
        if (Input.GetKeyUp(KeyCode.Escape) && !paused )
        {
            paused = true;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        } else if (Input.GetKeyUp(KeyCode.Escape) && paused)
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
            GOM.targetHit(collision.collider.gameObject);
            Destroy(gameObject);
            GOM.IncreaseScore();
        }
    }
}
