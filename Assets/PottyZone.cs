using DG.Tweening.Core.Easing;
using UnityEngine;

public class PottyZone : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Drop"))
        {
            gameManager.AddScore();
            Destroy(other.gameObject,0.2f);
        }
    }
}
