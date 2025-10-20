using Unity.Mathematics;
using UnityEngine;

public class AAfollow2 : MonoBehaviour
{
    public Transform creature;
    public quaternion rot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rot = creature.rotation;
        transform.rotation = rot;
    }
}
