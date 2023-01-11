using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorGrowShrink : MonoBehaviour
{
    private Vector3 targetSize;
    private float Timer;

    void Start()
    {
        Timer = 0;
        targetSize = new Vector3(.55f, .55f, .55f);
    }

    void Update()
    {
        if (Timer > 2)
        {
            targetSize = new Vector3(1, 1, 1);
            Timer = 0;
        }
        else if (Timer > 1)
        {
            targetSize = new Vector3(.55f, .55f, .55f);
        }
        Timer += Time.deltaTime;
        transform.localScale = Vector3.Lerp(transform.localScale, targetSize, Time.deltaTime);
    }
}
