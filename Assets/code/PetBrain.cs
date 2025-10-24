using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public enum PetState
{
    idle,          
    readyToWork,   
    working        
}

public enum PetMood
{
    normal,
    hungry,
    thirsty
}

public class PetBrain : MonoBehaviour
{
    [Header("主状态")]
    public Sprite idleSprite;
    public Sprite readySprite;
    public Sprite workSprite;

    [Header("心情")]
    public Sprite normalFace;
    public Sprite hungryFace;
    public Sprite thirstyFace;

    [Header("UI")]
    public Image petImage;
    public TextMeshProUGUI statusText;

    [Header("数值喵")]
    public float hungerValue = 100f;
    public float thirstValue = 100f;
    public float hungerDropSpeed = 2f;
    public float thirstDropSpeed = 1f;

    private PetMood currentMood = PetMood.normal;
    private PetState currentState = PetState.idle;

    private bool isProtected = false;   
    private float protectionTimer = 0f;

    private bool isFrozen = false;  
    private float freezeTimer = 0f;

    private float idleTimer = 0f;
    private float workTimer = 0f;

    void Start()
    {
        currentState = PetState.idle;
        petImage.sprite = idleSprite;
        updateText();

        idleTimer = Random.Range(5f, 10f);
    }

    void Update()
    {
   
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;

            idleTimer = Mathf.Max(idleTimer, 0);
            workTimer = Mathf.Max(workTimer, 0);
            protectionTimer = Mathf.Max(protectionTimer, 0);

            if (freezeTimer <= 0)
            {
                isFrozen = false;
                Debug.Log("Freeze ended — pet can act again.");
            }

            updateText();
            return; 
        }


        switch (currentState)
        {
            case PetState.idle:
                IdleBehavior();
                break;

            case PetState.readyToWork:
                ReadyBehavior();
                break;

            case PetState.working:
                WorkBehavior();
                break;
        }

        updateText();

        if (hungerValue <= 0f || thirstValue <= 0f)
        {
            Debug.Log("Pet fainted — switching to GameOver scene.");
            SceneManager.LoadScene("GameOver"); 
        }
    }

    void IdleBehavior()
    {
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0f)
        {
            currentState = PetState.readyToWork;
            petImage.sprite = readySprite;
            Debug.Log("Pet is now ready to work!");
        }
    }

    void ReadyBehavior()
    {
//喵喵喵
    }

 
    void WorkBehavior()
    {
        if (isProtected)
        {
            protectionTimer -= Time.deltaTime;
            if (protectionTimer <= 0)
                isProtected = false;
        }

        if (!isProtected)
        {
            hungerValue -= hungerDropSpeed * Time.deltaTime;
            thirstValue -= thirstDropSpeed * Time.deltaTime;
        }

        hungerValue = Mathf.Clamp(hungerValue, 0, 100);
        thirstValue = Mathf.Clamp(thirstValue, 0, 100);

  
        if (hungerValue < 60 && hungerValue < thirstValue && currentMood != PetMood.hungry)
            changeMood(PetMood.hungry);
        else if (thirstValue < 60 && thirstValue <= hungerValue && currentMood != PetMood.thirsty)
            changeMood(PetMood.thirsty);
        else if (hungerValue >= 60 && thirstValue >= 60 && currentMood != PetMood.normal)
            changeMood(PetMood.normal);


        workTimer -= Time.deltaTime;
        if (workTimer <= 0f)
        {
            if (hungerValue >= 60 && thirstValue >= 60)
            {
                currentState = PetState.idle;
                petImage.sprite = idleSprite;
                idleTimer = Random.Range(5f, 10f);
                Debug.Log("Pet finished work and returned to idle.");
            }
            else
            {
                currentState = PetState.readyToWork; 
                petImage.sprite = readySprite;
                Debug.Log("Pet is too hungry or thirsty to rest! Feed it first.");
            }
        }
    }


    public void feed()
    {
        hungerValue += 40f;
        hungerValue = Mathf.Clamp(hungerValue, 0, 100);
        checkMood();
    }

    public void drink()
    {
        thirstValue += 20f;
        thirstValue = Mathf.Clamp(thirstValue, 0, 100);
        checkMood();
    }


    public void activateProtection()
    {
        isProtected = true;
        protectionTimer = 15f;
        Debug.Log("Pet is protected for 15 seconds!");
    }

    public void activateFreeze()
    {
        isFrozen = true;
        freezeTimer = 15f;
        Debug.Log("Pet is frozen — no state or value changes for 15 seconds!");
    }


    public void startWork()
    {
        if (isFrozen) return; 
        if (currentState == PetState.readyToWork)
        {
            currentState = PetState.working;
            petImage.sprite = workSprite;
            workTimer = Random.Range(15f, 25f);
            Debug.Log("Pet started working!");
        }
    }

    void updateText()
    {
        string message = "";

        if (isFrozen)
        {
            message = "Frozen! No changes for " + Mathf.CeilToInt(freezeTimer) + "s.";
            statusText.text = message;
            return;
        }

        switch (currentState)
        {
            case PetState.idle:
                message = "lil guy is happy";
                break;
            case PetState.readyToWork:
                message = "Work time!";
                break;
            case PetState.working:
                if (currentMood == PetMood.hungry)
                {
                    if (hungerValue < 20) message = "Saturation: Starving";
                    else if (hungerValue < 40) message = "Saturation: Very Hungry";
                    else if (hungerValue < 60) message = "Saturation: A Bit Hungry";
                    else message = "Saturation: Full";
                }
                else if (currentMood == PetMood.thirsty)
                {
                    if (thirstValue < 20) message = "Hydration: Dehydrated";
                    else if (thirstValue < 40) message = "Hydration: Very Thirsty";
                    else if (thirstValue < 60) message = "Hydration: A Bit Thirsty";
                    else message = "Hydration: Fine";
                }
                else
                {
                    message = "Working hard!";
                }
                break;
        }

        if (isProtected)
            message += "\n(Rest mode: safe for " + Mathf.CeilToInt(protectionTimer) + "s)";

        statusText.text = message;
    }


    void checkMood()
    {
        if (currentState != PetState.working) return;

        if (hungerValue >= 60 && thirstValue >= 60)
            changeMood(PetMood.normal);
        else if (hungerValue < thirstValue)
            changeMood(PetMood.hungry);
        else
            changeMood(PetMood.thirsty);
    }

    void changeMood(PetMood newMood)
    {
        currentMood = newMood;
        if (currentState != PetState.working) return;

        switch (currentMood)
        {
            case PetMood.normal:
                petImage.sprite = normalFace;
                break;
            case PetMood.hungry:
                petImage.sprite = hungryFace;
                break;
            case PetMood.thirsty:
                petImage.sprite = thirstyFace;
                break;
        }
    }
}
