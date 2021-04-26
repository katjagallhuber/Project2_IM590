using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pulse : MonoBehaviour
{
    private float pulseStrength = 0.8f;
    private float maxScale = 1.05f;
    private float minScale = 0.95f;
    private bool grow = true;
    private Vector3 minScaleVector;
    private Vector3 maxScaleVector;
    private float interPolationTime;
    // Start is called before the first frame update
    void Start()
    {
        minScaleVector = new Vector3(minScale, minScale, minScale);
        maxScaleVector = new Vector3(maxScale, maxScale, maxScale);
        transform.localScale = minScaleVector;
    }

    // Update is called once per frame
    void Update()
    {
        interPolationTime += Time.deltaTime*pulseStrength;
        if (grow)
        {
            transform.localScale = Vector3.Lerp(minScaleVector, maxScaleVector, interPolationTime);
            if (transform.localScale == maxScaleVector)
            {
                grow = false;
                interPolationTime = 0;
            }

        } else
        {
            transform.localScale = Vector3.Lerp(maxScaleVector, minScaleVector, interPolationTime);
            if (transform.localScale == minScaleVector) 
            { 
                grow = true;
                interPolationTime = 0;
            }
        }
    }
}
