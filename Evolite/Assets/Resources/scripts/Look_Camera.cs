using UnityEngine;

public class Look_Camera : MonoBehaviour
{

    public Transform cameraTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = GameObject.Find("FreeLook Camera").GetComponent<Transform>(); ;
    }

    // Update is called once per frame
    void LateUpdate() 
    { 
        Quaternion gira = cameraTransform.rotation; 
        gira.x = 0; 
        gira.z = transform.parent.rotation.z; 
        transform.rotation = gira; 
    }
}