using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public bool CanSafeJump;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!CanSafeJump)
                other.gameObject.GetComponent<PlayerController>().ResetGame();
        }
    }
}
