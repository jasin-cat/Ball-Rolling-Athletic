using UnityEngine;

/// <summary>
/// 配列は、[y,x]
/// </summary>
public readonly struct StageData
{
    public readonly StageBlockData[,] StageBlocks;
    public readonly Vector2Int StartPosition;
    public readonly Vector2Int GoalPosition;

    public StageData
    (
        StageBlockData[,] stageBlocks,
        Vector2Int startPosition,
        Vector2Int goalPosition
    )
    {
        StageBlocks = stageBlocks;
        StartPosition = startPosition;
        GoalPosition = goalPosition;
    }
}