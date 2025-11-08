using UnityEngine;
using System.Collections.Generic;

public class randomspawnthing : MonoBehaviour
{
    public List<GameObject> things;
    public float distance = 1.5f;  
    public float heightScale = 0.6f; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (things.Count == 0) return;

            var t = things[Random.Range(0, things.Count)];
            if (t == null) return;

            Vector3 pos = transform.position + transform.forward * distance;
            var obj = Instantiate(t, pos, Quaternion.identity);

            Vector3 s = obj.transform.localScale;
            s.y *= heightScale;  
            obj.transform.localScale = s;
        }
    }
}
