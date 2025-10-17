using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private CombatManager manager;
    private CombatAnimationManager _animationManager;
    
    private Combo nextMove;
    private int inputIndex;
    private bool expectingInput;
    private bool correctInputGiven;
    private bool inputGiven;
    private int correctInputsTotal;
    private int mistakesTotal;

    public float perfectMultiplier;

    private bool playerPhase;

    public float prolongedInputUncheckedTime = 0.1f;
    private float uncheckedTimeReset = 0.1f;
    private bool prolongedMiss;

    [Header("Per input ripetuti")]
    public float timeToRepeatButton = 0.15f;
    private bool repeatedPressed;
    private string currentSecondaryInput;
    private bool expectingSecondaryInput;
    private float secondaryAxisValue;

    public void Init(CombatManager combatManager, CombatAnimationManager animationManager)
    {
        manager = combatManager;
        _animationManager = animationManager;
    }

    public void SetMove(Combo combo)
    {
        nextMove = combo;
        inputIndex = 0;
        correctInputsTotal = 0;
        mistakesTotal = 0;
    }

    public void SetSecondaryInput(bool externalInput, string input, float value)
    {
        currentSecondaryInput = input;
        expectingSecondaryInput = externalInput;
        secondaryAxisValue = value;
    }

    public void ButtonDetected(string buttonName)
    {
        if(inputGiven == false && correctInputGiven == false)
        {
            if (expectingSecondaryInput)
            {
                if (buttonName == currentSecondaryInput)
                {
                    manager.bossMarker.GetComponent<BossMarker>().SecondaryButton();
                    return;
                }
            }
            
            if (expectingInput == false)
            {
                UpdateCorrectInputs(false);
            }

            else if (buttonName != nextMove.inputs[inputIndex].inputName)
            {
                //Debug.Log("Mistake!");
                UpdateCorrectInputs(false);
                inputGiven = true;
            }

            else if (buttonName == nextMove.inputs[inputIndex].inputName)
            {
                correctInputGiven = true;

                if (nextMove.inputs[inputIndex].isContinued == false && nextMove.inputs[inputIndex].isRepeated == false)
                {
                    UpdateCorrectInputs(true);
                }               
                //Debug.Log("Correct!");
            }
        }

        if(nextMove.inputs[inputIndex].isRepeated == true &&  buttonName == nextMove.inputs[inputIndex].inputName)
        {
            repeatedPressed = true;
        }
        
    }

    public void AxisDetected(string axisName, float axisValue)
    {
        if(inputGiven == false && correctInputGiven == false)
        {
            if (expectingSecondaryInput)
            {
                if (axisName == currentSecondaryInput)
                {
                    if (axisValue == secondaryAxisValue)
                    {
                        return;
                    }
                }
            }
            
            if (!expectingInput)
            {
                inputGiven = true;
                UpdateCorrectInputs(false);
            }

            else if (axisName != nextMove.inputs[inputIndex].inputName || axisValue != nextMove.inputs[inputIndex].axisValue)
            {
                UpdateCorrectInputs(false);
                inputGiven = true;
            }

            else
            {
                correctInputGiven = true;

                if (nextMove.inputs[inputIndex].isContinued == false && nextMove.inputs[inputIndex].isRepeated == false)
                {
                    UpdateCorrectInputs(true);
                }
                //Debug.Log("Correct!");
            }
           
        }

        if(nextMove.inputs[inputIndex].isRepeated == true && axisName == nextMove.inputs[inputIndex].inputName && axisValue == nextMove.inputs[inputIndex].axisValue)
        {
            repeatedPressed = true;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        expectingInput = true;
        if(nextMove.inputs[inputIndex].isContinued || nextMove.inputs[inputIndex].isRepeated)
        {
            prolongedInputUncheckedTime = manager.inputObject.GetComponent<BoxCollider2D>().size.x / (manager.timelineSpeedX * 50);
            repeatedPressed = false;
            Invoke("startCheckingContinued", manager.distanceBetweenBeats / (manager.timelineSpeedX * 50));
            if(nextMove.inputs[inputIndex].isRepeated)
            {
                Invoke("StartCheckingRepeated", uncheckedTimeReset);
            }

            ButtonController buttonController = collision.GetComponent<ButtonController>();
            buttonController.buttonSprite.transform.position = transform.position;
            manager.moveOppositeToTimeline.Add(buttonController.buttonSprite.transform);
            if (nextMove.inputs[inputIndex].isContinued)
            {
                buttonController.SetAnimation("ContinuedPressed");
            }
        }

        if (inputIndex == 0)
        {
            _animationManager.SetPlayerState(nextMove.animationName);
            if (manager.phase > 1)
            {
                _animationManager.SetEnemyState("Passive");
            }

            else
            {
                _animationManager.SetEnemyState(nextMove.animationName);
            }
        }
    }


    void startCheckingContinued()
    {
        StartCoroutine("ProlongedInputCheck");
    }

    void StartCheckingRepeated()
    {
        StartCoroutine("RepeatedButtonCheck");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(nextMove.inputs[inputIndex].isContinued == true || nextMove.inputs[inputIndex].isRepeated == true)
        {
            if(prolongedInputUncheckedTime >= 0)
            {
                prolongedInputUncheckedTime -= Time.deltaTime;                
            }

            else
            {
                if(nextMove.inputs[inputIndex].isContinued == true)
                {
                    if (nextMove.inputs[inputIndex].axisValue != 0)
                    {
                        if (Input.GetAxisRaw(nextMove.inputs[inputIndex].inputName) != nextMove.inputs[inputIndex].axisValue)
                        {
                            prolongedMiss = true;
                        }
                    }

                    else
                    {
                        if (Input.GetButton(nextMove.inputs[inputIndex].inputName) == false)
                        {
                            prolongedMiss = true;
                        }
                    }
                }                
            }
        }
    }

    IEnumerator ProlongedInputCheck()
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

    IEnumerator RepeatedButtonCheck()
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

    private void OnTriggerExit2D(Collider2D collision)
    {       
        expectingInput = false;

        if(nextMove.inputs[inputIndex].isContinued == true || nextMove.inputs[inputIndex].isRepeated == true)
        {
            ProlongedExitLogic();
        }

        if (!correctInputGiven && !inputGiven)
        {
            //Debug.Log("Mistake!");
            UpdateCorrectInputs(false);
        }

        correctInputGiven = false;
        inputGiven = false;

        if(inputIndex < nextMove.inputs.Count - 1)
        {
            inputIndex += 1;
        }

        else
        {
            OnComboCompletion(playerPhase);
            playerPhase = !playerPhase;
        }
        
        ButtonController buttonController = collision.gameObject.GetComponent<ButtonController>();
        buttonController.SetAnimation("Idle");
        manager.moveOppositeToTimeline.Remove(buttonController.buttonSprite.transform);
    }

    void ProlongedExitLogic()
    {
        StopCoroutine("RepeatedButtonCheck");
        StopCoroutine("ProlongedInputCheck");
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

    void UpdateCorrectInputs(bool increase)
    {
        if (increase == true)
        {
            correctInputsTotal += 1;
            manager.feedbackManager.OnComboInput(true);
        }

        else
        {
            mistakesTotal += 1;
            manager.feedbackManager.OnComboInput(false);
        }
       
    }

    void OnComboCompletion(bool _playerPhase)
    {
        Debug.Log("Correct inputs " + correctInputsTotal);
        Debug.Log("Wrong inputs " + mistakesTotal);        

        float comboSuccessPercentage;

        if(mistakesTotal >= Mathf.RoundToInt(1.0f * nextMove.inputs.Count / 2))
        {
            comboSuccessPercentage = 0;
        }

        else
        {
            comboSuccessPercentage = 1 - (1.0f * mistakesTotal / nextMove.inputs.Count);
        }

        manager.AssignComboPoints(_playerPhase, comboSuccessPercentage, nextMove);

        if(_playerPhase == true)
        {
            manager.player.UpdateBuffDuration();
        }

        else
        {
            manager.opponent.UpdateBuffDuration();
        }
        ApplyComboModifier();
        _animationManager.SetCombatIdle();
    }

    void ApplyComboModifier()
    {
        if (nextMove.efficiencyModifier != 0)
        {
            if (nextMove.playerIsTarget == true)
            {
                manager.player.ChangeEfficiency(nextMove.efficiencyModifier, nextMove.lastsForTurns);
            }

            else
            {
                manager.opponent.ChangeEfficiency(nextMove.efficiencyModifier, nextMove.lastsForTurns);
            }
        }

        manager.feedbackManager.UpdateEfficiencyStatus();
    }
}
