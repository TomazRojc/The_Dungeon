using UnityEngine;

public class Portal : MonoBehaviour
{
	[SerializeField]
	private Portal portal2;

	private Vector3 _portalLocalDirection = Vector2.left;

	private Vector3 PortalGlobalDirection => (transform.rotation * _portalLocalDirection).normalized;

	public void OnTriggerEnter2D(Collider2D coll) {
		var gamePlayer = GetPlayerFromCollider(coll);
		if (gamePlayer == null) return;

		if (gamePlayer.wasJustTeleported)
		{
			return;
		}
			
		gamePlayer.transform.position = portal2.transform.position + PortalGlobalDirection * 0.1f;
		gamePlayer.SetDoubleJumped(false);
		gamePlayer.PortalChangeVelocityDirection(transform.rotation, portal2.transform.rotation);
		gamePlayer.wasJustTeleported = true;
	}
	
	public void OnTriggerExit2D(Collider2D coll)
	{
		var gamePlayer = GetPlayerFromCollider(coll);
		if (gamePlayer == null) return;
		
		gamePlayer.wasJustTeleported = false;
	}

	private GamePlayer GetPlayerFromCollider(Collider2D coll)
	{
		if (!coll.CompareTag("Player")) return null;
		
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players) {
			var gamePlayer = player.GetComponent<GamePlayer>();
			if (coll.gameObject.GetInstanceID() == player.GetInstanceID() && gamePlayer.hasAuthority)
			{
				return gamePlayer;
			}
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + PortalGlobalDirection * 2f);
	}
}