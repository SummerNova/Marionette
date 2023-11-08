using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GripColor
{
    Red, Green, Blue, Yellow
}

public class GripPoint : MonoBehaviour
{

    [SerializeField] public GripColor gripColor = GripColor.Yellow;
    [SerializeField] SpriteRenderer Graphic;
    [SerializeField] Collider2D Collider;


    private void Start()
    {
        switch (gripColor)
        {
            case GripColor.Red:
                Graphic.color = Color.red;
                break;
            case GripColor.Green:
                Graphic.color = Color.green;
                break;
            case GripColor.Blue:
                Graphic.color = Color.blue;
                break;
            case GripColor.Yellow:
                Graphic.color = Color.yellow;
                break;
            default:
                Debug.Log("Grip Point Missing Color");
                break;
        }
    }

    public IEnumerator Disable(float Distance)
    {
        yield return new WaitForSeconds(Distance/10);
        Graphic.color = new Color(Graphic.color.r, Graphic.color.g, Graphic.color.b, 0.2f);
        Collider.enabled = false;
    }

    public IEnumerator Enable(float Distance)
    {
        yield return new WaitForSeconds(Distance / 10);
        Graphic.color = new Color(Graphic.color.r, Graphic.color.g, Graphic.color.b, 1);
        Collider.enabled = true;
    }
}


