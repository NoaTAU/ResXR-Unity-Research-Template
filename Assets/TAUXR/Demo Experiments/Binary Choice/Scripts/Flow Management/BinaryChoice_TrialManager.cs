using Cysharp.Threading.Tasks;


namespace TXR.Demos.BinaryChoice
{
    public class TrialManager : TXRSingleton<TrialManager>
    {
        private Trial _currentTrial;

        public async UniTask RunTrialFlow(Trial trial)
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
}