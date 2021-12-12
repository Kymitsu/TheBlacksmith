using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class EndOfEventArgs : EventArgs
    {
        public Status Status { get; set; }

        public EndOfEventArgs(Status status)
        {
            Status = status;
        }
    }
}
