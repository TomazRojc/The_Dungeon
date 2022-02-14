using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    [Range(1,10)]
    public float smoothFactor;
    public Vector3 minValue, maxValue;

    void Update() {
        if(Input.GetKey(KeyCode.W)) {
            offset[1] = 10;
        } else if(Input.GetKey(KeyCode.S)) {
            offset[1] = -5;
        } else {
            offset[1] = 5;
        }
    }

    void FixedUpdate()
    {
        Vector3 playerPosition = player.position + offset;
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(playerPosition.x, minValue.x, maxValue.x),
            Mathf.Clamp(playerPosition.y, minValue.y, maxValue.y),
            Mathf.Clamp(playerPosition.z, minValue.z, maxValue.z)
        );

        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothFactor*Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }
}
