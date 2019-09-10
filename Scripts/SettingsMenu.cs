using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

	public AudioMixer mixer;
	public Slider masterVolumeSlider;
	public Slider musicVolumeSlider;
	public Slider narrationVolumeSlider;
	public Slider sfxVolumeSlider;
	public Dropdown qualitySettings;
	public Toggle fullscreenToggle;
	public Slider mouseSensitivitySlider;

	void Awake ()
	{
		qualitySettings.ClearOptions ();
		string[] names = QualitySettings.names;
		for (int i = names.Length - 1; i >= 0; i--) {
			Dropdown.OptionData option = new Dropdown.OptionData ();
			option.text = names [i];
			qualitySettings.options.Add (option);
		}
		if (PlayerPrefs.HasKey ("MasterVolume")) {
			masterVolumeSlider.value = PlayerPrefs.GetFloat ("MasterVolume");
		} else {
			masterVolumeSlider.value = masterVolumeSlider.maxValue;
		}

		if (PlayerPrefs.HasKey ("MusicVolume")) {
			musicVolumeSlider.value = PlayerPrefs.GetFloat ("MusicVolume");
		} else {
			musicVolumeSlider.value = musicVolumeSlider.maxValue;
		}

		if (PlayerPrefs.HasKey ("NarrationVolume")) {
			narrationVolumeSlider.value = PlayerPrefs.GetFloat ("NarrationVolume");
		} else {
			narrationVolumeSlider.value = narrationVolumeSlider.maxValue;
		}

		if (PlayerPrefs.HasKey ("SFXVolume")) {
			sfxVolumeSlider.value = PlayerPrefs.GetFloat ("SFXVolume");
		} else {
			sfxVolumeSlider.value = sfxVolumeSlider.maxValue;
		}

		int quality;
		if (PlayerPrefs.HasKey ("Quality")) {
			quality = PlayerPrefs.GetInt ("Quality");
		} else {
			quality = qualitySettings.options.Count - 1;
		}
		QualitySettings.SetQualityLevel (quality);
		qualitySettings.value = qualitySettings.options.Count - 1 - quality;
		fullscreenToggle.isOn = Screen.fullScreen;

		if (PlayerPrefs.HasKey ("MouseSensitivity")) {
			mouseSensitivitySlider.value = PlayerPrefs.GetFloat ("MouseSensitivity");
		} else {
			mouseSensitivitySlider.value = 2;
		}
	}

	public void SetMasterVolume (float masterVolume)
	{
		mixer.SetFloat ("MasterVolume", masterVolume);
		PlayerPrefs.SetFloat ("MasterVolume", masterVolume);
	}

	public void SetMusicVolume (float musicVolume)
	{
		mixer.SetFloat ("MusicVolume", musicVolume);
		PlayerPrefs.SetFloat ("MusicVolume", musicVolume);
	}


	public void SetNarrationVolume (float narrationVolume)
	{
		mixer.SetFloat ("NarrationVolume", narrationVolume);
		PlayerPrefs.SetFloat ("NarrationVolume", narrationVolume);
	}

	public void SetSFXVolume (float sfxVolume)
	{
		mixer.SetFloat ("SFXVolume", sfxVolume);
		PlayerPrefs.SetFloat ("SFXVolume", sfxVolume);
	}

	public void SetQuality (int quality)
	{
		QualitySettings.SetQualityLevel (qualitySettings.options.Count - 1 - quality);
		PlayerPrefs.SetInt ("Quality", qualitySettings.options.Count - 1 - quality);
	}

	public void SetFullscreen (bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
	}

	public void SetMouseSensitivity (float mouseSensitivity)
	{
		PlayerPrefs.SetFloat ("MouseSensitivity", mouseSensitivity);
	}

}
