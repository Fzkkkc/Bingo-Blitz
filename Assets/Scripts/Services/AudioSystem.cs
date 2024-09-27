using UnityEngine;

namespace Services
{
    public class AudioSystem : MonoBehaviour
    {
        [SerializeField] private int maxAudioCount = 3;
        private AudioSource[] sources;
        private int current;

        public AudioCueScriptableObject TapSound, WinGameEndSound, LoseGameEndSound;

        public float Volume;
        
        public void Init()
        {
            sources = new AudioSource[maxAudioCount];
            for (int i = 0; i < sources.Length; i++)
                sources[i] = gameObject.AddComponent<AudioSource>();
        }
            
        public void Play(AudioCueScriptableObject audioCue, bool usePitch = true)
        {
            var s = sources[current];
            if (s.isPlaying)
                s.Stop();
            audioCue.AppendTo(s, usePitch);
            s.volume = Volume;
            s.Play();
            current++;
            if (current >= maxAudioCount)
                current = 0;
        }
    }
}