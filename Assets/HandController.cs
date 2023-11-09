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
    [SerializeField] GripPointManager gripPointManager;
    [SerializeField, Range(1, 100)] float damping = 1;
    public bool isDragged = false;
    public bool Attached = false;
    public bool oldestGrip = false;

    private GameObject Anchor;
    private Vector3 DragTarget;
    private Vector3 MoveTarget;
    bool HasAnchor = false;


    private void Update()
    {
        if (!Attached && (otherHand.Attached || otherHand.isDragged) && Vector3.Distance(transform.position, otherHand.transform.position) > Reach) 
        {
            RB.AddForce((otherHand.transform.position- transform.position ).normalized * Reach/damping, ForceMode2D.Impulse);
        }
    }

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
                oldestGrip = false;
                otherHand.oldestGrip = true;
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
        if (isDragged)
        {
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
    }

    IEnumerator AttachToAnchor()
    {
        StartCoroutine(gripPointManager.DisableGripsBut(Anchor.GetComponent<GripPoint>().gripColor,transform.position));
        while (Attached)
        {
            transform.position = Anchor.transform.position;
            if (Vector3.Distance(transform.position, otherHand.transform.position) > Reach && oldestGrip && otherHand.Attached)
            {
                Attached = false; 
                RB.constraints = RigidbodyConstraints2D.None;
            } 
            yield return new WaitForEndOfFrame();
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Grip Point" && !Attached)
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
