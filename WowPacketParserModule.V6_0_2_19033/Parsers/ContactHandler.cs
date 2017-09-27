﻿using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V6_0_2_19033.Parsers
{
    public static class ContactHandler
    {

        [Parser(Opcode.SMSG_FRIEND_STATUS)]
        public static void HandleFriendStatus(Packet packet)
        {
            packet.ReadByte("FriendResult");

            packet.ReadPackedGuid128("Guid");
            packet.ReadPackedGuid128("WowAccount");

            packet.ReadInt32("VirtualRealmAddress");

            packet.ReadByteE<ContactStatus>("Status");

            packet.ReadInt32<AreaId>("AreaID");
            packet.ReadInt32("Level");
            packet.ReadInt32E<Class>("ClassID");

            packet.ResetBitReader();

            var bits28 = packet.ReadBits(10);
            packet.ReadWoWString("Notes", bits28);
        }

        [Parser(Opcode.SMSG_CONTACT_LIST)]
        public static void HandleContactList(Packet packet)
        {
            packet.ReadInt32E<ContactListFlag>("List Flags");
            var bits6 = packet.ReadBits("ContactInfoCount", 8);

            for (var i = 0; i < bits6; i++)
            {
                packet.ReadPackedGuid128("Guid", i);
                packet.ReadPackedGuid128("WowAccount", i);

                packet.ReadInt32("VirtualRealmAddr", i);
                packet.ReadInt32("NativeRealmAddr", i);
                packet.ReadInt32("TypeFlags", i);

                packet.ReadByte("Status", i);

                packet.ReadInt32<AreaId>("AreaID", i);
                packet.ReadInt32("Level", i);
                packet.ReadInt32("ClassID", i);

                packet.ResetBitReader();

                var bits44 = packet.ReadBits(10);
                packet.ReadWoWString("Notes", bits44, i);
            }
        }
    }
}
