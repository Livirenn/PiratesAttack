using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Pirate;

public class PirateLine : MonoBehaviour
{
    [SerializeField] private RectTransform _fieldOfPirateLine;
    [SerializeField] private Pirate pirate_prefab;
    [SerializeField] private List<Sprite> pirates_sprite = new List<Sprite>();
    private PirateLine _nextLine; //Переменная хранит следующую линию пиратов, необходима чтобы прокидывать ссылки на нижних пиратов для каждого пирата из текущей линии
    private PirateLine _prevLine; //Переменная хранит следующую линию пиратов, необходима чтобы прокидывать ссылки на верхних пиратов для каждого пирата из текущей линии
    public const int _COUNT_PIRATE_IN_LINE = 8; //Количество пиратов в линии
    private int _IndexSpriteAndIdColor; // Переменная в которой будет генерироваться рандомное число при создании линии, в этой переменной будет хранится число, по которому будет браться спрайт, это же число будет ColorId для сравнения
    private DoublyLinkedListPirate _piratesList = new DoublyLinkedListPirate(); // Общий список пиратов, при добавлении пирата в этот список ему задаются ссылки на соседних пиратов
    public float speedLine = 10f;
    //public System.Action<Pirate,float, Image> SendPositionAndImageFromPirate;
    public System.Action<Pirate> SendChangedPirateAndAllTopPirate;
    public PirateLine NextLine
    {
        get
        {
            return _nextLine;
        }
        set
        {
            _nextLine = value;
        }
    }
    public PirateLine PrevLine
    {
        get
        {
            return _prevLine;
        }
        set
        {
            _prevLine = value;
        }
    }

    private void Awake()
    {
        CreateLine();
    }
    void Start()
    {
        
    }
    void Update()
    {

    }
    private void CreateLine()
    {
        for (int i = 0; i <= _COUNT_PIRATE_IN_LINE; i++)
        {
            Pirate p = Instantiate(pirate_prefab, _fieldOfPirateLine);
            p.PirateClicked += GetPositionAndImageFromPirate;
            p.SendChangedPirateAndAllTopPirate += SendChangedPirateAndAllTopPirateToGameField;
            p._Line = this;
            _IndexSpriteAndIdColor = Random.Range(0, pirates_sprite.Count);
            //p.setIcon(pirates_sprite[_IndexSpriteAndIdColor]);
            p.PirateIcon = pirates_sprite[_IndexSpriteAndIdColor];
            p.ColorId = _IndexSpriteAndIdColor;
            p.Status = true;
            p._headerStat = false;
            _piratesList.Add(p);
        }
    }
    private void SendChangedPirateAndAllTopPirateToGameField(Pirate p)
    {
        SendChangedPirateAndAllTopPirate?.Invoke(p);
    }
    private void GetPositionAndImageFromPirate(Pirate p, Image img)
    {
        //RectTransform coordXPirate = (RectTransform)p.transform;
        RectTransform Y = (RectTransform)p.transform;
        float coordYPirate = Y.anchoredPosition.y;
        //SendPositionAndImageFromPirate?.Invoke(p,coordYPirate, img);

    }
    public List<Pirate> GetPirateList()
    {
        return _piratesList.GetPirateList();
    }
    public void DeleteLinksTopPirateInLine()
    {
        foreach(Pirate p in _piratesList) { p.DeleteLinksTopPirate(); }
    }
    public void DeleteLine()
    {
        _fieldOfPirateLine = null;
        pirate_prefab = null;
        pirates_sprite.Clear();
        pirates_sprite = null;
        _nextLine= null;
        _prevLine= null;
        SendChangedPirateAndAllTopPirate = null;
        foreach(Pirate p in _piratesList ) { p.DeletePirate(); }
        _piratesList.Clear();
        _piratesList= null;
    }

}
