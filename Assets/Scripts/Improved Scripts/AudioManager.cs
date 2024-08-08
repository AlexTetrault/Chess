using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] soundEffects; // Array to hold the sound effects

    public AudioSource pieceAudioSource; // Reference to the AudioSource component
    public Slider sfxSlider;

    public AudioSource musicAudioSource; // Reference to the AudioSource component
    public Slider musicSlider;

    public AudioClip musicClip;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

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

        musicSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFXVolumeKey, 1.0f);
    }

    public void ChangeMusicVolume()
    {
        musicAudioSource.volume = 0.5f * musicSlider.value;

        if (musicAudioSource.volume > 0.5f)
        {
            musicAudioSource.volume = 0.5f;
        }
    }

    public void ChangeSFXVolume()
    {
        pieceAudioSource.volume = 1 * sfxSlider.value;

        if (pieceAudioSource.volume > 1)
        {
            pieceAudioSource.volume = 1;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, musicSlider.value);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxSlider.value);
        PlayerPrefs.Save();
    }
}
