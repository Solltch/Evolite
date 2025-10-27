using UnityEngine;

[DefaultExecutionOrder(-200)]
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    [Header("Configurações")]
    public float distance = 5f;
    public float altura = 1.5f;
    public Vector2 sensi = new Vector2(1f, 1f);
    public Vector2 vertLimits = new Vector2(-30f, 60f);

    [Header("Colisão")]
    public float offSet = 0.2f;
    public float alturaRay = 1f;

    private float yaw;
    private float pitch;
    private Vector3 desiredPosition;
    private Vector3 targetPosRay;

    void Awake()
    {
        if (target == null)
            target = GameObject.Find("Player Collider")?.transform;

        if (!CompareTag("MainCamera"))
            gameObject.tag = "MainCamera";
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- 1. Rotação ---
        yaw += Input.GetAxis("Mouse X") * sensi.x;
        pitch -= Input.GetAxis("Mouse Y") * sensi.y;
        pitch = Mathf.Clamp(pitch, vertLimits.x, vertLimits.y);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPos = target.position + Vector3.up * altura;

        Vector3 castOrigin = target.position + Vector3.up * alturaRay;

        Vector3 dir =  target.transform.position - GetComponentInChildren<Transform>().position;

        float currentDistance = distance;

        if (Physics.SphereCast(castOrigin, 0.25f, dir, out RaycastHit hit, distance, ~0, QueryTriggerInteraction.Ignore))
        {
            currentDistance = hit.distance - 0.25f - offSet;

            if (currentDistance < 0)
            {
                currentDistance = 0;
            }

        }

        desiredPosition = targetPos + dir * currentDistance;


        if (Physics.Raycast(targetPos, dir, out RaycastHit finalHit, currentDistance + 0.1f, ~0, QueryTriggerInteraction.Ignore))
        {
            desiredPosition = finalHit.point + finalHit.normal * offSet;
        }

        transform.position = desiredPosition;
        transform.rotation = rot;
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;

        Gizmos.color = Color.yellow;
        Vector3 origin = target.position + Vector3.up * alturaRay;
        Gizmos.DrawRay(origin, transform.position);
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}