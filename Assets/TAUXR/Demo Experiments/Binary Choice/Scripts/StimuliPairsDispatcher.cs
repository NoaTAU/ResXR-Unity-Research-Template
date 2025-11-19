using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StimuliPairsDispatcher
{


    private Queue<StimuliPair> stimuliPairs = new Queue<StimuliPair>();

    public StimuliPairsDispatcher(string folderPath, StimuliOrder order)
    {
        LoadPairs(folderPath, order);
    }

    private void LoadPairs(string folderPath, StimuliOrder order)
    {
        // Validate folderPath
        if (folderPath == null || folderPath == "")
        {
            Debug.LogError("[StimuliPairsDispatcher] LoadPairs: folderPath is null or empty.");
            return;
        }

        // clear any previously loaded pairs
        stimuliPairs.Clear();

        Debug.Log($"[StimuliPairsDispatcher] Loading sprites from folder: {folderPath} with order: {order}");

        // Load all sprites from the specified folder
        List<Sprite> sprites = Resources.LoadAll<Sprite>(folderPath).ToList<Sprite>();

        switch (order)
        {
            case StimuliOrder.RandomOrder:
                {
                    // shuffle the list of sprites (using the TXR ListExtensions)
                    sprites.Shuffle();
                    break;
                }
            case StimuliOrder.FixedOrder:
                {
                    // sort the sprites by name to ensure fixed order
                    sprites = sprites.OrderBy(sprite => sprite.name).ToList();
                    break;
                }
        }

        // create pairs from the ordered sprites list
        for (int i = 0; i < sprites.Count - 1; i += 2)
        {
            StimuliPair pair = new StimuliPair(sprites[i], sprites[i + 1]);
            stimuliPairs.Enqueue(pair);
        }

        Debug.Log($"[StimuliPairsDispatcher] Created {stimuliPairs.Count} stimuli pairs from {sprites.Count} sprites.");
    }

    public bool HasMorePairs()
    {
        return stimuliPairs.Count > 0;
    }

    public StimuliPair GetNextPair()
    {
        return stimuliPairs.Dequeue();
    }
}



public class StimuliPair
///<summary>
///a class representing a pair of stimuli sprites for convinience.
{
    public Sprite stimulusASprite;
    public Sprite stimulusBSprite;

    public StimuliPair(Sprite StimulusASprite, Sprite StimulusBSprite)
    {
        this.stimulusASprite = StimulusASprite;
        this.stimulusBSprite = StimulusBSprite;
    }
}
