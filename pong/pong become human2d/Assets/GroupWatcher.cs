using UnityEngine;

public class GroupWatcher : MonoBehaviour
{
    [Header("��ħ A (a,b,c,d)")]
    public GameObject[] groupABCD = new GameObject[4];

    [Header("��ʹ B (e,f,g,h)")]
    public GameObject[] groupEFGH = new GameObject[4];

    [Header("������֮Ŀ��")]
    public GameObject obj1;  //��ħ��ս��cg
    public GameObject obj2;  // �ǵý����ӵ�
    public GameObject obj3;  // ��ʹ��ս��cg
    public GameObject obj4;  // ������������İ�ť��ʵ������ֻ��Ҫ����button�ͺ��˵����ҿ�������canvas�����ð�

    void Update()
    {
     //�����û��ȫ��һ��ȫ��������
     //Ϊ�˷�ֹңԶ��δ���ҿ������Լ�Ϊʲô��ôд��������Ϊ�����������սʤcg���߼�ʵ����
     //����ײ֮��С��л���ʧȻ��
     //С���ȫ��������֮��Ϳ�������սʤcg��
     //�����ʼplaytest��ʱ���ҷ��������һ��������ǡ�����Ϊ�ӵ�û�б����õĻ��ͻ����ײȻ���и��ʳ���һ��cg�ظ���ʵ������
     //����cg������֮���һ����ӵ����������������ʺɽ����
        if (AllDisabled(groupABCD))
        {
            if (obj1) obj1.SetActive(true);
            if (obj4) obj4.SetActive(true);   //ͬ����Ϊ�˱�֤���Ժ󿴵ö����Ǹ�ɶ�ģ�����Ϊ�������Ǹ���������С����
            if (obj2) obj2.SetActive(false);
        }

//ͬ��ɵ�
        if (AllDisabled(groupEFGH))
        {
            if (obj3) obj3.SetActive(true);
            if (obj4) obj4.SetActive(true);  
            if (obj2) obj2.SetActive(false);
        }
    }

    bool AllDisabled(GameObject[] group)
    {
        foreach (var go in group)
        {
            if (go != null && go.activeInHierarchy) return false;
        }
        return true;
    }
    //������ϵͳ
}
