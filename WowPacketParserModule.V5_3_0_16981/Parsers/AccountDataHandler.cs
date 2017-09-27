﻿using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V5_3_0_16981.Parsers
{
    public static class AccountDataHandler
    {

        [Parser(Opcode.SMSG_UPDATE_ACCOUNT_DATA)]
        public static void HandleServerUpdateAccountData(Packet packet)
        {
            var guid = new byte[8];

            var decompCount = packet.ReadInt32();
            var compCount = packet.ReadInt32();

            var pkt = packet.Inflate(compCount, decompCount, false);
            var data = pkt.ReadWoWString(decompCount);
            pkt.ClosePacket();
            packet.AddValue("Account Data", data);

            packet.ReadTime("Login Time");
            guid[7] = packet.ReadBit();
            packet.ReadBitsE<AccountDataType>("Data Type", 3);
            packet.StartBitStream(guid, 3, 6, 1, 5, 0, 4, 2);
            packet.ReadXORBytes(guid, 6, 7, 4, 1, 5, 0, 3, 2);

            packet.WriteGuid("GUID", guid);
        }
    }
}
