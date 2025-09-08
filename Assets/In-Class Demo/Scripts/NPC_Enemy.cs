using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC_Enemy : NPCController
{
    public bool metPlayer = false;
    public override void Attack()
    {
        Scream();
    }

    private void Update()
    {
        if (!metPlayer)
        {
            Vector3 pos = transform.position;
            pos.x -= 0.05f;
            transform.position = pos;
        }
    }

    public void Scream()
    {
        Debug.Log("I'm coming~");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            metPlayer = true;
            Debug.Log("Boo!");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(WaitBeforeGo(3f));
        }
    }
    private IEnumerator WaitBeforeGo(float delay)
    {
        yield return new WaitForSeconds(delay);

        metPlayer = false;
    }
}
