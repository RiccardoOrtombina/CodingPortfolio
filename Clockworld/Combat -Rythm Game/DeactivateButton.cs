using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateButton : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        other.gameObject.GetComponent<ButtonController>().ResetButton();
        other.gameObject.SetActive(false);
    }

}