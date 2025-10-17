using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFight : MonoBehaviour
{
    public FightInfo fightInfo;
    public float currentPoints = 0;
    [HideInInspector]
    public float efficiency = 1;

    List<int> buffDurations = new List<int>();
    List<float> buffValues = new List<float>();

    public void ResetValues()
    {
        currentPoints = 0;
        efficiency = 1;
        buffDurations.Clear();
        buffValues.Clear();
    }
    
    
    public void UpdateBuffDuration()
    {
        for (int i = 0; i < buffDurations.Count; i++)
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

    public void SelectMove(CombatManager manager)
    {
        manager.moveToMake = fightInfo.moves[Random.Range(0, fightInfo.moves.Length)];
    }

    public void AssignPoints(float success, Combo combo)
    {
        currentPoints += (combo.chargeValue - ((combo.chargeValue / 2) * success)) * efficiency;
    }

    public void AssignSecondaryPoints(float success, Combo combo)
    {
        currentPoints += combo.secondaryPointsValue * success;
    }

    public void ChangeEfficiency(float efficiencyModifier, int duration)
    {
        efficiency += efficiencyModifier;
        buffValues.Add(-efficiencyModifier);
        buffDurations.Add(duration);
    }
}
