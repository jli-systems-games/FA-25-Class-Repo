using UnityEngine;

public class Player : NPCController
{
    private Vector3 startPos;
    public bool metGhost = false;

    private void Update()
    {
        float distanceMoved = startPos.x - transform.position.x;

        if (metGhost && distanceMoved < 8)
        {
            Vector3 pos = transform.position;
            pos.x -= 0.2f;
            transform.position = pos;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        metGhost = true;
        Debug.Log("AHHHHHHHHH!");
        startPos = transform.position;
    }
}
