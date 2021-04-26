using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ParticlesMASTER : MonoBehaviour
{
    public struct Particle
    {
        public Vector2 position;
        public Vector2 direction;
        public float speed;
    }

    // Particles Settings
    [Header("Shader Settings")]
    [SerializeField]
    private ComputeShader computeShader;
    [SerializeField]
    private Shader particleShader;

    [Header("Particle Parameters")]
    [SerializeField]
    private int particleCount = 100;
    [SerializeField]
    private Vector2 emitterSize = Vector2.one;
    [SerializeField]
    private Vector2 emitterPosition = Vector2.zero;
    [SerializeField]
    private float particlesMinSpeed = 0.5f;
    [SerializeField]
    private float particlesMaxSpeed = 1.0f;
   
    private Material particlesMaterial;
    
    // Compute shader setup
    private ComputeBuffer particlesBuffer;
    private int computeKernel;

    void Start()
    {
        computeKernel = computeShader.FindKernel("CSParticles");
        CreateParticlesBuffer();
        
        particlesMaterial = new Material(particleShader);
    }

    void Update()
    {
        computeShader.SetBuffer(computeKernel, "particlesBuffer", particlesBuffer);
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        // Define kernel and count of thread groups
        computeShader.Dispatch(computeKernel, 256, 1, 1);
    }

    private void OnRenderObject()
    {
        particlesMaterial.SetPass(0);
        particlesMaterial.SetBuffer("particlesBuffer", particlesBuffer);
        Graphics.DrawProceduralNow(MeshTopology.Points, particlesBuffer.count);
    }

    private void CreateParticlesBuffer()
    {
        particlesBuffer = new ComputeBuffer(particleCount, Marshal.SizeOf(typeof(Particle)));

        var particles = new Particle[particleCount];

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i] = CreateParticle();
        }

        particlesBuffer.SetData(particles);
    }

    private Particle CreateParticle()
    {
        Particle particle = new Particle
        {
            position = new Vector2(emitterPosition.x + Random.value * emitterSize.x, emitterPosition.y + Random.value * emitterSize.y),
            direction = Random.insideUnitCircle,
            speed = Random.Range(particlesMinSpeed, particlesMaxSpeed)
        };

        return particle;
    }

    private void OnDisable()
    {
        if (particlesBuffer != null)
        {
            particlesBuffer.Release();
        }
    } 
}
