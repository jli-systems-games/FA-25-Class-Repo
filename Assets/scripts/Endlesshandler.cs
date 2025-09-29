using System.Collections;
using UnityEngine;

public class Endlesshandler : MonoBehaviour
{
    [SerializeField]
    GameObject[] sectionPrefabs;

    GameObject[] sectionPool = new GameObject[20];

    GameObject[] sections = new GameObject[10];

    Transform playerCarTransform;

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    const float sectionLength = 26;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int prefabIndex = 0;

        for(int i = 0; i < sectionPool.Length; i++)
        {
            sectionPool[i] = Instantiate(sectionPrefabs[prefabIndex]);
            sectionPool[i].SetActive(false);

            prefabIndex++;


            if (prefabIndex > sectionPrefabs.Length - 1)
                prefabIndex = 0;
        }

        for(int i=0; i< sections.Length; i++)
        {
            GameObject randomSection = GetRandomSectionFromPool();

            randomSection.transform.position = new Vector3(sectionPool[i].transform.position.x, 0, i * sectionLength);
            randomSection.SetActive(true);

            sections[i] = randomSection;
        }
        StartCoroutine(UpdateLessOftrnCO());
    }

    IEnumerator UpdateLessOftrnCO()
    {
        while (true)
        {
            UpdateSectionPositions();
            yield return waitFor100ms;
        }
    }
    void UpdateSectionPositions()
    {
        for(int i = 0; i < sections.Length; i++)
        {
            if(sections[i].transform.position.z - playerCarTransform.position.z< -sectionLength)
            {
                Vector3 lastSectionPostion = sections[i].transform.position;
                sections[i].SetActive(false);

                sections[i] = GetRandomSectionFromPool();


                sections[i].transform.position = new Vector3(lastSectionPostion.x, 0, lastSectionPostion.z + sectionLength * sections.Length);
                sections[i].SetActive(true);
            }
        }
    }
        GameObject GetRandomSectionFromPool()
        {
            int randomIndex = Random.Range(0, sectionPool.Length);

            bool isNewSectionFound = false;

            while (!isNewSectionFound)
            {
                if (!sectionPool[randomIndex].activeInHierarchy)
                    isNewSectionFound = true;
                else
                {
                    randomIndex++;

                    if (randomIndex > sectionPool.Length - 1)
                        randomIndex = 0;
                }

            }
        return sectionPool[randomIndex];

        }
    }
    
