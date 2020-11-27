using System;

namespace TowerOfHanoiLibrary
{
    public class InvalidHeightException : Exception
    {
        public InvalidHeightException() : base()
        {        }

        public InvalidHeightException(string message) : base(message)
        {        }
    }

    public class InvalidMoveException : Exception
    {
        public InvalidMoveException() : base()
        { }

        public InvalidMoveException(string message) : base(message)
        { }
    }

}
