using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMoleculeLives : MonoBehaviour
{

    [SerializeField] GameObject Heart;
    GameObjectsManager Manage;
    GameObject[] Hearts;

    public int Lives = 3;
    public bool Alive = true;
    // Start is called before the first frame update
    void Start()
    {
        Hearts = new GameObject[3];
        Manage = gameObject.GetComponent<GameObjectsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void looseLife()
    {
        Manage.DecreaseScore();
        Destroy(Hearts[Lives-1]);
        Lives--;
        if (Lives == 0)
        {
            Alive = false;
            Manage.Pause = true;
            Manage.ResetList();
        }

    }

    public void SpawnHearts()
    {
        for (int i = 0; i < Lives; i++)
        {
            GameObject heart = Instantiate(Heart);
            heart.transform.position = new Vector3((i * -2) + heart.transform.position.x, heart.transform.position.y, heart.transform.position.z);
            Hearts[i] = heart;
        }
    }

}
