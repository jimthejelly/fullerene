using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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





    // Update is called once per frame
    void Update()
    {
        
    }
}
