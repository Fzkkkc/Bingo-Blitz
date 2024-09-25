using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class FXController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _tapFX;
        [SerializeField] private ParticleSystem _potionChanFX;
        [SerializeField] private ParticleSystem _potionDoneWaveFX;
        [SerializeField] private ParticleSystem _coinShowerFX;
        [SerializeField] private ParticleSystem _coinBlastFx;
        [SerializeField] private ParticleSystem _boilingFX;

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
        
        public void PlayGameBackgroundParticle()
        {
            /*foreach (var particle in _backgroundGameParticleSystems)
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }*/
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
            /*foreach (var particle in _fireworksFX)
            {
                particle.gameObject.SetActive(false);
            }*/
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
            /*foreach (var particle in _backgroundMenuParticleSystems)
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }*/
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
        
        public void PlayChanPotionFX(Color startColor)
        {
            var mainModule = _potionChanFX.main;
            mainModule.startColor = startColor;
            _potionChanFX.gameObject.SetActive(true);
            _potionChanFX.Play();
        }
        
        public void PlayDonePotionFX()
        {
            _potionDoneWaveFX.gameObject.SetActive(true);
            _potionDoneWaveFX.Play();
        }

        public void PlayCoinsFX()
        {
            _coinBlastFx.gameObject.SetActive(true);
            _coinBlastFx.Play();
            _coinShowerFX.gameObject.SetActive(true);
            _coinShowerFX.Play();
        }

        public void StateBoilingFX(bool isActive)
        {
            _boilingFX.gameObject.SetActive(isActive);
            _boilingFX.Play();
        }

        public void ChangeBoilingFXColor(Color startColor)
        {
            var mainModule = _boilingFX.main;
            mainModule.startColor = startColor;
        }
        
        private void DisableParticles()
        {
            AddChildTransformsToBackgroundGameParticleSystems();
            AddChildTransformsToBackgroundMenuParticleSystems();
            _particleSystems.AddRange(_fireworksFX);
            _particleSystems.AddRange(_backgroundGameParticleSystems);
            _particleSystems.AddRange(_backgroundMenuParticleSystems);
            _particleSystems.Add(_tapFX);
            _particleSystems.Add(_potionChanFX);
            _particleSystems.Add(_potionDoneWaveFX);
            _particleSystems.Add(_coinBlastFx);
            _particleSystems.Add(_coinShowerFX);
            _particleSystems.Add(_boilingFX);
            foreach (var particle in _particleSystems)
            {
                particle.gameObject.SetActive(false);
            }
        }
        
        public void Init()
        {
            //DisableParticles();
        }
    }
}