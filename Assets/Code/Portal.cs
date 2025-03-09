using UnityEngine;

namespace Code
{
	public class Portal : MonoBehaviour
	{
		[SerializeField] private BoxCollider2D boxCollider;
		[SerializeField] private Portal portal2;
		[SerializeField] private Vector3 portalLocalDirection = Vector3.right;

		private Vector3 PortalGlobalDirection => (transform.rotation * portalLocalDirection).normalized;
		public BoxCollider2D BoxCollider => boxCollider;

		public void OnTriggerEnter2D(Collider2D coll)
		{
			var gamePlayer = GetPlayerFromCollider(coll);
			if (gamePlayer == null) return;

			if (!gamePlayer.CanTeleport) return;
			
			gamePlayer.transform.position = portal2.transform.position + PortalGlobalDirection;
			gamePlayer.TeleportPlayer(PortalGlobalDirection, portal2.PortalGlobalDirection, portal2);
		}

		private PlayerController GetPlayerFromCollider(Collider2D coll)
		{
			if (!coll.CompareTag("Player")) return null;

			return coll.gameObject.GetComponentInParent<PlayerController>();
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + PortalGlobalDirection * 3f);
		}
	}
}