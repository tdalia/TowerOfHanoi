namespace TowerOfHanoiLibrary
{
    public class MoveRecord
    {
        public int MoveNumber { get; set; }
        public int  DiscNumber { get; set; }
        public int FromPole { get; set; }
        public int ToPole { get; set; }

        public MoveRecord(int moveNumber, int discNumber, int fromPole, int toPole)
        {
            MoveNumber = moveNumber;
            DiscNumber = discNumber;
            FromPole = fromPole;
            ToPole = toPole;
        }
    }
}
