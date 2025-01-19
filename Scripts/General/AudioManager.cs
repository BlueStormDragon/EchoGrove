using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Slider soundSlider;
    public AudioMixer audioMixer;

    private void Start()
    {
        SetVolume(SaveManager.instance.GetVolumeLevel());
    }
    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("Volume", volume);
        SaveManager.instance.SetVolumeLevel(volume);
        RefreshSlider(volume);
    }

    private void RefreshSlider(float value)
    {
        soundSlider.value = value;
    }
}
