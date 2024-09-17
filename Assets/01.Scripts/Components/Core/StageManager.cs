using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public List<GameObject> stageList { get; set; } = new List<GameObject>();

    public int stageLevel { get; set; } = 0;
    public Stage currentStage { get; set; }

    public Dictionary<MovementFlag, Vector2> directionsDic = new Dictionary<MovementFlag, Vector2>()
    {
        { MovementFlag.Down, new Vector2(0, -1) },
        { MovementFlag.Up, new Vector2(0, 1) },
        { MovementFlag.Right, new Vector2(1, 0) },
        { MovementFlag.Left, new Vector2(-1, 0) },
        { MovementFlag.LeftUp, new Vector2(-1, 1) },
        { MovementFlag.RightUp, new Vector2(1, 1) }
    };

    new void Awake()
    {
        LoadStages();
        CreateStage();

        InitStageDatas();


        StartCoroutine(ie());
    }

    void CreateStage()
    {
        if (currentStage != null) DestroyImmediate(currentStage);
        currentStage = Instantiate(stageList[stageLevel], Vector2.zero, Quaternion.identity).GetComponent<Stage>();
    }

    public void LoadStages()
    {
        GameObject[] _stages = Resources.LoadAll<GameObject>("Stage");
        foreach (GameObject _prefab in _stages)
        {
            stageList.Add(_prefab);
        }
    }

    IEnumerator ie()
    {
        while (count < 15)
        {
            yield return new WaitForSeconds(0.1f);
            NextStep();
        }
    }

    int count = 0;
    public void NextStep()
    {
        count++;
        foreach (var item in BubbleManager.Instance.bubbleDatasDic)
        {
            Bubble _bubble = BubbleManager.Instance.bubbleDatasDic[item.Key].bubble;

            if (HexagonGridManager.Instance.hexaGridDatasDic.TryGetValue(_bubble.nextStep, out HexaGridData _value))
            {
                Vector2 _nextStep = _value.gridXY;
                //_bubble.transform.position = _nextStep;
                _bubble.transform.DOMove(_nextStep, 0.1f).SetEase(Ease.InOutQuad);
           
            }
        }
        foreach (var item in HexagonGridManager.Instance.hexaGridDatasDic)
        {
            HexagonGridManager.Instance.hexaGridDatasDic[item.Key].slot.RefleshBubbleData();
        }
    }

    public void NextStage()
    {
        if (stageList.Count - 1 > stageLevel)
        {
            Debug.Log("다음 스테이지");
            stageLevel++;

            InitStageDatas();
            CreateStage();
        }
        else Debug.Log("모든 스테이지 클리어");
    }

    void InitStageDatas()
    {
        HexagonGridManager.Instance.SetGrid();
        HexagonGridManager.Instance.SetBubbles();
    }

    public bool IsCloseBubble(GridSlot _slot)
    {
        bool _isClose = false;
        foreach (var _dir in directionsDic)
        {
            // 각 방향에 대해 검사
            if (BubbleManager.Instance.bubbleDatasDic.ContainsKey(_slot.gridXY + _dir.Value))
            {
                Debug.Log(_slot.gridXY);
                _isClose = true;
                break;  // 하나라도 발견되면 종료
            }
        }
        return _isClose;
    }
}
