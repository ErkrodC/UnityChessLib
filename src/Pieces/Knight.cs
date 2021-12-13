using System;

namespace UnityChess {
	public class Knight : Piece {
		public Knight(Square startingPosition, Side owningSide) : base(startingPosition, owningSide) {}
		public Knight(Knight knightCopy) : base(knightCopy) {}

		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			LegalMoves.Clear();
			CheckKnightCircleSquares(board);
		}

		private void CheckKnightCircleSquares(Board board) {
			for (int fileOffset = -2; fileOffset <= 2; fileOffset++) {
				if (fileOffset == 0) continue;

				foreach (int rankOffset in Math.Abs(fileOffset) == 2 ? new[] {-1, 1} : new[] {-2, 2}) {
					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);

					Square enemyKingPosition = OwningSide == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
					if (testSquare.IsValid() && !board.IsOccupiedBySide(testSquare, OwningSide) && Rules.MoveObeysRules(board, testMove, OwningSide) && testSquare != enemyKingPosition)
						LegalMoves.Add(new Movement(testMove));
				}
			}
		}
	}
}