using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMoveSounds : MonoBehaviour
{
    public AudioClip[] pieceSounds;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }

    public void PlayRandomSound()
    {
        if (pieceSounds.Length == 0) return;

        // Select a random sound effect
        int randomIndex = Random.Range(0, pieceSounds.Length);
        AudioClip randomClip = pieceSounds[randomIndex];

        // Play the selected sound effect
        audioSource.clip = randomClip;
        audioSource.Play();
    }
}
