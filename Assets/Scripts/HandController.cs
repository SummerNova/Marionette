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
    [SerializeField] float handReachMultiplier = 1;
    [SerializeField] Rigidbody2D Ragdoll;
    [SerializeField] CircleCollider2D Detection;

    public bool isDragged = false;
    public bool Attached = false;
    public bool oldestGrip = false;
    public bool isGrounded = false;

    private Vector3 DragOrigin;
    private Vector2 newTarget;
    private GameObject Anchor;
    private Vector3 DragTarget;
    private Vector3 MoveTarget;
    private int AnchorIncrement = 0;
    bool HasAnchor = false;


    private void Update()
    {
        if (!Attached && (otherHand.Attached || otherHand.isDragged) && Vector3.Distance(transform.position, otherHand.transform.position) > Reach) 
        {
            RB.AddForce((otherHand.transform.position- transform.position ).normalized * Reach/damping, ForceMode2D.Impulse);
        }
    }
    private void FixedUpdate()
    {
        if (isDragged && !otherHand.Attached && !isGrounded)
        {
            RB.constraints = RigidbodyConstraints2D.None;
            RB.AddForce(100 * (MoveTarget - transform.position), ForceMode2D.Force);

        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        if (isDragged)
        {
            
            RB.gravityScale = 0;

            Attached = false;
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            Detection.radius = 0.55f;
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
            else RB.constraints = RigidbodyConstraints2D.None;
            transform.localScale = new Vector3(0.73f, 0.73f, 0.73f);
            Detection.radius = 1;
            RB.gravityScale = 1;
            
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (otherHand.isDragged == false)
        {
            isDragged = true;
            //DragOrigin = eventData.position;
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        if (isDragged)
        {
            DragTarget = MultiplyTarget(eventData.position);
            if (Vector3.Distance(DragTarget, otherHand.transform.position) < Reach)
            {
                MoveTarget = DragTarget;
            }
            else
            {

                MoveTarget = otherHand.transform.position + (Reach) * (DragTarget - otherHand.transform.position).normalized;
            }

            if (!otherHand.Attached && !isGrounded)
            {
                Ragdoll.gravityScale = 2;
                Ragdoll.mass = 10;
            }
            else
            {
                RB.constraints = RigidbodyConstraints2D.FreezeAll;
                transform.position = Vector3.Lerp(transform.position,MoveTarget,Time.deltaTime*10);
            }
        }
    }

    IEnumerator AttachToAnchor()
    {
        StartCoroutine(gripPointManager.DisableGripsBut(Anchor.GetComponent<GripPoint>().gripColor,transform.position));
        RB.constraints = RigidbodyConstraints2D.FreezeAll;
        Ragdoll.gravityScale = 0;
        Ragdoll.mass = 0.01f;
        while (Attached)
        {
            transform.position = Anchor.transform.position;
            if (Vector3.Distance(transform.position, otherHand.transform.position) > Reach+1 && oldestGrip && otherHand.Attached)
            {
                Attached = false; 
                RB.constraints = RigidbodyConstraints2D.None;
            } 
            yield return null;
        }
        
    }

    private Vector3 MultiplyTarget(Vector2 CurrentDrag)
    {
        DragOrigin = Camera.main.WorldToScreenPoint((otherHand.transform.position + transform.position)/2);
        newTarget = new Vector2(DragOrigin.x, DragOrigin.y) + (CurrentDrag - new Vector2(DragOrigin.x,DragOrigin.y)) * handReachMultiplier;
        return new Vector3(Camera.main.ScreenToWorldPoint(newTarget).x, Camera.main.ScreenToWorldPoint(newTarget).y, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Grip Point" && !Attached)
        {
            Anchor = collision.gameObject;
            HasAnchor = true;
            AnchorIncrement++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Grip Point")
        {
            AnchorIncrement--;
            if (AnchorIncrement==0) HasAnchor = false;
        }
    }

}
