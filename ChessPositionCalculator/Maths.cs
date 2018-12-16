using System;
using System.Collections.Generic;
using System.Numerics;

namespace ChessPositionCalculator
{
    public static class Maths
    {
        // x choose y  (x!/y!/(x-y)!
        private static Dictionary<(int, int), long> _combinations = CalcCombinations( );

        // x choose y  (x!/y!/(x-y)!
        public static Dictionary<(int, int), long> _chooseAtMost = CalcChooseAtMost( );

        public static long Combinations( int fromX, int chooseY ) => _combinations [ (fromX, chooseY) ];

        public static long ChooseAtMost( int fromX, int chooseZeroToY ) => _chooseAtMost [ (fromX, chooseZeroToY) ];

        private static Dictionary<(int, int), long> CalcCombinations( )
        {
            var lookup = new Dictionary<(int, int), long>( );

            for ( int x = 1; x <= 64; x++ )
            {
                lookup [ (x, 0) ] = 1;

                for ( int y = 1; y <= 8; y++ )
                {
                    if ( y > x )
                        continue;
                    
                    lookup [ (x, y) ] = CalcComb(x,y);
                }
            }
            return lookup;
        }

        public static long CalcComb(int x, int y)
        {
            long res = 1;
            for ( int i = x; i > x - y; i-- )
            {
                res = res * i;
            }
            for ( int i = 2; i <= y; i++ )
            {
                res = res / i;
            }
            return res;
        }

        private static Dictionary<(int, int), long> CalcChooseAtMost( )
        {
            var lookup = new Dictionary<(int, int), long>( );

            for ( int x = 1; x <= 64; x++ )
            {
                lookup [ (x, 0) ] = 1;

                for ( int y = 1; y <= 8; y++ )
                {
                    if ( y > x )
                        continue;

                    long accum = 0;
                    for ( int i = 0; i <= y; i++ )
                    {
                        long combs = Combinations(x, i);
                        accum += combs;
                    }
                    lookup [ (x, y) ] = accum;
                }
            }
            return lookup;
        }

        public static void Display( BigInteger number, int baseN )
        {
            int add = 0;
            while ( number > int.MaxValue )
            {
                number = number / baseN;
                add++;
            }
            int remainder = ( int ) number;
            double result = add + Math.Log( remainder ) / Math.Log( baseN );
            Console.WriteLine( $"{baseN}^{result:N3}" );
        }


    }
}
