using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField]
        GameObject player = null;

    const string PLAYER_TAG = "Player";

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
    }

    void FixedUpdate()
    {
        transform.position = new Vector3() { x = player.transform.position.x, y = transform.position.y, z = transform.position.z };
    }
}
