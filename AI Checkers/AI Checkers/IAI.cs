using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AICheckers
{ 
    interface IAI
    {
        CheckerColour Colour { get; set; }
        KeyValuePair<Square[,], float> MinMax(Square[,] Board);
        KeyValuePair<Square[,], float> MinMax2(Square[,] Board);
    }
}
