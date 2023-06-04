using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PirateLine;
namespace Assets.Script
{
    public class DoulbyLinkedListPirateLine
    {
        private List<PirateLine> _pirateLineList;
        private List<Pirate>_pirateListCurrent;
        private List<Pirate> _pirateListPrev;
        private PirateLine _head;
        private PirateLine _tail;
        private Pirate _tmpCurrentPirate;
        private Pirate _tmpPrevPirate;
        public DoulbyLinkedListPirateLine() { _pirateLineList = new List<PirateLine>(); }
        public void Add(PirateLine _pirate)
        {
            PirateLine node = _pirate;
            if (_head == null)
            {
                _head = node;
                _pirateLineList.Add(_head);

            }
            else
            {
                _tail.NextLine = node;
                node.PrevLine = _tail;
                _pirateListCurrent = node.GetPirateList();
                _pirateListPrev = _tail.GetPirateList();
                for (int i = 0; i < _pirateListCurrent.Count; i++)
                {
                    _tmpCurrentPirate = _pirateListCurrent[i];
                    _tmpPrevPirate= _pirateListPrev[i];
                    _tmpCurrentPirate.SetLinkPirates(Pirate.LinksPosition.Top, _tmpPrevPirate);
                    _tmpPrevPirate.SetLinkPirates(Pirate.LinksPosition.Bottom, _tmpCurrentPirate);
                }
                _pirateLineList.Add(node);
            }
            _tail = node;

        }
        public void RemoveFirst()
        {
            _pirateLineList.First().DeleteLine();
            _pirateLineList.RemoveAt(0);
        }
        public PirateLine First()
        {
            return _pirateLineList.First();
        }

    }
}
