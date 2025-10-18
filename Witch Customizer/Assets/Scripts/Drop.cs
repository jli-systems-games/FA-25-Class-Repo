using UnityEngine;
using UnityEngine.UI;

public class Drop : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickCollect);
    }

    void OnClickCollect()
    {
        FindObjectOfType<WaterMiniGame>().CollectDrop(gameObject);
    }
}
