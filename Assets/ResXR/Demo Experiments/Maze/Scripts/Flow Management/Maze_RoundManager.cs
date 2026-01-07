using Cysharp.Threading.Tasks;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

public class Maze_RoundManager : ResXRSingleton<Maze_RoundManager>
{
    [SerializeField, ReadOnly] private Maze_Trial[] _trials;
    private int _currentTrial = 0;
    private Maze_Round _currentRound;
    private Maze _maze;
    private float fadeToBlackDuration = 1.0f;
    private InstructionsPanel _trialStartPanel;
    private float _trialNumberPanelVisibleDuration;

    public async UniTask RunRoundFlow(Maze_Round round)
    {
        _currentRound = round;
        StartRound();

        while (_currentTrial < _trials.Length)
        {
            await SetTrialNumberAndShowTrialStartInstructions(); 

            await Maze_TrialManager.Instance.RunTrialFlow(_trials[_currentTrial]);
            await BetweenTrialsFlow();
            _currentTrial++;
        }

        EndRound();
    }

    private void StartRound()
    {
        // setup round initial conditions.
        InitReferences();

        _trials = new Maze_Trial[_currentRound.numOfTrials];
    }


    private void EndRound()
    {
        // setup end round conditions
    }

    private async UniTask BetweenTrialsFlow()
    {
        await RotateMaze();
    }

    private async UniTask SetTrialNumberAndShowTrialStartInstructions()
    {
        _trialStartPanel.SetTitle("Trial #" + (_currentTrial + 1).ToString());
        await _trialStartPanel.ShowForSeconds(_trialNumberPanelVisibleDuration, true);
    }


    private async UniTask RotateMaze()
    {
        await ResXRPlayer.Instance.FadeViewToColor(Color.black, fadeToBlackDuration);
        _maze.Rotate180Degrees();
        await ResXRPlayer.Instance.FadeViewToColor(Color.clear, fadeToBlackDuration);
    }

    private void InitReferences()
    {
        _maze = Maze_SceneReferencer.Instance.maze;
        _trialStartPanel = Maze_SceneReferencer.Instance.trialStartPanel;
        _trialNumberPanelVisibleDuration = Maze_SceneReferencer.Instance.trialNumberPanelVisibleDuration;
    }

}
