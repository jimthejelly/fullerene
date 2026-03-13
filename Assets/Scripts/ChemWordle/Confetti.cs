using UnityEngine;

namespace ChemWordle
{
    /// <summary>
    /// Class for the objects that spawn confetti particles in the victory screen.
    /// Consists of several particle objects that play as soon as this object is created.
    /// </summary>
    public class Confetti : MonoBehaviour
    {

        public ParticleSystem confettiCannon1;
        public ParticleSystem confettiCannon2;
        public ParticleSystem confettiCannon3;

        // Start is called before the first frame update
        void Start()
        {
            confettiCannon1.Play(); confettiCannon2.Play(); confettiCannon3.Play();
        }
        
    }
}
