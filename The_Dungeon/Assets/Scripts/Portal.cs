using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform portal2;

    public void OnTriggerEnter2D(Collider2D coll) {
        if (coll.CompareTag("Player")) {
            
            GameObject.Find("TestPlayer(Clone)").transform.position = portal2.transform.position;
        }
    }
}
