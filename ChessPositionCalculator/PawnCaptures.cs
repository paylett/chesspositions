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

        /// <summary>
        /// Builds a lookup array indexed by the encoded pawn distribution.
        /// Only positions with 0-6 pawns per file and ≤ 8 total are populated;
        /// all other indices hold -1.
        /// </summary>
        public static int[] BuildLookup()
        {
            int[] lookup = new int[1 << 24];
            for ( int i = 0; i < lookup.Length; i++ )
                lookup[i] = -1;

            FillLookup( lookup, new int[8], file: 0, total: 0 );
            return lookup;
        }

        private static void FillLookup( int[] lookup, int[] counts, int file, int total )
        {
            if ( file == 8 )
            {
                lookup[Encode( counts )] = MinCaptures( counts );
                return;
            }

            int max = Math.Min( 6, 8 - total );
            for ( int c = 0; c <= max; c++ )
            {
                counts[file] = c;
                FillLookup( lookup, counts, file + 1, total + c );
            }
            counts[file] = 0;
        }

        // Layout: bits [i*3 .. i*3+2] hold the pawn count (0-7) for file i.
        // 8 files × 3 bits = 24 bits, always fits in a non-negative int.

        public static int Encode( int[] whitePawnCount )
        {
            if ( whitePawnCount.Length != 8 )
                throw new ArgumentException( "Expected exactly 8 files." );

            int encoded = 0;
            for ( int i = 0; i < 8; i++ )
            {
                if ( whitePawnCount[i] < 0 || whitePawnCount[i] > 7 )
                    throw new ArgumentOutOfRangeException( $"File {i} count must be 0-7." );

                encoded |= whitePawnCount[i] << ( i * 3 );
            }
            return encoded;
        }

        public static int[] Decode( int encoded )
        {
            int[] counts = new int[8];
            for ( int i = 0; i < 8; i++ )
                counts[i] = ( encoded >> ( i * 3 ) ) & 0b111;
            return counts;
        }

        private static IEnumerable<(int index, T value)> Enumerate<T>( T[] array )
        {
            for ( int i = 0; i < array.Length; i++ )
                yield return (i, array[i]);
        }
    }
}
