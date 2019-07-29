using System;
using System.Runtime.InteropServices;

namespace UnityChess.Networking {
	[Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UnityChessDataPacket {
		public UserCommand UserCommand;
		public byte byte0;
		public byte byte1;
		public byte byte2;
		public byte byte3;
	}
}