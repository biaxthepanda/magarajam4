using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    
    [SerializeField]
    private GameObject _bloodPrefab;
    
    [SerializeField]
    private Animator _animator;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (_bloodPrefab == null)
            _bloodPrefab = transform.GetChild(0).gameObject;

        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += OnStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= OnStateChange;
    }

    private void OnStateChange(GameManager.GameState state)
    {
        switch (state)
        { 
            case GameManager.GameState.Menu: 
                IdleAnim();
                break;
            case GameManager.GameState.Idle:
                WalkingAnim();
                break;
            case GameManager.GameState.Attacking:
                AttackAnim();
                break;
            case GameManager.GameState.Defending:
                IdleAnim();
                break;
            case GameManager.GameState.Win:
                AttackDoneAnim();
                break;
            case GameManager.GameState.Lose:
                DieAnim();
                break;
               
        }
    }


    private void AttackAnim()
    {
        int i = (int)UnityEngine.Random.Range(0, 2);
        if(i == 0)
        {
            _animator.Play("Attack");
        }
        else
        {
            _animator.Play("AttackSecond");
        }

        //play at the start of Attacking state
        //on animator connect attack and attackidle together
    }
    private void AttackDoneAnim()
    {
        _animator.Play("PuttingSwordBack");
        SoundController.Instance.PlaySFX(SoundController.SoundEffects.SwordIn);
        DOVirtual.DelayedCall(1f, () =>
        {
            _animator.Play("Walk");
            transform.DOMove(transform.right * 10f, 3f).SetEase(Ease.Linear).OnComplete((() =>
            {
                GameManager.Instance.ChangeState(GameManager.GameState.Upgrade);
            }));
        });
    }
    private void WalkingAnim()
    {
        transform.DOKill();
        transform.position = Vector3.zero;
        
         _animator.Play("WalkSecond");
        
        //play at the start of idle scene
    }
    
    private void IdleAnim()
    {
         _animator.Play("Idle");
    }

    private void DieAnim()
    {
        //play at the start of Lose state
        _animator.Play("Death");
        _bloodPrefab.SetActive(true);
        SoundController.Instance.PlaySFX(SoundController.SoundEffects.DamageFirst);
        SoundController.Instance.PlaySFX(SoundController.SoundEffects.Blood);

        DOVirtual.DelayedCall(3f, () => _bloodPrefab.SetActive(false));
        
        DOVirtual.DelayedCall(5f, () => GameManager.Instance.ChangeState(GameManager.GameState.SceneChange));
    }


    public void PlayWalkSound()
    {
        SoundController.Instance.PlaySFX(SoundController.SoundEffects.Walking);
    }
}
