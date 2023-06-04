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
    private bool _status; //Булевая переменная здесь мы храним статус пирата активен он или нет, очень важна при рекурсивном поиске пиратов, в случае если мы уничтожили у выбранного пирата соседнего пирата, или опустили верхних пиратов она просигнализирует о приостановке поиска
    [SerializeField] private Image _pirateIcon; // Переменная иконки пирата
    private float _coordXPirate; //Переменная которая хранит позицию пирата по оси X
    private PirateLine _line; // Ссылка на линию в которой хранится данный пират, через нее будем получать позицию пирата по Y координате
    private Dictionary<LinksPosition, Pirate> _neighboringPirates; // Словарь хранящий соседних пиратов
    private List<Pirate> _matchPirate; // Список который хранит все обнаруженные копии пиратов
    private List<Pirate> _allTopPirates; // Список который хранит всех пиратов сверху
    private List<Pirate> _bottomMatchPirate = new List<Pirate>(); // Список пиратов в которые будут добавляться пираты из _matchPirate, предназначен для хранения пирата до которого будут опускаться верхние пираты при уничтожении.
    public float XPositionPirate //Геттер сеттер пирата возвращающего координату по X оси
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
     } // Переменная хранит код иконки изображения, конкретный номер берется из массива, номер соответствует определенному цвету в массиве
    public enum LinksPosition
    {
        Next,
        Prev,
        Top,
        Bottom

    } //Перечисление с помощью которого мы будем указывать каких соседних пиратов нам нужно будет взять из словаря или добавить в него
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
    private void RunRecursionForMatchPirate() //Метод запускает рекурсию поиска одинаковых пиратов во все стороны, только в этот раз прочесываем список обнаруженных пиратов _matchPirate, который заполнился после первой итерации поиска выбранного пирата
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
    private void RunRecursionInAllDirections(Pirate p) //Метод запускает рекурсию поиска одинаковых объектов во все стороны
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

    } //Рекурсивная функция которая сканирует правого верхнего и левого пирата находя одинаковых и добавляя их в отдельный список
    private void RecurseGetAllTopPirate(Pirate p) //Рекурсивная функция, которая добавляет в список всех пиратов которые находятся над уничтожаемыми пиратами, исключая одинаковых
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
    private void FoundBottomPirateFromMatchPirate() // Из найденных одинаковых пиратов отсекаем лишних, ищем самых нижних пиратов до которого будем опускать верхних пиратов, у нижнего пирата не должно быть такого же пирата слева или справа и обязательно не должно быть снизу
    {
        foreach (var p in _matchPirate)
        {
            Pirate _tmpNextPirate = p.GetLinkPirates(LinksPosition.Next);
            Pirate _tmpPrevPirate = p.GetLinkPirates(LinksPosition.Prev);
            Pirate _tmpBottomPirate = p.GetLinkPirates(LinksPosition.Bottom);

            if (_tmpNextPirate == null || _tmpPrevPirate == null) //!!имеется ошибка!!! когда не до конца выдвинулось
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
    private void RecurseDownTopPirate(Pirate p, int countPirate ,int countControlAndIndex) //Рекурсивный метод опускает всех обнаруженных верхних пиратов из списка _allTopPirates
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
    public void DeletePirate() //Метод уничтожающий все ссылки и вычищающий списки 
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
    public void DownAllTopPirate() //Функция запускает рекурсию которая ищет всех верхних пиратов, затем запускается вторая рекурсия которая как бы опускает верхних пиратов до выбранного (изменяет статус и меняет картинку)
    {
        RecurseGetAllTopPirate(this);//Сначала получаем всех верхних кроме выбранных
        int countControlIndex = 0; // Переменная для контроля рекурсии и получения пирата из листа по индексу
        int countPirate = this._allTopPirates.Count;
        RecurseDownTopPirate(this,countPirate,countControlIndex);

    }
    public void SetLinkPirates(LinksPosition position, Pirate pirate) //Добавить пирата в словарь указывая позицию и ссылку на него
    {
        _neighboringPirates.Add(position, pirate);
    }
    public Pirate GetLinkPirates(LinksPosition position) //Получить ссылку на пирата из словаря по заданной позиции
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
    public void Clear() //Очищает все списки у выбранного пирата после всех операций
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
        RunRecursionInAllDirections(this); //Запускаем во все стороны рекурсия для поиска одинаковых пиратов
        if (_matchPirate.Count == 0) //Если обнаружен один пират просто опустим верхних пиратов
        {
            this.DownAllTopPirate();
        }
        else
        {
            RunRecursionForMatchPirate(); //В противном случае запускаем метод рекурсивного поиска во все стороны у всех обнаруженных пиратов из списка matchPirate
            _matchPirate.Add(this); //Для того чтобы поиск в не
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
