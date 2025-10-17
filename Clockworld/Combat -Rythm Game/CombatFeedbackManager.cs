using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatFeedbackManager : MonoBehaviour
{
    public GameObject moveSelectionPanel;
    public Image enemyBar;
    public Image playerBar;
    public Image playerEfficiency;
    public Image enemyEfficiency;
    CombatManager manager;
    public Image inputResultFeedback;

    private void Start()
    {
        enemyBar.fillAmount = 0;
        playerBar.fillAmount = 0;
        manager = GetComponent<CombatManager>();
    }

    public void OnMoveSelectionPhase()
    {
        moveSelectionPanel.SetActive(true);
    }

    public void OnMoveSelected()
    {
        moveSelectionPanel.SetActive(false);
    }

    public void OnBarUpdate(bool player, float currentPoints)
    {
        if (player)
        {
            playerBar.fillAmount = currentPoints / manager.maxPoints;
        }

        else
        {
            enemyBar.fillAmount = currentPoints / manager.maxPoints;
        }

        UpdateEfficiencyStatus();
    }

    public void UpdateEfficiencyStatus()
    {
        if (manager.player.efficiency > 1)
        {
            playerEfficiency.color = Color.green;
        }

        else if (manager.player.efficiency < 1)
        {
            playerEfficiency.color = Color.red;
        }

        else
        {
            playerEfficiency.color = Color.white;
        }

        if (manager.opponent.efficiency > 1)
        {
            enemyEfficiency.color = Color.green;
        }

        else if (manager.opponent.efficiency < 1)
        {
            enemyEfficiency.color = Color.red;
        }

        else
        {
            enemyEfficiency.color = Color.white;
        }
    }

    public void OnComboInput(bool correct)
    {
        if(correct == true)
        {
            inputResultFeedback.color = Color.green;
        }

        else
        {
            inputResultFeedback.color = Color.red;
        }

        Invoke("ResetFeedbackColor", 0.15f);
    }

    void ResetFeedbackColor()
    {
        inputResultFeedback.color = Color.white;
    }
}
