using Code.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay {
    
    public class MovingPlatform : MonoBehaviour {

        [SerializeField]
        private Transform _firstound;
        [SerializeField] 
        private Transform _secondBound;
        [SerializeField] 
        private float _movementDuration;

        private SimpleTimer _timer;
        private Vector3 _targetPosition;
        private Vector3 _oppositePosition;
        
        private void Awake() {
            var random = Random.Range(0, 2);
            _targetPosition = random > 0.5 ? _firstound.position : _secondBound.position;
            _oppositePosition = random > 0.5 ? _secondBound.position : _firstound.position;
            _timer.OnUpdate += normalizedTime => {
                transform.position = Vector3.Lerp(_oppositePosition, _targetPosition, normalizedTime);
            };
            _timer.OnLoopComplete += () => {
                (_targetPosition, _oppositePosition) = (_oppositePosition, _targetPosition);
            };
            _timer.Start(_movementDuration, true);
        }
        
        private void Update() {
            _timer.Update(Time.deltaTime);
        }

        private void CenterPlatform() {
            var pos1 = _firstound.position;
            var pos2 = _secondBound.position;
            transform.position = (pos1 + pos2) / 2f;
            _firstound.position = pos1;
            _secondBound.position = pos2;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            if (Application.isPlaying) {
                Gizmos.DrawLine(_targetPosition, _oppositePosition);
            } else {
                Gizmos.DrawLine(_firstound.position, _secondBound.position);
                CenterPlatform();
            }
        }
    }
    
}