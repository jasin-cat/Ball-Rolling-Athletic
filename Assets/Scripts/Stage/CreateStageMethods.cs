using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateStageMethods
{
    private List<Vector2Int> _directions = new List<Vector2Int>
    {
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up,
        Vector2Int.down
    };

    /// <summary>
    /// targetの側面にreferenceがあるならTrue
    /// </summary>
    /// <param name="target"></param>
    /// <param name="reference"></param>
    /// <returns></returns>
    public bool IsAdjacent(int target, int reference)
    {
        return Mathf.Abs(target) + 1 == Mathf.Abs(reference);
    }

    /// <summary>
    /// 奇数を出す
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public int ToOddNumber(int num)
    {
        return num % 2 is 0 ? num + 1 : num;
    }

    public List<T> Shuffle<T>(List<T> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            var tmp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = tmp;
        }
        return array;
    }

    public List<Vector2Int> GetEvenPosition(StageBlockData[,] datas)
    {
        List<Vector2Int> evenPosition = new();

        for (int y = 1; y < datas.GetLength(0) - 1; y++)
        {
            for (int x = 1; x < datas.GetLength(1) - 1; x++)
            {
                // x,yの両方が偶数でないなら除外
                if (y % 2 is not 0 || x % 2 is not 0) continue;

                evenPosition.Add(new Vector2Int(x,y));
            }
        }
        return evenPosition;
    }

    public bool TryGetRandomMovableDirection
    (
        StageBlockData[,] datas, 
        Vector2Int position, 
        out Vector2Int dir
    )
    {
        dir = Vector2Int.zero;

        List<Vector2Int> MovableDirections = new();

        int height = datas.GetLength(0);
        int width = datas.GetLength(1);

        for (int i = 0; i < _directions.Count; i++)
        {
            Vector2Int nextPosition = position + _directions[i];
            Vector2Int nextToNextPosition = nextPosition + _directions[i];

            // 2マス先が範囲外なら終
            if (nextToNextPosition.x < 0
                || nextToNextPosition.x >= width
                || nextToNextPosition.y < 0
                || nextToNextPosition.y >= height)
            {
                continue;
            }

            // 1マス先が空いているか
            if (datas[nextPosition.y,nextPosition.x].BlockType
                is not StageBlockType.None)
            {
                continue;
            }

            // 2マス先が空いているか
            if (datas[nextToNextPosition.y,nextToNextPosition.x].BlockType
                is not StageBlockType.None)
            {
                continue;
            }

            MovableDirections.Add(_directions[i]);
        }

        if (MovableDirections.Count is 0)
        {
            return false;
        }

        int random = UnityEngine.Random.Range(
            0, MovableDirections.Count);

        dir = MovableDirections[random];

        return true;
    }
}