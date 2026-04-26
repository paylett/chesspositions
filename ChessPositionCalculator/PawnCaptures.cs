using System;
using System.Collections.Generic;

namespace ChessPositionCalculator
{
    public static class PawnCaptures
    {
        /// <summary>
        /// Returns the minimum number of captures (one-step horizontal pawn moves)
        /// needed to reach a position with at most one white pawn per file.
        ///
        /// whitePawnCount[i] is the number of white pawns on file i (zero-based).
        /// </summary>
        public static int MinCaptures( int[] whitePawnCount )
        {
            int numFiles = whitePawnCount.Length;

            // Collect sorted pawn positions: if file i has k pawns, add i to the list k times.
            // Because we iterate files in order the list is already sorted.
            var positions = new List<int>();
            foreach ( ( int file, int count ) in Enumerate( whitePawnCount ) )
            {
                for ( int p = 0; p < count; p++ )
                    positions.Add( file );
            }

            int numPawns = positions.Count;

            if ( numPawns == 0 )
                return 0;

            if ( numPawns > numFiles )
                throw new ArgumentException(
                    $"Cannot place {numPawns} pawns across {numFiles} files with at most one per file." );

            // dp[i] = minimum total captures to assign the first j pawns (sorted) to j distinct
            // files chosen from 0..numFiles-1, with the j-th pawn assigned to file i.
            //
            // Transition (for the next pawn):
            //   newDp[i] = abs(positions[j] - i) + min(dp[i'] for i' < i)
            //
            // A running minimum as we scan i left-to-right keeps this O(numFiles) per pawn.

            const int Inf = int.MaxValue / 2;

            int[] dp = new int[numFiles];
            for ( int i = 0; i < numFiles; i++ )
                dp[i] = Math.Abs( positions[0] - i );

            for ( int j = 1; j < numPawns; j++ )
            {
                int[] next = new int[numFiles];
                int runMin = Inf;

                for ( int i = 0; i < numFiles; i++ )
                {
                    // The previous pawn must occupy some file i' < i.
                    if ( i > 0 )
                        runMin = Math.Min( runMin, dp[i - 1] );

                    next[i] = ( runMin == Inf ) ? Inf : runMin + Math.Abs( positions[j] - i );
                }

                dp = next;
            }

            int result = Inf;
            foreach ( int v in dp )
                result = Math.Min( result, v );

            return result;
        }

        private static IEnumerable<(int index, T value)> Enumerate<T>( T[] array )
        {
            for ( int i = 0; i < array.Length; i++ )
                yield return (i, array[i]);
        }
    }
}
