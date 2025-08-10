using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform posi;
    public Transform clldr;
    public Vector3 sensi = new Vector3(1, 1, 1);
    public Vector2 vertLimits = new Vector2(-30, 60);
    public float distance = 5f;
    public float altura = 1.5f;

    private float horin;
    private float vertin;

    void Update()
    {
        horin += Input.GetAxis("Mouse X") * sensi.x;
        vertin -= Input.GetAxis("Mouse Y") * sensi.y;
        vertin = Mathf.Clamp(vertin, vertLimits.x, vertLimits.y);

        Quaternion rotation = Quaternion.Euler(vertin, horin, 0f);
        Vector3 targetPosition = posi.position + Vector3.up * altura;
        Vector3 offset = targetPosition + rotation * new Vector3(0f, 0f, -distance);

        Vector3 dir = (posi.position - clldr.position).normalized;

        Vector3 posicao;

        Debug.DrawLine(posi.position, clldr.position);
        if (Physics.Linecast(targetPosition, offset, out RaycastHit hit))
        {
            posicao = hit.point;
            transform.position = posicao;
        }
        else 
        {
            transform.position = offset;
        }

        transform.rotation = rotation;
    }

    private void LateUpdate()
    {
        
    }
}