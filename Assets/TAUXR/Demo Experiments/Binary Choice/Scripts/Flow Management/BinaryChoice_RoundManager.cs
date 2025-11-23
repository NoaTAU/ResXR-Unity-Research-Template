using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;


public class BinaryChoice_RoundManager : TXRSingleton<BinaryChoice_RoundManager>
{
    [SerializeField] private BinaryChoice_Trial[] _trials;
    private int _currentTrial = 0;
    private BinaryChoice_Round _currentRound;

    private StimuliPairsLoader _stimuliPairsLoader;
    private float _timeBetweenStimuli;
    private float _instructionsDisplayTime;

    public async UniTask RunRoundFlow(BinaryChoice_Round round)
    {
        _currentRound = round;
        StartRound();

        await _currentRound.roundInstructions.ShowForSeconds(_instructionsDisplayTime);

        while (_currentTrial < _trials.Length)
        {
            await BinaryChoice_TrialManager.Instance.RunTrialFlow(_trials[_currentTrial]);
            await BetweenTrialsFlow();
            _currentTrial++;
        }

        EndRound();
    }

    private void StartRound()
    {
        // initialize variables
        _stimuliPairsLoader = new StimuliPairsLoader(_currentRound.stimuliFolderPath, _currentRound.stimuliOrder);
        _timeBetweenStimuli = BinaryChoice_SceneReferencer.Instance.SecondsBetweenStimuli;
        _instructionsDisplayTime = BinaryChoice_SceneReferencer.Instance.instructionsDisplayTime;

        _currentTrial = 0;
        CreateTrials();

        Debug.Log($"[RoundManager] Round {_currentRound.roundName} Started");
    }

    private void EndRound()
    {
        Debug.Log($"[RoundManager] Round {_currentRound.roundName} Ended");
    }

    private async UniTask BetweenTrialsFlow()
    {
        await BinaryChoice_SceneReferencer.Instance.fixationCross.ShowForSeconds(_timeBetweenStimuli);
    }

    private void CreateTrials()
    {
        // create trials for the round, using the stimuli pairs dispatcher

        if (_stimuliPairsLoader == null)
        {
            Debug.LogError("[RoundManager] CreateTrials: stimuliPairsDispatcher is null. Cannot create trials. Make sure it is initialized in StartRound.");
            return;
        }

        List<BinaryChoice_Trial> trialsList = new List<BinaryChoice_Trial>();

        while (_stimuliPairsLoader.HasMorePairs())
        {
            StimuliPair pair = _stimuliPairsLoader.GetNextPair();
            BinaryChoice_Trial trial = new BinaryChoice_Trial(pair);
            trialsList.Add(trial);
        }
        _trials = trialsList.ToArray();

        Debug.Log($"[RoundManager] Created {_trials.Length} trials for round {_currentRound.roundName}");
    }

}
