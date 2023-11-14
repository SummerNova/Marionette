using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{
    [SerializeField] HandController[] controllers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            controllers[0].isGrounded = true;
            controllers[1].isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            controllers[0].isGrounded = false;
            controllers[1].isGrounded = false;
        }
    }
}
