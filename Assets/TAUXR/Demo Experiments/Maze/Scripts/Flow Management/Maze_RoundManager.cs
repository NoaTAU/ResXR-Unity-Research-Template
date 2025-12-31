using Cysharp.Threading.Tasks;
using UnityEngine;

public class Maze_RoundManager : TXRSingleton<Maze_RoundManager>
{
    [SerializeField] private Maze_Trial[] _trials;
    private int _currentTrial = 0;
    private Maze_Round _currentRound;

    public async UniTask RunRoundFlow(Maze_Round round)
    {
        _currentRound = round;
        StartRound();

        while (_currentTrial < _trials.Length)
        {
            await Maze_TrialManager.Instance.RunTrialFlow(_trials[_currentTrial]);
            await BetweenTrialsFlow();
            _currentTrial++;
        }

        EndRound();
    }

    private void StartRound()
    {
        // setup round initial conditions.
        _trials = new Maze_Trial[_currentRound.numOfTrials];
    }


    private void EndRound()
    {
        // setup end round conditions
    }

    private async UniTask BetweenTrialsFlow()
    {
        await UniTask.Yield();

    }
}
