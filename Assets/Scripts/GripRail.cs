using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GripRail : MonoBehaviour
{
    [SerializeField] public GripPoint gripPoint;
    [SerializeField] public RailCorner[] corners;
    Vector3 XZ = new Vector3 (0.1f, 0, 1);
    Vector3 Y = new Vector3 (0,1,0);
 
    void Update()
    {
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i].Rail.transform.position = (corners[i].transform.position + corners[(i+1)%corners.Length].transform.position)/2;
            corners[i].Rail.transform.localScale = XZ + Y* Vector3.Distance(corners[i].transform.position, corners[(i + 1) % corners.Length].transform.position);
            corners[i].Rail.transform.up = corners[(i + 1) % corners.Length].transform.position - corners[i].transform.position;
        }
        if (!Application.IsPlaying(this)) 
        { 
            gripPoint.transform.position = corners[0].transform.position - new Vector3(0,0, 0.5f);
        }
    }
}
