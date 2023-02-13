using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneComponent : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    public void PlayParticle()
    {
        particle.Play();
    }
}
