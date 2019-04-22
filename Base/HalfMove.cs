using System;
using System.Collections.Generic;

namespace UnityChess {
	public struct HalfMove {
		public readonly Piece Piece;
		public readonly Movement Move;
		public readonly bool CapturedPiece;
		public readonly bool CausedCheck;
		public readonly bool CausedStalemate;
		public readonly bool CausedCheckmate;
		
		private static readonly Dictionary<Type, string> pieceTypeToANSymbolMap = new Dictionary<Type, string> {
			{ typeof(Pawn), "" },
			{ typeof(Knight), "N" },
			{ typeof(Bishop), "B" },
			{ typeof(Rook), "R" },
			{ typeof(Queen), "Q" },
			{ typeof(King), "K" },		
		};

		public HalfMove(Piece piece, Movement move, bool capturedPiece, bool causedCheck, bool causedStalemate, bool causedCheckmate) {
			Piece = piece;
			Move = move;
			CapturedPiece = capturedPiece;
			CausedCheck = causedCheck;
			CausedCheckmate = causedCheckmate;
			CausedStalemate = causedStalemate;
		}
		
		// TODO handle ambiguous piece moves.
		public string ToAlgebraicNotation() {
			string moveText = "";
			string pieceSymbol = Piece is Pawn ?
				                     CapturedPiece ? SquareUtil.FileIntToCharMap[Move.Start.File] : "" :
				                     pieceTypeToANSymbolMap[Piece.GetType()];
			string captureText = CapturedPiece ? "x" : "";
			string endSquareString = SquareUtil.SquareToString(Move.End);
			string suffix = CausedCheckmate ? "#" :
			                CausedCheck     ? "+" : "";

			if (Piece is King) {
				if (Move is CastlingMove) moveText += Move.End.File == 3 ? $"O-O-O{suffix}" : $"O-O{suffix}";
				else moveText += $"{pieceSymbol}{captureText}{endSquareString}{suffix}";
			} else if (Piece is Pawn) {
				string pawnPromotionPieceSymbol = Move is PromotionMove promotionMove ? pieceTypeToANSymbolMap[promotionMove.AssociatedPiece.GetType()] : "";
				moveText += $"{pieceSymbol}{captureText}{endSquareString}{pawnPromotionPieceSymbol}{suffix}";
			} else if (Piece is Knight || Piece is Bishop || Piece is Rook || Piece is Queen ) {
				moveText += $"{pieceSymbol}{captureText}{endSquareString}{suffix}";
			}

			return moveText;
		}
	}
}
