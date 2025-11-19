using Cysharp.Threading.Tasks;
using UnityEngine;

public class TrialManager : TXRSingleton<TrialManager>
{
    private BinaryChoice_Trial _currentTrial;
    private ChoicesManager _choicesManager;

    public async UniTask RunTrialFlow(BinaryChoice_Trial trial)
    {
        _currentTrial = trial;

        StartTrial();

        // actual trial flow:
        string chosenImageName = await _choicesManager.SetImagesAndWaitForChoice(_currentTrial.StimuliPair);

        EndTrial();
    }

    private void StartTrial()
    {
        if (_currentTrial == null || _currentTrial.StimuliPair == null)
        {
            Debug.LogError("[TrialManager] StartTrial: currentTrial is null or wasn't initialized with a stimuli pair. Cannot start trial.");
            return;
        }

        // initialize variables
        _choicesManager = BinaryChoice_SceneReferencer.Instance.choicesManager;

        Debug.Log("Trial Started with Stimuli Pair: " +
                   $"{_currentTrial.StimuliPair.stimulusASprite.name} and " +
                   $"{_currentTrial.StimuliPair.stimulusBSprite.name}");
    }


    private void EndTrial()
    {
        Debug.Log("Trial Ended");
    }
}