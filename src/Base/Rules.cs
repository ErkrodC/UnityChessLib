using System;

namespace UnityChess {
	/// <summary>Contains methods for checking legality of moves and board positions.</summary>
	public static class Rules {
		private static readonly Square[] knightOffsets = {
			new Square(-2, -1),
			new Square(-2, 1),
			new Square(2, -1),
			new Square(2, 1),
			new Square(-1, -2),
			new Square(-1, 2),
			new Square(1, -2),
			new Square(1, 2),
		};
		
		private static readonly Square[] surroundingOffsets = {
			new Square(-1, 0),
			new Square(1, 0),
			new Square(0, -1),
			new Square(0, 1),
			new Square(-1, 1),
			new Square(-1, -1),
			new Square(1, -1),
			new Square(1, 1)
		};

		/// <summary>Checks if the player of the given side has been checkmated.</summary>
		public static bool IsPlayerCheckmated(Board board, Side player) => PlayerHasNoLegalMoves(player, board) && IsPlayerInCheck(board, player);

		/// <summary>Checks if the player of the given side has been stalemated.</summary>
		public static bool IsPlayerStalemated(Board board, Side player) => PlayerHasNoLegalMoves(player, board) && !IsPlayerInCheck(board, player);

		/// <summary>Checks if the player of the given side is in check.</summary>
		public static bool IsPlayerInCheck(Board board, Side player) => IsPieceAttacked(board, player == Side.White ? board.WhiteKing : board.BlackKing);

		internal static bool MoveObeysRules(Board board, Movement move, Side movedPieceSide) => !MovePutsMoverInCheck(board, move, movedPieceSide) && MoveRemovesCheckFromPlayerIfNeeded(board, move, movedPieceSide);

		private static bool MoveRemovesCheckFromPlayerIfNeeded(Board board, Movement move, Side side) {
			if (!IsPlayerInCheck(board, side)) return true;

			Board resultingBoard = new Board(board);
			resultingBoard.MovePiece(new Movement(move.Start, move.End));

			return !IsPlayerInCheck(resultingBoard, side);
		}

		private static bool MovePutsMoverInCheck(Board board, Movement move, Side moverSide) {
			Board resultingBoard = new Board(board);
			resultingBoard.MovePiece(new Movement(move.Start, move.End));

			return IsPlayerInCheck(resultingBoard, moverSide);
		}

		private static bool PlayerHasNoLegalMoves(Side player, Board board) {
			int sumOfLegalMoves = 0;

			for (int file = 1; file <= 8; file++) {
				for (int rank = 1; rank <= 8; rank++) {
					Piece piece = board[file, rank];
					if (piece != null && piece.OwningSide == player) sumOfLegalMoves += piece.LegalMoves.Count;;
				}
			}

			return sumOfLegalMoves == 0;
		}

		private static bool IsPieceAttacked(Board board, Piece friendlyPiece) {
			Side friendlySide = friendlyPiece.OwningSide;
			Side enemySide = friendlySide.Complement();
			int friendlyForward = friendlySide.ForwardDirection();

			foreach (Square offset in surroundingOffsets) {
				bool isDiagonalOffset = Math.Abs(offset.File) == Math.Abs(offset.Rank);
				Square friendlySquare = friendlyPiece.Position;
				Square testSquare = friendlySquare + offset;

				while (testSquare.IsValid() && !board.IsOccupiedBySide(testSquare, friendlySide)) {
					if (board.IsOccupiedBySide(testSquare, enemySide)) {
						int fileDistance = Math.Abs(testSquare.File - friendlySquare.File);
						int rankDistance = Math.Abs(testSquare.Rank - friendlySquare.Rank);

						Piece enemyPiece = board[testSquare];
						switch (enemyPiece) {
							case Queen:
							case Bishop when isDiagonalOffset:
							case Rook when !isDiagonalOffset:
							case King when fileDistance <= 1 && rankDistance <= 1:
							case Pawn when fileDistance == 1
							               && testSquare.Rank == friendlySquare.Rank + friendlyForward:
								return true;
						}
						
						// stop checking this diagonal in the case of enemy knight
						break;
					}

					testSquare += offset;
				}
			}
			
			foreach (Square offset in knightOffsets) {
				Square testSquare = friendlyPiece.Position + offset;
				
				if (testSquare.IsValid()
				    && board[testSquare] is Knight knight
				    && knight.OwningSide == enemySide
				) {
					return true;
				}
			}

			return false;
		}
	}
}