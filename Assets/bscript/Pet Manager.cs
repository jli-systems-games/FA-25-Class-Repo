using UnityEngine;

public class PetManager : MonoBehaviour
{
    public PetData petData;
    public PetState currentState;

    [Header("Pet State GameObjects")]
    public GameObject happyFace;
    public GameObject hungryFace;
    public GameObject dirtyFace;
    public GameObject sadFace;

    public AudioSource petsound;
    public AudioSource foodsound;
    public AudioSource cleansound;
    private void Start()
    {
        currentState = PetState.Happy;

        
        petData.hunger = 100f;
        petData.cleanliness = 100f;
        petData.happiness = 100f;

        UpdateStateVisuals();
        currentState = PetState.Happy;
        UpdateStateVisuals();
    }


    private void Update()
    {
       
        petData.hunger -= Time.deltaTime * 2f;
        petData.cleanliness -= Time.deltaTime * 1.5f;
        petData.happiness -= Time.deltaTime * 1f;

        UpdateState();
    }

    void UpdateState()
    {
        PetState previousState = currentState;

        if (petData.hunger < 50)
            currentState = PetState.Hungry;
        else if (petData.cleanliness < 40)
            currentState = PetState.Dirty;
        else if (petData.happiness < 60)
            currentState = PetState.Sad;
        else
            currentState = PetState.Happy;

        if (previousState != currentState)
            UpdateStateVisuals();
    }

    void UpdateStateVisuals()
    {
       
        happyFace.SetActive(false);
        hungryFace.SetActive(false);
        dirtyFace.SetActive(false);
        sadFace.SetActive(false);

      
        switch (currentState)
        {
            case PetState.Hungry:
                hungryFace.SetActive(true);
                break;
            case PetState.Dirty:
                dirtyFace.SetActive(true);
                break;
            case PetState.Sad:
                sadFace.SetActive(true);
                break;
            default:
                happyFace.SetActive(true);
                break;
        }
    }

    public void FeedPet()
    {
        foodsound.Play();
        petData.hunger = Mathf.Min(petData.hunger + 30, 100);
        UpdateState();
    }

    public void CleanPet()
    {
        cleansound.Play();
        petData.cleanliness = Mathf.Min(petData.cleanliness + 30, 100);
        UpdateState();
    }

    public void PetThePet()
    {
        petsound.Play();
        petData.happiness = Mathf.Min(petData.happiness + 30, 100);
        UpdateState();
    }
}
