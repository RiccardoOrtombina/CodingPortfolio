using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimationManager : MonoBehaviour
{
    private Animator _animator;
    private bool pulse;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetAnimatorBoolTrue(string parameterName)
    {
        _animator.SetBool(parameterName, true);
    }
    
    public void SetAnimatorBoolFalse(string parameterName)
    {
        _animator.SetBool(parameterName, false);
    }

    public void ResetAnimations()
    {
        _animator.SetBool("Pulse", true);
        _animator.SetBool("Continued", false);
        _animator.SetBool("Repeated", false);
        _animator.SetBool("Idle", false);
        _animator.SetBool("ContinuedPressed", false);
    }
}
