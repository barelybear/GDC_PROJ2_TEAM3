using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource vfxAudioSource;

    // luu tru audio clip

    public AudioClip BGClip;
    public AudioClip deathClip;
    public AudioClip winClip;
    void Start()
    {
        musicAudioSource.clip = BGClip;
        musicAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
