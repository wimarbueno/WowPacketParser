using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V5_4_8_18291.Parsers
{
    public static class AccountDataHandler
    {
        [Parser(Opcode.SMSG_UPDATE_ACCOUNT_DATA)]
        public static void HandleServerUpdateAccountData(Packet packet)
        {
            var guid = new byte[8];

            packet.ReadBitsE<AccountDataType>("Data Type", 3);

            packet.StartBitStream(guid, 5, 1, 3, 7, 0, 4, 2, 6);

            packet.ReadXORBytes(guid, 3, 1, 5);

            var decompCount = packet.ReadInt32();
            var compCount = packet.ReadInt32();

            var pkt = packet.Inflate(compCount, decompCount, false);
            var data = pkt.ReadWoWString(decompCount);
            pkt.ClosePacket(false);

            packet.AddValue("Account Data", data);

            packet.ReadXORBytes(guid, 7, 4, 0, 6, 2);

            packet.ReadTime("Login Time");

            packet.WriteGuid("GUID", guid);
        }

    }
}