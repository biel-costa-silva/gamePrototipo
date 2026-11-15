using UnityEngine;

[DefaultExecutionOrder(100)]   // roda depois da MinhaCamera
public class CameraReflexo : MonoBehaviour
{
    [SerializeField] private Camera principal;
    private Camera reflexo;

    void Awake()
    {
        reflexo = GetComponent<Camera>();
        if (principal == null) principal = Camera.main;
    }

    void LateUpdate()
    {
        reflexo.orthographic = true;
        reflexo.orthographicSize = principal.orthographicSize;

        // mesmo X da principal; o Y você mantém como já está no seu reflexo
        Vector3 p = transform.position;
        p.x = principal.transform.position.x;
        transform.position = p;
    }
}