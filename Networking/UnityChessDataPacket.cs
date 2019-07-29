using System;
using System.Runtime.InteropServices;

namespace UnityChess.Networking {
	[Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UnityChessDataPacket {
		public UnityChessDataPacketHeader Header;
		public UserCommand UserCommand;
		public byte[] UserData;

		public UnityChessDataPacket(UserCommand userCommand, byte[] userData) {
			Header = new UnityChessDataPacketHeader(userData.Length);
			UserCommand = userCommand;
			UserData = userData;
		}
	}
}