using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance {get; private set;}

    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    /* -------------------------- Unity event functions ------------------------- */
    private void Awake() {
        if (Instance == null)
            Instance = this;
    }
    private void Start() {
        audioSource.loop = true;
    }

    /* ---------------------------- Public functions ---------------------------- */
    
    public void PlaySoundRepeating(AudioClip clip) {
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void PlaySound(AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }
}
