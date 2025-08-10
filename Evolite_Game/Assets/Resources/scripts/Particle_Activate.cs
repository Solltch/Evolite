using UnityEngine;

public class Particle_Activate : MonoBehaviour
{
    public Test_Movement stats;
    public bool isRunning;
    public bool isGrounded;
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

        if (isRunning && isGrounded)
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
