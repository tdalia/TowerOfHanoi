using System;
using System.Collections.Generic;
using System.Linq;

namespace TowerOfHanoiLibrary
{
    [Serializable]
    public class Towers
    {
        private int numberOfDiscs;

        private Stack<Disc>[] towers;

        public Towers(int numberOfDics)
        {
            NumberOfDiscs = numberOfDics;
            MinimumPossibleMoves = (int)Math.Pow(2, numberOfDics) - 1;

            towers = new Stack<Disc>[3];
            for(int i=0; i < towers.Length; i++)
            {
                towers[i] = new Stack<Disc>();
            }

            // Initialize firstStack
            int id = numberOfDics;
            for(int i=0; i < numberOfDics; i++)
            {
                towers[0].Push(new Disc(id--));
            }
        }

        #region "Properties"

        public int NumberOfDiscs
        {
            get => numberOfDiscs; 
            internal set
            {
                if (value < 1 || value > 9)
                {
                    throw new InvalidHeightException("Discs should be between 1 and 9");
                }
                numberOfDiscs = value;
            }
        }

        public int NumberOfMoves { get; set; } = 0;
        public bool IsComplete { get; set; } = false;
        public int MinimumPossibleMoves { get; set; }

        #endregion

        #region "Methods"

        public MoveRecord Move(int from, int to)
        {
            if ((to < 1 || to > 3) || (from < 1 || from > 3) )
            {
                throw new InvalidMoveException("Invalid tower number.");
            }
            if (to == from)
            {
                throw new InvalidMoveException("Move Cancelled.");
            }
            if (towers[from-1].Count == 0)
            {
                throw new InvalidMoveException($"Tower {from} is empty.");
            }
            if ((towers[to - 1].Count > 0) && (towers[from - 1].Peek().Id > towers[to - 1].Peek().Id))
            {
                throw new InvalidMoveException($"Top disc of tower {from} is larger than top disc on tower {to}.");
            }

            Disc disc = towers[from - 1].Pop();
            towers[to-1].Push(disc);
            NumberOfMoves++;

            if (towers[2].Count == NumberOfDiscs)
            {
                IsComplete = true;
            }
            MoveRecord moveRecord = new MoveRecord(NumberOfMoves, disc.Id, from, to, this);

            return moveRecord;
        }

        public int[][] ToArray()
        {
            int[][] ids = new int[3][];
            
            for(int i=0; i < ids.Length; i++)
            {
                int[] arr = new int[towers[i].Count];
                //Disc[] discsInTower = towers[i].ToArray();
                // Retrieve all Id's of Disc from Tower
                arr = (towers[i].ToArray()).Select(d => d.Id).ToArray();

                ids[i] = arr;         
            }

            return ids;
        }

        #endregion

    }
}
