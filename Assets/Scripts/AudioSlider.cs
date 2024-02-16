using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public Slider slider;


    void Start()
    {
        if (PlayerPrefs.HasKey(MainManager.Instance.VolumePrefsKey))
        {
            MainManager.Instance.audioSource.volume = PlayerPrefs.GetFloat(MainManager.Instance.VolumePrefsKey);
        }

        slider.value = MainManager.Instance.audioSource.volume;
        // Add a listener to respond to changes in the slider value
        slider.onValueChanged.AddListener(delegate { OnSliderChanged(); });
    }

    // Update the audio volume when the slider value changes
    void OnSliderChanged()
    {
        MainManager.Instance.audioSource.volume = slider.value;
        PlayerPrefs.SetFloat(MainManager.Instance.VolumePrefsKey, MainManager.Instance.audioSource.volume);
        PlayerPrefs.Save();
    }
}
