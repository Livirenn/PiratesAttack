using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class Pirate : MonoBehaviour, IPointerClickHandler
{

    public System.Action<Pirate, Image> PirateClicked;
    public System.Action<Pirate> SendChangedPirateAndAllTopPirate;
    private int _colorId;
    public bool _headerStat;
    private bool _status; //������� ���������� ����� �� ������ ������ ������ ������� �� ��� ���, ����� ����� ��� ����������� ������ �������, � ������ ���� �� ���������� � ���������� ������ ��������� ������, ��� �������� ������� ������� ��� ���������������� � ������������ ������
    [SerializeField] private Image _pirateIcon; // ���������� ������ ������
    private float _coordXPirate; //���������� ������� ������ ������� ������ �� ��� X
    private PirateLine _line; // ������ �� ����� � ������� �������� ������ �����, ����� ��� ����� �������� ������� ������ �� Y ����������
    private Dictionary<LinksPosition, Pirate> _neighboringPirates; // ������� �������� �������� �������
    private List<Pirate> _matchPirate; // ������ ������� ������ ��� ������������ ����� �������
    private List<Pirate> _allTopPirates; // ������ ������� ������ ���� ������� ������
    private List<Pirate> _bottomMatchPirate = new List<Pirate>(); // ������ ������� � ������� ����� ����������� ������ �� _matchPirate, ������������ ��� �������� ������ �� �������� ����� ���������� ������� ������ ��� �����������.
    public float XPositionPirate //������ ������ ������ ������������� ���������� �� X ���
    {
        get
        {
            return _coordXPirate;
        }
        set
        {
            _coordXPirate = value;
        }
    }
    public PirateLine _Line
    {
        get
        {
            return _line;
        }
        set
        {
            _line = value;
        }
    }
    public bool PirateIconEnableDisable
    {
        get { return _pirateIcon.enabled; }
        set { _pirateIcon.enabled = value; }
    }
    public Sprite PirateIcon
    {
        get { return _pirateIcon.sprite; }
        set { _pirateIcon.sprite = value; }
    }
    public bool Status
    {
        get
        {
            return _status;
        }
        set
        {
            _status = value;
        }
    }
    [SerializeField] public int ColorId 
    { 
        get
        { 
            return _colorId;
        }
        set
        {
            _colorId = value;
        }
     } // ���������� ������ ��� ������ �����������, ���������� ����� ������� �� �������, ����� ������������� ������������� ����� � �������
    public enum LinksPosition
    {
        Next,
        Prev,
        Top,
        Bottom

    } //������������ � ������� �������� �� ����� ��������� ����� �������� ������� ��� ����� ����� ����� �� ������� ��� �������� � ����
    private void Awake()
    {
        _status = true;
        _neighboringPirates = new Dictionary<LinksPosition, Pirate>();
        _matchPirate = new List<Pirate>();
        _allTopPirates= new List<Pirate>();
    }
    void Start()
    {
    }
    void Update()
    {
        
    }
    //public void setIcon(Sprite icon)
    //{
    //    if(_pirateIcon)
    //    {
    //        _pirateIcon.sprite = icon;
    //    }
    //}
    private void RunRecursionForMatchPirate() //����� ��������� �������� ������ ���������� ������� �� ��� �������, ������ � ���� ��� ����������� ������ ������������ ������� _matchPirate, ������� ���������� ����� ������ �������� ������ ���������� ������
    {

           for(int i = 0; i < _matchPirate.Count;i++)
            {
            RunRecursionInAllDirections(_matchPirate[i]);
            }
        //if (_matchPirate.Count == 1)
        //{
        //    this._status = false;
        //    this._pirateIcon.enabled = false;
        //}

    }
    private void RunRecursionInAllDirections(Pirate p) //����� ��������� �������� ������ ���������� �������� �� ��� �������
    {
        RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Next), LinksPosition.Next);
        RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Prev), LinksPosition.Prev);
        RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Bottom), LinksPosition.Bottom);
        RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Top), LinksPosition.Top);

    }
    private void RecurseMatchPirate(Pirate p, LinksPosition lp)
    { 
        if (p == null || ColorId != p.ColorId || p.Status == false)
        {
            return;
        }
        else
        if (ColorId == p.ColorId & p._headerStat != true)
        {
            _matchPirate.Add(p);
            p.Status = false;
            p.PirateIconEnableDisable = false;

        }
        switch (lp)
        {
            case LinksPosition.Next: { RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Bottom), LinksPosition.Bottom); RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Next), LinksPosition.Next); RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Top), LinksPosition.Top); } return;
            case LinksPosition.Prev: { RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Bottom), LinksPosition.Bottom); RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Prev), LinksPosition.Prev); RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Top), LinksPosition.Top); } return;
            case LinksPosition.Top: { RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Prev), LinksPosition.Prev); RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Next), LinksPosition.Next); RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Top), LinksPosition.Top); } return;
            case LinksPosition.Bottom: { RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Bottom), LinksPosition.Bottom); RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Next), LinksPosition.Next); RecurseMatchPirate(p.GetLinkPirates(LinksPosition.Prev), LinksPosition.Prev); } return;
        }

    } //����������� ������� ������� ��������� ������� �������� � ������ ������ ������ ���������� � �������� �� � ��������� ������
    private void RecurseGetAllTopPirate(Pirate p) //����������� �������, ������� ��������� � ������ ���� ������� ������� ��������� ��� ������������� ��������, �������� ����������
    {
        if (p == null) 
        {
            return; 
        }
        else if (p.Status == false)

        { RecurseGetAllTopPirate(p.GetLinkPirates(LinksPosition.Top)); }
        else
        {
            this._allTopPirates.Add(p);
            RecurseGetAllTopPirate(p.GetLinkPirates(LinksPosition.Top));
        }

    }
    private void FoundBottomPirateFromMatchPirate() // �� ��������� ���������� ������� �������� ������, ���� ����� ������ ������� �� �������� ����� �������� ������� �������, � ������� ������ �� ������ ���� ������ �� ������ ����� ��� ������ � ����������� �� ������ ���� �����
    {
        foreach (var p in _matchPirate)
        {
            Pirate _tmpNextPirate = p.GetLinkPirates(LinksPosition.Next);
            Pirate _tmpPrevPirate = p.GetLinkPirates(LinksPosition.Prev);
            Pirate _tmpBottomPirate = p.GetLinkPirates(LinksPosition.Bottom);

            if (_tmpNextPirate == null || _tmpPrevPirate == null) //!!������� ������!!! ����� �� �� ����� �����������
            {
                if (_tmpBottomPirate == null)
                {
                    _bottomMatchPirate.Add(p);
                }
                else if (_tmpBottomPirate.ColorId != p.ColorId)
                {
                    _bottomMatchPirate.Add(p);
                }
            }
            else if (_tmpNextPirate.ColorId != p.ColorId || _tmpPrevPirate.ColorId != p.ColorId)
            {
                if (_tmpBottomPirate == null)
                {
                    _bottomMatchPirate.Add(p);
                }
                else if (_tmpBottomPirate.ColorId != p.ColorId)
                {
                    _bottomMatchPirate.Add(p);
                }
            }
            else if (_tmpNextPirate.ColorId == p.ColorId & _tmpPrevPirate.ColorId == p.ColorId)
            {
                if (_tmpBottomPirate == null)
                {
                    _bottomMatchPirate.Add(p);
                }
                else if (_tmpBottomPirate.ColorId != p.ColorId)
                {
                    _bottomMatchPirate.Add(p);
                }
            }
        }
    }
    private void RecurseDownTopPirate(Pirate p, int countPirate ,int countControlAndIndex) //����������� ����� �������� ���� ������������ ������� ������� �� ������ _allTopPirates
    {
        if (countControlAndIndex < countPirate) 
        {
            this._allTopPirates[countControlAndIndex].Status = false;
            this._allTopPirates[countControlAndIndex].PirateIconEnableDisable = false;
            p.ColorId = this._allTopPirates[countControlAndIndex].ColorId;
            p.PirateIcon = this._allTopPirates[countControlAndIndex].PirateIcon;
            p.PirateIconEnableDisable = true;
            p.Status = true;
            RecurseDownTopPirate(p.GetLinkPirates(LinksPosition.Top), countPirate, ++countControlAndIndex);
        }
        else
        {
            return;
        }
    }
    public void DeletePirate() //����� ������������ ��� ������ � ���������� ������ 
    {
        PirateClicked = null;
        SendChangedPirateAndAllTopPirate = null;
        _pirateIcon.enabled= false;
        _pirateIcon= null;
        _line = null;
        _status= false;
        _neighboringPirates.Clear();
        _neighboringPirates = null;
        _matchPirate.Clear();
        _matchPirate = null;
        _allTopPirates.Clear();
        _allTopPirates = null;
        _bottomMatchPirate.Clear();
        _bottomMatchPirate = null;

    }
    public void DownAllTopPirate() //������� ��������� �������� ������� ���� ���� ������� �������, ����� ����������� ������ �������� ������� ��� �� �������� ������� ������� �� ���������� (�������� ������ � ������ ��������)
    {
        RecurseGetAllTopPirate(this);//������� �������� ���� ������� ����� ���������
        int countControlIndex = 0; // ���������� ��� �������� �������� � ��������� ������ �� ����� �� �������
        int countPirate = this._allTopPirates.Count;
        RecurseDownTopPirate(this,countPirate,countControlIndex);

    }
    public void SetLinkPirates(LinksPosition position, Pirate pirate) //�������� ������ � ������� �������� ������� � ������ �� ����
    {
        _neighboringPirates.Add(position, pirate);
    }
    public Pirate GetLinkPirates(LinksPosition position) //�������� ������ �� ������ �� ������� �� �������� �������
    {
        if (_neighboringPirates.ContainsKey(position))
        {
            Pirate p = _neighboringPirates[position];
            return p;
        }
        else
        {
            return null;
        }
    }
    public void Clear() //������� ��� ������ � ���������� ������ ����� ���� ��������
    {
      _allTopPirates.Clear();
       _matchPirate.Clear();
       _bottomMatchPirate.Clear();
    }
    public void DeleteLinksTopPirate()
    {
        _neighboringPirates[LinksPosition.Top] = null;
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        this._headerStat = true;
        _pirateIcon.enabled = false;
        //_matchPirate.Add(this);
        this._status = false;
        RunRecursionInAllDirections(this); //��������� �� ��� ������� �������� ��� ������ ���������� �������
        if (_matchPirate.Count == 0) //���� ��������� ���� ����� ������ ������� ������� �������
        {
            this.DownAllTopPirate();
        }
        else
        {
            RunRecursionForMatchPirate(); //� ��������� ������ ��������� ����� ������������ ������ �� ��� ������� � ���� ������������ ������� �� ������ matchPirate
            _matchPirate.Add(this); //��� ���� ����� ����� � ��
            FoundBottomPirateFromMatchPirate();
            foreach (Pirate p in _bottomMatchPirate)
            {
                p.DownAllTopPirate();
            }
        }



        //for(int i = 0; i < _bottomMatchPirate.Count; i++)
        //{
        //    _bottomMatchPirate[i].Clear();
        //}
        for (int i = 0; i < _matchPirate.Count; i++)
        {
            _matchPirate[i].Clear();
        }
        _matchPirate.Clear();
        _bottomMatchPirate.Clear();
        _allTopPirates.Clear();
        _headerStat = false;









        //foreach (var p in _bottomMatchPirate)
        //{
        //SendChangedPirateAndAllTopPirate?.Invoke(this);
        //}

        //RectTransform X = (RectTransform)p.transform;
        //XPositionPirate = X.anchoredPosition.x;
        ////Debug.Log(XPositionPirate);

    }


}
