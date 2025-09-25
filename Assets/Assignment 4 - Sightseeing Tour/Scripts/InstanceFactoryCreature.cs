using UnityEngine;

public class InstanceFactoryCreature : MonoBehaviour
{
    public float speed = 0.1f;
    public float moveTime = 32f/24f;
    public float pauseTime = 23f/24f;
    public float distancePerCycle = 10f;

    private Animator wiggleAnimator;

    private bool isMoving = true;
    private bool readyToSpawn = false;
    private float timer = 0f;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Transform parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wiggleAnimator = transform.GetChild(0).GetComponent<Animator>();
        parent = transform.parent;

        SetNewTarget();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (isMoving)
        {
            float t = timer / moveTime;
            parent.localPosition = Vector3.Lerp(startPos, targetPos, t);

            if (timer >= moveTime)
            {
                isMoving = false;
                timer = 0;
                parent.localPosition = targetPos;
            }
        }
        else
        {
            if (timer >= pauseTime)
            {
                readyToSpawn = true;
            }
        }

        if (parent.localPosition.x <= 0 && readyToSpawn)
        {
            transform.parent.localPosition = new Vector3(70, 0, 0);
            wiggleAnimator.Play("Factory", -1, 0f);

            isMoving = true;
            readyToSpawn = false;
            timer = 0;
            SetNewTarget();
        }
        else if (readyToSpawn)
        {
            isMoving = true;
            readyToSpawn = false;
            timer = 0;
            SetNewTarget();
        }
    }

    void SetNewTarget()
    {
        startPos = parent.localPosition;
        targetPos = startPos + Vector3.left * distancePerCycle;
    }
}
