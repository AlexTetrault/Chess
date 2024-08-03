using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] soundEffects; // Array to hold the sound effects
    public AudioSource pieceAudioSource; // Reference to the AudioSource component

    public AudioSource musicAudioSource; // Reference to the AudioSource component

    public AudioClip musicClip;

    public void PlayRandomSound()
    {
        if (soundEffects.Length == 0) return;

        // Select a random sound effect
        int randomIndex = Random.Range(0, soundEffects.Length);
        AudioClip randomClip = soundEffects[randomIndex];

        // Play the selected sound effect
        pieceAudioSource.clip = randomClip;
        pieceAudioSource.Play();
    }

    void Start()
    {
        // Assign the music clip to the AudioSource
        musicAudioSource.clip = musicClip;

        // Set the AudioSource to loop
        musicAudioSource.loop = true;

        // Play the music
        musicAudioSource.Play();
    }
}
