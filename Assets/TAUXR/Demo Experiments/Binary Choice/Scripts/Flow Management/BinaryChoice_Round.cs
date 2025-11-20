
using NaughtyAttributes;

[System.Serializable]
public class BinaryChoice_Round
{
    public string roundName = "Round Name";

    [InfoBox("Select how the pairs should be loaded and presented during the experiment.\n" +
             "Random Order: All stimuli images found in the specified folder will be randomly paired and presented.\n" +
             "Fixed Order: Stimuli pairs will be determined by their file names")]
    public StimuliOrder stimuliOrder;

    [InfoBox("Change this field to your desired folder. Stimuli images should be set single sprites in unity import settings, and must be located under a 'Resources' folder to be retrievable on runtime.")]
    public string stimuliFolderPath = "BinaryChoice/StimuliPairs";

    public InstructionsPanel roundInstructions;
}

public enum StimuliOrder
{
    RandomOrder,
    FixedOrder
}
