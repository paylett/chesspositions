using System;

namespace ChessPositionCalculator
{
    public static class PawnCapturesTests
    {
        public static void Run()
        {
            CheckRoundTrip();
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

        private static void CheckCodec( string label, int[] counts )
        {
            int encoded = PawnCaptures.Encode( counts );
            int[] decoded = PawnCaptures.Decode( encoded );

            bool ok = true;
            for ( int i = 0; i < 8; i++ )
                if ( decoded[i] != counts[i] ) { ok = false; break; }

            Console.WriteLine( $"[{( ok ? "PASS" : "FAIL" )}] codec {label}: encoded=0x{encoded:X6}" );
        }

        private static void CheckRoundTrip()
        {
            CheckCodec( "all zeros",          new int[] { 0, 0, 0, 0, 0, 0, 0, 0 } );
            CheckCodec( "all sevens",         new int[] { 7, 7, 7, 7, 7, 7, 7, 7 } );
            CheckCodec( "ascending",          new int[] { 0, 1, 2, 3, 4, 5, 6, 7 } );
            CheckCodec( "single file 3",      new int[] { 0, 0, 0, 3, 0, 0, 0, 0 } );
            CheckCodec( "single file 7",      new int[] { 0, 0, 0, 0, 0, 0, 0, 7 } );
        }

        private static void Check( string label, int[] input, int expected )
        {
            int actual = PawnCaptures.MinCaptures( input );
            string status = actual == expected ? "PASS" : "FAIL";
            Console.WriteLine( $"[{status}] {label}: expected={expected}, got={actual}" );
        }
    }
}
