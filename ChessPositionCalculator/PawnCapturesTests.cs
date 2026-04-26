using System;

namespace ChessPositionCalculator
{
    public static class PawnCapturesTests
    {
        public static void Run()
        {
            Check( "empty board",                   new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0 );
            Check( "already one per file",          new int[] { 1, 1, 1, 0, 0, 0, 0, 0 }, 0 );
            Check( "one excess, adjacent empty",    new int[] { 2, 0, 0 },                1 );
            Check( "pile in middle",                new int[] { 0, 3, 0 },                2 );
            Check( "two piles",                     new int[] { 2, 2, 0, 0 },             4 );
            Check( "all on file 0",                 new int[] { 3, 0, 0, 0 },             3 );
            Check( "all 8 on file 0",               new int[] { 8, 0, 0, 0, 0, 0, 0, 0 }, 28 );
            Check( "spread evenly",                 new int[] { 1, 0, 1, 0, 1, 0, 1, 0 }, 0 );
            Check( "two on each of first two",      new int[] { 2, 2, 0, 0, 0, 0, 0, 0 }, 4 );
        }

        private static void Check( string label, int[] input, int expected )
        {
            int actual = PawnCaptures.MinCaptures( input );
            string status = actual == expected ? "PASS" : "FAIL";
            Console.WriteLine( $"[{status}] {label}: expected={expected}, got={actual}" );
        }
    }
}
