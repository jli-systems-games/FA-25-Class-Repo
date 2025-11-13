using TMPro;
using UnityEngine;

public class NPCUILogic : MonoBehaviour
{

    public TextMeshProUGUI swordText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


    }

    public void ShowNewSword()
    {
        Sword swordToShow = Data.swordDatabase[Random.Range(0, 9)];

        string displaytext = swordToShow.swordName + ": Quickness: " + swordToShow.quickness + ": stabAbilty " + swordToShow.stabAbilty;

        swordText.text = displaytext;

    }

    // Update is called once per frame
    void Update()
    {
        ShowNewSword();

    }
}
