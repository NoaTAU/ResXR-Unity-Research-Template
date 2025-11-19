using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChoicesManager : MonoBehaviour
{
    public Choice choiceA;
    public Choice choiceB;

    public bool hideOnAwake = true;

    private void Awake()
    {
        if (hideOnAwake)
        {
            HideChoices();
        }
    }

    private void HideChoices()
    {
        choiceA.gameObject.SetActive(false);
        choiceB.gameObject.SetActive(false);
    }

    private void ShowChoices()
    {
        choiceA.gameObject.SetActive(true);
        choiceB.gameObject.SetActive(true);
    }

    public async UniTask<string> SetImagesAndWaitForChoice(StimuliPair pair)
    {

        choiceA.SetImage(pair.stimulusASprite);
        choiceB.SetImage(pair.stimulusBSprite);

        ShowChoices();
        float appearanceTime = Time.time;

        Debug.Log($"[ChoicesManager] Choice displayed between {choiceA.GetCurrentImageName()} and {choiceB.GetCurrentImageName()}, waiting for user...");

        // Wait for either choice to be touched and get which one finished first
        int chosenIndex = await UniTask.WhenAny(
            choiceA.WaitForTouch(),
            choiceB.WaitForTouch()
        );

        float reactionTime = Time.time - appearanceTime;

        string chosenImageName = chosenIndex == 0 ? choiceA.GetCurrentImageName() : choiceB.GetCurrentImageName();

        HideChoices();

        Debug.Log($"[ChoicesManager] Chosen Image: {chosenImageName}, Reaction Time: {reactionTime} seconds.");

        return chosenImageName;
    }
}
