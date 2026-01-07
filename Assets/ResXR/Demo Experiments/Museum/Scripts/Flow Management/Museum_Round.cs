using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class Museum_Round 
{
    public Museum_RoundType roundType;

    public bool isFreeExploration => roundType == Museum_RoundType.FreeExploration;
    public float durationInSeconds;

}

public enum Museum_RoundType
{
    ImagesRating,
    FreeExploration
}
