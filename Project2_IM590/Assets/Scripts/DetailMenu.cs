using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetailMenu : MonoBehaviour
{

    private float currentAlpha;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0.0f;
        currentAlpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Blend()
    {
        Debug.Log(currentAlpha);
        if (GetComponent<CanvasGroup>().alpha == 0.0f) { 
            GetComponent<CanvasGroup>().alpha = 1.0f;
            currentAlpha = 1.0f;
        }
        else 
        {
            GetComponent<CanvasGroup>().alpha = 0.0f;
            currentAlpha = 0.0f;
        }
    }
}
