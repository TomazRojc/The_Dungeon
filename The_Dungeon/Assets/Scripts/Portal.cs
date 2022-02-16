using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform portal2;

    public void OnTriggerEnter2D(Collider2D coll) {
        if (coll.CompareTag("Player")) {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players) {
                if (coll.gameObject.GetInstanceID() == player.GetInstanceID())
                GameObject.Find("GamePlayer(Clone)").transform.position = portal2.transform.position;
            }
        }
    }
}