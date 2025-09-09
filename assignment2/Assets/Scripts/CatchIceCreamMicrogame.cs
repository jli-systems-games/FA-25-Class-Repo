using UnityEngine;

public class CatchIceCreamMicrogame : MicrogameBase
{
    [Header("References")]
    public Transform cone;
    public GameObject iceCreamBallPrefab;
    public float moveSpeed = 12f;
    public float spawnY = 5f;
    public float leftX = -7f, rightX = 7f;

    GameObject ball;

    protected override void OnBegin()
    {
        // spawn ball
        Vector3 spawn = new Vector3(Random.Range(leftX * 0.7f, rightX * 0.7f), spawnY, 0);
        ball = Instantiate(iceCreamBallPrefab, spawn, Quaternion.identity, transform);
        var rb = ball.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.down * Random.Range(3.5f, 5.5f);

        // ensure cone has catcher component wired up
        var catcher = cone.GetComponent<ConeCatcher>();
        if (catcher == null) catcher = cone.gameObject.AddComponent<ConeCatcher>();
        catcher.parent = this;
    }

    public void NotifyCaught()
    {
        if (running) Finish(true);
    }

    void Update()
    {
        if (!running) return;

        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) > 0.01f)
        {
            cone.transform.position += Vector3.right * h * moveSpeed * Time.deltaTime;
        }
        else
        {
            var world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cone.position = new Vector3(
                Mathf.Lerp(cone.position.x, world.x, 12f * Time.deltaTime),
                cone.position.y, 0);
        }

        float clampedX = Mathf.Clamp(cone.position.x, leftX, rightX);
        cone.position = new Vector3(clampedX, cone.position.y, 0);
    }

    protected override bool CheckAutoComplete()
    {
        if (ball && ball.transform.position.y < -6f)
        {
            Finish(false);
            return true;
        }
        return false;
    }
}
