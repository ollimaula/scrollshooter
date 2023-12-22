using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource[] _audio_sources;

    private void Awake() => _audio_sources = GetComponentsInChildren<AudioSource>();
    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        foreach (AudioSource source in _audio_sources)
            source.volume = volume;
    }
}
