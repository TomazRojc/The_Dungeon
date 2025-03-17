using Code.Utils;
using UnityEngine;

namespace Code {
    public class PlayerTrailController : MonoBehaviour {
        
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int AlbedoColor = Shader.PropertyToID("_Color");
		
        private readonly Color _blackZeroAlpha = new Color(0f, 0f, 0f, 0f);
        
        [Header("References")]
        [SerializeField]
		private TrailRenderer trailRenderer;
        [SerializeField]
		private Rigidbody2D rigidbody;

        [Header("Params")]
        [SerializeField]
        private float transitionDuration;
        [SerializeField]
        private float minVelocity;
        
        private SimpleTimer _timer;
        private bool _isActive;
        private int _teleportFrameCounter = int.MinValue;
        private Color _currentColor;

        public void Init(Color color) {
            trailRenderer.material.SetColor(EmissionColor, color);
            trailRenderer.material.SetColor(AlbedoColor, _blackZeroAlpha);
            _currentColor = _blackZeroAlpha;
        }

        public void OnUpdate(float deltaTime) {
            _timer.Update(deltaTime);
            
            var velocity = rigidbody.velocity.magnitude;
            if (velocity >= minVelocity && !_isActive) {
                ShowTrail();
            } else if (velocity < minVelocity && _isActive) {
                HideTrail();
            }

            UpdateTeleportFrameCounter();
        }

        public void TeleportPlayer() {
            _teleportFrameCounter = 2;
            trailRenderer.emitting = false;
        }

        private void UpdateTeleportFrameCounter() {
            if (_teleportFrameCounter == 0) {
                trailRenderer.emitting = true;
            }

            if (_teleportFrameCounter >= 0) {
                _teleportFrameCounter--;
            }
        }

        private void ShowTrail() {
            _isActive = true;
            var startColor = _currentColor;
            _timer = new SimpleTimer();
            _timer.OnUpdate += normalizedTime => {
                _currentColor = Color.Lerp(startColor, Color.black, normalizedTime);
                trailRenderer.material.SetColor(AlbedoColor, _currentColor);
            };
            _timer.Start(transitionDuration);
        }

        private void HideTrail() {
            _isActive = false;
            var startColor = _currentColor;
            _timer = new SimpleTimer();
            _timer.OnUpdate += normalizedTime => {
                _currentColor = Color.Lerp(startColor, _blackZeroAlpha, normalizedTime);
                trailRenderer.material.SetColor(AlbedoColor, _currentColor);
            };
            _timer.Start(transitionDuration);
        }
    }
}