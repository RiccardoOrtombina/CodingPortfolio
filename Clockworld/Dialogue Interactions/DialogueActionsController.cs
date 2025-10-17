using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueActionsController : MonoBehaviour
{
    private Transform _transform;
    private Transform _player;
    [SerializeField]
    private GameObject activeModel;
    private Animator _animator;
    private string _currentState = "Idle";
    [SerializeField]
    Vector3 speed = new Vector3(0, 0, 0.1f);

    void Start()
    {
        _transform = this.transform;
        _animator = activeModel.GetComponent<Animator>();
    }

    void MoveToPlayer()
    {
        _player = GManager.gameManager.player.transform;
        if (Vector3.Distance(_transform.position, _player.position) > GManager.gameManager.dialogueDistance)
        {
            StartCoroutine("Move");
        }

        else
        {
            Vector3 lookPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
            _transform.LookAt(lookPosition);
            _player.GetComponent<PlayerDialogueController>().LookAtConversant(transform);
            Invoke("MovementComplete", 0.1f);
        }
    }

    void MovementComplete()
    {
        Debug.Log("Continua");
        Sequencer.Message("Moved");
    }

    void StartFight()
    {
        GManager.gameManager.SwitchToCombat(gameObject);
    }

    public void SetDialogueAnimation(string state)
    {
        _animator.SetBool(_currentState, false);
        _animator.SetBool(state, true);
        _currentState = state;
    }

    IEnumerator Move()
    {
        Vector3 lookPosition = new Vector3(_player.position.x, transform.position.y, _player.position.z);
        SetDialogueAnimation("Walk");
        while (Vector3.Distance(_transform.position, _player.position) > GManager.gameManager.dialogueDistance)
        {
            _transform.LookAt(lookPosition);
            _player.GetComponent<PlayerDialogueController>().LookAtConversant(transform);
            _transform.Translate(speed, Space.Self);
            yield return new WaitForFixedUpdate();
        }
        SetDialogueAnimation("Idle");
        MovementComplete();
    }
}
