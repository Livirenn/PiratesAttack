using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using Assets.Script;
using System.Linq;

public class GameField : MonoBehaviour
{
    [SerializeField] private RectTransform _gameField; // RectTransform �������� ����
    [SerializeField] private PirateLine _pirateLinePrefab; // ������ ����� � ��������
    private RectTransform _lineRectTr; // ���������� ��� �������������� ���������� Pirate Line � ��������� RectTransform - �
    private RectTransform _lastSpawnRectTr; // ���������� ��� �������� ��������� ��������� �����
    private List<RectTransform> _pirateLineRectTransformList; // ������ ���������� �����
    private DoulbyLinkedListPirateLine _piratesLineList = new DoulbyLinkedListPirateLine();
    private float _speed = 100f; // �������� ����������� �����
    private float _spawnPosition; // ��� ����������� ��������� ��������� ����� ������ ����� ��������� ����� �����
    private Vector2 _targetPosition; // ����� �� ������� ����� ��������� ����� (��� ��������� ���������� �� ���� ��� ���� �� ���� ������ ������ ����� ��� ����� ����� ����������)
    private Vector2 _startPosition; // ��������� ������� ������ �����
    private int _countLine = 0; // ���������� ���������� ��������� �����
    // Start is called before the first frame update
    void Start()
    {
        _pirateLineRectTransformList = new List<RectTransform>();
        _targetPosition = new Vector2(0.0f, Screen.height);
        _spawnPosition = 152.0f;
        CreateLine();
    }
    // Update is called once per frame
    void Update()
    {
        SpawnLineControl();
        MoveLine();
        GameOverControl();
    }
    private void GameOverControl()
    {
        RectTransform tmpLastRctr = _pirateLineRectTransformList.First();

        if(tmpLastRctr.anchoredPosition.y > _targetPosition.y-100)
        {
            _piratesLineList.RemoveFirst();
            _pirateLineRectTransformList.RemoveAt(0);
            _piratesLineList.First().DeleteLinksTopPirateInLine();
        }
    }
    private void MoveLine()
    {
        foreach (RectTransform _lineRecTr in _pirateLineRectTransformList)
        {
            _lineRecTr.anchoredPosition = Vector2.MoveTowards(_lineRecTr.anchoredPosition, _targetPosition, Time.deltaTime * _speed);
        }
    }

    private void CreateLine()
    {
        PirateLine pl= Instantiate(_pirateLinePrefab, _gameField);
        //pl.SendPositionAndImageFromPirate += SpawnImage;
        pl.SendChangedPirateAndAllTopPirate += SpawnImage;
        //pl.SetYPositionPirate = _countLine;
        _countLine++;
        _lineRectTr = (RectTransform)pl.transform;
        _lastSpawnRectTr = _lineRectTr;
        _startPosition = new Vector2(0.0f,28.0f);
        _lineRectTr.anchoredPosition = _startPosition;
        _pirateLineRectTransformList.Add(_lineRectTr);
        _piratesLineList.Add(pl);
    }
    private void SpawnLineControl()
    {
        if(_lastSpawnRectTr.anchoredPosition.y > _spawnPosition)
        {
            CreateLine();
        }
    }
    private void SpawnImage(Pirate p)
    {
        RectTransform X = (RectTransform)p.transform;
        p.XPositionPirate = X.anchoredPosition.x;
        //RectTransform Y = (RectTransform)p.getLine().transform;
        //Debug.Log(Y.anchoredPosition.y);
        //Debug.Log(p.XPositionPirate);
    }


}
