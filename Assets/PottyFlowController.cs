using UnityEngine;
using System.Collections;

public class PottyFlowController : MonoBehaviour
{
    public GameManager gameManager;
    public PottyZone[] pottyZones;
    public int mainPottyIndex = 1;

    public PottyShuffleManager shuffleManager;

    public Transform centerPoint;

    private Vector3[] originalPositions;
    private Quaternion[] originalRotations;

    private int currentPhase = 0;
    public int currentLevel = 1;
    private int nextThreshold = 500;
    private int maxScoreSeen = 0;

    private float floatSpeed;
    private float floatAmplitude = 5f;

    private float spinOrbitRadius;
    private float spinOrbitSpeed;
    private float spinSelfSpeed;
    private float spinPulseAmplitude;
    private float spinBaseScale;
    private Vector3 spinCenter;
    private float spinAngle;

    private Vector3 chaosCenter;
    private float chaosAngle;
    private float chaosSpeed;
    private float chaosRadius;

    private bool tripleShuffleStarted = false;
    private bool tripleReadyForMove = false;
    private Vector3[] tripleBasePositions;
    private float tripleMoveSpeed;
    private float tripleMoveAmplitude = 3f;

    [HideInInspector] public bool firstShotThisLevel = true;
    [HideInInspector] public int chaosHitCount = 0;
    [HideInInspector] public float levelStartTime = 0f;
    [HideInInspector] public bool tripleSolved = false;


    private void Awake()
    {
        originalPositions = new Vector3[pottyZones.Length];
        originalRotations = new Quaternion[pottyZones.Length];

        for (int i = 0; i < pottyZones.Length; i++)
        {
            originalPositions[i] = pottyZones[i].transform.position;
            originalRotations[i] = pottyZones[i].transform.rotation;
        }
    }

    private void Start()
    {
        SetupSinglePotty();
        currentPhase = 0;
        currentLevel = 1;
        nextThreshold = 50;
        maxScoreSeen = 0;

        InitLevel(currentLevel);
        gameManager.AllowShooting();
    }

    private void Update()
    {
        for (int i = 0; i < pottyZones.Length; i++)
        {
            if (currentLevel != 5)
                pottyZones[i].transform.localScale = new Vector3(2.49f, 2.49f, 1f);
        }

        int currentScore = gameManager.CurrentScore;
        if (currentScore > maxScoreSeen)
        {
            maxScoreSeen = currentScore;

            while (maxScoreSeen >= nextThreshold)
            {
                AdvancePhase();
            }
        }

        RunLevelBehaviour();
    }

    void AdvancePhase()
    {
        currentPhase++;
        nextThreshold += 50;

        int newLevel = ChooseLevelForPhase(currentPhase);

        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            InitLevel(currentLevel);
        }
    }

    int ChooseLevelForPhase(int phase)
    {
        if (phase <= 0) return 1;
        if (phase == 1) return RandomChoice(2, 3, 4);
        if (phase == 2) return 1;
        if (phase == 3) return RandomChoice(2, 3, 4, 5);

        return RandomChoice(1, 2, 3, 4, 5);
    }

    int RandomChoice(params int[] options)
    {
        return options[Random.Range(0, options.Length)];
    }

    void InitLevel(int level)
    {
        firstShotThisLevel = true;
        chaosHitCount = 0;
        levelStartTime = Time.time;
        tripleSolved = false;

        for (int i = 0; i < pottyZones.Length; i++)
        {
            pottyZones[i].transform.localScale = new Vector3(2.49f, 2.49f, 1f);
        }

        tripleShuffleStarted = false;
        tripleReadyForMove = false;

        SetupSinglePotty();

        switch (level)
        {
            case 1:
                gameManager.AllowShooting();
                break;

            case 2:
                floatSpeed = Random.Range(0.5f, 1.5f);
                gameManager.AllowShooting();
                break;

            case 3: 
                spinOrbitRadius = Random.Range(1.5f, 3.5f);

                spinOrbitSpeed = Random.Range(0.8f, 2.0f);

                spinSelfSpeed = Random.Range(70f, 150f);

                spinPulseAmplitude = Random.Range(0.08f, 0.15f);

                spinBaseScale = 2.49f;

                spinCenter = originalPositions[mainPottyIndex];

                gameManager.AllowShooting();
                break;


            case 4:
                StartTripleLevel();
                break;

            case 5:
                StartChaosLevel();
                break;
        }
    }

    void RunLevelBehaviour()
    {
        switch (currentLevel)
        {
            case 1:
                break;

            case 2:
                UpdateFloating();
                break;

            case 3:
                UpdateSpinning();
                break;

            case 4:
                UpdateTripleMove();
                break;

            case 5:
                UpdateChaos();
                break;
        }
    }
    void UpdateFloating()
    {
        Vector3 basePos = originalPositions[mainPottyIndex];
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        MainPotty().transform.position = new Vector3(
            basePos.x,
            basePos.y + offsetY,
            basePos.z
        );
    }

    void UpdateSpinning()
    {
        Transform t = MainPotty().transform;

        spinAngle += spinOrbitSpeed * Time.deltaTime;
        float x = Mathf.Cos(spinAngle) * spinOrbitRadius;
        float y = Mathf.Sin(spinAngle) * spinOrbitRadius;

        t.position = spinCenter + new Vector3(x, y, 0f);

        t.Rotate(0f, 0f, spinSelfSpeed * Time.deltaTime);

        float pulse = 1f + Mathf.Sin(Time.time * 3f) * spinPulseAmplitude;
        t.localScale = new Vector3(spinBaseScale, spinBaseScale, 1f) * pulse;
    }

    void StartTripleLevel()
    {
        if (shuffleManager == null)
        {
            currentLevel = 1;
            InitLevel(1);
            return;
        }

        for (int i = 0; i < pottyZones.Length; i++)
        {
            pottyZones[i].gameObject.SetActive(true);
        }

        tripleShuffleStarted = true;
        tripleReadyForMove = false;

        shuffleManager.StartShuffle();
        StartCoroutine(WaitShuffleThenPrepareTripleMove());
    }

    IEnumerator WaitShuffleThenPrepareTripleMove()
    {
        while (shuffleManager.IsShuffling())
            yield return null;

        gameManager.AllowShooting();

        tripleBasePositions = new Vector3[pottyZones.Length];
        for (int i = 0; i < pottyZones.Length; i++)
        {
            tripleBasePositions[i] = pottyZones[i].transform.position;
        }

        tripleMoveSpeed = Random.Range(0.5f, 1.5f);
        tripleReadyForMove = true;
        firstShotThisLevel = true;

    }

    void UpdateTripleMove()
    {
        if (!tripleReadyForMove)
            return;

        float offsetX = Mathf.Sin(Time.time * tripleMoveSpeed) * tripleMoveAmplitude;

        for (int i = 0; i < pottyZones.Length; i++)
        {
            Vector3 p = tripleBasePositions[i];
            p.x += offsetX;
            pottyZones[i].transform.position = p;
        }
    }

    void StartChaosLevel()
    {
        chaosHitCount = 0;

        for (int i = 0; i < pottyZones.Length; i++)
        {
            pottyZones[i].gameObject.SetActive(true);

            Vector3 pos = pottyZones[i].transform.position;
            pos.z = 0f;
            pottyZones[i].transform.position = pos;

            pottyZones[i].transform.localScale = new Vector3(2.49f * 0.7f, 2.49f * 0.7f, 0.7f);
            pottyZones[i].transform.rotation = Quaternion.identity;
            pottyZones[i].isReal = false;
        }

        pottyZones[Random.Range(0, pottyZones.Length)].isReal = true;

        chaosCenter = centerPoint != null
            ? centerPoint.position
            : originalPositions[mainPottyIndex];

        chaosCenter.z = 0f;

        chaosAngle = Random.Range(0f, 360f);
        chaosSpeed = Random.Range(60f, 80f);
        chaosRadius = Random.Range(2.5f, 3.5f);
    }

    void UpdateChaos()
    {
        chaosAngle += chaosSpeed * Time.deltaTime;

        for (int i = 0; i < pottyZones.Length; i++)
        {
            float rad = Mathf.Deg2Rad * (chaosAngle + i * 120f);

            Vector3 pos;
            pos.x = chaosCenter.x + Mathf.Cos(rad) * chaosRadius;
            pos.y = chaosCenter.y + Mathf.Sin(rad) * chaosRadius;
            pos.z = 0f;

            pottyZones[i].transform.position = pos;
        }
    }
    void SetupSinglePotty()
    {
        for (int i = 0; i < pottyZones.Length; i++)
        {
            bool isMain = (i == mainPottyIndex);

            pottyZones[i].gameObject.SetActive(isMain);
            pottyZones[i].isReal = isMain;

            Vector3 pos = originalPositions[i];
            pos.z = 0f;
            pottyZones[i].transform.position = pos;

            pottyZones[i].transform.rotation = originalRotations[i];
            pottyZones[i].transform.localScale = Vector3.one;
        }
    }


    PottyZone MainPotty()
    {
        return pottyZones[mainPottyIndex];
    }
}
