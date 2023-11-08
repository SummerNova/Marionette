using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GripPointManager : MonoBehaviour
{
    [SerializeField] GripPoint[] gripPoints;
    [SerializeField] GameObject Body;
    [SerializeField] GripColor LastEnabled;
    private bool Disabled = false;

    private void Start()
    {
        gripPoints = GetComponentsInChildren<GripPoint>();
    }

    public IEnumerator DisableGripsBut(GripColor color, Vector3 Origin)
    {
        if (!Disabled)
        {
            LastEnabled = color;
            foreach (GripPoint gripPoint in gripPoints)
            {
                if (gripPoint.gripColor != LastEnabled)
                {
                    StartCoroutine(gripPoint.Disable(Vector3.Distance(Origin, gripPoint.transform.position)));
                }
            }
            Disabled = true;
        }
        yield return null;
    } 

    public void Enable() 
    {
        foreach (GripPoint gripPoint in gripPoints)
        {
            
            StartCoroutine(gripPoint.Enable(Vector3.Distance(Body.transform.position, gripPoint.transform.position)));
            
        }
        Disabled = false;
    }
}
