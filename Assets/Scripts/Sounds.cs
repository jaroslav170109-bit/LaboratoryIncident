using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioClip[] sounds;

    // Свойство для быстрого доступа к AudioSource
    private AudioSource audioSrc => GetComponent<AudioSource>();

    public void PlaySound(AudioClip clip, float volume = 1f, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f)
    {
        if (clip == null) return; // Защита, если забыли вставить звук

        // Рандомим высоту звука, чтобы он не был монотонным
        audioSrc.pitch = Random.Range(p1, p2);

        // Воспроизводим звук поверх других (не прерывая предыдущие)
        audioSrc.PlayOneShot(clip, volume);
    }
}