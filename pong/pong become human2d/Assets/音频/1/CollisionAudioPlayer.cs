using UnityEngine;

public class CollisionAudioPlayer : MonoBehaviour
{
    [Header("��ħ��ײ A (a, b, c, d)")]
    public GameObject[] groupA;

    [Header("��ʹ��ײ B (e, f, g, h)")]
    public GameObject[] groupB;

    [Header("��ħ���� A (1, 2, 3)")]
    public AudioClip[] audioGroupA;

    [Header("��ʹ���� B (4, 5, 6)")]
    public AudioClip[] audioGroupB;

    private AudioSource audioSource;
    private bool[] groupAStates;
    private bool[] groupBStates;


    //��������ڿ�������һ�����ֻ��һ������������ԭ���㣬����Ĳ���д���롣
    //�����������gpt��ʦ��ָ����д�ġ��������ľ������ÿһ�е����岢��ƴ��һЩ���Լ���idea��
    //�Ƚϸ߼��Ĳ��ֶ�������д�ġ��Ҵ�bվ����ʵ�ֹ����˳��ġ���Ȼ���ܿ��������Ҿ���д�������Ǻ�

    // If anyone is reading this, I��m sorry.  
    // I��m just a concept artist. I really don��t know how to code.  
    // This script was written under the guidance of GPT.  
    // The best I can do is understand what each line means and piece together some of my own ideas.  
    // The more advanced parts definitely aren��t written by me.  
    // I copied them from people who had already implemented it on Bilibili.  
    // I can understand it, but I could never write it myself, haha.

    void Start()
    {
        //��ÿ�����壨û�еĻ�������һ�������audio������
    //��Ϊ�ұ�����û�и��������ɲ������ģ�ʵ�������ʼ��˼·����ײ֮�󲥷ŵ�����������е㲻��
    //�������ڵ�״̬����Ʒ��ʧ�ٲ���
    //�����ⲿ�ֵ��׻�Ҫ��Ҫ�Ҵ��ɵ����Ҳ�̫��ɾ��������������
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    
        groupAStates = new bool[groupA.Length];
        for (int i = 0; i < groupA.Length; i++)
        {
            if (groupA[i] != null) groupAStates[i] = groupA[i].activeSelf;
        }

        groupBStates = new bool[groupB.Length];
        for (int i = 0; i < groupB.Length; i++)
        {
            if (groupB[i] != null) groupBStates[i] = groupB[i].activeSelf;
        }
    }

    void Update()
    {
        //��׽ GameObject ������ �� ���� ��˲��������þ�������Ч
        //��ԭ���Ĵ�������û�м����� if (groupA[i] == null) continue;���ⲿ������gpt˵���Լ��Ϸ�ֹ������
        //groupAStates[i] = groupA[i].activeSelf; �ⲿ������Ϊ֮ǰ��Ī���ظ�������Ч���Լ�����
        //���þ��Ǽ����û�п�������ֹ���θ������������

        for (int i = 0; i < groupA.Length; i++)
        {
            if (groupA[i] == null) continue;

            if (groupAStates[i] && !groupA[i].activeSelf) 
            {
                PlayRandomAudio(audioGroupA);
            }

            groupAStates[i] = groupA[i].activeSelf; 
        }

       
        for (int i = 0; i < groupB.Length; i++)
        {
            if (groupB[i] == null) continue;

            if (groupBStates[i] && !groupB[i].activeSelf) 
            {
                PlayRandomAudio(audioGroupB);
            }

            groupBStates[i] = groupB[i].activeSelf; 
        }
    }

    void PlayRandomAudio(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        audioSource.clip = clips[index];
        audioSource.Play();
        //������Զ��庯����������
        // if (clips == null || clips.Length == 0) return;Ҳ��gpt debug��ʱ��ӵ�
        //���Լ����Ǻ�������д��ֹ����Ĵ����ϰ�ߣ�������
        //�������һ����ֵ�����Ŷ�Ӧ����Ƶ������ط�����д����lengthֻ��3��������Ϊ�ĳ������飨������𣩣��͸ĳ���clips.Length
        //ʲôʱ�������...�����ƴ������ʶ������
    }
}