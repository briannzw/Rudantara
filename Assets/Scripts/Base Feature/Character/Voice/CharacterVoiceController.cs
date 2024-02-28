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
        if (audioSource.isPlaying) return;

        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
