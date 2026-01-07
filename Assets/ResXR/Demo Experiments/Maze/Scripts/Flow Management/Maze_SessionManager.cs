using Cysharp.Threading.Tasks;
using Meta.WitAi;
using System;
using UnityEngine;

public class Maze_SessionManager : ResXRSingleton<Maze_SessionManager>
{
    [SerializeField] private Maze_Round[] _rounds;
    private int _currentRound;

    private PlayerPositionMark _startingPositionMark;
    private InstructionsPanelWithConfirmation _generalInstructions;
    private InstructionsPanel _endInstructions;

    private float endInstructionsDuration = 5f;


    //If there is a higher level flow manager, remove this and use his start method
    private void Start()
    {
        RunSessionFlow().Forget();
    }

    public async UniTask RunSessionFlow()
    {
        StartSession();
        await _startingPositionMark.WaitForPlayerAsync();
        await _generalInstructions.ShowAndWaitForConfirmation();

        while (_currentRound < _rounds.Length)
        {
            await Maze_RoundManager.Instance.RunRoundFlow(_rounds[_currentRound]);
            await BetweenRoundsFlow();
            _currentRound++;
        }

        await _endInstructions.ShowForSeconds(endInstructionsDuration);

        EndSession();
    }


    private void StartSession()
    {
        // setup session initial conditions.
        InitReferences();
    }


    private void EndSession()
    {
        // setup end session conditions
    }

    private async UniTask BetweenRoundsFlow()
    {
        await UniTask.Yield();

        
    }

    private void InitReferences()
    {
        _startingPositionMark = Maze_SceneReferencer.Instance.startingPositionMark;
        _generalInstructions = Maze_SceneReferencer.Instance.generalInstructions;
        _endInstructions = Maze_SceneReferencer.Instance.endInstructions;
    }

}
