using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAnimationManager : MonoBehaviour
{
    /* Lista degli stati di animator del combattimento
     * Idle
     * JumpDance
     * Piroette
     * BreakDance
     * RobotDance
     * MoonWalk
     */
    private Animator opponentAnimator;
    private Animator playerAnimator;

    private string currentPlayerState;
    private string currentOpponentState;

    private const string _idle = "Idle";

    public void SetFighters(GameObject player, GameObject opponent)
    {
        opponentAnimator = opponent.GetComponent<Animator>();
        playerAnimator = player.GetComponent<Animator>();
    }

    public void SetCombatIdle()
    {
        playerAnimator.SetBool(_idle, true);
        opponentAnimator.SetBool(_idle, true);
        playerAnimator.SetBool(currentPlayerState, false);
        opponentAnimator.SetBool(currentOpponentState, false);
    }

    void ExitIdle(Animator animator)
    {
        animator.SetBool(_idle, false);
    }

    public void SetPlayerState(string state)
    {
        ExitIdle(playerAnimator);
        playerAnimator.SetBool(state, true);
        currentPlayerState = state;
    }

    public void SetEnemyState(string state)
    {
        ExitIdle(opponentAnimator);
        opponentAnimator.SetBool(state, true);
        currentOpponentState = state;
    }
}
