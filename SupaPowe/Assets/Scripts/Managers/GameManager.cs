using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnGameStateChanged;

    public GameState State { get; private set; }

    [SerializeField]
    private UIController _uiController;
    
    void Start()
    {
        if (_uiController == null)
            _uiController = GameObject.Find("UIController").GetComponent<UIController>();
        
        
        ChangeState(GameState.Menu);
    }
    
    public void ChangeState(GameState newState)
    {
        State = newState;
        
        switch (newState)
        {
            case GameState.Menu:
                Menu();
                break;
            case GameState.Idle:
                Idle();
                break;
            case GameState.Defending:
                break;
            case GameState.Attacking:
                break;
            case GameState.Act:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
                
        }
        
        OnGameStateChanged?.Invoke(newState);
        
        Debug.Log($"New State: {newState}");
    }

    private void Menu()
    {
        _uiController.ChangeCanvasView(UIController.CurrentUI.MainMenu);
        SoundController.Instance.PlayMusic(SoundController.Musics.Menu);
    }
    private void Idle()
    {
        SoundController.Instance.PlayMusic(SoundController.Musics.GameLoop);
    }
    private void Defending()
    {
        //EnemyController.Initialize();
    }
    private void Attacking()
    {
        _uiController.ChangeCanvasView(UIController.CurrentUI.InGame);
    }
    private void Act()
    {
        
    }
    private void Win()
    {
        
    }
    private void Lose()
    {
        
    }


    [Serializable]
    public enum GameState
    {
        Menu = 0,
        Idle = 1, // Idle, then Run
        Defending = 2, // Enemies starts to attack
        Attacking = 3, // Where we'll take inputs
        Act = 4, // Cutting animations etc.
        Win = 5,
        Lose = 6,
    }
}
