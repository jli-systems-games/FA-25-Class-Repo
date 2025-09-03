using UnityEngine;

public class CollisionSwap : MonoBehaviour
    //Ϊ���Է����Ժ󿴲���
    //��������������ǣ���Ϊ����Ҫÿ����ֻ�ܹ��������С���
    //���Ծ��ǣ�������߼����ǣ���С��������x����ߣ�����y���ұߣ�֮��
    //�Ż����ö����������ײ��
    //������ײ֮�󣬼�������ײ���Ǳ����õ�
    //���header���ҳ���pongbasic������÷�����������һ��t��ʦ���壬�о������õ�
    //������Լ�ûд���ðɺð�ʵ�����Ǵ�T��ʦ����˼·
{
    [Header("Ҫ�滻�Ķ�Ӧ�Ķ���")]
    [Tooltip("a,b,c,d,e,f,g,h")]
    public GameObject[] targetsAH = new GameObject[8];   
    [Tooltip("1,2,3,4,5,6,7,8")]
    public GameObject[] replacements18 = new GameObject[8]; 

    [Header("���� ����֮�󴥷�block ����")]
    [Tooltip("����x������ʹ�����ײ��")]
    public GameObject xTrigger;
    [Tooltip("���� y ���ö�ħ�����ײ��")]
    public GameObject yTrigger;

    [Header("�����Ķ���ײ���������񣬲�Ӱ�����屾����ͼɶ��")]
    public bool onlyToggleCollidersOnXY = true;

    private Collider2D[] targetColsA = new Collider2D[4]; // a..d
    private Collider2D[] targetColsB = new Collider2D[4]; // e..h
    private Collider2D xCol, yCol;

    // ������߼���true => A���� (abcd)��false => B���� (efgh)
    private bool groupAEnabled = false;

    void Awake()
    {
        //ץ�����ǵ� Collider
        for (int i = 0; i < 4; i++)
        {
            targetColsA[i] = SafeGetCollider2D(targetsAH[i]);       // a,b,c,d
            targetColsB[i] = SafeGetCollider2D(targetsAH[i + 4]);   // e,f,g,h
        }

        xCol = SafeGetCollider2D(xTrigger);
        yCol = SafeGetCollider2D(yTrigger);

        //��ʼ�˸�����ȫ������collider 
        SetGroupColliders(targetColsA, false);
        SetGroupColliders(targetColsB, false);
        groupAEnabled = false; // �ʼ����
    }

    // �ⲿ�� ���������ҵ�С��ײ��
    void OnTriggerEnter2D(Collider2D other) { HandleContact(other.gameObject); }
    void OnCollisionEnter2D(Collision2D collision) { HandleContact(collision.gameObject); }

    private void HandleContact(GameObject other)
    {
        if (other == null) return;

        // �����Ӧ�Ķ�������
        if (xTrigger && other == xTrigger) { EnableGroupB_DisableGroupA(); return; }
        if (yTrigger && other == yTrigger) { EnableGroupA_DisableGroupB(); return; }

        //abcdefgh��Ӧ��12345678�ֱ����á�Ϊ�˷�ֹ�ҿ�����12345678���Ǹ����ߵ��ǲ��֡�
        for (int i = 0; i < targetsAH.Length; i++)
        {
            if (targetsAH[i] && other == targetsAH[i])
            {
                targetsAH[i].SetActive(false);
                if (i < replacements18.Length && replacements18[i]) replacements18[i].SetActive(true);
                return;
            }
        }
    }


    private void EnableGroupB_DisableGroupA()
    {
        if (onlyToggleCollidersOnXY)
        {
            SetGroupColliders(targetColsA, false); // abcd ��
            SetGroupColliders(targetColsB, true);  // efgh ��
        }
        else
        {
            SetGroupActive(targetsAH, 0, 4, false); // abcd��
            SetGroupActive(targetsAH, 4, 4, true);  
        }
        groupAEnabled = false;
    }

    private void EnableGroupA_DisableGroupB()
    {
        if (onlyToggleCollidersOnXY)
        {
            SetGroupColliders(targetColsB, false); // efgh ��
            SetGroupColliders(targetColsA, true);  // abcdd ��
        }
        else
        {
            SetGroupActive(targetsAH, 4, 4, false); // efgh��
            SetGroupActive(targetsAH, 0, 4, true);  // abcd ��
        }
        groupAEnabled = true;
    }

    //���¶����Զ��庯���ϼƣ�Ϊ�˴����Ǹ���ײ�Ĳ��֣�
    //�Ҹо��ⲿ��Ӧ�ò�ע��Ҳ�ܿ�������
    private Collider2D SafeGetCollider2D(GameObject go)
    {
        if (go == null) return null;
        var col = go.GetComponent<Collider2D>();
        if (col == null) col = go.GetComponentInChildren<Collider2D>(true);
        return col;
    }

    private void SetGroupColliders(Collider2D[] group, bool enabled)
    {
        if (group == null) return;
        foreach (var c in group)
        {
            if (c != null) c.enabled = enabled;
        }
    }

    private void SetGroupActive(GameObject[] arr, int start, int count, bool active)
    {
        if (arr == null) return;
        for (int i = 0; i < count; i++)
        {
            int idx = start + i;
            if (idx >= 0 && idx < arr.Length && arr[idx] != null)
            {
                arr[idx].SetActive(active);
            }
        }
    }
}
