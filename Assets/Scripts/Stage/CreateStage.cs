using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大きさ：幅高さ5以上の奇数
/// </summary>
public class CreateStage
{
    private int _width;
    private int _height;
    private Vector2Int? _startPosition;
    public Vector2Int StartPosition
    {
        get
        {
            return _startPosition.Value;
        }
    }
    private Vector2Int? _goalPosition;
    public Vector2Int GoalPosition
    {
        get
        {
            return _goalPosition.Value;
        }
    }
    private List<Vector2Int> _point = new();

    private CreateStageMethods _methods = new();

    public CreateStage(int width, int height)
    {
        _width = _methods.ToOddNumber(width);
        _height = _methods.ToOddNumber(height);

        SelectMazePoints(_width, _height);
    }

    /// <summary>
    /// _startPositionと_goalPositionを選ぶ
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private void SelectMazePoints(int width, int height)
    {
        int sy, sx, ey, ex;
        int innerFirstIndex = 1;

        sy = UnityEngine.Random.Range(innerFirstIndex, height - 1);
        sx = UnityEngine.Random.Range(innerFirstIndex, width - 1);

        ey = UnityEngine.Random.Range(innerFirstIndex, height - 1);
        ex = UnityEngine.Random.Range(innerFirstIndex, width - 1);

        _startPosition = new Vector2Int(sx, sy);

        while (_methods.IsAdjacent(sy, ey)
            || _methods.IsAdjacent(sx, ex))
        {
            ey = UnityEngine.Random.Range(innerFirstIndex, height - 1);
            ex = UnityEngine.Random.Range(innerFirstIndex, width - 1);
        }

        _goalPosition = new Vector2Int(ex, ey);
    }

    public StageBlockData[,] Create(int pointCount)
    {
        var s = new StageBlockData[_height, _width];

        // 周りをWallに
        SetStageBlockData(datas: s);
        // pointをpointCount分作成
        SetPoint(pointCount);
        // startからpointをすべて通りgoalまでの道を作成
        CreateShortestPath(datas: s);
        // 壁を作成
        CreateWall(datas: s);
        // 道を作成
        CreatePath(datas: s);
        return s;
    }

    /// <summary>
    /// 周りのTypeをWallにしてそれ以外のTypeをNoneにする
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    private void SetStageBlockData(StageBlockData[,] datas)
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var xWall = x == 0 || x == _width - 1;
                var yWall = y == 0 || y == _height - 1;

                if (xWall || yWall)
                {
                    datas[y,x].SetStageBlockType(StageBlockType.Wall);
                }
                else
                {
                    datas[y,x].SetStageBlockType(StageBlockType.None);
                }
            }
        }
    }

    /// <summary>
    /// countの数だけポイントを作る
    /// </summary>
    /// <param name="count"></param>
    private void SetPoint(int count)
    {
        if (_startPosition is null || _goalPosition is null) return;

        for (int i = 0; i < count; i++)
        {
            Vector2Int point = CreatePointValue();

            while (_point.Contains(point))
            {
                point = CreatePointValue();
            }

            _point.Add(point);
        }
    }

    /// <summary>
    /// スタートとゴールの周りにいないようにする
    /// </summary>
    /// <returns></returns>
    private Vector2Int CreatePointValue()
    {
        int firstPointIndex = 2;

        int x = UnityEngine.Random.Range(firstPointIndex, _width - firstPointIndex);
        int y = UnityEngine.Random.Range(firstPointIndex, _height - firstPointIndex);

        while (_methods.IsAdjacent(_startPosition.Value.x, x)
            || _methods.IsAdjacent(_startPosition.Value.y, y)
            || _methods.IsAdjacent(_goalPosition.Value.x, x)
            || _methods.IsAdjacent(_goalPosition.Value.y, y))
        {
            x = UnityEngine.Random.Range(firstPointIndex, _width - firstPointIndex);
            y = UnityEngine.Random.Range(firstPointIndex, _height - firstPointIndex);
        }

        return new Vector2Int(x, y);
    }

    /// <summary>
    /// pointをすべて通るルートを作成（最短）
    /// </summary>
    /// <param name="datas"></param>
    private void CreateShortestPath(StageBlockData[,] datas)
    {
        Vector2Int p = _startPosition is null ? 
            Vector2Int.zero : (Vector2Int)_startPosition;
        _point = _methods.Shuffle<Vector2Int>(_point);

        datas[p.y, p.x].SetStageBlockType(StageBlockType.Path);

        for (int index = 0; index < _point.Count; index++)
        {
            Vector2Int target = _point[index];

            int x = p.x;
            int y = p.y;

            // 横方向
            while (x != target.x)
            {
                datas[y,x].SetStageBlockType(StageBlockType.Path);

                x += x < target.x ? 1 : -1;
            }

            // 縦方向
            while (y != target.y)
            {
                datas[y,x].SetStageBlockType(StageBlockType.Path);

                y += y <target.y ? 1 : -1;
            }

            // 到着
            datas[target.y, target.x]
                .SetStageBlockType(StageBlockType.Path);

            p = target;
        }

        // ゴールまでの道を作る
        int goalx = p.x;
        int goaly = p.y;

        while (goalx != _goalPosition.Value.x)
        {
            datas[goaly, goalx].SetStageBlockType(StageBlockType.Path);

            goalx += goalx < _goalPosition.Value.x ? 1 : -1;
        }

        while (goaly != _goalPosition.Value.y)
        {
            datas[goaly,goalx].SetStageBlockType(StageBlockType.Path);

            goaly += goaly < _goalPosition.Value.y ? 1 : -1;
        }

        datas[_goalPosition.Value.y, _goalPosition.Value.x]
            .SetStageBlockType(StageBlockType.Path);
    }

    /// <summary>
    /// 壁を作る
    /// </summary>
    /// <param name="datas"></param>
    private void CreateWall(StageBlockData[,] datas)
    {
        // 偶数位置をリストで取得（基準位置）
        List<Vector2Int> evenPosition = 
            _methods.GetEvenPosition(datas);

        while(evenPosition.Count is not 0)
        {
            // ランダムな位置のIndexを取得
            int randomIndex = 
                UnityEngine.Random.Range(0, evenPosition.Count);

            // 偶数のランダムな位置を取得
            Vector2Int position = evenPosition[randomIndex];

            // ランダムな位置が何かしらあるならevenPositionから除外して戻す
            if (datas[position.y, position.x].BlockType
                is not StageBlockType.None)
            {
                evenPosition.RemoveAt(randomIndex);
                continue;
            }

            // 基準位置を壁に
            datas[position.y, position.x]
                .SetStageBlockType(StageBlockType.Wall);

            // ランダムな方向を取得
            Vector2Int dir;

            while (_methods.TryGetRandomMovableDirection
            (
                datas,
                position,
                out dir
            ))
            {
                // 1マス先
                Vector2Int nextPosition = position + dir;                
                // 2マス先
                Vector2Int nextToNextPosition = nextPosition + dir;

                // 1マス先を壁にする
                datas[nextPosition.y, nextPosition.x]
                    .SetStageBlockType(StageBlockType.Wall);                
                // 2マス先を壁にする
                datas[nextToNextPosition.y, nextToNextPosition.x]
                    .SetStageBlockType(StageBlockType.Wall);

                position = nextToNextPosition;
            }

            // 基準位置をevenPositionから除外
            evenPosition.RemoveAt(randomIndex);
        }
    }

    /// <summary>
    /// 道を作る
    /// </summary>
    /// <param name="datas"></param>
    private void CreatePath(StageBlockData[,] datas)
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                if (datas[y,x].BlockType is StageBlockType.None)
                {
                    datas[y,x].SetStageBlockType(StageBlockType.Path);
                }
            }
        }
    }
}

public enum DirectionType
{
    None,
    Left,
    Right,
    Up,
    Down,
    Max,
}