using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour 
{
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 10, 44100); // Device name, loop recording, length in seconds, sample rate
        while (!(Microphone.GetPosition(null) > 0)){} // Wait until the recording has started
        audioSource.Play(); // For testing, play the audio back through speakers
    }
}
