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
public class selectPreset : MonoBehaviour
{
    public static string preset;
    public int presetNumber;
    void Start() {
        // numberText.text = "Loaded";
    }

    public void OnClick() {
        preset = "Preset" + presetNumber;
    }
    

}