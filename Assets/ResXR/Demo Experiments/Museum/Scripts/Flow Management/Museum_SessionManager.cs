using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using UnityEngine;

public class Museum_SessionManager : ResXRSingleton<Museum_SessionManager>
{
    [SerializeField] private Museum_Round[] _rounds;
    private int _currentRound;

    private void Start()
    {
        RunSessionFlow().Forget();
    }

    public async UniTask RunSessionFlow()
    {
        StartSession();

        await Museum_SceneReferencer.Instance.welcomeInstructions.ShowAndWaitForConfirmation(false);

        while (_currentRound < _rounds.Length)
        {
            await Museum_RoundManager.Instance.RunRoundFlow(_rounds[_currentRound]);
            await BetweenRoundsFlow();
            _currentRound++;
        }
        PlaceEndInstructionsInFrontOfPlayer();
        await Museum_SceneReferencer.Instance.endInstructions.ShowAndWaitForConfirmation(false);

        EndSession();
    }


    private void StartSession()
    {   

    }


    private void EndSession()
    {  
        //exit
        Application.Quit();
    }

    private async UniTask BetweenRoundsFlow()
    {
        await UniTask.Yield();

    }


    private void PlaceEndInstructionsInFrontOfPlayer()
    {
        PlaceInFrontOfPlayerHead placementHelper = Museum_SceneReferencer.Instance.endInstructions.GetComponent<PlaceInFrontOfPlayerHead>();
        if (placementHelper == null)
        {
            Debug.LogWarning("No PlaceInFrontOfPlayerHead component found on end instructions panel. Cannot place it in front of player.");
        }
        else
        {
            placementHelper.RepositionNow();
        }
    }
}
