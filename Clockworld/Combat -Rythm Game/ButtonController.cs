using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ButtonController : MonoBehaviour
{
    [FormerlySerializedAs("_buttonSprite")] [HideInInspector]
    public GameObject buttonSprite;
    private GameObject _durationLine;
    private float _lineLengthX;
    private Vector2 _colliderSize;
    
    public bool stopButton;
    
    void Start()
    {
        buttonSprite = transform.GetChild(0).gameObject;
        _durationLine = transform.GetChild(1).gameObject;
        _colliderSize = GetComponent<BoxCollider2D>().size;
        //c'è la y alla fine invece della x perchè lo sprite usato aql momento è girato di 90 gradi
        _lineLengthX = _durationLine.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        _durationLine.SetActive(false);
    }

    public void ResetButton()
    {
        GetComponent<BoxCollider2D>().size = _colliderSize;
        GetComponent<BoxCollider2D>().offset = Vector2.zero;
        _durationLine.transform.localPosition = Vector3.zero;
        _durationLine.transform.localScale = Vector3.one;
        _durationLine.SetActive(false);
        buttonSprite.transform.localPosition = Vector3.zero;
        buttonSprite.GetComponent<ButtonAnimationManager>().ResetAnimations();
    }

    public void SetButtonSprite(Sprite newSprite)
    {
        buttonSprite.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    public void SetSpritesPositions(float newPosition)
    {
        buttonSprite.transform.localPosition -= new Vector3(newPosition, 0, 0);
        _durationLine.SetActive(true);
        _durationLine.transform.localPosition -= new Vector3(newPosition / 2, 0, 0);
        _durationLine.transform.localScale = new Vector3(1, newPosition / _lineLengthX);
    }

    public void SetAnimation(string animationName)
    {
        buttonSprite.GetComponent<ButtonAnimationManager>().SetAnimatorBoolTrue(animationName);
    }

    public void StopAnimation(string animationName)
    {
        buttonSprite.GetComponent<ButtonAnimationManager>().SetAnimatorBoolFalse(animationName);
    }
    
}
