using Cysharp.Threading.Tasks;
using UnityEngine;


public class BinaryChoice_SessionManager : TXRSingleton<BinaryChoice_SessionManager>
{
    private BinaryChoice_Round[] _rounds;
    private int _currentRound;

    private InstructionsPanel generalInstructions;
    private float instructionsDisplayTime;



    //If there is a higher level flow manager, remove this and use his start method
    private void Start()
    {
        RunSessionFlow().Forget();
    }

    public async UniTask RunSessionFlow()
    {
        StartSession();

        await generalInstructions.ShowForSeconds(instructionsDisplayTime);

        while (_currentRound < _rounds.Length)
        {

            await BinaryChoice_RoundManager.Instance.RunRoundFlow(_rounds[_currentRound]);
            await BetweenRoundsFlow();
            _currentRound++;
        }

        EndSession();
    }


    private void StartSession()
    {
        // setup session initial conditions.
        LoadRoundsFromConfig();
        InitializeReferences();

        // Log experiment settings
        Debug.Log($"Total Rounds: {_rounds.Length}\n" +
                  $"Experiment settings:\n" +
                  $"Time between stimuli: {BinaryChoice_SceneReferencer.Instance.SecondsBetweenStimuli}");

        Debug.Log("Session Started");

    }

    private void InitializeReferences()
    {
        generalInstructions = BinaryChoice_SceneReferencer.Instance.generalInstructions;
        instructionsDisplayTime = BinaryChoice_SceneReferencer.Instance.instructionsDisplayTime;
    }


    private void EndSession()
    {
        // setup end session conditions
        Debug.Log("Session Ended");
    }

    private async UniTask BetweenRoundsFlow()
    {
        await UniTask.Yield();
    }

    private void LoadRoundsFromConfig()
    {
        _rounds = BinaryChoice_RoundsConfiguration.Instance.rounds;
    }

}
