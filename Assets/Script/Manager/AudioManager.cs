using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton instance
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get { return instance; }
    }

    // Audio clips
    public AudioClip[] chainReactionSound;
    public AudioClip explosionSound;
    public AudioClip addSphereSound;

    // Audio source to play the clips
    private AudioSource explosionClip;
    private AudioSource chainReactionAudioClip;

    // Initialize the singleton instance
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            explosionClip = gameObject.AddComponent<AudioSource>();
            chainReactionAudioClip = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to play a random audio clip from the array
    public void PlayRandomChainReactionSoundEffect()
    {
        if (chainReactionSound.Length > 0)
        {
            int randomIndex = Random.Range(0, chainReactionSound.Length);
            chainReactionAudioClip.clip = chainReactionSound[randomIndex];
            chainReactionAudioClip.Play();
        }
        else
        {
            Debug.LogWarning("No random clips available.");
        }
    }

    // Method to play a specific audio clip
    public void ExplosionSound()
    {
        if (explosionSound != null)
        {
            explosionClip.clip = explosionSound;
            explosionClip.Play();
        }
        else
        {
            Debug.LogWarning("No specific clip available.");
        }
    }

    public void PlayAddSphereSound()
    {
        if (addSphereSound != null)
        {
            explosionClip.clip = addSphereSound;
            explosionClip.Play();
        }
        else
        {
            Debug.LogWarning("No specific clip available.");
        }
    }

    public void StopExplosionSound()
    {
        if (explosionSound != null)
        {
            if (explosionClip.isPlaying)
                explosionClip.Stop();
        }
        else
        {
            Debug.LogWarning("No specific clip available.");
        }
    }
}