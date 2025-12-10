
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System;



// public static class Globals {
//     public static GameObject element;
// }

public class selectPreset : MonoBehaviour
{
    public static string preset;
    public int presetNumber;
    string path;
    void Start()
    {
        // numberText.text = "Loaded";
    }

    private void saveMolecule()
    {
        GameObject molecule = GameObject.Find("moleculeBody");
        if (molecule.transform.childCount == 0)
        {
            Debug.Log("No molecule to save!");
            return;
        }
        XmlTextWriter writer = new XmlTextWriter("./Assets/Resources/molecule.cml", null);
        writer.WriteStartDocument();

        writer.Formatting = Formatting.Indented;
        writer.Indentation = 4;

        writer.WriteStartElement("molecule");

        writer.WriteStartElement("atomArray");

        Dictionary<Elements, string> atomIDs = new Dictionary<Elements, string>();
        Dictionary<string, Elements> IDToAtom = new Dictionary<string, Elements>();
        int count = 1;
        foreach (Transform item in molecule.transform)
        {
            if (item.tag.Equals("Element"))
            {
                atomIDs.Add(item.gameObject.GetComponent<Elements>() as Elements, "a" + count);
                IDToAtom.Add("a" + count++, item.gameObject.GetComponent<Elements>() as Elements);
                writer.WriteStartElement("atom");
                writer.WriteAttributeString("id", atomIDs[item.gameObject.GetComponent<Elements>() as Elements]);
                writer.WriteAttributeString("elementType", ((ElementSymbols)((item.gameObject.GetComponent<Elements>() as Elements).protons)).ToString("F"));
                writer.WriteAttributeString("x3", item.position.x.ToString());
                writer.WriteAttributeString("y3", item.position.y.ToString());
                writer.WriteAttributeString("z3", item.position.z.ToString());
                writer.WriteEndElement(); // end of atom
            }
        }

        writer.WriteEndElement(); // end of atomArray

        writer.WriteStartElement("bondArray");

        foreach (Transform item in molecule.transform)
        {
            if (item.tag.Equals("Bond"))
            {
                writer.WriteStartElement("bond");
                writer.WriteAttributeString("atomRefs2", atomIDs[(item.gameObject.GetComponent<Bonds>() as Bonds).parent] + " " + atomIDs[(item.gameObject.GetComponent<Bonds>() as Bonds).child]);
                writer.WriteAttributeString("order", ((item.gameObject.GetComponent<Bonds>() as Bonds).bondOrder).ToString());
                writer.WriteEndElement(); // end of bond
            }
        }

        writer.WriteEndElement(); // end of bondArray

        writer.WriteEndElement(); // end of molecule

        writer.WriteEndDocument();
        writer.Close();
    }

    private void saveNeighbors()
    {
        GameObject molecule = GameObject.Find("moleculeBody");
        if (molecule.transform.childCount == 0)
        {
            Debug.Log("No molecule to save!");
            return;
        }
        string path = "Assets/Resources/Presets/template.txt";
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        else
        {
            File.WriteAllText(path, string.Empty);
        }

        foreach (Transform item in molecule.transform)
        {
            if (item.tag.Equals("Element"))
            {
                string line = "";
                List<Tuple<int, int>> neighbors = (item.gameObject.GetComponent<Elements>() as Elements).GetStringNeighbors();
                line += neighbors[0].Item1 + ": ";
                foreach (Tuple<int, int> neighbor in neighbors)
                {
                    line += neighbor.Item2 + " ";
                }
                line += "\n";
                File.AppendAllText(path, line);
            }
        }
    }

    public void savePreset()
    {
        saveNeighbors();
        saveMolecule();
        GameObject body = GameObject.Find("moleculeBody");
        preset = "Preset " + presetNumber;
        path = "Assets/Resources/Presets/" + preset + ".prefab";
        if (File.Exists("Assets/Resources/Presets/" + preset + ".prefab"))
        {
            Debug.Log("Existing Asset Deleted" + path);
            AssetDatabase.DeleteAsset(path);
        }
        GameObject obj = GameObject.Find("moleculeBody");
        if (obj.transform.childCount > 0)
        {
            bool success;
            PrefabUtility.SaveAsPrefabAsset(obj, path, out success);
            if (success)
            {
                Debug.Log("Preset Saved: " + path);
            }
            else
            {
                Debug.Log("Error Saving Preset");
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.Log("No molecule to save");
        }
    }

    public void getPreset()
    {
        GameObject body = GameObject.Find("moleculeBody");
        if (body == null)
        {
            Debug.LogError("No moleculeBody object in scene.");
            return;
        }
        foreach (Transform child in body.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        //Load CML
        string cmlPath = "./Assets/Resources/molecule.cml";
        if (!File.Exists(cmlPath))
        {
            Debug.LogError("CML file not found at: " + cmlPath);
            return;
        }

        XmlDocument doc = new XmlDocument();
        try
        {
            doc.Load(cmlPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load CML: " + e);
            return;
        }

        XmlNode moleculeNode = doc.SelectSingleNode("/molecule");
        if (moleculeNode == null)
        {
            Debug.LogError("CML has no <molecule> root.");
            return;
        }
    }
}

 public enum ElementNames
    {
        Hydrogen = 1,
        Helium = 2,
        Lithium = 3,
        Beryllium = 4,
        Boron = 5,
        Carbon = 6,
        Nitrogen = 7,
        Oxygen = 8,
        Fluorine = 9,
        Neon = 10,
        Sodium = 11,
        Magnesium = 12,
        Aluminum = 13,
        Silicon = 14,
        Phosphorus = 15,
        Sulfur = 16,
        Chlorine = 17,
        Argon = 18,
        Potassium = 19,
        Calcium = 20,
        Scandium = 21,
        Titanium = 22,
        Vanadium = 23,
        Chromium = 24,
        Manganese = 25,
        Iron = 26,
        Cobalt = 27,
        Nickel = 28,
        Copper = 29,
        Zinc = 30,
        Gallium = 31,
        Germanium = 32,
        Arsenic = 33,
        Selenium = 34,
        Bromine = 35,
        Krypton = 36,
        Rubidium = 37,
        Strontium = 38,
        Yttrium = 39,
        Zirconium = 40,
        Niobium = 41,
        Molybdenum = 42,
        Technetium = 43,
        Ruthenium = 44,
        Rhodium = 45,
        Palladium = 46,
        Silver = 47,
        Cadmium = 48,
        Indium = 49,
        Tin = 50,
        Antimony = 51,
        Tellurium = 52,
        Iodine = 53,
        Xenon = 54,
        Cesium = 55,
        Barium = 56,
        Lanthanum = 57, 
        Cerium = 58,
        Praseodymium = 59,
        Neodymium = 60,
        Promethium = 61,
        Samarium = 62,
        Europium = 63,
        Gadolinium = 64,
        Terbium = 65,
        Dysprosium = 66,
        Holmium = 67,
        Erbium = 68,
        Thulium = 69,
        Ytterbium = 70,
        Lutetium = 71, 
        Hafnium = 72,
        Tantalum = 73,
        Tungsten = 74,
        Rhenium = 75,
        Osmium = 76,
        Iridium = 77,
        Platinum = 78,
        Gold = 79,
        Mercury = 80,
        Thallium = 81,
        Lead = 82,
        Bismuth = 83,
        Polonium = 84,
        Astatine = 85,
        Radon = 86,
        Francium = 87,
        Radium = 88,
        Actinium = 89, 
        Thorium = 90,
        Protactinium = 91,
        Uranium = 92,
        Neptunium = 93,
        Plutonium = 94,
        Americium = 95,
        Curium = 96,
        Berkelium = 97,
        Californium = 98,
        Einsteinium = 99,
        Fermium = 100,
        Mendelevium = 101,
        Nobelium = 102,
        Lawrencium = 103, 
        Rutherfordium = 104,
        Dubnium = 105,
        Seaborgium = 106,
        Bohrium = 107,
        Hassium = 108,
        Meitnerium = 109,
        Darmstadtium = 110,
        Roentgenium = 111,
        Copernicium = 112,
        Nihonium = 113,
        Flerovium = 114,
        Moscovium = 115,
        Livermorium = 116,
        Tennessine = 117,
        Oganesson = 118
    }