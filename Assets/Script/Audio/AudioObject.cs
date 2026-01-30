using UnityEngine;

namespace GGJ2026
{
    [RequireComponent(typeof(IAudioObject))]
    [RequireComponent(typeof(AudioSource))]
    public class AudioObject
    {
        private IAudioObject audioObject;
        private AudioSource au;
        private AudioClip clip;
    }

    public interface IAudioObject
    {
        
    }
}