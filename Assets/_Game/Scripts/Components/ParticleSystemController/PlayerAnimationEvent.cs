using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    [SerializeField] private ParticleSystem slashEffect;
    [SerializeField] private ParticleSystem shotEffect;

    public string slashSound;
    private void Slash()
    {
        slashEffect.Play();
    }
    private void SlashSound()
    {
        TPRLSoundManager.Instance.PlaySoundFx(slashSound);
    }
    private void Shot()
    {
        shotEffect.Play();
        TPRLSoundManager.Instance.PlaySoundFx("voxel_explosion_bullet");
    }
    private void SlowTimeScale()
    {
        Time.timeScale = 0.4f;
    }
    private void ResetTimeScale()
    {
        Time.timeScale = 1;
    }
}
