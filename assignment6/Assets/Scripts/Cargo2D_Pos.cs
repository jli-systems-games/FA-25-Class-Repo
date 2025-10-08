using UnityEngine;

public class Cargo2D_Pos : MonoBehaviour
{
    public int score = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var train = other.GetComponentInParent<Train2DController_Pos>();
        if (!train) return;

        GameManager2D.I?.AddScore(score);
        gameObject.SetActive(false);
    }
}
