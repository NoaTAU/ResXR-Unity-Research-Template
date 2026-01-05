using UnityEngine;

public class Museum_SceneReferencer : TXRSingleton<Museum_SceneReferencer>
{
    [Header("References")]
    public InstructionsPanelWithConfirmation welcomeInstructions;
    public InstructionsPanelWithConfirmation endInstructions;
    public InstructionsPanelWithConfirmation endOfExplorationInstructions;
    public InstructionsPanelWithConfirmation ratingInstructions;
    public PlayerPositionMark ratingTaskPlayerPositionMark;

    public ImagesRating imagesRating;
}
