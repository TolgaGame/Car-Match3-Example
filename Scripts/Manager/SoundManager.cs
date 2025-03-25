using UnityEngine;

public class SoundManager : MonoBehaviour {

    #region Fields

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] soundEffects;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    #endregion

    #region Unity Methods

        private void Awake() {
            LoadVolumeSettings();
        }

    #endregion

    #region Public Methods

        public void PlayBGM(AudioClip clip, bool loop = true) {
            if (clip == null) {
                return;
            }
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }

        public void StopBGM() {
            bgmSource.Stop();
        }

        public void PlaySFX(string soundName) {
            AudioClip clip = GetClipByName(soundName);
            if (clip == null) {
                return;
            }
            sfxSource.PlayOneShot(clip);
        }

        public void SetBGMVolume(float volume) {
            volume = Mathf.Clamp01(volume);
            bgmSource.volume = volume;
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        }

        public void SetSFXVolume(float volume) {
            volume = Mathf.Clamp01(volume);
            sfxSource.volume = volume;
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        }

        public void ToggleMute() {
            bool isMuted = bgmSource.volume == 0 && sfxSource.volume == 0;
            float volume = isMuted ? 1f : 0f;
            SetBGMVolume(volume);
            SetSFXVolume(volume);
        }

    #endregion

    #region Private Methods

        private void LoadVolumeSettings() {
            bgmSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
            sfxSource.volume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        }

        private AudioClip GetClipByName(string name) {
            foreach (var clip in soundEffects) {
                if (clip.name == name) {
                    return clip;
                }
            }
            Debug.LogWarning($"SoundManager: {name} audioclip not found!");
            return null;
        }

    #endregion

}