using UnityEngine;

namespace Code
{
	public class Portal : MonoBehaviour
	{
		[SerializeField] private Portal portal2;
		[SerializeField] private Vector3 portalLocalDirection = Vector3.right;

		private Vector3 PortalGlobalDirection => (transform.rotation * portalLocalDirection).normalized;

		private bool playerJustTeleported { get; set; }

		public void OnTriggerEnter2D(Collider2D coll)
		{
			var gamePlayer = GetPlayerFromCollider(coll);
			if (gamePlayer == null) return;

			if (playerJustTeleported)
			{
				return;
			}

			gamePlayer.transform.position = portal2.transform.position + PortalGlobalDirection;
			gamePlayer.SetDoubleJumped(false);
			gamePlayer.PortalChangeVelocityDirection(PortalGlobalDirection, portal2.PortalGlobalDirection);
			portal2.playerJustTeleported = true;
		}

		public void OnTriggerExit2D(Collider2D coll)
		{
			var gamePlayer = GetPlayerFromCollider(coll);
			if (gamePlayer == null) return;

			playerJustTeleported = false;
		}

		private PlayerController GetPlayerFromCollider(Collider2D coll)
		{
			if (!coll.CompareTag("Player")) return null;

			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject player in players)
			{
				var gamePlayer = player.GetComponent<PlayerController>();
				if (coll.gameObject.GetInstanceID() == player.GetInstanceID())
				{
					return gamePlayer;
				}
			}

			return null;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + PortalGlobalDirection * 3f);
		}
	}
}