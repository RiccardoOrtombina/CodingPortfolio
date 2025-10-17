using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInputManager : MonoBehaviour
{
    public Marker marker;
    private CombatManager combatManager;
    private PlayerMoves player;
    bool currentAxisReleased = true;
    string pressedAxis;

    void Start()
    {
        combatManager = GetComponent<CombatManager>();
        player = GManager.gameManager.player.GetComponent<PlayerMoves>();
    }

    void Update()
    {                
        if(combatManager.phase == 0)
        {
            return;
        }

        if (combatManager.phase == 1 || combatManager.phase == 3)
        {
            CheckForInput();
        }

        if(combatManager.feedbackManager.moveSelectionPanel.activeInHierarchy)
        {
            CheckMoveSelectionButtons();
        }
    }

    void CheckForInput()
    {
        CheckAllButtons();
        CheckAllAxis();
    }

    void CheckAllButtons()
    {
        if (Input.GetButtonDown("A"))
        {
            marker.ButtonDetected("A");
        }

        if (Input.GetButtonDown("B"))
        {
            marker.ButtonDetected("B");
        }

        if (Input.GetButtonDown("X"))
        {
            marker.ButtonDetected("X");
        }

        if (Input.GetButtonDown("Y"))
        {
            marker.ButtonDetected("Y");
        }

        if (Input.GetButtonDown("RB"))
        {
            marker.ButtonDetected("RB");
        }

        if (Input.GetButtonDown("LB"))
        {
            marker.ButtonDetected("LB");
        }

        if (Input.GetButtonDown("L3"))
        {
            marker.ButtonDetected("L3");
        }

        if (Input.GetButtonDown("R3"))
        {
            marker.ButtonDetected("R3");
        }
    }

    void CheckMoveSelectionButtons()
    {
        if (Input.GetButtonDown("A"))
        {
            player.SelectMove("A");
        }

        if (Input.GetButtonDown("B"))
        {
            player.SelectMove("B");
        }

        if (Input.GetButtonDown("X"))
        {
            player.SelectMove("X");
        }

        if (Input.GetButtonDown("Y"))
        {
            player.SelectMove("Y");
        }
    }

    void CheckAllAxis()
    {
        if(currentAxisReleased)
        {
            if (Input.GetAxisRaw("LeftStickX") != 0)
            {
                marker.AxisDetected("LeftStickX", Input.GetAxisRaw("LeftStickX"));
                pressedAxis = "LeftStickX";
                currentAxisReleased = false;
            }

            if (Input.GetAxisRaw("LeftStickY") != 0)
            {
                marker.AxisDetected("LeftStickY", Input.GetAxisRaw("LeftStickY"));
                pressedAxis = "LeftStickY";
                currentAxisReleased = false;
            }

            if (Input.GetAxisRaw("RightStickX") != 0)
            {
                marker.AxisDetected("RightStickX", Input.GetAxisRaw("RightStickX"));
                pressedAxis = "RightStickX";
                currentAxisReleased = false;
            }

            if (Input.GetAxisRaw("RightStickY") != 0)
            {
                marker.AxisDetected("RightStickY", Input.GetAxisRaw("RightStickY"));
                pressedAxis = "RightStickY";
                currentAxisReleased = false;
            }

            if (Input.GetAxisRaw("ArrowsX") != 0)
            {
                marker.AxisDetected("ArrowsX", Input.GetAxisRaw("ArrowsX"));
                pressedAxis = "ArrowsX";
                currentAxisReleased = false;
            }

            if (Input.GetAxisRaw("ArrowsY") != 0)
            {
                marker.AxisDetected("ArrowsY", Input.GetAxisRaw("ArrowsY"));
                pressedAxis = "ArrowsY";
                currentAxisReleased = false;
            }

            if (Input.GetAxisRaw("Triggers") != 0)
            {
                marker.AxisDetected("Triggers", Input.GetAxisRaw("Triggers"));
                pressedAxis = "Triggers";
                currentAxisReleased = false;
            }
        }

        else
        {
            ManageAxisInput();
        }
        
    }

    void ManageAxisInput()
    {
        if(Input.GetAxisRaw(pressedAxis) == 0)
        {
            currentAxisReleased = true;
        }
    }
}
