using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ← 别忘了引入场景管理命名空间

public class CollisionMaterialChanger : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;
    public float checkDistance = 1f;

    public GameObject[] enableList;
    public GameObject[] disableList;

    public Material changeToMaterial;

    public float delayEach = 0.05f;

    public bool onlyOnce = true;

    public string nextScene = "2";

    private bool alreadyTrigger = false;
    private bool isChanging = false;

    void Update()
    {
        if (objectA == null || objectB == null) return;

        float dis = Vector3.Distance(objectA.transform.position, objectB.transform.position);

        if (dis <= checkDistance && !alreadyTrigger)
        {
            turnOnOffObject();
            StartCoroutine(changeAllMaterialSlow());
            if (onlyOnce) alreadyTrigger = true;
        }
    }

    void turnOnOffObject()
    {
        foreach (var o in enableList)
            if (o != null) o.SetActive(true);

        foreach (var o in disableList)
            if (o != null) o.SetActive(false);
    }

    IEnumerator changeAllMaterialSlow()
    {
        if (isChanging) yield break;
        isChanging = true;

        if (changeToMaterial == null)
        {
    
            yield break;
        }

        List<Renderer> allRenderer = new List<Renderer>(FindObjectsOfType<Renderer>());
        List<Terrain> allTerrain = new List<Terrain>(FindObjectsOfType<Terrain>());

        foreach (Renderer r in allRenderer)
        {
            if (r == null) continue;
            if (r.gameObject.layer == LayerMask.NameToLayer("UI")) continue;
            if (r is SpriteRenderer) continue;

            r.sharedMaterials = new Material[] { changeToMaterial };
            yield return new WaitForSeconds(delayEach);
        }

        foreach (Terrain t in allTerrain)
        {
            if (t == null) continue;
            t.materialTemplate = changeToMaterial;
            t.drawInstanced = false;
            t.Flush();

            yield return new WaitForSeconds(delayEach);
        }


        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextScene);

        isChanging = false;
    }
}
