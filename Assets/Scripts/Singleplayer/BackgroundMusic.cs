using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    AudioSource musicAS;

    // Start is called before the first frame update
    void Start()
    {
        musicAS = GetComponent<AudioSource>();
        musicAS.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicAS.Play();
        musicAS.loop = true;
    }
}
