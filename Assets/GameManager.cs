using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject[] items;
    public Transform canvasTransform;
    private GameObject currentItem;
    public RawImage displayImage;

    public void OnButtonClick(int index)
    {
        currentItem = Instantiate(items[index], canvasTransform);
        currentItem.transform.position = Input.mousePosition;
    }


    void Start()
    {
        if (DrawingStorage.savedTexture != null)
        {
            displayImage.texture = DrawingStorage.savedTexture;
        }
    }

    void Update()
    {
        if (currentItem != null)
        {
            if (Input.GetMouseButton(0))
            {
                currentItem.transform.position = Input.mousePosition;
                currentItem.transform.SetSiblingIndex(3);
            }

            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                currentItem.transform.localScale += Vector3.one * scroll * 0.1f;
            }

            if (Input.GetKey(KeyCode.Q)) currentItem.transform.Rotate(Vector3.forward * 100 * Time.deltaTime);
            if (Input.GetKey(KeyCode.E)) currentItem.transform.Rotate(Vector3.forward * -100 * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                currentItem = null;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var r in results)
                {
                    if (r.gameObject.CompareTag("Respawn"))
                    {
                        Destroy(r.gameObject);
                        break;
                    }
                }
            }
        }
    }

}
