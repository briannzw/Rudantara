using UnityEngine;

public class CharacterVoiceController : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayCharaVoice(AudioClip audioClip)
    {
        // Prevent Frequent Same Voices
        if (Random.Range(0, 100) < 10f) return;

        if (audioSource.isPlaying) audioSource.Stop();

        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
