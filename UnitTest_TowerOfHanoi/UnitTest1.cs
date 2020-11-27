using Microsoft.VisualStudio.TestTools.UnitTesting;
using TowerOfHanoiLibrary;

namespace UnitTest_TowerOfHanoi
{
    [TestClass]
    public class Instantiate_Should
    {
        [TestMethod]
        public void ThrowInvalidHeightException_WhenInvalidRangeofDiscs()
        {
            Assert.ThrowsException<InvalidHeightException>(() => new Towers(0));
            Assert.ThrowsException<InvalidHeightException>(() => new Towers(10));
        }

        [TestMethod]
        public void HaveNumDiscs_MinMoves_WhenCreateSuccefully()
        {
            int discs = 5;
            int minMoves = 31;
            Towers t1 = new Towers(discs);

            Assert.AreEqual(t1.NumberOfDiscs, discs);
            Assert.AreEqual(t1.MinimumPossibleMoves, minMoves);

            t1 = new Towers(3);
            Assert.AreEqual(t1.MinimumPossibleMoves, 7);
        }
    }

    [TestClass]
    public class Move_Should
    {
        [TestMethod]
        public void ThrowsException_WhenFrom_To_RangeInvalid()
        {
            int discs = 3;
            Towers t1 = new Towers(discs);

            Assert.ThrowsException<InvalidMoveException>(() => t1.Move(4, 1));
            Assert.ThrowsException<InvalidMoveException>(() => t1.Move(0, 3));
        }

        [TestMethod]
        public void ThrowsException_WhenFromNToAreSame()
        {
            int discs = 3;
            Towers t1 = new Towers(discs);

            Assert.ThrowsException<InvalidMoveException>(() => t1.Move(1, 1));
        }

        [TestMethod]
        public void ThrowsException_WhenFromTowerHaveNoDiscs()
        {
            int discs = 3;
            Towers t1 = new Towers(discs);

            Assert.ThrowsException<InvalidMoveException>(() => t1.Move(2, 1));
        }

        [TestMethod]
        public void ThrowsException_WhenFromDiscIsGreaterThanToTopDisc()
        {
            int discs = 3;
            Towers t1 = new Towers(discs);
            t1.Move(1, 3);

            // Trying to move 2 on top of 1
            Assert.ThrowsException<InvalidMoveException>(() => t1.Move(1, 3));
        }

    }

    [TestClass]
    public class ToArray_Should
    {
        // Tests - [][] size is 3 & [0][].length = numberOfDiscs
        [TestMethod]
        public void HaveAllDiscsInTowerOne_WhenCreatedSuccessfully()
        {
            int discs = 5;
            Towers t1 = new Towers(discs);
            int[][] towers = t1.ToArray();

            Assert.AreEqual(towers.Length, 3);    // # of towers
            Assert.AreEqual(towers[0].Length, discs); // 1st tower has all discs 
        }
    }

}
