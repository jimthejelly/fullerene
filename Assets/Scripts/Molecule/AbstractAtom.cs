using System;
using System.Linq;
using UnityEngine;

namespace Molecule
{
    
    /// <summary> An atom. Holds fundamental properties like number of protons. </summary>
    public class AbstractAtom
    {

        /// <summary> A reference back to this atom's molecule. </summary>
        private readonly AbstractMolecule _molecule;
        public AbstractMolecule GetMolecule() => _molecule;
        
        private readonly VisualAtom _visualAtom;
        public VisualAtom GetVisualAtom() => _visualAtom;

        /// <param name="molecule"> The molecule this atom belongs to. </param>
        /// <param name="element"> The element of this atom. </param>
        public AbstractAtom(AbstractMolecule molecule, Element element)
        {
            _molecule = molecule;
            
            // TODO: set protons, neutrons, etc. based on element
            
            // instantiate visual atom
            _visualAtom = new GameObject().AddComponent<VisualAtom>();
            _visualAtom.Init(this);
            
        }

        /// <summary> How many protons are in the nucleus of the atom. </summary>
        private readonly int _protons;
        public int GetProtons() => _protons;
        
        /// <summary> How many neutrons are in the nucleus of the atom. </summary>
        private readonly int _neutrons;
        public int GetNeutrons() => _neutrons;
        
        /// <summary> How many electrons are around the nucleus of atom. </summary>
        private readonly int _electrons;
        public int GetElectrons() => _electrons;

        /// <summary> Something important that I don't understand. </summary>
        private readonly bool _expandedOctet;
        public bool IsExpandedOctet() => _expandedOctet;
        
        public bool physicsOn = false;
    
        public bool hasMoved = false;

        private readonly float _covalentRadius;
        public float GetCovalentRadius() => _covalentRadius;
        
        private readonly float _epsilon;
        public float GetEpsilon() => _epsilon;
        
        private readonly float _sigma;
        public float GetSigma() => _sigma;

        public int GetBondingElectrons()
        {
            // TODO: fact check this
            return _molecule.GetTotalOutgoingBonds(this);
        }
        
        public int GetLonePairs()
        {
            // TODO: fact check this
            return (int)Math.Ceiling((_electrons - _molecule.GetTotalOutgoingBonds(this)) / 2.0);
        }

        /// <returns> Whether this atom is considered equal to the given atom. </returns>
        public bool EqualsAtom(AbstractAtom other)
        {
            
            // make sure properties match
            if (_protons != other._protons) return false;
            if (_neutrons != other._neutrons) return false;

            // elaborate bond matching check
            return _molecule.GetBondedNeighbors(this).All(thisNeighbor =>
                    other._molecule.GetBondedNeighbors(other).Any(otherNeighbor =>
                        _molecule.GetBondStrength(this, thisNeighbor) ==
                        other._molecule.GetBondStrength(other, otherNeighbor) &&
                        thisNeighbor._protons == otherNeighbor._protons)
                );
            
        }
        

        /// <returns> Whether this atom is able to form additional bonds. </returns>
        public bool CanBondMore()
        {
            if (_expandedOctet)
                return GetBondingElectrons() + 2 * GetLonePairs() > 0;
            return GetBondingElectrons() > 0;
        }

    }
    
}