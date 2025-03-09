using System.Collections.Generic;
using Code.Gameplay;
using UnityEngine;

namespace Code
{
	public class CameraFollow : MonoBehaviour
	{
		[SerializeField]
		private Camera _camera;
		[SerializeField]
		private float smoothFactor;
		
		private float _leftBound;
		private float _rightBound;
		private float _upBound;
		private float _downBound;

		private List<Transform> _players = new List<Transform>();
		private float _cameraZPos;

		public void Init(List<PlayerController> players, LevelBounds bounds) {
			_leftBound = bounds._left.position.x;
			_rightBound = bounds._right.position.x;
			_upBound = bounds._up.position.y;
			_downBound = bounds._down.position.y;
			
			foreach (var player in players) {
				_players.Add(player.transform);
			}

			_cameraZPos = transform.position.z;
			transform.position = GetTargetPosition();
		}

		void Update() {
			var target = GetTargetPosition();
			Vector3 smoothPosition = Vector3.Lerp(transform.position, target, smoothFactor * Time.deltaTime);
			transform.position = smoothPosition;
		}

		private Vector3 GetTargetPosition() {
			var average = Vector3.zero;
			foreach (var player in _players) {
				average += player.position;
			}
			average /= _players.Count;
			
			return ClampBounds(average);
		}

		private Vector3 ClampBounds(Vector3 playerAverage) {
			var aspectRatio = (float)Screen.width / Screen.height;
			var vertExtent = _camera.orthographicSize;
			var horizExtent = vertExtent * aspectRatio;
			
			var maxV = playerAverage.y + vertExtent;
			if (maxV > _upBound) {
				playerAverage.y = _upBound - vertExtent;
			}
			var maxH = playerAverage.x + horizExtent;
			if (maxH > _rightBound) {
				playerAverage.x = _rightBound - horizExtent;
			}
			var minV = playerAverage.y - vertExtent;
			if (minV < _downBound) {
				playerAverage.y = _downBound + vertExtent;
			}
			var minH = playerAverage.x - horizExtent;
			if (minH < _leftBound) {
				playerAverage.x = _leftBound + horizExtent;
			}
			
			playerAverage.z = _cameraZPos;
			return playerAverage;
		}
	}
}