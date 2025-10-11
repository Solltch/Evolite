using UnityEngine;

public class Particle_Activate : MonoBehaviour
{
    public Player_Movement stats;
    public bool isRunning;
    public bool isGrounded;
    public bool onSlope;
    public ParticleSystem particles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        isRunning = stats.isRunning;
        isGrounded = stats.isGrounded;
        onSlope = stats.onSlope;

        if (isRunning && isGrounded || onSlope && stats.currentAngle > stats.maxSlopeAngle)
        {
            if (!particles.isPlaying)
                particles.Play();
        }
        else
        {
            if (particles.isPlaying)
                particles.Stop();
        }
    }
}
