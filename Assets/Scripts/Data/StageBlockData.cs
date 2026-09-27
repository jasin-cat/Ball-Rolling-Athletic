using System;
using UnityEngine;

[System.Serializable]
public struct StageBlockData
{
    public Vector2Int Posision;
    public StageBlockType BlockType;

    public void SetStageBlockType(StageBlockType type)
    {
        if (BlockType == type) return;

        BlockType = type;
    }
}

public enum StageBlockType
{
    None,
    Path,
    Wall,
}