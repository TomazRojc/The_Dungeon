﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay
{
    [Serializable]
    public struct LevelBounds {
        public Transform _left;
        public Transform _right;
        public Transform _up;
        public Transform _down;
    }
    
    public class LevelComponent : MonoBehaviour
    {
        [SerializeField] private SpawnPointsComponent spawnPointsComponent;
        [SerializeField] private LevelBounds _levelBounds;
        
        private CameraFollow _cameraFollow;

        public void StartLevel(List<PlayerController> players, CameraFollow cameraFollow) {
            SpawnPlayers(players);
            _cameraFollow = cameraFollow;
            _cameraFollow.Init(players, _levelBounds);
        }
        
        private void SpawnPlayers(List<PlayerController> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].transform.position = spawnPointsComponent.SpawnPoints[i].transform.position;
                players[i].OnSpawn();
            }
        }
    }
}