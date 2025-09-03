using UnityEngine;

public class Ball : MonoBehaviour
{
    public float speed = 200.0f;

    private Rigidbody2D _rigidbody;
    private AudioSource _audio; 

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ResetPosition();
        AddStartingForce();
    }

    public void ResetPosition()
    {
        _rigidbody.position = Vector3.zero;
        _rigidbody.linearVelocity = Vector3.zero;
    }

    public void AddStartingForce()
    {
        float x = Random.value < 0.5f ? -1.0f : 1.0f;
        float y = Random.value < 0.5f ? Random.Range(-1.0f, -0.5f) :
                                       Random.Range(0.5f, 1.0f);

        Vector2 direction = new Vector2(x, y);
        _rigidbody.AddForce(direction * this.speed);
    }

    public void AddForce(Vector2 force)
    {
        _rigidbody.AddForce(force);
    }

 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_audio && _audio.clip != null)
        {
            _audio.Play();
        }
    }
}
