using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class EndOfEventArgs : EventArgs
    {
        public State Status { get; set; }

        public EndOfEventArgs(State status)
        {
            Status = status;
        }
    }
}
