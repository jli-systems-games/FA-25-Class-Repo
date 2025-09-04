using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool isPlayer1Goal;
    public AudioSource scoresound;
    public AudioSource pikaScore;
    public AudioSource narutoScore;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball")|| collision.gameObject.CompareTag("Pika") || collision.gameObject.CompareTag("Naruto"))
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
          
        }
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (scoresound != null)
                scoresound.Play();
            Debug.Log("playsound");
        }
        if (collision.gameObject.CompareTag("Pika"))
         { 
            if (pikaScore != null)
                pikaScore.Play();
            Debug.Log("playsound");
        }
        if (collision.gameObject.CompareTag("Naruto"))
        {
            if (narutoScore != null)
                narutoScore.Play();
            Debug.Log("playsound");
        }
        
    }
}
