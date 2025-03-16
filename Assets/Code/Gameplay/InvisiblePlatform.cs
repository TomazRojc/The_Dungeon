using UnityEngine;
using UnityEngine.Assertions;

namespace Code.Gameplay
{
    public class InvisiblePlatform : MonoBehaviour
    {
        private MagicTorchComponent[] _magicTorches;
        private Material _material;
        private int _numTorches;
        
        void Start()
        {
            _magicTorches = Main.LevelManager.WorldGameObject.GetComponentsInChildren<MagicTorchComponent>();
            _numTorches = _magicTorches.Length;
            _material = GetComponentsInChildren<Renderer>()[0].material;
            _material.SetFloat("_TorchCount", _numTorches);
            
            Assert.IsTrue(_magicTorches.Length <= 10, "Invisible platform can only have 10 magic torches");
        }

        private void Update()
        {
            Vector4[] torchPositions = new Vector4[_numTorches];

            for (int i = 0; i < _numTorches; i++)
            {
                torchPositions[i] = new Vector4(_magicTorches[i].transform.position.x, _magicTorches[i].transform.position.y, 0, 0);
            }

            _material.SetVectorArray("_TorchPositions", torchPositions);
        }
    }
}
