using UnityEngine;

public class AAfollow : MonoBehaviour
{
    public Transform player;
    public Vector3 posi;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        posi = player.position;
        transform.position = posi;
    }
}
