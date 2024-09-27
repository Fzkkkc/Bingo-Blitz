using UnityEngine;

namespace Services
{
    public class MusicSystem : AudioEmitter
    {
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip _gameMusic;
        
        public void Init()
        {
            if (hasSource && audioCue != null)
            {
                source.loop = true;
                PlayAudio(false);
            }
        }

        public void ChangeMusicClip(bool isMenu = true)
        {
            if (isMenu && source != _menuMusic)
            {
                source.clip = _menuMusic;
            }
            else if(!isMenu && source != _gameMusic)
            {
                source.clip = _gameMusic;
            }

            source.Play();
            source.loop = true;
        }
    }
}