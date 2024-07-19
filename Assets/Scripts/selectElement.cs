using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
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
    public static string element;
    public TMP_Text txt;
    public static string element = "6-Carbon";

    public string elementNumber;
    void Start() {
        // numberText.text = "Loaded";
    }

    public void OnClick() {
        element = elementNumber;
        txt.text = element;
        Debug.Log(element);
    }
}