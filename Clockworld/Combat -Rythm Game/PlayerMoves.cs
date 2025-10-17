using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoves : MonoBehaviour
{
    public List<Combo> moves;
    bool moveSelected;
    public Combo selectedMove;
    public float currentEnergyPoints = 0;
    public float efficiency = 1;

    public float superEffectiveMultiplier = 1.2f;
    public float notEffectiveMultiplier = 0.5f;

    List<int> buffDurations = new List<int>();
    List<float> buffValues = new List<float>();

    public void ResetValues()
    {
        currentEnergyPoints = 0;
        efficiency = 1;
        buffDurations.Clear();
        buffValues.Clear();
        DeselectMove();
    }
    
    public void UpdateBuffDuration()
    {
        for(int i = 0; i < buffDurations.Count; i++)
        {
            buffDurations[i] -= 1;
        }

        for (int i = 0; i < buffDurations.Count; i++)
        {
            if (buffDurations[i] <= 0)
            {
                efficiency += buffValues[i];
                buffValues.RemoveAt(i);
                buffDurations.RemoveAt(i);
                i = 0;
            }
        }
    }

    public void SelectRandomMove()
    {
        if(selectedMove == null)
        {
            selectedMove = moves[Random.Range(0, moves.Count)];
        }
    }

    public void SelectMove(string inputName)
    {
        Debug.Log("Scegli mossa");
        
        if(moveSelected == false)
        {
            if(inputName == "A")
            {
                selectedMove = moves[0];
                moveSelected = true;
            }

            else if (inputName == "B")
            {
                selectedMove = moves[1];
                moveSelected = true;
            }

            else if (inputName == "X")
            {
                selectedMove = moves[2];
                moveSelected = true;
            }

            else if (inputName == "Y")
            {
                selectedMove = moves[3];
                moveSelected = true;
            }
        }
    }

    public void DeselectMove()
    {
        moveSelected = false;
        selectedMove = null;
    }

    public void ChangeEfficiency(float efficiencyModifier, int duration)
    {
        efficiency += efficiencyModifier;
        buffValues.Add(-efficiencyModifier);
        buffDurations.Add(duration);
    }

    public void TakeDamage(float damage)
    {
        currentEnergyPoints -= damage;
    }

    public void AssignPoints(float success, Combo combo, string enemyType)
    {
        float typeEfficiency = 1;

        if(enemyType == combo.type)
        {
            typeEfficiency = notEffectiveMultiplier;
        }

        else
        {
            if (enemyType == "A")
            {
                if (combo.type == "C")
                {
                    typeEfficiency = superEffectiveMultiplier;
                }

                else typeEfficiency = 1;
            }

            else if (enemyType == "B")
            {
                if (combo.type == "A")
                {
                    typeEfficiency = superEffectiveMultiplier;
                }

                else typeEfficiency = 1;
            }

            else if (enemyType == "C")
            {
                if (combo.type == "B")
                {
                    typeEfficiency = superEffectiveMultiplier;
                }

                else typeEfficiency = 1;
            }
        }
        
        Debug.Log(combo.chargeValue * success * typeEfficiency * efficiency);
        currentEnergyPoints += combo.chargeValue * success * typeEfficiency * efficiency;
    }
}
