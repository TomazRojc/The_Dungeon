using System.Collections.Generic;
using UnityEngine;

namespace Code
{
    public class LevelManager
    {
        private GameObject _worldObject;

        public void Init(List<GameObject> playerObjects)
        {
            _worldObject = new GameObject("3D");
            foreach (var player in playerObjects)
            {
                player.transform.SetParent(_worldObject.transform);
            }
        }
    }
}