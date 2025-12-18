using Cysharp.Threading.Tasks;
using UnityEngine;

public class Museum_RoundManager : TXRSingleton<Museum_RoundManager>
{
    [SerializeField] private Museum_Trial[] _trials;
    private int _currentTrial = 0;
    private Museum_Round _currentRound;

    public async UniTask RunRoundFlow(Museum_Round round)
    {
        _currentRound = round;
        StartRound();

        while (_currentTrial < _trials.Length)
        {
            await Museum_TrialManager.Instance.RunTrialFlow(_trials[_currentTrial]);
            await BetweenTrialsFlow();
            _currentTrial++;
        }

        EndRound();
    }

    private void StartRound()
    {
        // setup round initial conditions.
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
