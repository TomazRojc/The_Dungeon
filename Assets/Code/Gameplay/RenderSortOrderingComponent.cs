using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Gameplay
{
    public class RenderSortOrderingComponent : MonoBehaviour
    {

        [SerializeField]
        [HideInInspector]
        private List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();

        [SerializeField]
        [HideInInspector]
        private List<TrailRenderer> _trailRenderers = new List<TrailRenderer>();

        [SerializeField]
        [HideInInspector]
        private List<ParticleSystemRenderer> _particleSystemRenderers = new List<ParticleSystemRenderer>();

        public List<SpriteRenderer> SpriteRenderers => _spriteRenderers;
    
        public List<TrailRenderer> TrailRenderers => _trailRenderers;
    
        public List<ParticleSystemRenderer> ParticleSystemRenderers => _particleSystemRenderers;
    
        private void Reset()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
            _trailRenderers = GetComponentsInChildren<TrailRenderer>().ToList();
            _particleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>().ToList();
        }

        public void ChangeOrderInLayer(int amount)
        {
            _spriteRenderers.ForEach(x => x.sortingOrder += amount);
            _trailRenderers.ForEach(x => x.sortingOrder += amount);
            _particleSystemRenderers.ForEach(x => x.sortingOrder += amount);
        }

        public void ChangeOrderInLayer(Renderer givenRenderer, int amount)
        {
            if (givenRenderer == null)
            {
                ChangeOrderInLayer(amount);
                return;
            }
        
            givenRenderer.sortingOrder += amount;
        }
    
    }
}
