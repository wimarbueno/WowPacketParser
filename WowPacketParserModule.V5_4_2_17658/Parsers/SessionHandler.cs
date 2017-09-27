using System;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;
using CoreParsers = WowPacketParser.Parsing.Parsers;

namespace WowPacketParserModule.V5_4_2_17658.Parsers
{
    public static class SessionHandler
    {


        [Parser(Opcode.SMSG_MOTD)]
        public static void HandleMessageOfTheDay(Packet packet)
        {
            var lineCount = packet.ReadBits("Line Count", 4);
            var lineLength = new int[lineCount];

            for (var i = 0; i < lineCount; i++)
                lineLength[i] = (int)packet.ReadBits(7);

            for (var i = 0; i < lineCount; i++)
                packet.ReadWoWString("Line", lineLength[i], i);
        }

        [Parser(Opcode.SMSG_AUTH_RESPONSE)]
        public static void HandleAuthResponse(Packet packet)
        {
            var bit78 = false;
            var bit7C = false;

            var bits20 = 0u;
            var classCount = 0u;
            var raceCount = 0u;
            var bits64 = 0u;

            uint[] bits0 = null;
            uint[] bits0C = null;
            uint[] bits45 = null;
            uint[] bits68 = null;
            uint[] bits448 = null;

            packet.ReadByteE<ResponseCode>("Auth Code");

            var hasAccountData = packet.ReadBit("Has Account Data");
            if (hasAccountData)
            {
                bits20 = packet.ReadBits(21);
                bit78 = packet.ReadBit();

                bits0 = new uint[bits20];
                bits0C = new uint[bits20];

                for (var i = 0; i < bits20; ++i)
                {
                    bits0[i] = packet.ReadBits(8);
                    packet.ReadBit("unk bit", i);
                    bits0C[i] = packet.ReadBits(8);
                }

                packet.ReadBit();
                packet.ReadBit();
                classCount = packet.ReadBits("Class Activation Count", 23);
                raceCount = packet.ReadBits("Race Activation Count", 23);
                bit7C = packet.ReadBit();
                packet.ReadBit();
                bits64 = packet.ReadBits(21);

                bits45 = new uint[bits64];
                bits68 = new uint[bits64];
                bits448 = new uint[bits64];

                for (var i = 0; i < bits64; ++i)
                {
                    bits45[i] = packet.ReadBits(10);
                    bits68[i] = packet.ReadBits(7);
                    bits448[i] = packet.ReadBits(23);
                }
            }

            var isQueued = packet.ReadBit("Is In Queue");

            if (isQueued)
                packet.ReadBit("unk0");

            if (isQueued)
                packet.ReadUInt32("Unk 11");

            if (hasAccountData)
            {
                for (var i = 0; i < bits64; ++i)
                {
                    for (var j = 0; j < bits448[i]; ++j)
                    {
                        packet.ReadByte("Unk byte 1", i, j);
                        packet.ReadByte("Unk byte 0", i, j);
                    }

                    packet.ReadInt32("Int30", i);
                    packet.ReadWoWString("StringED", bits45[i], i);
                    packet.ReadWoWString("StringED", bits68[i], i);
                }

                packet.ReadInt32("Int30");
                packet.ReadByte("Byte3C");

                for (var i = 0; i < bits20; ++i)
                {
                    packet.ReadInt32("RealmId", i);
                    packet.ReadWoWString("Realm", bits0C[i], i);
                    packet.ReadWoWString("Realm", bits0[i], i);
                }

                if (bit78)
                    packet.ReadInt16("Int76");

                packet.ReadByte("Byte3D");

                for (var i = 0; i < raceCount; ++i)
                {
                    packet.ReadByteE<Race>("Race", i);
                    packet.ReadByteE<ClientType>("Race Expansion", i);
                }

                packet.ReadInt32("Int34");
                packet.ReadInt32("Int1C");
                packet.ReadInt32("Int38");

                for (var i = 0; i < classCount; ++i)
                {
                    packet.ReadByteE<Class>("Class", i);
                    packet.ReadByteE<ClientType>("Class Expansion", i);
                }

                if (bit7C)
                    packet.ReadInt16("Int7A");

                packet.ReadInt32("Int40");
            }
        }

        [Parser(Opcode.SMSG_LOGOUT_RESPONSE)]
        public static void HandlePlayerLogoutResponse(Packet packet)
        {
            packet.ReadBit("Instant");
            packet.ReadInt32("Reason");
            // From TC:
            // Reason 1: IsInCombat
            // Reason 2: InDuel or frozen by GM
            // Reason 3: Jumping or Falling
        }
    }
}
