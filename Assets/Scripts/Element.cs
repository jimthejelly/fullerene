using System;
using UnityEditor;
using UnityEngine;

public class Element : MonoBehaviour
{
    private GameObject _obj;
    private GameObject[] _neighbors;

    private String elementName;
    
    private int lonePairs;
    private int bondingElectrons;
    private bool expandedOctet;

    public Element(String element, int pairs, int elec, bool oct, GameObject tar)
    {
        elementName = element;
        lonePairs = pairs;
        bondingElectrons = elec;
        expandedOctet = oct;
        _obj = tar;
        _neighbors = null;
    }
    
    public Element(String element, int pairs, int elec, bool oct, GameObject tar, GameObject[] neigh)
    {
        elementName = element;
        lonePairs = pairs;
        bondingElectrons = elec;
        expandedOctet = oct;
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

    public String GetName()
    {
        return name;
    }

    public bool isObject(GameObject cur)
    {
        return cur == _obj;
    }

}
