using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class HandController : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] HandController otherHand;
    [SerializeField] float Reach = 10f;
    [SerializeField] Rigidbody2D RB;
    public bool isDragged = false;
    public bool Attached = false;

    private GameObject Anchor;
    private Vector3 DragTarget;
    private Vector3 MoveTarget;
    bool HasAnchor = false;
    



    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        if (isDragged)
        {
            RB.constraints = RigidbodyConstraints2D.FreezeAll;
            Attached = false;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        if (isDragged)
        {
            isDragged = false;
            if (HasAnchor)
            {
                Attached = true;
                StartCoroutine(AttachToAnchor());
            }
            else
            {
                RB.constraints = RigidbodyConstraints2D.None;
            }
            
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (otherHand.isDragged == false)
        {
            isDragged = true;
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        DragTarget = new Vector3(Camera.main.ScreenToWorldPoint(eventData.position).x, Camera.main.ScreenToWorldPoint(eventData.position).y);
        if (Vector3.Distance(DragTarget, otherHand.transform.position) < Reach)
        {
            MoveTarget = DragTarget;
        }
        else
        {

            MoveTarget = otherHand.transform.position + (Reach) * (DragTarget - otherHand.transform.position).normalized;
        }
        transform.position = Vector3.Lerp(transform.position, MoveTarget, Time.deltaTime * 10);
    }

    IEnumerator AttachToAnchor()
    {
        while (Attached)
        {
            transform.position = Anchor.transform.position;
            yield return new WaitForEndOfFrame();
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Grip Point")
        {
            Anchor = collision.gameObject;
            HasAnchor = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Grip Point")
        {
            HasAnchor = false;
        }
    }

}
