using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBounds : MonoBehaviour
{

    [SerializeField] GameObject GameObjectManager;
    ShootMoleculeLives lives;

    bool paused = false;

    private void Awake()
    {
        GameObjectManager = GameObject.Find("GameObjectManager");
        lives = GameObjectManager.GetComponent<ShootMoleculeLives>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!lives.Alive)
        {
            Destroy(gameObject);
        }
        if (transform.position.y < -5)
        {
            lives.looseLife();
            Destroy(gameObject);
        }

        if (Input.GetKeyUp(KeyCode.Escape) && paused)
        {
            unPause();
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && !paused)
        {
            Pause();
        }
    }

    public void unPause()
    {
        paused = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector3(0, -.02f, 0);
    }

    public void Pause()
    {
        paused = true;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
    }



}
