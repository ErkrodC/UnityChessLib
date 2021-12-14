using System.Collections.Generic;

namespace UnityChess {
	public class Bishop : Piece<Bishop> {
		public Bishop() : base(Square.Invalid, Side.None) {}
		public Bishop(Square startingPosition, Side owner) : base(startingPosition, owner) {}

		public override Dictionary<(Square, Square), Movement> CalculateLegalMoves(
			Board board,
			GameConditions gameConditions,
			Square position
		) {
			Dictionary<(Square, Square), Movement> result = null;

			foreach (Square offset in SquareUtil.DiagonalOffsets) {
				Square endSquare = position + offset;

				while (endSquare.IsValid()) {
					Movement testMove = new Movement(position, endSquare);

					if (Rules.MoveObeysRules(board, testMove, Owner)) {
						if (result == null) {
							result = new Dictionary<(Square, Square), Movement>();
						}

						result[(testMove.Start, testMove.End)] = new Movement(testMove);
					}
					
					if (board.IsOccupiedAt(endSquare)) {
						break;
					}

					endSquare += offset;
				}
			}

			return result;
		}
	}
}