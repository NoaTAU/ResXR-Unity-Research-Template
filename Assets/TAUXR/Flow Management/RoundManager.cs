using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : TXRSingleton<RoundManager>
{
    [SerializeField] private BinaryChoice_Trial[] _trials;
    private int _currentTrial = 0;
    private BinaryChoice_Round _currentRound;

    private StimuliPairsDispatcher _stimuliPairsDispatcher;
    private FixationCross _fixationCross;
    private float _timeBetweenStimuli;


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
        // initialize variables
        _fixationCross = BinaryChoice_SceneReferencer.Instance.fixationCross;
        _stimuliPairsDispatcher = new StimuliPairsDispatcher(_currentRound.stimuliFolderPath, _currentRound.stimuliOrder);
        _timeBetweenStimuli = BinaryChoice_SceneReferencer.Instance.SecondsBetweenStimuli;

        CreateTrials();

        Debug.Log($"Round {_currentRound.roundName} Started");
    }

    private void EndRound()
    {
        Debug.Log($"Round {_currentRound.roundName} Ended");
    }

    private async UniTask BetweenTrialsFlow()
    {
        await BinaryChoice_SceneReferencer.Instance.fixationCross.ShowForSeconds(_timeBetweenStimuli);
    }

    private void CreateTrials()
    {
        // create trials for the round, using the stimuli pairs dispatcher

        if (_stimuliPairsDispatcher == null)
        {
            Debug.LogError("[RoundManager] CreateTrials: stimuliPairsDispatcher is null. Cannot create trials. Make sure it is initialized in StartRound.");
            return;
        }

        List<BinaryChoice_Trial> trialsList = new List<BinaryChoice_Trial>();

        while (_stimuliPairsDispatcher.HasMorePairs())
        {
            StimuliPair pair = _stimuliPairsDispatcher.GetNextPair();
            BinaryChoice_Trial trial = new BinaryChoice_Trial(pair);
            trialsList.Add(trial);
        }
        _trials = trialsList.ToArray();

        Debug.Log($"[RoundManager] Created {_trials.Length} trials for round {_currentRound.roundName}");
    }
}