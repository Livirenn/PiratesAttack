using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Pirate;

public class DoublyLinkedListPirate:IEnumerable
{
    private List<Pirate> _pirateList = new List<Pirate>();
    private Pirate _head;
    private Pirate _tail;
    public IEnumerator GetEnumerator() => _pirateList.GetEnumerator();
    public void Add(Pirate _pirate)
    {
        Pirate node = _pirate;
        if (_head == null)
        {
            _head = node;
            _pirateList.Add(_head);

        }
        else
        {
            _tail.SetLinkPirates(LinksPosition.Next, node);
            node.SetLinkPirates(LinksPosition.Prev, _tail);
            _pirateList.Add(node);
        }
        _tail = node;
    }
    public List<Pirate> GetPirateList()
    {
        return _pirateList;
    }
    public void Clear()
    {
        _pirateList.Clear();
        _pirateList = null;
        _head= null;
        _tail= null;
    }


}
