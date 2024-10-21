using UnityEngine.UI;
using UnityEngine;

public class SFXSlider : MonoBehaviour
{
    Slider sfxSlider;
    public AudioSource[] sfxAS;

    // Start is called before the first frame update
    void Start()
    {
        sfxSlider = GetComponent<Slider>();
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }

    public void AdjustSFXVolume()
    {
        foreach (AudioSource sfx in sfxAS)
        {
            sfx.volume = sfxSlider.value;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }
}
