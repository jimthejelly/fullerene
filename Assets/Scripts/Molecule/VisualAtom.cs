using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Molecule
{
    
    /// <summary> Handles the visual aspects of an atom. </summary>
    public class VisualAtom : MonoBehaviour
    {

        private AbstractAtom _abstractAtom;
        public AbstractAtom GetAbstractAtom() => _abstractAtom;


        private Vector3 _forceVector = Vector3.zero;
        public Vector3 GetForceVector() => _forceVector;
        public void SetForceVector(Vector3 forceVector) => _forceVector = forceVector;

        public void Init(AbstractAtom abstractAtom)
        {
            _abstractAtom = abstractAtom;
            
            GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + (Element)abstractAtom.GetProtons() + ".prefab", typeof(GameObject)) as GameObject;

            
            string name = transform.name;
            string[] split = name.Split('-');
            string nameNumber = split[0].Trim();
            int nameInt = int.Parse(nameNumber);
            Material mat = Elements.GetMaterialForElement(nameInt);
            transform.GetComponent<Renderer>().material = mat;
        }
        
        
        public void Start()
        {
            
            
        }

        
        
        
        public void CalculateForceVector() {
            
            // initialize force to zero
            _forceVector = Vector3.zero;
            
            // set up a variable to track how many forces are considered
            var numVectors = 0;

            var molecule = _abstractAtom.GetMolecule();
            foreach (var otherAtom in molecule.GetAtoms()
                         .Where(a => a != _abstractAtom))
            {
                
                var otherVisualAtom = otherAtom.GetVisualAtom();

                var r = Vector3.Distance(transform.position, otherVisualAtom.transform.position);
                var eps = Mathf.Sqrt(_abstractAtom.GetEpsilon() * otherAtom.GetEpsilon());
                var sig = (_abstractAtom.GetEpsilon() + otherAtom.GetSigma()) / 2;
                var force = 24 * eps * (2 * Mathf.Pow(sig / r, 12) - Mathf.Pow(sig / r, 6)) * (1 / r);

                if (molecule.GetBondStrength(_abstractAtom, otherAtom) > 0)
                {
                    // element is bonded; attract
                    
                    // increasing the pulling force if double or triple bonded to element
                    if (molecule.GetBondStrength(_abstractAtom, otherAtom) == 2) {
                        force = 24 * eps * (0.867f * Mathf.Pow(sig / r, 12) - Mathf.Pow(sig / r, 6)) * (1 / r);
                    }
                    else if (molecule.GetBondStrength(_abstractAtom, otherAtom) == 3) {
                        force = 24 * eps * (0.45f * Mathf.Pow(sig / r, 12) - Mathf.Pow(sig / r, 6)) * (1 / r);
                    }
                    // capping the force so the elements don't explode out
                    // this wouldn't be as necessary if I were better at math, but I'm not so here we are
                    if (force > 0) {
                        force = 0.02f;
                    }
                    Vector3 forceDirection = otherVisualAtom.transform.position - transform.position;
                    forceDirection.Normalize();
                    _forceVector += (forceDirection * force);
                    numVectors++;
                }
                else
                {
                    // element is not bonded; repel
                    // capping force so molecules don't explode out as much
                    if (force > 2f) force = 2f;
                    if (force < -2f) force = -2f;

                    Vector3 forceDirection = transform.position - otherVisualAtom.transform.position;
                    forceDirection.Normalize();
                    _forceVector += (forceDirection * force);
                    numVectors++;
                }
                
            }
            
            // average the total force vectors by the number of vectors
            if(numVectors > 0) {
                _forceVector /= numVectors;
            }

            Vector3 temp = _forceVector;
            temp.Normalize();
            Debug.DrawRay(transform.position, temp, Color.red);
        }
        
        
        public void UpdatePosition() {
            transform.position = Vector3.MoveTowards(
                transform.position,
                transform.position - _forceVector,
                _forceVector.magnitude * Time.deltaTime
            );
        }
        
    }
}