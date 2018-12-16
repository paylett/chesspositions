using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPositionCalculator
{
    struct Available
    {
        public int Squares => BlackSquares + WhiteSquares;

        public int BlackSquares;

        public int WhiteSquares;

        public Available MinusBlack( int squares )
        {
            Available res = this;
            res.BlackSquares = res.BlackSquares - squares;
            return res;
        }

        public Available MinusWhite( int squares )
        {
            Available res = this;
            res.WhiteSquares = res.WhiteSquares - squares;
            return res;
        }

        public Available Minus( int squares ) => MinusWhite( squares );
    }
}
