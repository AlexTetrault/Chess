using UnityEngine;

public class SFX : MonoBehaviour
{
    AudioSource SFX_AS;

    // Start is called before the first frame update
    void Start()
    {
        SFX_AS = GetComponent<AudioSource>();
        SFX_AS.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }
}
