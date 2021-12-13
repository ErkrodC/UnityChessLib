using System.Collections.Generic;

namespace UnityChess {
	/// Non-board, non-move-record game state
	public struct GameConditions {
		public static GameConditions NormalStartingConditions = new GameConditions(
			sideToMove: Side.White,
			whiteCanCastleKingside: true,
			whiteCanCastleQueenside: true,
			blackCanCastleKingside: true,
			blackCanCastleQueenside: true,
			enPassantSquare: Square.Invalid,
			halfMoveClock: 0,
			turnNumber: 1
		);
		
		public readonly Side SideToMove;
		public readonly bool WhiteCanCastleKingside;
		public readonly bool WhiteCanCastleQueenside;
		public readonly bool BlackCanCastleKingside;
		public readonly bool BlackCanCastleQueenside;
		public readonly Square EnPassantSquare;
		public readonly int HalfMoveClock;
		public readonly int TurnNumber;
	
		public GameConditions(
			Side sideToMove,
			bool whiteCanCastleKingside,
			bool whiteCanCastleQueenside,
			bool blackCanCastleKingside,
			bool blackCanCastleQueenside,
			Square enPassantSquare,
			int halfMoveClock,
			int turnNumber
		) {
			SideToMove = sideToMove;
			WhiteCanCastleKingside = whiteCanCastleKingside;
			WhiteCanCastleQueenside = whiteCanCastleQueenside;
			BlackCanCastleKingside = blackCanCastleKingside;
			BlackCanCastleQueenside = blackCanCastleQueenside;
			EnPassantSquare = enPassantSquare;
			HalfMoveClock = halfMoveClock;
			TurnNumber = turnNumber;
		}
		
		public GameConditions CalculateEndingConditions(Board endingBoard, IList<HalfMove> halfMovesFromStart) {
			if (halfMovesFromStart.Count == 0) return this;
			
			bool whiteEligibleForCastling = endingBoard.WhiteKing.Position.Equals(5, 1) && !endingBoard.WhiteKing.HasMoved;
			bool blackEligibleForCastling = endingBoard.BlackKing.Position.Equals(5, 8) && !endingBoard.BlackKing.HasMoved;
			int turnNumberAddend = SideToMove == Side.White
				? halfMovesFromStart.Count / 2
				: (halfMovesFromStart.Count + 1) / 2;
			
			return new GameConditions(
				sideToMove: halfMovesFromStart.Count % 2 == 0 ? SideToMove : SideToMove.Complement(),
				whiteCanCastleKingside: whiteEligibleForCastling && endingBoard[8, 1] is Rook { HasMoved: false },
				whiteCanCastleQueenside: whiteEligibleForCastling && endingBoard[1, 1] is Rook { HasMoved: false },
				blackCanCastleKingside: blackEligibleForCastling && endingBoard[8, 8] is Rook { HasMoved: false },
				blackCanCastleQueenside: blackEligibleForCastling && endingBoard[1, 8] is Rook { HasMoved: false },
				enPassantSquare: GetEndingEnPassantSquare(halfMovesFromStart[^1]),
				halfMoveClock: GetEndingHalfMoveClock(halfMovesFromStart),
				turnNumber: TurnNumber + turnNumberAddend
			);
		}
		
		public GameConditions CalculateEndingConditions(Board endingBoard, HalfMove lastHalfMove) {
			bool whiteEligibleForCastling = endingBoard.WhiteKing.Position.Equals(5, 1) && !endingBoard.WhiteKing.HasMoved;
			bool blackEligibleForCastling = endingBoard.BlackKing.Position.Equals(5, 8) && !endingBoard.BlackKing.HasMoved;

			return new GameConditions(
				sideToMove: SideToMove.Complement(),
				whiteCanCastleKingside: whiteEligibleForCastling && endingBoard[8, 1] is Rook { HasMoved: false },
				whiteCanCastleQueenside: whiteEligibleForCastling && endingBoard[1, 1] is Rook { HasMoved: false },
				blackCanCastleKingside: blackEligibleForCastling && endingBoard[8, 8] is Rook { HasMoved: false },
				blackCanCastleQueenside: blackEligibleForCastling && endingBoard[1, 8] is Rook { HasMoved: false },
				enPassantSquare: GetEndingEnPassantSquare(lastHalfMove),
				halfMoveClock: GetNextHalfMoveClock(lastHalfMove, HalfMoveClock),
				turnNumber: TurnNumber + (SideToMove == Side.White ? 0 : 1)
			);
		}

		private int GetEndingHalfMoveClock(IEnumerable<HalfMove> movesFromStart) {
			int endingHalfMoveClock = HalfMoveClock;
			
			foreach (HalfMove halfMove in movesFromStart) {
				endingHalfMoveClock = GetNextHalfMoveClock(halfMove, endingHalfMoveClock);
			}

			return endingHalfMoveClock;
		}
		
		private static int GetNextHalfMoveClock(HalfMove halfMove, int endingHalfMoveClock) {
			return halfMove.Piece is Pawn || halfMove.CapturedPiece
				? 0
				: endingHalfMoveClock + 1;
		}

		// NOTE ending en passant square can be determined from simply the last half move made.
		private static Square GetEndingEnPassantSquare(HalfMove lastHalfMove) {
			Side lastTurnPieceColor = lastHalfMove.Piece.OwningSide;
			int pawnStartingRank = lastTurnPieceColor == Side.White ? 2 : 7;
			int pawnEndingRank = lastTurnPieceColor == Side.White ? 4 : 5;

			Square enPassantSquare = Square.Invalid;
			if (lastHalfMove.Piece is Pawn && lastHalfMove.Move.Start.Rank == pawnStartingRank && lastHalfMove.Move.End.Rank == pawnEndingRank) {
				int rankOffset = lastTurnPieceColor == Side.White ? -1 : 1;
				enPassantSquare = new Square(lastHalfMove.Move.End, 0, rankOffset);
			}

			return enPassantSquare;
		}
	}
}