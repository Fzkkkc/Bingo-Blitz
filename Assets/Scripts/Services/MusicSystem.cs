using UnityEngine;

namespace Services
{
    public class MusicSystem : AudioEmitter
    {
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip _shopMusic;
        
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
            else if(!isMenu && source != _shopMusic)
            {
                source.clip = _shopMusic;
            }

            source.Play();
            source.loop = true;
        }

        private void Update()
        {
            if (GameInstance.UINavigation._isInLoad)
                source.volume = 0;
            else
                source.volume = 0.3f;
        }
    }
}