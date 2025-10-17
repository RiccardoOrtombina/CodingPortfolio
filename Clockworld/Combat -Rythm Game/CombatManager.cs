using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using PixelCrushers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [Header("Debug")] 
    public Button button;
    [FMODUnity.EventRef]
    public string beatDebugSound;
    
    [Space(10)]
    public GameObject bossMarker;

    [Space(10)] 
    public GameObject playerCombatModel;
    public GameObject[] charactersPrefabs;
    public GameObject inputObject;
    public GameObject[] inputButtons;
    public GameObject prolongedStartSprite;
    public GameObject inputMarker;
    public StudioEventEmitter music;
    private CombatInputManager _combatInputManager;
    [HideInInspector]
    public CombatFeedbackManager feedbackManager;
    private CombatAnimationManager _animationManager;

    //Entrance time non funziona
    public int entranceTime = 3; 
    private int trackBpm = 120;
    //Dura un beat in più del valore assegnato
    public int selectionPauseDuration = 4;
    public int pauseBetweenTurns = 2;
    public NPCFight opponent;
    public PlayerMoves player;
    public float maxPoints = 100;

    //Tempo che trascorre tra un beat e l'altro in secondi
    [HideInInspector]
    public float beatTime;
    public Transform timeline;
    public float distanceBetweenBeats;
    [HideInInspector]
    public float timelineSpeedX;
    Vector3 timelineSpeed;
    bool moveTimeline = false;
    [HideInInspector]
    public List<Transform> moveOppositeToTimeline = new List<Transform>();

    // 0 = pausa, 1 = enemy move, 2 = player move selection, 3 = player move
    [HideInInspector]
    public int phase = 0;
    int beatCounter;
    bool initiated;

    [HideInInspector]
    public Combo moveToMake;

    private EVENT_CALLBACK beatCallback;
    private EventInstance musicInstance;
    public static UnityEvent OnBeatCallback;
    

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr)
    {
        if (type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            OnBeatCallback.Invoke();
        }

        return FMOD.RESULT.OK;
    }

    void OnFMODBeat()
    {
        //TimelinePulse();
        //RuntimeManager.PlayOneShot(beatDebugSound);
        if (initiated == false)
        {
            if (phase == 0)
            {
                beatCounter = pauseBetweenTurns;
            }

            else if (phase == 1)
            {
                opponent.SelectMove(this);                    
                inputMarker.GetComponent<Marker>().SetMove(moveToMake);
                if (moveToMake.secondaryInputs.Length > 0)
                {
                    bossMarker.GetComponent<BossMarker>().SetMove2(moveToMake);
                    PlaceComboObjects2();
                }
                PlaceComboObjects();
                beatCounter = moveToMake.inputBeats[moveToMake.inputBeats.Count-1] + 1;
            }

            else if (phase == 2)
            {
                beatCounter = selectionPauseDuration;
                feedbackManager.OnMoveSelectionPhase();
            }

            else if (phase == 3)
            {
                if(player.selectedMove == null)
                {
                    player.SelectRandomMove();
                }
                feedbackManager.OnMoveSelected();
                moveToMake = player.selectedMove;
                player.DeselectMove();
                inputMarker.GetComponent<Marker>().SetMove(moveToMake);
                PlaceComboObjects();
                beatCounter = moveToMake.inputBeats[moveToMake.inputBeats.Count-1] + 1;
            }

            initiated = true;
        }

        beatCounter -= 1;
        //Debug.Log(beatCounter);
        if (beatCounter == 0)
        {
            ChangePhase();
        }
    }

    private void OnEnable()
    {
        if (OnBeatCallback == null)
        {
            OnBeatCallback = new UnityEvent();
        }
        
        OnBeatCallback.AddListener(OnFMODBeat);
        beatCallback = new EVENT_CALLBACK(BeatEventCallback);
        
        feedbackManager = GetComponent<CombatFeedbackManager>();
        _combatInputManager = GetComponent<CombatInputManager>();
        _animationManager = GetComponent<CombatAnimationManager>();
    }

    public void OnCombatSceneActivation()
    {
        Destroy(button);
        bossMarker.SetActive(false);
        inputMarker.GetComponent<Marker>().Init(this, _animationManager);
        StartFight();
    }

    void StartFight()
    {
        SetCurrentFIghtInfo();
        UpdateBeatData();
        ResetFightValues();
        feedbackManager.OnBarUpdate(true, player.currentEnergyPoints);
        feedbackManager.OnBarUpdate(false, opponent.currentPoints);
        foreach (Combo combo in opponent.fightInfo.moves)
        {
            if (combo.secondaryInputs.Length > 0)
            {
                bossMarker.SetActive(true);
                bossMarker.GetComponent<BossMarker>().Init(this);
            }
        }
        musicInstance = RuntimeManager.CreateInstance(music.Event);
        musicInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        musicInstance.start();
        moveTimeline = true;
    }

    void SetCurrentFIghtInfo()
    {
        music.Event = opponent.fightInfo.music;
        trackBpm = opponent.fightInfo.trackBPM;
        
        foreach (GameObject character in charactersPrefabs)
        {
            if (character.name == opponent.fightInfo.characterPrefab.name)
            {
                character.SetActive(true);
                _animationManager.SetFighters(playerCombatModel, character);
            }

            else
            {
                character.SetActive(false);
            }
        }

        foreach (GameObject inputButton in inputButtons)
        {
            // 120:trackbpm = 1 : x
            inputButton.GetComponent<ButtonController>().buttonSprite.GetComponent<Animator>().speed = trackBpm / 120;
            print(inputButton.GetComponent<ButtonController>().buttonSprite.GetComponent<Animator>().speed);
        }
    }

    void UpdateBeatData()
    {
        beatTime = 60f / trackBpm;
        timelineSpeedX = distanceBetweenBeats / beatTime * 0.02f;
        timelineSpeed = new Vector3(timelineSpeedX, 0, 0);
        beatCounter = entranceTime;
    }

    void ResetFightValues()
    {
        player.ResetValues();
        opponent.ResetValues();
    }

    void ChangePhase()
    {        
        if(phase == 3)
        {
            phase = 0;
        }

        else
        {
            phase += 1;
        }
        //Debug.Log("Phase " + phase);
        initiated = false;
    }

    void PlaceComboObjects()
    {
        int i = 0;

        foreach(ClockworldInput ci in moveToMake.inputs)
        {
            Vector3 distance = new Vector3(moveToMake.inputBeats[i] * distanceBetweenBeats, 0, 0);
            GameObject assignedButton = null;

            foreach (GameObject emptyButton in inputButtons)
            {
                if (emptyButton.activeInHierarchy == false)
                {
                    assignedButton = emptyButton;
                    assignedButton.SetActive(true);
                    break;
                }
            }

            if (assignedButton == null)
            {
                Debug.LogError("Nessun bottone disponibile, controllare CombatManager e aggiungere bottoni");
                return;
            }

            assignedButton.transform.position = inputMarker.transform.position + distance;
            assignedButton.GetComponent<ButtonController>().SetButtonSprite(ci.inputSprite);

            if(ci.isContinued || ci.isRepeated)
            {
                Vector2 newSize = new Vector2(distanceBetweenBeats * moveToMake.inputBeats[i] - distanceBetweenBeats, 0);
                if(i - 1 >= 0)
                {
                    newSize = new Vector2((distanceBetweenBeats * (moveToMake.inputBeats[i] - moveToMake.inputBeats[i - 1] - 1)), 0);
                }
                assignedButton.GetComponent<BoxCollider2D>().offset = -new Vector2(newSize.x / 2, 0);
                assignedButton.GetComponent<BoxCollider2D>().size += newSize;
                assignedButton.GetComponent<ButtonController>().SetSpritesPositions(newSize.x);
                //GameObject sprite = Instantiate(prolongedStartSprite, inputMarker.transform.position + distance - startSpriteDistance, inputMarker.transform.rotation, input.transform);
                if(ci.isContinued == true)
                {
                    assignedButton.GetComponent<ButtonController>().SetAnimation("Continued");
                }

                else
                {
                    assignedButton.GetComponent<ButtonController>().SetAnimation("Repeated");
                }
                
            }

            if (i < moveToMake.inputBeats.Count)
            {
                i += 1;
            }
                
            
            
        }
        
    }
    
    void PlaceComboObjects2()
    {
        int i = 0;

        foreach(ClockworldInput ci in moveToMake.secondaryInputs)
        {
            Vector3 distance = new Vector3(moveToMake.secondaryInputBeats[i] * distanceBetweenBeats, 0, 0);
            GameObject assignedButton2 = null;

            foreach (GameObject emptyButton in inputButtons)
            {
                if (emptyButton.activeInHierarchy == false)
                {
                    assignedButton2 = emptyButton;
                    assignedButton2.SetActive(true);
                    break;
                }
            }

            if (assignedButton2 == null)
            {
                Debug.LogError("Nessun bottone disponibile, controllare CombatManager e aggiungere bottoni");
                return;
            }
            
            assignedButton2.transform.position = bossMarker.transform.position + distance;
            assignedButton2.GetComponent<ButtonController>().SetButtonSprite(ci.inputSprite);
            //GameObject input = Instantiate(inputObject, bossMarker.transform.position + distance, bossMarker.transform.rotation, timeline);
            if(ci.isContinued || ci.isRepeated)
            {
                Vector2 newSize = new Vector2(distanceBetweenBeats * moveToMake.secondaryInputBeats[i] - distanceBetweenBeats, 0);
                if(i - 1 >= 0)
                {
                    newSize = new Vector2((distanceBetweenBeats * (moveToMake.secondaryInputBeats[i] - moveToMake.secondaryInputBeats[i - 1] - 1)), 0);
                }
                //Vector2 newSize = new Vector2((distanceBetweenBeats * (moveToMake.inputBeats[i - 1] - moveToMake.inputBeats[i] - 1)), 0);
                assignedButton2.GetComponent<BoxCollider2D>().offset = -new Vector2(newSize.x / 2, 0);
                assignedButton2.GetComponent<BoxCollider2D>().size += newSize;
                assignedButton2.GetComponent<ButtonController>().SetSpritesPositions(newSize.x);
                
                if(ci.isContinued)
                {
                    assignedButton2.GetComponent<ButtonController>().SetAnimation("Continued");
                }

                else
                {
                    assignedButton2.GetComponent<ButtonController>().SetAnimation("Repeated");
                }
            }

            if (i < moveToMake.inputBeats.Count)
            {
                i += 1;
            }
            
        }
    }

    void TimelinePulse()
    {
        foreach (GameObject button in inputButtons)
        {
            if (button.activeInHierarchy)
            {
                button.GetComponent<ButtonController>().SetAnimation("Pulse");
            }
        }
    }

    private void FixedUpdate()
    {
        if (moveTimeline)
        {
            timeline.position -= timelineSpeed;
        }

        if (moveOppositeToTimeline.Count > 0)
        {
            foreach (Transform buttonToMove in moveOppositeToTimeline)
            {
                buttonToMove.position += timelineSpeed;
            }
        }
        
    }

    public void AssignComboPoints(bool playerPhase, float comboSuccess, Combo combo)
    {
        if (playerPhase == false)
        {
            player.TakeDamage(combo.dischargeValue * (1 - comboSuccess));
            //opponent.AssignPoints(comboSuccess, combo);
            feedbackManager.OnBarUpdate(true, player.currentEnergyPoints);
        }

        else
        {
            player.AssignPoints(comboSuccess, combo, opponent.fightInfo.type);
            feedbackManager.OnBarUpdate(true, player.currentEnergyPoints);
        }

        if(player.currentEnergyPoints >= maxPoints || opponent.currentPoints >= maxPoints)
        {
            if (player.currentEnergyPoints >= maxPoints)
            {
                EndFight(true);
            }
            else
            {
                EndFight(false);
            }
        }
    }

    public void AssignSecondLinePoints(float multiplier, Combo combo)
    {
        opponent.AssignSecondaryPoints(multiplier, combo);
        if (opponent.currentPoints >= maxPoints)
        {
            EndFight(false);
        }
    }

    void EndFight(bool win)
    {
        //StopCoroutine("OnBeat");
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        if (win == true)
        {
            GManager.gameManager.SwitchToOverworld();
        }

        else
        {
            SaveSystem.LoadFromSlot(0);
        }
    }
}
