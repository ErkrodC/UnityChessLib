using System;
using System.Collections.Generic;

namespace UnityChess {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		public Mode Mode { get; }
		public Timeline<GameConditions> ConditionsTimeline { get; }
		public Timeline<Board> BoardTimeline { get; }
		public Timeline<HalfMove> HalfMoveTimeline { get; }
		public Timeline<Dictionary<Piece, Dictionary<(Square, Square), Movement>>> LegalMovesTimeline { get; }

		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		/// <param name="mode">Describes which players are human or AI.</param>
		public Game(Mode mode) : this(mode, GameConditions.NormalStartingConditions, Board.GetStartingPositionPieces()) { }

		public Game(Mode mode, GameConditions startingConditions, params Piece[] pieces) {
			Mode = mode;

			BoardTimeline = new Timeline<Board> { new Board(pieces) };
			HalfMoveTimeline = new Timeline<HalfMove>();
			ConditionsTimeline = new Timeline<GameConditions> { startingConditions };
			LegalMovesTimeline = new Timeline<Dictionary<Piece, Dictionary<(Square, Square), Movement>>> {
				CalculateLegalMovesForPosition(BoardTimeline.Current, ConditionsTimeline.Current)
			};
		}

		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public bool TryExecuteMove(Movement move) {
			if (!TryGetLegalMove(move.Start, move.End, out Movement validatedMove)) {
				return false;
			}

			//create new copy of previous current board, and execute the move on it
			Board boardBeforeMove = BoardTimeline.Current;
			Board resultingBoard = new Board(boardBeforeMove);
			resultingBoard.MovePiece(validatedMove);
			BoardTimeline.AddNext(resultingBoard);

			GameConditions conditionsBeforeMove = ConditionsTimeline.Current;
			Side updatedSideToMove = conditionsBeforeMove.SideToMove.Complement();
			bool causedCheck = Rules.IsPlayerInCheck(resultingBoard, updatedSideToMove);
			bool capturedPiece = boardBeforeMove[validatedMove.End] != null || validatedMove is EnPassantMove;
			
			HalfMove halfMove = new HalfMove(boardBeforeMove[validatedMove.Start], validatedMove, capturedPiece, causedCheck);
			GameConditions resultingGameConditions = conditionsBeforeMove.CalculateEndingConditions(boardBeforeMove, halfMove);
			ConditionsTimeline.AddNext(resultingGameConditions);

			Dictionary<Piece, Dictionary<(Square, Square), Movement>> legalMovesByPiece
				= CalculateLegalMovesForPosition(resultingBoard, resultingGameConditions);

			int numLegalMoves = GetNumLegalMoves(legalMovesByPiece);

			LegalMovesTimeline.AddNext(legalMovesByPiece);

			halfMove.SetGameEndBools(
				Rules.IsPlayerStalemated(resultingBoard, updatedSideToMove, numLegalMoves),
				Rules.IsPlayerCheckmated(resultingBoard, updatedSideToMove, numLegalMoves)
			);
			HalfMoveTimeline.AddNext(halfMove);
			
			return true;
		}

		public bool TryGetLegalMove(Square startSquare, Square endSquare, out Movement move) {
			move = null;

			return BoardTimeline.Current[startSquare] is { } movingPiece
			       && LegalMovesTimeline.Current.TryGetValue(movingPiece, out Dictionary<(Square, Square), Movement> movesByStartEndSquares)
			       && movesByStartEndSquares.TryGetValue((startSquare, endSquare), out move);
		}
		
		public bool TryGetLegalMovesForPiece(Piece movingPiece, out ICollection<Movement> legalMoves) {
			legalMoves = null;

			if (movingPiece != null
			    && LegalMovesTimeline.Current is { } legalMovesByPiece
			    && legalMovesByPiece.TryGetValue(movingPiece, out var movesByStartEndSquares)
			    && movesByStartEndSquares != null
			) {
				legalMoves = movesByStartEndSquares.Values;
				return true;
			}

			return false;
		}

		public bool ResetGameToHalfMoveIndex(int halfMoveIndex) {
			if (HalfMoveTimeline.HeadIndex == -1) {
				return false;
			}

			BoardTimeline.HeadIndex = halfMoveIndex + 1;
			ConditionsTimeline.HeadIndex = halfMoveIndex + 1;
			LegalMovesTimeline.HeadIndex = halfMoveIndex + 1;
			HalfMoveTimeline.HeadIndex = halfMoveIndex;

			return true;
		}
		
		internal static int GetNumLegalMoves(Dictionary<Piece, Dictionary<(Square, Square), Movement>> legalMovesByPiece) {
			int result = 0;
			
			if (legalMovesByPiece != null) {
				foreach (Dictionary<(Square, Square), Movement> movesByStartEndSquares in legalMovesByPiece.Values) {
					result += movesByStartEndSquares.Count;
				}
			}

			return result;
		}
		
		internal static Dictionary<Piece, Dictionary<(Square, Square), Movement>> CalculateLegalMovesForPosition(
			Board board,
			GameConditions gameConditions
		) {
			Dictionary<Piece, Dictionary<(Square, Square), Movement>> result = null;
			
			for (int file = 1; file <= 8; file++) {
				for (int rank = 1; rank <= 8; rank++) {
					if (board[file, rank] is Piece piece
					    && piece.Owner == gameConditions.SideToMove
					    && piece.CalculateLegalMoves(board, gameConditions, new Square(file, rank)) is
						    { } movesByStartEndSquares
					) {
						if (result == null) {
							result = new Dictionary<Piece, Dictionary<(Square, Square), Movement>>();
						}

						result[piece] = movesByStartEndSquares;
					}
				}
			}

			return result;
		}
	}
}