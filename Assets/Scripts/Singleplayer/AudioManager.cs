using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] soundEffects;

    public AudioSource pieceAudioSource;

    private void Start()
    {
        pieceAudioSource = GetComponent<AudioSource>();
        pieceAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }

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
}
