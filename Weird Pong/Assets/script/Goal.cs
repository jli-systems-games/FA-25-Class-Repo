using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool isPlayer1Goal;
    public AudioSource scoresound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball")|| collision.gameObject.CompareTag("Pika"))
        {
            if (isPlayer1Goal)
            {
                Debug.Log("Player2 scored");
                GameObject.Find("GameM").GetComponent<GameManager>().Player2Scored();
            }
            else
            {
                Debug.Log("Player1 scored");
                GameObject.Find("GameM").GetComponent<GameManager>().Player1Scored();
            }
            if (scoresound != null)
                scoresound.Play();
        }
        
    }
}
