using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueController : MonoBehaviour
{
    private Vector3 _position;
    private Rigidbody rb;
    private CharacterAnimationManager _animationManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _animationManager = GetComponent<CharacterAnimationManager>();
    }

    public void LookAtConversant(Transform conversant)
    {
        Vector3 lookAt = new Vector3(conversant.position.x, transform.position.y, conversant.position.z);
        transform.LookAt(lookAt);
    }

    void MoveToPosition()
    {
        //_position = GameObject.Find(position).transform.position;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    IEnumerator MoveOnFixedUpdate()
    {
        while (Vector3.Distance(transform.position, _position) > 1f)
        {
             yield return new WaitForFixedUpdate();
        }
    }
}
