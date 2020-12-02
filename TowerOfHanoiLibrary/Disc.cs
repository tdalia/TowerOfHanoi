using System;
using System.Collections.Generic;
using System.Text;

namespace TowerOfHanoiLibrary
{
    [Serializable]
    class Disc
    {
        public Disc(int discId)
        {
            Id = discId;
        }

        public int Id { get; set; }
    }
}
