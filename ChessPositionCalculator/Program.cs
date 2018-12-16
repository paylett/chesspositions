using System;
using System.Numerics;

namespace ChessPositionCalculator
{
    class Program
    {
        static void Main( string [ ] args )
        {
            Func<Available, BigInteger> fn = x => 1;

            fn = Pawns( fn ); // pawn
            fn = BlackAndWhite( GenericPiece( 1 ), fn ); // queen
            fn = BlackAndWhite( GenericPiece( 2 ), fn ); // knight
            fn = BlackAndWhite( GenericPiece( 1, 32 ), fn ); // black-square bishop
            fn = BlackAndWhite( GenericPiece( 1, 32 ), fn ); // white-square bishop
            fn = KingsAndRooks( fn );

            Available available = new Available { BlackSquares = 32, WhiteSquares = 32 };
            // 2* - white or black to play
            BigInteger result = 2 * fn( available );

            Console.WriteLine( result );
            Console.WriteLine( result.ToString().Length + " digits" );
            Maths.Display( result, 10 );
            Maths.Display( result, 2 );
        }

        static Func<Available, BigInteger> KingsAndRooks( Func<Available, BigInteger> downstream )
        {
            // two rooks, on any of 64 squares.
            return ( Available available ) =>
            {
                BigInteger total = 0;
                for ( int whiteCastleValid = 0; whiteCastleValid <= 2; whiteCastleValid++ )
                {
                    for ( int blackCastleValid = 0; blackCastleValid <= 2; blackCastleValid++ )
                    {
                        // Number of configurations of kings.
                        Func<Available, BigInteger> fn = downstream;
                        fn = Rook( blackCastleValid, fn );
                        fn = Rook( whiteCastleValid, fn );
                        fn = Kings( whiteCastleValid, blackCastleValid, fn );
                        BigInteger subtotal = fn( available );
                        total += subtotal;
                    }
                }
                return total;
            };                
        }

        static Func<Available, BigInteger> Kings( int whiteCastleValid, int blackCastleValid, Func<Available, BigInteger> downstream )
        {
            // Only one position for kings if both players have valid castling options.
            if ( whiteCastleValid != 0 && blackCastleValid != 0 )
                return a => downstream( a.MinusBlack(1).MinusWhite( 1 ) );

            // White king unmoved for all valid casling positions, black anywhere except adjact.
            // White king occupies black square, black king occupies one of 29 of either black/white squares
            if ( whiteCastleValid != 0 && blackCastleValid == 0 )
                return a =>
                    29 * downstream( a.MinusBlack( 1 ).MinusWhite( 1 ) ) +
                    29 * downstream( a.MinusBlack( 2 ) );

            // Black king unmoved for all valid casling positions, white anywhere except adjact.
            // Black king occupies white square, white king occupies one of 29 of either black/white squares
            if ( blackCastleValid != 0 && whiteCastleValid == 0 )
                return a =>
                    29 * downstream( a.MinusWhite( 1 ).MinusBlack( 1 ) ) +
                    29 * downstream( a.MinusWhite( 2 ) );

            // White anywhere, black anywhere except adjacent
            return a =>
                // 2 white king on white corner x 30 black king on non-adjacent white square
                // 12 white king on white edge x 29 black king on non-adjacent white square
                // 18 white king on white middle x 27 black king on non-adjacent white square
                ( 2 * 30 + 12 * 29 + 18 * 27 ) * downstream( a.MinusWhite( 2 ) ) +
                // 2 white king on white corner x 30 black king on non-adjacent black square
                // 12 white king on white edge x 29 black king on non-adjacent black square
                // 18 white king on white middle x 28 black king on non-adjacent black square
                // 2x for opposite.
                2 * ( 2 * 30 + 12 * 29 + 18 * 28 ) * downstream( a.MinusWhite( 1 ).MinusBlack( 1 ) ) +
                // 2 white king on black corner x 30 black king on non-adjacent black square
                // 12 white king on black edge x 29 black king on non-adjacent black square
                // 18 white king on black middle x 27 black king on non-adjacent black square
                ( 2 * 30 + 12 * 29 + 18 * 27 ) * downstream( a.MinusBlack( 2 ) );
        }

        static Func<Available, BigInteger> Rook( int castlingValid, Func<Available, BigInteger> downstream )
        {
            if (castlingValid == 0)
                // one king anywhere, two rooks anywhere
                return GenericPieceTrackBlackAndWhite( 2)(downstream);

            if ( castlingValid == 1 )
                // king fixed, one rook unmoved, one anywhere (or taken) - but could be either rook
                return ( Available available ) =>
                    GenericPieceTrackBlackAndWhite( 1 )( downstream )( available.MinusBlack( 1 ) ) +
                    GenericPieceTrackBlackAndWhite( 1 )( downstream )( available.MinusWhite( 1 ) );

            if ( castlingValid == 2 )
                // king fixed, both rooks unmoved
                return ( Available available ) =>
                    downstream( available.MinusBlack( 1 ).MinusWhite( 1 ) );

            throw new InvalidOperationException( );
        }

        static Func<Available, BigInteger> KingAnywhere( Func<Available, BigInteger> downstream )
        {
            return ( Available available ) =>
            {
                return available.Squares * downstream( available.Minus(1) );
            };
        }

        static Func<Available, BigInteger> Pawns( Func<Available, BigInteger> downstream )
        {
            return (Available available) =>
            {
                BigInteger total = 0;
                // no enpassant
                total += BlackAndWhite( GenericPiece( 8, 32 ), downstream )( available );

                // enpassant
                // (ties up one pawn of player X, and one of play Y positioned to attack)
                // (storing current turn separately means we only have to worry about enpassant
                // on one side).
                // 14* = 6 with attack form either either, 2x at edges with attack from one side only
                // Also makes 3x pawn squares otherwise unavailable
                total += BlackAndWhite( GenericPiece( 8, 29 ), downstream )( available.Minus(2) );
                return total;
            };
        }


        static Func<Func<Available, BigInteger>, Func<Available, BigInteger>> GenericPiece( int pieces, int maxSquares = 64 )
        {
            return (Func<Available, BigInteger> downstream) =>
                ( Available available ) =>
                    CombinationsForPieces( available, pieces, maxSquares, downstream );
        }

        static Func<Available, BigInteger> BlackAndWhite( Func<Func<Available, BigInteger>, Func<Available, BigInteger>> fn, Func<Available, BigInteger> downstream )
        {
            return fn( fn( downstream ) );
        }

        static Func<Available, BigInteger> BuildBlackAndWhite( int pieces, int maxSquares, Func<Available, BigInteger> fn )
        {
            return
                Build( pieces, maxSquares, Build( pieces, maxSquares, fn ) );
        }

        static Func<Available, BigInteger> Build( int pieces, int maxSquares, Func<Available, BigInteger> down )
        {
            return ( Available available ) =>
            {
                return CombinationsForPieces( available, pieces, maxSquares, down );
            };
        }

        static BigInteger CombinationsForPieces( Available available, int pieces, int maxSquares, Func<Available, BigInteger> downStream)
        {
            BigInteger sum = 0;

            for (int i=0; i<= pieces; i++ )
            {
                Available nextAvailable = available.Minus( i );

                int squares = Math.Min( nextAvailable.Squares, maxSquares );
                
                long combinations = Maths.Combinations( squares, pieces );

                BigInteger mult = downStream( nextAvailable );

                BigInteger xy = combinations * mult;

                sum = sum + xy;
            }

            return sum;
        }

        static Func<Func<Available, BigInteger>, Func<Available, BigInteger>> GenericPieceTrackBlackAndWhite( int pieces, int maxSquares = 64 )
        {
            return ( Func<Available, BigInteger> downstream ) =>
                ( Available available ) =>
                    CombinationsForPiecesOnBlackOrWhite( available, pieces, maxSquares, downstream );
        }

        static BigInteger CombinationsForPiecesOnBlackOrWhite( Available available, int pieces, int maxSquares, Func<Available, BigInteger> downStream )
        {
            BigInteger sum = 0;

            // place between 0 and 'pieces' pieces 
            for ( int i = 0; i <= pieces; i++ )
            {
                //for (int white = 0; white <= pieces; white++ )
                //{
                //    int black = i - white;
                //    Available nextAvailable = available.MinusWhite( white ).MinusBlack( black );
                Available nextAvailable = available.Minus( i );

                int squares = Math.Min( nextAvailable.Squares, maxSquares );

                    long combinations = Maths.Combinations( squares, i );

                    BigInteger mult = downStream( nextAvailable );

                    BigInteger xy = combinations * mult;

                    sum = sum + xy;
                //}
            }

            return sum;
        }

    }

}
