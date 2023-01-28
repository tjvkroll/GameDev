using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorGrowShrink : MonoBehaviour
{
    private Vector3 targetSize;
    private Vector3 previousSize;
    private float Timer;
    private bool shrinking;

    void Start()
    {
        Timer = 0;
        previousSize = transform.localScale;
        targetSize = new Vector3(1, 1, 1);
        shrinking = true;
    }

    void Update()
    {
        if (Timer >= 1 && shrinking)
        {
            targetSize = new Vector3(1.5f, 1.5f, 1);
            previousSize = transform.localScale;
            Timer = 0;
            shrinking = false;
        }
        else if (Timer >= 1 && !shrinking)
        {
            targetSize = new Vector3(1, 1, 1);
            previousSize = transform.localScale;
            Timer = 0;
            shrinking = true;
        }
        Timer += Time.deltaTime;
        transform.localScale = Vector3.Lerp(previousSize, targetSize, Timer / 1);
    }
}
