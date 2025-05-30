using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;
using TMPro;
using System.Diagnostics.Tracing;
using System;


// public static class Globals {
//     public static GameObject element;
// }
public class selectElement : MonoBehaviour
{
    public static string element = "6-Carbon";
    public TMP_Text txt;
    public TMP_Text txt_table;

    public string elementNumber;
    void Start() {
        // numberText.text = "Loaded";
    }

    public void OnClick() {
        element = elementNumber;
        txt.text = element;
        txt_table.text = element;
        Debug.Log(element);
        // if we need to spawn the element
        if(GameObject.Find("moleculeBody").transform.childCount == 0) {
            GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + element + ".prefab", typeof(GameObject)) as GameObject;
            GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity, GameObject.Find("moleculeBody").transform);
            creationUser.head = clone;
            clone.transform.Rotate(180,0,0);
        }
    }
}