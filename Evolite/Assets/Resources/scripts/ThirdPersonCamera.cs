using UnityEngine;

[DefaultExecutionOrder(-200)] 
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float altura = 1.5f;
    public float collisionCorrection;
    public Vector2 sensi = new Vector2(1f, 1f);
    public Vector2 vertLimits = new Vector2(-30f, 60f);
    public float collisionRadius = 0.2f;
    public LayerMask collisionMask = ~0;

    private float yaw;
    private float pitch;

    void Awake()
    {
        var cam = GetComponent<UnityEngine.Camera>();
        if (cam && !CompareTag("MainCamera")) gameObject.tag = "MainCamera";
    }

    void Update()
    {
        yaw += Input.GetAxis("Mouse X") * sensi.x;
        pitch -= Input.GetAxis("Mouse Y") * sensi.y;
        pitch = Mathf.Clamp(pitch, vertLimits.x, vertLimits.y);
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensi.x;
        pitch -= Input.GetAxis("Mouse Y") * sensi.y;
        pitch = Mathf.Clamp(pitch, vertLimits.x, vertLimits.y);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPos = target.position + Vector3.up * altura;
        Vector3 desired = targetPos + rot * new Vector3(0f, 0f, -distance);

        Vector3 finalPos = Colission(targetPos, desired);

        transform.SetPositionAndRotation(finalPos, rot);
    }

    Vector3 Colission(Vector3 targetPos, Vector3 desiredPos)
    {
        Vector3 dir = desiredPos - targetPos;   // direção do alvo para a câmera
        float dist = dir.magnitude;
        Vector3 finalPos = desiredPos;

            if (Physics.SphereCast(
                targetPos,                  // origem = personagem
                collisionRadius,            // raio da esfera
                dir.normalized,             // direção para a câmera
                out RaycastHit hit,
                dist,
                collisionMask,
                QueryTriggerInteraction.Ignore))
            {
                // Coloca a câmera antes do obstáculo
                finalPos = hit.point - dir.normalized * collisionCorrection;
            }
        
        // Para debug
        Debug.DrawLine(targetPos, finalPos, Color.magenta);
        return finalPos;
    }
}