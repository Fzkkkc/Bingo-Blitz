using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class FXController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _tapFX;
        [SerializeField] private ParticleSystem _bingoLeft;
        [SerializeField] private ParticleSystem _bingoRight;
        
        [SerializeField] private List<ParticleSystem> _particleSystems;
        [SerializeField] private List<ParticleSystem> _backgroundGameParticleSystems;
        [SerializeField] private List<ParticleSystem> _backgroundMenuParticleSystems;
        [SerializeField] private List<ParticleSystem> _backgroundMenuHideParticleSystems;
        [SerializeField] private List<ParticleSystem> _fireworksFX;

        [SerializeField] private Transform _fieldTransformGame;
        [SerializeField] private Transform _fieldTransformMenu;

        private void AddChildTransformsToBackgroundGameParticleSystems()
        {
            _backgroundGameParticleSystems.Clear(); 

            foreach (Transform child in _fieldTransformGame)
            {
                var fxComponent = child.GetComponent<ParticleSystem>();
                _backgroundGameParticleSystems.Add(fxComponent);
            }
        }

        private void AddChildTransformsToBackgroundMenuParticleSystems()
        {
            _backgroundMenuParticleSystems.Clear(); 
            
            foreach (Transform child in _fieldTransformMenu)
            {
                var fxComponent = child.GetComponent<ParticleSystem>();
                _backgroundMenuParticleSystems.Add(fxComponent);
            }

        }

        public void PlayBingoParticle(int indexBingo)
        {
            switch (indexBingo)
            {
                case 1:
                    _bingoLeft.gameObject.SetActive(true);
                    _bingoLeft.Play();
                    break;
                case 2:
                    _bingoRight.gameObject.SetActive(true);
                    _bingoRight.Play();
                    break;
            }
        }
        
        public void PlayGameBackgroundParticle()
        {
            foreach (var particle in _backgroundGameParticleSystems)
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }
        }
        
        public void StopGameBackgroundParticle()
        {
            foreach (var particle in _backgroundGameParticleSystems)
            {
                particle.gameObject.SetActive(false);
            }
        }
        
        public void PlayFireworksParticle()
        {
            foreach (var particle in _fireworksFX)
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }
        }
        
        public void StopFireworksParticle()
        {
            foreach (var particle in _fireworksFX)
            {
                particle.gameObject.SetActive(false);
            }
        }
        
        public void StopMenuHideBackgroundParticle()
        {
            foreach (var particle in _backgroundMenuHideParticleSystems)
            {
                particle.gameObject.SetActive(false);
            }
        }

        public void PlayMenuBackgroundParticle()
        {
            foreach (var particle in _backgroundMenuParticleSystems)
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }
        }

        public void StopMenuBackgroundParticle()
        {
            foreach (var particle in _backgroundMenuParticleSystems)
            {
                particle.gameObject.SetActive(false);
            }
        }
        
        public void PlayTapParticle(Vector3 position)
        {
            _tapFX.transform.position = position;
            _tapFX.gameObject.SetActive(true);
            _tapFX.Play();
        }
        
        private void DisableParticles()
        {
            AddChildTransformsToBackgroundGameParticleSystems();
            AddChildTransformsToBackgroundMenuParticleSystems();
            _particleSystems.AddRange(_fireworksFX);
            _particleSystems.AddRange(_backgroundGameParticleSystems);
            _particleSystems.AddRange(_backgroundMenuParticleSystems);
            _particleSystems.Add(_tapFX);
            _particleSystems.Add(_bingoLeft);
            _particleSystems.Add(_bingoRight);
           
            foreach (var particle in _particleSystems)
            {
                particle.gameObject.SetActive(false);
            }
        }
        
        public void Init()
        {
            DisableParticles();
        }
    }
}