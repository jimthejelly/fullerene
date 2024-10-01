using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    private GameObject _obj;
    private GameObject[] _neighbors;

    public Element(GameObject tar, GameObject[] neigh)
    {
        _obj = tar;
        _neighbors = neigh;
    }

    public GameObject GetObj()
    {
        return _obj;
    }

    public GameObject[] GetNeighbors()
    {
        return _neighbors;
    }
    
}
