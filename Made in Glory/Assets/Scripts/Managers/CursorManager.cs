using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIG.Scripts.Common;

public class CursorManager : Singleton<CursorManager> {

    [Range(0, 5)]
    [Header("The hovering height of the cursor")]
    public float Height = 1.0f;

    Vector3 basePosition;
    public GameObject target;

    void Start()
    {
        basePosition = transform.position;
    }

    public void MoveCursor(GameObject target)
    {
        basePosition = target.transform.position;
        this.target = target;
    }

    void Update()
    {
        if (target != null)
        {
            basePosition = target.transform.position;
        }

        Vector3 actualPosition = basePosition;

        actualPosition.y += Height;

        actualPosition.y += Mathf.Cos(Time.timeSinceLevelLoad);

        transform.Rotate(0, 0, Time.deltaTime * 100);
        transform.position = actualPosition;

    }

}
