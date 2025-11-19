using Cysharp.Threading.Tasks;
using UnityEngine;


public class BinaryChoice_RoundManager : TXRSingleton<RoundManager>
{
    [SerializeField] private BinaryChoice_Trial[] _trials;
    private int _currentTrial = 0;
    private BinaryChoice_Round _currentRound;

    public async UniTask RunRoundFlow(BinaryChoice_Round round)
    {
        _currentRound = round;
        StartRound();

        while (_currentTrial < _trials.Length)
        {
            await TrialManager.Instance.RunTrialFlow(_trials[_currentTrial]);
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
