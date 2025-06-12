using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;

    private void Awake()
    {
        _audioMixer.GetFloat("masterVolume", out float masterVolume);
        _masterSlider.SetValueWithoutNotify(Mathf.Pow(10, masterVolume / 20f));
        _audioMixer.GetFloat("sfxVolume", out float sfxVolume);
        _sfxSlider.SetValueWithoutNotify(Mathf.Pow(10, sfxVolume / 20f));
        _audioMixer.GetFloat("musicVolume", out float musicVolume);
        _musicSlider.SetValueWithoutNotify(Mathf.Pow(10, musicVolume / 20f));
    }

    public void SetMasterVolume(float level)
    {
        _audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }
    public void SetSoundFXVolume(float level)
    {
        _audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);

    }
    public void SetMusicVolume(float level)
    {
        _audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);

    }
}
