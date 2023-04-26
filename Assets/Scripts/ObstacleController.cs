using UnityEngine;

public class ObstacleController : MonoBehaviour {
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.Instance.GameOver();
        }
    }
}
