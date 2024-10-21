using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    Slider musicSlider;
    public AudioSource musicAS;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider = GetComponent<Slider>();
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    public void AdjustMusicVolume()
    {
        musicAS.volume = musicSlider.value;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }
}
