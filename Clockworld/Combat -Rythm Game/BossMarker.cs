using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMarker : MonoBehaviour
{
    CombatManager manager;
    Combo move;
    private int inputIndex;
    private int correctInputs;
    private int wrongInputs;

    public float prolongedInputUncheckedTime = 0.1f;
    float uncheckedTimeReset = 0.1f;
    bool prolongedMiss;
    public float timeToRepeatButton = 0.15f;
    bool repeatedPressed;

    private string _currentPrimaryInput;
    private bool _expectingInput;
    
    public void Init(CombatManager _manager)
    {
        manager = _manager;
    }

    public void SetMove2(Combo combo)
    {
        move = combo;
        inputIndex = 0;
        correctInputs = 0;
        wrongInputs = 0;
    }

    public void SecondaryButton()
    {
        repeatedPressed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _expectingInput = true;
        manager.inputMarker.GetComponent<Marker>().SetSecondaryInput(_expectingInput, move.secondaryInputs[inputIndex].inputName, move.secondaryInputs[inputIndex].axisValue);
        if(move.secondaryInputs[inputIndex].isContinued || move.secondaryInputs[inputIndex].isRepeated)
        {
            prolongedInputUncheckedTime = manager.inputObject.GetComponent<BoxCollider2D>().size.x / (manager.timelineSpeedX * 50);
            repeatedPressed = false;
            Invoke("startCheckingContinued", manager.distanceBetweenBeats / (manager.timelineSpeedX * 50));
            if(move.secondaryInputs[inputIndex].isRepeated)
            {
                Invoke("StartCheckingRepeated", uncheckedTimeReset);
            }
            
            ButtonController buttonController = collision.GetComponent<ButtonController>();
            buttonController.buttonSprite.transform.position = transform.position;
            manager.moveOppositeToTimeline.Add(buttonController.buttonSprite.transform);
            if (move.secondaryInputs[inputIndex].isContinued)
            {
                buttonController.SetAnimation("ContinuedPressed");
            }
                     
        }
    }


    void startCheckingContinued()
    {
        StartCoroutine("ProlongedInputCheck2");
    }

    void StartCheckingRepeated()
    {
        StartCoroutine("RepeatedButtonCheck2");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(move.secondaryInputs[inputIndex].isContinued == true || move.secondaryInputs[inputIndex].isRepeated == true)
        {
            if(prolongedInputUncheckedTime >= 0)
            {
                prolongedInputUncheckedTime -= Time.deltaTime;                
            }

            else
            {
                if(move.secondaryInputs[inputIndex].isContinued == true)
                {
                    if (move.secondaryInputs[inputIndex].axisValue != 0)
                    {
                        if (Input.GetAxisRaw(move.secondaryInputs[inputIndex].inputName) != move.secondaryInputs[inputIndex].axisValue)
                        {
                            prolongedMiss = true;
                        }
                    }

                    else
                    {
                        if (Input.GetButton(move.secondaryInputs[inputIndex].inputName) == false)
                        {
                            prolongedMiss = true;
                        }
                    }
                }                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _expectingInput = false;
        manager.inputMarker.GetComponent<Marker>().SetSecondaryInput(_expectingInput, move.secondaryInputs[inputIndex].inputName, move.secondaryInputs[inputIndex].axisValue);

        if(move.secondaryInputs[inputIndex].isContinued == true || move.secondaryInputs[inputIndex].isRepeated == true)
        {
            ProlongedExitLogic();
        }
        
        ButtonController buttonController = collision.gameObject.GetComponent<ButtonController>();
        buttonController.SetAnimation("Idle");
        manager.moveOppositeToTimeline.Remove(buttonController.buttonSprite.transform);

        if(inputIndex < move.secondaryInputs.Length - 1)
        {
            inputIndex += 1;
        }

        else
        {
            LineEnd();
        }
    }
    
    void ProlongedExitLogic()
    {
        StopCoroutine("RepeatedButtonCheck2");
        StopCoroutine("ProlongedInputCheck2");
        prolongedInputUncheckedTime = uncheckedTimeReset;
        if (prolongedMiss == true)
        {
            UpdateCorrectInputs(false);
            prolongedMiss = false;
        }

        else
        {
            UpdateCorrectInputs(true);
        }
    }
    
    IEnumerator ProlongedInputCheck2()
    {
        while (true)
        {
            if(prolongedMiss == true)
            {
                UpdateCorrectInputs(false);
                prolongedMiss = false;
            }

            else
            {
                UpdateCorrectInputs(true);
            }
            yield return new WaitForSeconds(manager.beatTime);
        }
        
    }

    IEnumerator RepeatedButtonCheck2()
    {
        while (true)
        {
            if(repeatedPressed == false)
            {
                prolongedMiss = true;
            }

            repeatedPressed = false;

            yield return new WaitForSeconds(timeToRepeatButton);
        }        
    }
    
    void UpdateCorrectInputs(bool increase)
    {
        if (increase == true)
        {
            correctInputs += 1;
        }

        else
        {
            wrongInputs += 1;
        }
       
    }

    void LineEnd()
    {
        float comboSuccessMultiplier = 1;

        if(wrongInputs >= 1.0 * move.secondaryInputs.Length / 2 + 1)
        {
            comboSuccessMultiplier = 1.2f;
        }

        else if(correctInputs >= move.secondaryInputs.Length)
        {
            comboSuccessMultiplier = 0;
        }
        
        manager.AssignSecondLinePoints(comboSuccessMultiplier, move);
    }
}
