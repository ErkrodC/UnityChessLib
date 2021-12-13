using System;

namespace UnityChess {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		public Mode Mode { get; }
		public Side SideToMove { get; private set; }
		public int LatestHalfMoveIndex => HalfMoveTimeline.HeadIndex;
		public Timeline<GameConditions> ConditionsTimeline { get; }
		public Timeline<Board> BoardTimeline { get; }
		public Timeline<HalfMove> HalfMoveTimeline { get; }

		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		/// <param name="mode">Describes which players are human or AI.</param>
		/// <param name="startingConditions">Conditions at the time the board was set up.</param>
		public Game(Mode mode, GameConditions startingConditions) {
			Mode = mode;
			SideToMove = Side.White;

			BoardTimeline = new Timeline<Board> { new Board() };
			HalfMoveTimeline = new Timeline<HalfMove>();
			ConditionsTimeline = new Timeline<GameConditions> { startingConditions };
			
			UpdateAllPiecesLegalMoves(BoardTimeline.Current, ConditionsTimeline.Current);
		}

		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public bool TryExecuteMove(Movement move) {
			if (!TryGetLegalMove(move.Start, move.End, out Movement validatedMove)) return false;
			
			//create new copy of previous current board, and execute the move on it
			Board boardBeforeMove = BoardTimeline.Current;
			Board resultingBoard = new Board(boardBeforeMove);
			resultingBoard.MovePiece(validatedMove);
			BoardTimeline.AddNext(resultingBoard);

			SideToMove = SideToMove.Complement();
			
			bool capturedPiece = boardBeforeMove[validatedMove.End] != null || validatedMove is EnPassantMove;
			bool causedCheck = Rules.IsPlayerInCheck(resultingBoard, SideToMove);
			
			HalfMove halfMove = new HalfMove(boardBeforeMove[validatedMove.Start], validatedMove, capturedPiece, causedCheck);
			GameConditions resultingGameConditions = ConditionsTimeline.Current.CalculateEndingConditions(boardBeforeMove, halfMove);
			ConditionsTimeline.AddNext(resultingGameConditions);
			
			UpdateAllPiecesLegalMoves(resultingBoard, resultingGameConditions);

			halfMove.SetGameEndBools(
				Rules.IsPlayerStalemated(resultingBoard, SideToMove),
				Rules.IsPlayerCheckmated(resultingBoard, SideToMove)
			);
			HalfMoveTimeline.AddNext(halfMove);
			
			return true;
		}
		
		public bool TryGetLegalMove(Square startSquare, Square endSquare, out Movement move) {
			Piece movingPiece = BoardTimeline.Current[startSquare];

			if (movingPiece == null) {
				move = null;
				return false;
			}

			bool foundMove = movingPiece.LegalMoves.TryGetLegalMove(startSquare, endSquare, out move);
			return movingPiece.OwningSide == SideToMove && foundMove;
		}

		public bool ResetGameToHalfMoveIndex(int halfMoveIndex) {
			if (LatestHalfMoveIndex == -1) return false; // i.e. No possible move to reset to

			BoardTimeline.HeadIndex = halfMoveIndex + 1;
			ConditionsTimeline.HeadIndex = halfMoveIndex + 1;
			HalfMoveTimeline.HeadIndex = halfMoveIndex;
			SideToMove = halfMoveIndex % 2 == 0 ? Side.Black : Side.White;
			
			UpdateAllPiecesLegalMoves(BoardTimeline.Current, ConditionsTimeline.Current);
			return true;
		}

		internal static void UpdateAllPiecesLegalMoves(Board board, GameConditions gameConditions) {
			for (int file = 1; file <= 8; file++) {
				for (int rank = 1; rank <= 8; rank++) {
					if (board[file, rank] is Piece piece) {
						piece.LegalMoves.Clear();
						if (piece.OwningSide == gameConditions.SideToMove) {
							piece.UpdateLegalMoves(board, gameConditions);
						}
					}
				}
			}
		}
	}
}