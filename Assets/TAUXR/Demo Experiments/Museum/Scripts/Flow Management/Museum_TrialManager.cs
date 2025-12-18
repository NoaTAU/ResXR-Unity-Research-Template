using Cysharp.Threading.Tasks;
using UnityEngine;

public class Museum_TrialManager : TXRSingleton<Museum_TrialManager>
{
    private Museum_Trial _currentTrial;

    public async UniTask RunTrialFlow(Museum_Trial trial)
    {
        StartTrial();

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
}
