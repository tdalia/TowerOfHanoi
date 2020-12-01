using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TowerOfHanoiLibrary
{
    public class MoveRecord
    {
        public int MoveNumber { get; set; }
        public int  DiscNumber { get; set; }
        public int FromPole { get; set; }
        public int ToPole { get; set; }

        public Towers TowerState { get; set; }

        public MoveRecord(int moveNumber, int discNumber, int fromPole, int toPole)
        {
            MoveNumber = moveNumber;
            DiscNumber = discNumber;
            FromPole = fromPole;
            ToPole = toPole;
        }

        public MoveRecord(int moveNumber, int discNumber, int fromPole, int toPole, Towers towers)
        {
            MoveNumber = moveNumber;
            DiscNumber = discNumber;
            FromPole = fromPole;
            ToPole = toPole;
            TowerState = CloneTowerObject(towers);
        }

        // Deep Copy
        private Towers CloneTowerObject(Towers towers)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, towers);
            ms.Position = 0;
            object result = bf.Deserialize(ms);
            ms.Close();
            return (Towers)result;
        }
    }
}
