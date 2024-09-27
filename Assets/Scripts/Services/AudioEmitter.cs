using GameCore;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif
namespace Services
{
    public class AudioEmitter : MonoBehaviour
    {
        [SerializeField] public AudioSource source;
        [SerializeField, HideInInspector] protected bool hasSource;
        [SerializeField] protected AudioCueScriptableObject audioCue;

        private void OnValidate()
        {
            if (hasSource && source == null)
            {
                hasSource = false;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
            else if (!hasSource && source != null)
            {
                hasSource = true;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        public void PlayAudio(bool usePitch = true)
        {
            if (enabled)
            {
                if (hasSource)
                {
                    if (source.isPlaying)
                        source.Stop();
                    audioCue.AppendTo(source, usePitch);
                    source.Play();
                }
                else
                {
                    GameInstance.Audio.Play(audioCue, usePitch);
                }
            }
        }
    }
}