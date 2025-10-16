using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NameEntryUI : MonoBehaviour
{
    public TMP_InputField p1Input;
    public TMP_InputField p2Input;
    public Button continueBtn;

    void Start()
    {
        if (p1Input) p1Input.text = CentralData.I.p1Name;
        if (p2Input) p2Input.text = CentralData.I.p2Name;

        if (continueBtn) continueBtn.onClick.AddListener(OnContinue);
    }

    void OnContinue()
    {
        string n1 = p1Input ? p1Input.text : null;
        string n2 = p2Input ? p2Input.text : null;
        CentralData.I.SetNames(n1, n2);

        SceneManager.LoadScene("Choose");
    }
}