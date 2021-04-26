using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FlowfieldMASTER : MonoBehaviour
{
    public struct FlowfieldCell
    {
        public Vector2 position;
        public Vector2 direction;
        public float intensity;
    }

    // Flowfield Settings
    [Header("Shader Settings")]
    [SerializeField]
    private ComputeShader computeShader;
    [SerializeField]
    private Shader flowfieldShader;

    [Header("Flowfield Parameters")]
    [SerializeField]
    private float flowfieldCellSize = 1.0f;
    [SerializeField]
    private Vector2 simulationSpace = Vector2.one;
    [SerializeField]
    private bool displayFlowfieldGizmos = true;
    [SerializeField]
    private Vector2[] cellDirection;

    private int cellsCountX, cellsCountY;
    private Material flowfieldMaterial;

    // Compute shader setup
    private ComputeBuffer flowfieldBuffer;
    private int maxCellsCount;
    private int flowfieldKernel, particlesKernel;

    void Start()
    {
        // STEP 1: Init compute shader
        // Kernel equals function that does parallel computation
        flowfieldKernel = computeShader.FindKernel("CSFlowfield");
        particlesKernel = computeShader.FindKernel("CSParticles");

        // STEP 2: Create buffers
        CreateFlowfieldBuffer();

        // STEP 3: Render flowfield
        flowfieldMaterial = new Material(flowfieldShader);
    }

    void Update()
    {
        SendDataToCompute();
    }

    // Send data to compute shader as uniforms
    private void SendDataToCompute ()
    {
        // Define input/ output data of compute shader
        computeShader.SetBuffer(flowfieldKernel, "flowfieldBuffer", flowfieldBuffer);
        computeShader.SetBuffer(particlesKernel, "flowfieldBuffer", flowfieldBuffer);

        // Define uniforms
        computeShader.SetVector("simluationSpace", simulationSpace);
        computeShader.SetVector("simulationPosition", new Vector2(this.transform.position.x, this.transform.position.y));
        computeShader.SetInt("cellCountX", cellsCountX);
        computeShader.SetInt("cellCountY", cellsCountY);

        // Execute compute shader with thread groups X, Y, Z
        computeShader.Dispatch(flowfieldKernel, 256, 1, 1);
    }

    // Enable rendering of flowfield
    private void OnRenderObject()
    {
        flowfieldMaterial.SetPass(0);
        flowfieldMaterial.SetBuffer("flowfieldBuffer", flowfieldBuffer);
        Graphics.DrawProceduralNow(MeshTopology.Points, flowfieldBuffer.count);
    }

    // Release memory if script is disabled
    private void OnDisable()
    {
        if (flowfieldBuffer != null)
        {
            flowfieldBuffer.Release();
        }
    }

    // Create one cell with a specific position
    private FlowfieldCell CreateCell (Vector2 position, Vector2 direction)
    {
        FlowfieldCell cell = new FlowfieldCell
        {
            position = position,
            direction = direction,
            intensity = Random.value
        };

        return cell;
    }

    // Prepare compute buffer for compute shader
    private void CreateFlowfieldBuffer ()
    {
        // Calculate cells count for rows & columns
        cellsCountX = (int)Mathf.Floor(simulationSpace.x / flowfieldCellSize);
        cellsCountY = (int)Mathf.Floor(simulationSpace.y / flowfieldCellSize);

        maxCellsCount = cellsCountX * cellsCountY;

        // Create buffer for flowfield
        flowfieldBuffer = new ComputeBuffer(maxCellsCount, Marshal.SizeOf(typeof(FlowfieldCell)));

        var cellsArray = new FlowfieldCell[maxCellsCount];

        int iterations = 0;
   
        // Set position for each cell in array
        for (int y = 0; y < cellsCountY; y++)
        {
            for (int x = 0; x < cellsCountX; x++)
            {
                var currentCell = (x * cellsCountY + y);

                Vector2 position = new Vector2(
                    simulationSpace.x / cellsCountX * x + flowfieldCellSize / 2,
                    simulationSpace.y / cellsCountY * y + flowfieldCellSize / 2
                );

                position.x += this.transform.position.x - simulationSpace.x / 2;
                position.y += this.transform.position.y - simulationSpace.y / 2;

                cellsArray[currentCell] = CreateCell(position, cellDirection[iterations]);

                iterations++;
            }
        }

        // Define data for compute buffer 
        flowfieldBuffer.SetData(cellsArray);
    }

    // Debug for flowfield
    private void OnDrawGizmos()
    {
        if (displayFlowfieldGizmos)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Vector3.zero, simulationSpace);
        }
    }

    
}
