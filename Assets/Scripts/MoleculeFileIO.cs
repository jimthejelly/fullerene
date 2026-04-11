using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Handles conversion between in-engine molecules and .cml files.
/// </summary>
public static class MoleculeFileIO
{

    /// <summary>
    /// Saves the given molecule to the given file.
    /// </summary>
    /// <param name="file"> The file to save the molecule to. </param>
    /// <param name="molecule"> The molecule to save. </param>
    public static void SaveMolecule(FileInfo file, GameObject molecule)
    {
        if(molecule.transform.childCount == 0) {
            Debug.Log("No molecule to save!");
            return;
        }
        XmlTextWriter writer = new XmlTextWriter(file.ToString(), null);
        writer.WriteStartDocument();

        writer.Formatting = Formatting.Indented;
        writer.Indentation = 4;

        writer.WriteStartElement("molecule");

        writer.WriteStartElement("atomArray");

        // used for referring to elements when defining bonds
        Dictionary<Elements, string> atomIDs = new();
        
        int count = 1;
        foreach(Transform item in molecule.transform) {
            Elements atom = item.gameObject.GetComponent<Elements>();
            if (!atom) continue; // not an element; ignore
            
            string atomID = "a" + count;
            atomIDs.Add(atom, atomID);
            count++;
            
            writer.WriteStartElement("atom");
            writer.WriteAttributeString("id", atomID);
            writer.WriteAttributeString("elementType", atom.protons.ToString());
            writer.WriteAttributeString("x3", item.position.x.ToString());
            writer.WriteAttributeString("y3", item.position.y.ToString());
            writer.WriteAttributeString("z3", item.position.z.ToString());
            writer.WriteEndElement(); // end of atom
        }

        writer.WriteEndElement(); // end of atomArray

        writer.WriteStartElement("bondArray");

        foreach(Transform item in molecule.transform) {
            Bonds bond = item.gameObject.GetComponent<Bonds>();
            if (!bond) continue; // not a bond; ignore
            writer.WriteStartElement("bond");
            writer.WriteAttributeString("atomRefs2", atomIDs[bond.parent] + " " + atomIDs[bond.child]);
            writer.WriteAttributeString("order", bond.bondOrder.ToString());
            writer.WriteEndElement(); // end of bond
        }
        
        writer.WriteEndElement(); // end of bondArray

        writer.WriteEndElement(); // end of molecule

        writer.WriteEndDocument();
        writer.Close();
    }

    private record IntermediateElement
    {
        private readonly int elementType;
        public int GetElementType() => elementType;
        private readonly float x3;
        private readonly float y3;
        private readonly float z3;

        public IntermediateElement(int elementType, float x3, float y3, float z3)
        {
            this.elementType = elementType;
            this.x3 = x3;
            this.y3 = y3;
            this.z3 = z3;
        }

        public void InitializeToElement(Elements element)
        {
            element.protons = elementType;
            element.transform.position = new Vector3(x3, y3, z3);
        }
        
    }

    /// <summary>
    /// Loads a molecule from the given file.
    /// </summary>
    /// <param name="file"> The file to read from. </param>
    /// <returns> The newly created molecule. </returns>
    public static GameObject LoadMolecule(FileInfo file)
    {
        //if(atomIDs.Count == 0) {
        //    Debug.Log("no IDs stored");
        //    return;
        //}
        // move atoms into place
        // create molecule game object
        GameObject molecule = new();
        molecule.AddComponent<keepMolecule>();
        
        Dictionary<Elements, string> atomIDs = new();
        Dictionary<string, IntermediateElement> idToIntermediate = new();
        Dictionary<string, Elements> idToAtom = new();

        string firstAtomID = null;
        
        // "id1 id2" -> bond order
        Dictionary<string, int> bondOrders = new();
        
        XmlTextReader reader = new XmlTextReader(file.ToString());
        
        while (reader.Read()) {
            
            if (reader.NodeType == XmlNodeType.Element) {
                
                if (reader.Name.Equals("atom"))
                {
                    // can't create the element yet since its parent is unknown,
                    // so store it as this instead
                    string id = reader.GetAttribute("id");
                    string elementType = reader.GetAttribute("elementType");
                    string x3 = reader.GetAttribute("x3");
                    string y3 = reader.GetAttribute("y3");
                    string z3 = reader.GetAttribute("z3");
                    idToIntermediate[id] = new IntermediateElement(int.Parse(elementType),
                        float.Parse(x3), float.Parse(y3), float.Parse(z3));

                    if (firstAtomID == null) firstAtomID = id;

                }

                if (reader.Name.Equals("bondArray"))
                {
                    // done with atoms.
                    // take this opportunity to initialize the root atom
                    IntermediateElement intermediate = idToIntermediate[firstAtomID];
                    GameObject elementPrefab = Elements.GetElementPrefab(intermediate.GetElementType());
                    GameObject rootObject = UnityEngine.Object.Instantiate(elementPrefab, Vector3.zero, Quaternion.identity);
                    Elements atom = rootObject.GetComponent<Elements>();
                    intermediate.InitializeToElement(atom);
                    atom.SetMolecule(molecule);
                    idToAtom[firstAtomID] = atom;
                    atomIDs[atom] = firstAtomID;
                    rootObject.transform.SetParent(molecule.transform);
                }
                
                if (reader.Name.Equals("bond"))
                {
                    string atoms = reader.GetAttribute("atomRefs2");
                    if (atoms == null) throw new Exception("missing field for bond: atomRefs2");
                    string parentID = atoms[..atoms.IndexOf(" ")];
                    string childID = atoms[(atoms.IndexOf(" ") + 1)..];
                    
                    string orderString = reader.GetAttribute("order");
                    if (orderString == null) throw new Exception("missing field for bond: order");
                    int order = int.Parse(orderString);
                    
                    // if the child doesn't exist, create it here
                    if (!idToAtom.ContainsKey(childID))
                    {
                        IntermediateElement childIntermediate = idToIntermediate[childID];
                        Elements parent = idToAtom[parentID];
                        parent.SpawnElement(childIntermediate.GetElementType(), order, 0);
                        // newly spawned atom will be last in parent's list of elements
                        Elements atom = parent.GetNeighbors()[^1].Item2.GetComponent<Elements>();
                        childIntermediate.InitializeToElement(atom);
                        idToAtom[childID] = atom;
                        atomIDs[atom] = childID;
                    }
                    
                }
                
            }
            
        }
        reader.Close();

        // move bonds
        foreach(Transform item in molecule.transform) {
            
            Bonds bonds = item.gameObject.GetComponent<Bonds>();
            if (!bonds) continue;
            
            // setting bond position
            Vector3 parentPos = bonds.parent.transform.position;
            Vector3 childPos = bonds.child.transform.position;
            Debug.Log(parentPos + " " + childPos);
            item.position = new Vector3((parentPos.x + childPos.x) / 2, (parentPos.y + childPos.y) / 2, (parentPos.z + childPos.z) / 2);

            // setting bond rotation
            item.LookAt(parentPos);
            item.Rotate(90, 0, 0);

            // setting bond length
            item.localScale = new Vector3(0.15f, Mathf.Sqrt(
                Mathf.Pow(parentPos.x - childPos.x, 2) + Mathf.Pow(parentPos.y - childPos.y, 2) + Mathf.Pow(parentPos.z - childPos.z, 2)) / 2, 0.15f);
            
        }

        return molecule;

    }
    
}