using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectOnTop : MonoBehaviour
{

    private CanvasGroup[] elements;
    // Start is called before the first frame update
    void Start()
    {
        elements = GetComponentsInChildren<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
       float highestY = 0;
       int highestIndex = 0;
       for(int i = 0; i < elements.Length; i++)
       {
            if(elements[i].gameObject.transform.position.y > highestY)
            {
                highestY = elements[i].transform.position.y;
                highestIndex = i;
            } 
       }

        Vector3 newRotation = new Vector3(0, 0, 0.05f);
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].GetComponentInChildren<Text>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            if (i== highestIndex)
            {
                elements[i].GetComponentInChildren<Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            elements[i].gameObject.transform.Rotate(-newRotation);
        }
        transform.Rotate(newRotation);
    }
}
