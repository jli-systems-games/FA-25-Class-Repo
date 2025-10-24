using UnityEngine;

public class ButtonGameModes : MonoBehaviour
{
    public GameObject homeParent;
    public GameObject othersParent;
    public GameObject hungerParent;
    public GameObject cleanParent;
    public GameObject statParent;

    void Start()
    {
        Data.isHungerMode = false;
        Data.isCleanMode = false;

        homeParent.SetActive(false);
        othersParent.SetActive(true);
        hungerParent.SetActive(false);
        cleanParent.SetActive(false);
        statParent.SetActive(true);
    }

    private void Update()
    {
        if (Data.isFinished)
        {
            statParent.SetActive(false);
            ResetValues();
        }
    }

    public void ResetValues()
    {
        Data.isHungerMode = false;
        Data.isCleanMode = false;

        homeParent.SetActive(false);
        othersParent.SetActive(false);
        hungerParent.SetActive(false);
        cleanParent.SetActive(false);
    }

    public void StartHungerMode()
    {
        ResetValues();

        homeParent.SetActive(true);

        hungerParent.SetActive(true);
        Data.isHungerMode = true;
    }

    public void StartCleanMode()
    {
        ResetValues();

        homeParent.SetActive(true);

        cleanParent.SetActive(true);
        Data.isCleanMode = true;
    }

    public void StartHomeMode()
    {
        ResetValues();

        othersParent.SetActive(true);
    }
}
