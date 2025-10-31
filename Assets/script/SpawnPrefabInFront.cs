using UnityEngine;
using UnityEngine.UI;

public class SpawnPrefabInFront : MonoBehaviour
{
    public GameObject[] prefabList = new GameObject[6];

    [SerializeField] private int nowIndex = 0;

    public Slider distanceSlider;
    [Range(1f, 10f)] public float spawnDis = 5f;

    public Slider scaleSlider;
    [Range(1f, 30f)] public float spawnScale = 1f;

    public LayerMask rayMask = Physics.DefaultRaycastLayers;

  
    public Image colorImg;

    public string colorKey = "_BaseColor";

    public bool randomRotate = true;

    public GameObject playerObj;

    public MonoBehaviour playerJumpScript; 
    public float buffRange = 3f;
    public float jumpBoost = 3f;

    private float normalJump;
    private GameObject prefab6Obj;
    private Vector3 prefab6Pos;
    private bool jumpSaved = false;

    void Update()
    {
      
        if (distanceSlider != null)
            spawnDis = distanceSlider.value;
        if (scaleSlider != null)
            spawnScale = scaleSlider.value;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ChoosePrefab(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChoosePrefab(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChoosePrefab(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChoosePrefab(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ChoosePrefab(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) ChoosePrefab(5);

  
        if (Input.GetKeyDown(KeyCode.F))
            SpawnOne();

      
        if (prefab6Obj != null && playerObj != null && playerJumpScript != null)
        {
            float dis = Vector3.Distance(playerObj.transform.position, prefab6Pos);

            var type = playerJumpScript.GetType();
            var field = type.GetField("JumpHeight");
            if (field != null)
            {
                float nowJump = (float)field.GetValue(playerJumpScript);

                if (!jumpSaved)
                {
                    normalJump = nowJump;
                    jumpSaved = true;
                }

                if (dis < buffRange)
                    field.SetValue(playerJumpScript, jumpBoost);
                else
                    field.SetValue(playerJumpScript, normalJump);
            }
        }
    }

    void ChoosePrefab(int index)
    {
 
        nowIndex = index;
    }

    void SpawnOne()
    {
 

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Vector3 spawnPos = Physics.Raycast(ray, out hit, spawnDis, rayMask)
            ? hit.point
            : transform.position + transform.forward * spawnDis;

        Quaternion rot = (nowIndex == 5)
            ? Quaternion.identity
            : (randomRotate ? Random.rotation : Quaternion.identity);

 
        if (nowIndex == 5)
        {
            if (prefab6Obj != null)
                Destroy(prefab6Obj);

            prefab6Obj = Instantiate(prefabList[nowIndex], spawnPos, rot);
            prefab6Pos = prefab6Obj.transform.position;

   
            return;
        }

        GameObject obj = Instantiate(prefabList[nowIndex], spawnPos, rot);
        obj.transform.localScale = Vector3.one * spawnScale;

        if (colorImg != null)
        {
            Color c = colorImg.color;
            Renderer r = obj.GetComponent<Renderer>();
            if (r != null)
            {
                Material m = new Material(r.material);
                if (m.HasProperty(colorKey))
                {
                    m.SetColor(colorKey, c);
                    r.material = m;
                }
            }
        }

    }
}