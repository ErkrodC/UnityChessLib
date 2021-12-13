using System.Collections.Generic;

namespace UnityChess {
	public class King : Piece {
		public King(Square startingPosition, Side owningSide) : base(startingPosition, owningSide) {}
		public King(King kingCopy) : base(kingCopy) {}

		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			LegalMoves.Clear();

			CheckSurroundingSquares(board);
			CheckCastlingMoves(board);
		}

		private void CheckSurroundingSquares(Board board) {
			for (int fileOffset = -1; fileOffset <= 1; fileOffset++) {
				for (int rankOffset = -1; rankOffset <= 1; rankOffset++) {
					if (fileOffset == 0 && rankOffset == 0) continue;

					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);
					Square enemyKingPosition = OwningSide == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
					if (testSquare.IsValid() && !board.IsOccupiedBySide(testSquare, OwningSide) && Rules.MoveObeysRules(board, testMove, OwningSide) && testSquare != enemyKingPosition)
						LegalMoves.Add(new Movement(testMove));
				}
			}
		}

		private void CheckCastlingMoves(Board board) {
			if (!HasMoved && !Rules.IsPlayerInCheck(board, OwningSide)) {
				int castlingRank = OwningSide == Side.White ? 1 : 8;
				List<Square> rookSquares = new List<Square> { new Square(1, castlingRank), new Square(8, castlingRank) };
				List<Square> inBetweenSquares = new List<Square>();
				List<Movement> inBetweenMoves = new List<Movement>();

				for (int rookSquareIndex = 0; rookSquareIndex < rookSquares.Count; rookSquareIndex++) {
					if (board[rookSquares[rookSquareIndex]] is Rook rook && !rook.HasMoved && rook.OwningSide == OwningSide) {
						bool checkingQueensideCastle = rookSquareIndex == 0;
						inBetweenSquares.Add(new Square(checkingQueensideCastle ? 4 : 6, castlingRank));
						inBetweenSquares.Add(new Square(checkingQueensideCastle ? 3 : 7, castlingRank));
						if (checkingQueensideCastle) inBetweenSquares.Add(new Square(2, castlingRank));

						if (!board.IsOccupied(inBetweenSquares[0]) && !board.IsOccupied(inBetweenSquares[1]) && (!checkingQueensideCastle || !board.IsOccupied(inBetweenSquares[2]))) {
							inBetweenMoves.Add(new Movement(Position, inBetweenSquares[0]));
							inBetweenMoves.Add(new Movement(Position, inBetweenSquares[1]));

							if (Rules.MoveObeysRules(board, inBetweenMoves[0], OwningSide) && Rules.MoveObeysRules(board, inBetweenMoves[1], OwningSide)) {
								LegalMoves.Add(new CastlingMove(Position, inBetweenSquares[1], rook));
							}
						}

						inBetweenSquares.Clear();
						inBetweenMoves.Clear();
					}
				}
			}
		}
	}
}