using Cysharp.Threading.Tasks;
using UnityEngine;

public class Museum_RoundManager : TXRSingleton<Museum_RoundManager>
{
    private Museum_Trial[] _trials;
    private int _currentTrial = 0;
    private Museum_Round _currentRound;

    public async UniTask RunRoundFlow(Museum_Round round)
    {
        _currentRound = round;
        StartRound();

        if (_currentRound.roundType == Museum_RoundType.ImagesRating)
        {
            // Show specific instructions before starting image rating trials, in front of the player,
            // because they could be anywhere in the scene after free exploration.
            await PlaceInstructionsInFrontOfPlayer(Museum_SceneReferencer.Instance.endOfExplorationInstructions);
            await Museum_SceneReferencer.Instance.endOfExplorationInstructions.ShowAndWaitForConfirmation(false);

            // Move player to the designated position for the rating task, to ensure consistency.
            await Museum_SceneReferencer.Instance.ratingTaskPlayerPositionMark.WaitForPlayerAsync(false);

            await Museum_SceneReferencer.Instance.ratingInstructions.ShowAndWaitForConfirmation(false);
        }

        while (_currentTrial < _trials.Length) 
        {
            await Museum_TrialManager.Instance.RunTrialFlow(_trials[_currentTrial]);
            await BetweenTrialsFlow();
            _currentTrial++;
        }
        
        if (_currentRound.roundType == Museum_RoundType.ImagesRating)
        {
            Museum_SceneReferencer.Instance.imagesRating.gameObject.SetActive(false); // Ensure the rating component is deactivated after use.
        }
        EndRound();
    }

    private void StartRound()
    {
        // setup round initial conditions.

        InitializeTrialsForThisRound();


    }

    private void InitializeTrialsForThisRound()
    {
        // Initialize trials based on the current round type
        switch (_currentRound.roundType)
        {
            case Museum_RoundType.FreeExploration:
                _trials = new Museum_Trial[]
                {
                    new Museum_Trial { trialType = Museum_RoundType.FreeExploration, _trialDurationInSeconds = _currentRound.durationInSeconds },
                };
                Debug.Log($"[RoundManager] Free Exploration round initialized with 1 trial of {_currentRound.durationInSeconds} seconds.");
                break;

            case Museum_RoundType.ImagesRating:
                int numOfRatingTrials = Museum_SceneReferencer.Instance.imagesRating.GetNumOfImagesToRate();

                _trials = new Museum_Trial[numOfRatingTrials];
                for (int i = 0; i < numOfRatingTrials; i++)
                {
                    _trials[i] = new Museum_Trial { trialType = Museum_RoundType.ImagesRating, _trialDurationInSeconds = 0f }; // Duration is not used for rating trials
                }
                Debug.Log($"[RoundManager] Images Rating round initialized with {numOfRatingTrials} rating trials.");
                break;

            default:
                _trials = new Museum_Trial[0];
                Debug.LogWarning("[RoundManager] Unknown round type. No trials initialized.");
                break;
        }
    }


    private void EndRound()
    {
        // setup end round conditions
    }

    private async UniTask BetweenTrialsFlow()
    {
        await UniTask.Yield();

    }

    private async UniTask PlaceInstructionsInFrontOfPlayer(InstructionsPanel panel)
    {
        PlaceInFrontOfPlayerHead placementHelper = panel.GetComponent<PlaceInFrontOfPlayerHead>();
        if (placementHelper == null)
        {
            Debug.LogWarning($"No PlaceInFrontOfPlayerHead component found on {panel.gameObject.name} instructions panel. Cannot place it in front of player.");
        }
        else
        {
            placementHelper.RepositionNow();
            Debug.Log($"[PlaceInFrontOfPlayerHead]Caller immediately after: {panel.transform.position}");
            await UniTask.Yield();
            Debug.Log($"[PlaceInFrontOfPlayerHead]Caller after 1 frame: {panel.transform.position}");
        }
    }
}
