using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;

public class WallBlock : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    private bool _isMovementCompleted = true;
    private Vector3 _blockSize = Vector3.one;

    void Start()
    {
        if (_renderer is null)
        {
            _renderer = this.GetComponent<Renderer>();

            if (_renderer is null) return;
        }

        var size = _renderer.bounds.size;

        float x = (float)Math.Round(size.x);
        float y = (float)Math.Round(size.y);
        float z = (float)Math.Round(size.z);

        size = new Vector3(x, y, z);

        _blockSize = size;
    }
    public void SetInitPosition(Vector3 position)
    {
        this.transform.position = position;
    }

    [Button]
    void MoveLeft()
    {
        RollingMovement(DirectionType.Left, 2f, destroyCancellationToken).Forget();
    }

    [Button]
    void MoveRight()
    {
        RollingMovement(DirectionType.Right, 2f, destroyCancellationToken).Forget();
    }

    [Button]
    void MoveUp()
    {
        RollingMovement(DirectionType.Up, 2f, destroyCancellationToken).Forget();
    }

    [Button]
    void MoveDown()
    {
        RollingMovement(DirectionType.Down, 2f, destroyCancellationToken).Forget();
    }

    public async UniTask RollingMovement
    (
        DirectionType type,
        float duration,
        CancellationToken ct
    )
    {
        if (_renderer is null) return;
        if (!_isMovementCompleted) return;

        _isMovementCompleted = false;

        (Vector3 rot, Vector3 move) dir;

        if (!TryGetDirection(
            type,
            out dir))
        {
            return;
        }

        Vector3 startRotation = this.transform.eulerAngles;
        Vector3 startPosition = this.transform.position;

        Vector3 endRotation = dir.rot * 90f;

        Vector3 endPosition =
            Ex.Multiply(dir.move, _blockSize) + startPosition;

        List<UniTask> tasks = new();

        try
        {
            // 回転
            var rotHandle = LMotion.Create
                (
                    Vector3.zero,
                    endRotation,
                    duration
                )
                .BindToEulerAngles(this.transform)
                .ToUniTask(ct);

            // 移動
            var moveHandle = LMotion.Create
                (
                    startPosition,
                    endPosition,
                    duration
                )
                .BindToPosition(this.transform)
                .ToUniTask(ct);

            tasks.Add(rotHandle);
            tasks.Add(moveHandle);

            await tasks;
        }
        catch (OperationCanceledException)
        {
            this.transform.rotation = Quaternion.Euler(endRotation);

            this.transform.position = endPosition;
        }
        finally
        {
            _isMovementCompleted = true;
        }
    }

    private bool TryGetDirection
    (
        DirectionType type, 
        out (Vector3 rot, Vector3 move) dir
    )
    {
        dir = (Vector3.zero, Vector3.zero);

        switch (type)
        {
            case DirectionType.Left: 
                dir = (Vector3.forward, Vector3.left);
                break;
            case DirectionType.Right:
                dir = (Vector3.back, Vector3.right);
                break;
            case DirectionType.Up:
                dir = (Vector3.right, Vector3.forward);
                break;
            case DirectionType.Down:
                dir = (Vector3.left, Vector3.back);
                break;
        }

        if (dir.rot == Vector3.zero 
            && dir.move == Vector3.zero)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}