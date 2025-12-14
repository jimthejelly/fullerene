using System;
using UnityEditor;
using UnityEngine;

namespace Molecule
{
    
    public class VisualBond : MonoBehaviour
    {

        private readonly VisualAtom _atom1;
        public VisualAtom GetAtom1() => _atom1;
        
        private readonly VisualAtom _atom2;
        public VisualAtom GetAtom2() => _atom2;

        public bool Matches(VisualAtom atom1, VisualAtom atom2)
        {
            return atom1 == _atom1 && atom2 == _atom2 || atom1 == _atom2 && atom2 == _atom1;
        }

        private VisualBond(VisualAtom atom1, VisualAtom atom2)
        {
            _atom1 = atom1;
            _atom2 = atom2;
        }
        

        public static VisualBond CreateVisualBond(VisualAtom atom1, VisualAtom atom2, int order)
        {
            
            var radius =
                atom1.GetAbstractAtom().GetCovalentRadius() +
                atom2.GetAbstractAtom().GetCovalentRadius();

            VisualBond visualBond = new(atom1, atom2);
            
            // loading new bond asset
            // Assets/Resources/[bond type].prefab
            var obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/" + order switch {
                1 => "SingleBond",
                2 => "DoubleBond",
                3 => "TripleBond",
                _ => throw new ArgumentException("given bond order " + order)
            } + ".prefab", typeof(GameObject)) as GameObject;
            
            var bondGameObject = Instantiate(obj, Vector3.zero, Quaternion.identity);
            bondGameObject.transform.localScale = new Vector3(0.15f, radius / 2, 0.15f);
            bondGameObject.transform.SetParent(atom1.GetAbstractAtom().GetMolecule().GetVisualMolecule().transform, true);
            
            var mat = Resources.Load<Material>("BondColor");
            var invis = Resources.Load<Material>("Transparent");
            visualBond.GetComponentInParent<MeshRenderer>().material = invis;
            foreach (Transform child in bondGameObject.transform)
            {
                child.transform.GetComponent<Renderer>().material = mat; // This needs to loop for all children
            }
            
            return visualBond;

        }
        

        public void UpdatePosition() {
            
            // setting bond position
            Vector3 parentPos = _atom1.transform.position;
            Vector3 childPos = _atom2.transform.position;
            transform.position = new Vector3((parentPos.x + childPos.x) / 2, (parentPos.y + childPos.y) / 2, (parentPos.z + childPos.z) / 2);

            // setting bond rotation
            transform.LookAt(parentPos);
            transform.Rotate(90, 0, 0);

            // setting bond length
            transform.localScale = new Vector3(0.15f, Mathf.Sqrt(
                Mathf.Pow(parentPos.x - childPos.x, 2) + Mathf.Pow(parentPos.y - childPos.y, 2) + Mathf.Pow(parentPos.z - childPos.z, 2)) / 2, 0.15f);
            
        }

    }
    
}