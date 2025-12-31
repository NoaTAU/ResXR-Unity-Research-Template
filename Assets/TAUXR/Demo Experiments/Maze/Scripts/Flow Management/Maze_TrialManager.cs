using Cysharp.Threading.Tasks;
using UnityEngine;

public class Maze_TrialManager : TXRSingleton<Maze_TrialManager>
{
    private Maze_Trial _currentTrial;
    private InstructionsPanel _trialStartPanel;
    private float _trialNumberPanelVisibleDuration;
    private Coin _coin;
    private Maze _maze;

    private float fadeToBlackDuration = 1.0f;
    public async UniTask RunTrialFlow(Maze_Trial trial)
    {
        _currentTrial = trial;
        StartTrial();

        await _trialStartPanel.ShowForSeconds(_trialNumberPanelVisibleDuration);
        await _coin.WaitForCoinPickup();


        // all trial flow. Activating and waiting for project specific functionalities.
        await UniTask.Yield();

        EndTrial();
    }

    private void StartTrial()
    {
        // setup trial initial conditions.
    }


    private void EndTrial()
    {
        // setup trial end conditions.
    }

    private async UniTask RotateMaze()
    {
        await TXRPlayer.Instance.FadeViewToColor(Color.black, fadeToBlackDuration);
        _maze.Rotate180Degrees();
        await TXRPlayer.Instance.FadeViewToColor(Color.clear, fadeToBlackDuration);
    }


    private void InitReferences()
    {
        _trialStartPanel = Maze_SceneReferencer.Instance.trialStartPanel;
        _trialNumberPanelVisibleDuration = Maze_SceneReferencer.Instance.trialNumberPanelVisibleDuration;
        _coin = Maze_SceneReferencer.Instance.coin;
        _maze = Maze_SceneReferencer.Instance.maze;
    }
}
