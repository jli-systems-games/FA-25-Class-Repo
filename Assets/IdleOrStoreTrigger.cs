using UnityEngine;
using UnityEngine.SceneManagement;

public class IdleOrStoreTrigger : MonoBehaviour
{
    public GameObject[] storePanels;

    public GameManager hintButton;

    public float idleThreshold = 10f;
    public float storeOpenThreshold = 12f;

    public GameObject triggerPrefab;
    private GameObject spawnedTrigger;
    private Animator anim;

    public string animationStateName = "motheZoomAnim";
    public string nextScene = "Lobby";

    private float idleTimer = 0f;
    private float storeTimer = 0f;

    private bool triggered = false;
    private bool wasStoreOpen = false;

    void Update()
    {
        if (PlayerDidSomething())
        {
            idleTimer = 0f;

            if (triggered)
            {
                CancelTrigger();
            }
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        if (IsAnyStoreOpen())
            storeTimer += Time.deltaTime;
        else
            storeTimer = 0f;


        if (!triggered && (idleTimer >= idleThreshold || storeTimer >= storeOpenThreshold))
        {
            TriggerSequence();
        }

        if (triggered && spawnedTrigger != null)
        {
            if (anim != null &&
                anim.GetCurrentAnimatorStateInfo(0).IsName(animationStateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
    bool IsAnyStoreOpen()
    {
        if (storePanels == null || storePanels.Length == 0)
            return false;

        foreach (var panel in storePanels)
        {
            if (panel != null && panel.activeInHierarchy)
                return true;
        }
        return false;
    }


    bool PlayerDidSomething()
    {
        bool storeOpen = IsAnyStoreOpen();

        if (wasStoreOpen && !storeOpen)
        {
            wasStoreOpen = storeOpen;
            return true;
        }

        wasStoreOpen = storeOpen;

        if (storeOpen)
            return false;

        return Input.anyKeyDown ||
               Input.GetMouseButtonDown(0) ||
               Input.GetMouseButtonDown(1) ||
               Input.GetMouseButton(0) ||
               Input.GetMouseButton(1);
    }


    void TriggerSequence()
    {
        triggered = true;

        if (triggerPrefab != null)
        {
            hintButton.gameObject.SetActive(true);
            spawnedTrigger = Instantiate(triggerPrefab);
            anim = spawnedTrigger.GetComponent<Animator>();
        }
    }

    void CancelTrigger()
    {
        triggered = false;

        if (spawnedTrigger != null)
            Destroy(spawnedTrigger);

        spawnedTrigger = null;
        anim = null;
    }
}
