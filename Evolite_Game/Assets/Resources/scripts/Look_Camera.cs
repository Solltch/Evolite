using UnityEngine;

public class Look_Camera : MonoBehaviour
{

    public Transform cameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = UnityEngine.Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion gira = cameraTransform.rotation;
        gira.x = 0;
        gira.z = 0;
        transform.rotation = gira;
    }
}