using UnityEngine;

public class AUDIO : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource vfxAudioSource;

    // luu tru audio clip
    public AudioClip menuAUDIOClip;
    void Start()
    {
        musicAudioSource.clip = menuAUDIOClip;
        musicAudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
