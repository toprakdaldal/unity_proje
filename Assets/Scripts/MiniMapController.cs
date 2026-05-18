using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    [Header("── Takip ──")]
    [SerializeField] Transform target;
    [SerializeField] float     followSmooth = 8f;
    [SerializeField] Vector3   offset       = new Vector3(0f, 0f, -10f);

    [Header("── Yakınlaştırma ──")]
    [SerializeField] float zoomLevel = 15f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic     = true;
            cam.orthographicSize = zoomLevel;
        }
    }

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired   = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
        transform.position = Vector3.Lerp(transform.position, desired, followSmooth * Time.deltaTime);
    }
}
