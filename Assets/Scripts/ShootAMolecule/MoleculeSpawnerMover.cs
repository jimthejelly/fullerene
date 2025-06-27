using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoleculeSpawnerMover : MonoBehaviour
{

    public float Speed = 0.0f;
    public Vector2 XDirection = new Vector2(1.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = XDirection * Speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.x > 7)
        {
            XDirection = new Vector2(-1.0f, 0.0f);
            gameObject.GetComponent<Rigidbody2D>().velocity = XDirection * Speed;
        } else if (gameObject.transform.position.x < -7) {
            XDirection = new Vector2(1.0f, 0.0f);
            gameObject.GetComponent<Rigidbody2D>().velocity = XDirection * Speed;
        }
    }
}
