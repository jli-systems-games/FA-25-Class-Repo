using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DraggableConsumable : MonoBehaviour
{
    public ConsumableSO data;
    public float resetHeight = 0.2f;
    public float followSpeed = 25f;

    bool dragging;
    Vector3 startPos;
    bool consumed;
    Rigidbody rb;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnMouseDown() { dragging = true; }
    void OnMouseUp() { dragging = false; if (!consumed) ResetBack(); }

    void Update()
    {
        if (!dragging || consumed) return;
        var cam = Camera.main;
        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        new Plane(Vector3.up, Vector3.zero).Raycast(r, out float d);
        var p = r.GetPoint(d);
        var target = new Vector3(p.x, resetHeight, p.z);
        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (consumed) return;
        var pet = other.GetComponentInParent<PetController>();
        if (!pet || data == null) return;
        consumed = true;
        pet.Consume(data);
        Destroy(gameObject);
    }

    void ResetBack() { transform.position = startPos; }
}
