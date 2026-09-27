using UnityEngine;

public class StagePresenter : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] StageView _view;
    // 生成
    private CreateStage _createStage;
    // データ
    private StageData _data = default;

    void Awake()
    {
        SetCreateStage();
    }

    public void SetStageRange(int width, int height)
    {
        _width = width;
        _height = height;
    }

    private void SetCreateStage()
    {
        _createStage = new(_width, _height);
    }

    public void Init(int pointCount)
    {
        if (_createStage is null) return;

        var blocks = _createStage.Create(pointCount);

        _data = new StageData
        (
            blocks,
            _createStage.StartPosition,
            _createStage.GoalPosition
        );
    }

}