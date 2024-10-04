using System;
using UnityEngine;

public class element : MonoBehaviour
{
    private GameObject _obj;
    private GameObject[] _neighbors;

    private String name;
    
    private int lonePairs;
    private int bondingElectrons;
    private bool expandedOctet;

    public element(String element, int pairs, int elec, bool oct, GameObject tar)
    {
        name = element;
        lonePairs = pairs;
        bondingElectrons = elec;
        expandedOctet = oct;
        _obj = tar;
        _neighbors = null;
    }
    
    public element(String element, int pairs, int elec, bool oct, GameObject tar, GameObject[] neigh)
    {
        name = element;
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
    
}
