using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager2D : MonoBehaviour
{
    [Header("Scene References")]
    public Transform p1Spawn;
    public Transform p2Spawn;
    public GameObject[] beybladePrefabs;

    [Header("UI")]
    public Slider p1StaminaSlider;
    public Slider p2StaminaSlider;
    public GameObject winnerPanel;
    public TMP_Text winnerText;

    [Header("Battle Settings")]
    public float launchForce = 15f;
    public float spinTorque = 2000f;
    public float stopThreshold = 0.05f;
    public float minBattleTime = 1.0f;

    private GameObject p1Blade, p2Blade;
    private BeybladeStats2D p1Stats, p2Stats;
    private bool battleEnded = false;
    private float startTime;

    void Start()
    {
        int p1Index = GameData.Instance.p1Index;
        int p2Index = GameData.Instance.p2Index;

        // Spawn
        p1Blade = Instantiate(beybladePrefabs[p1Index], p1Spawn.position, Quaternion.identity);
        p2Blade = Instantiate(beybladePrefabs[p2Index], p2Spawn.position, Quaternion.identity);

        // Get Stats
        p1Stats = p1Blade.GetComponent<BeybladeStats2D>();
        p2Stats = p2Blade.GetComponent<BeybladeStats2D>();

        // ? PRESET STATS ?
        ApplyPresetStats(p1Stats, p1Index);
        ApplyPresetStats(p2Stats, p2Index);

        // UI Slider setup
        p1StaminaSlider.maxValue = p1Stats.maxStamina;
        p2StaminaSlider.maxValue = p2Stats.maxStamina;
        p1StaminaSlider.value = p1Stats.stamina;
        p2StaminaSlider.value = p2Stats.stamina;

        winnerPanel.SetActive(false);

        // Launch toward center
        Vector2 center = Vector2.zero;
        LaunchBeyblade(p1Blade, (center - (Vector2)p1Spawn.position).normalized, 1);
        LaunchBeyblade(p2Blade, (center - (Vector2)p2Spawn.position).normalized, -1);

        startTime = Time.time;
    }

    void Update()
    {
        if (battleEnded) return;

        // CONSTANT stamina drain
        p1Stats.stamina = Mathf.Max(0, p1Stats.stamina - Time.deltaTime * 2.5f);
        p2Stats.stamina = Mathf.Max(0, p2Stats.stamina - Time.deltaTime * 2.5f);

        p1StaminaSlider.value = p1Stats.stamina;
        p2StaminaSlider.value = p2Stats.stamina;

        // Don't declare winner too early
        if (Time.time - startTime < minBattleTime)
            return;

        // ? WINNER CHECK (now works because IsDead works) ?
        if (p1Stats.IsDead(stopThreshold) && !p2Stats.IsDead(stopThreshold))
            DeclareWinner("PLAYER 2 WINS!");

        else if (!p1Stats.IsDead(stopThreshold) && p2Stats.IsDead(stopThreshold))
            DeclareWinner("PLAYER 1 WINS!");

        else if (p1Stats.IsDead(stopThreshold) && p2Stats.IsDead(stopThreshold))
            DeclareWinner("DRAW!");
    }

    void LaunchBeyblade(GameObject blade, Vector2 direction, int spinDir)
    {
        Rigidbody2D rb = blade.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * launchForce, ForceMode2D.Impulse);
        rb.AddTorque(spinDir * spinTorque, ForceMode2D.Impulse);
    }

    void ApplyPresetStats(BeybladeStats2D stats, int index)
    {
        switch (index)
        {
            case 0: stats.SetupStats(30, 10, 3, 3); break; // Attack
            case 1: stats.SetupStats(10, 30, 5, 5); break; // Defense
            case 2: stats.SetupStats(20, 20, 4, 4); break; // Balance
            case 3: stats.SetupStats(8, 12, 8, 3); break;  // Stamina
        }
    }

    void DeclareWinner(string winner)
    {
        battleEnded = true;
        winnerPanel.SetActive(true);
        winnerText.text = winner;
    }
}
