using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Temporal.Common.System
{
    internal class BossDownedSystem : ModSystem
    {
        public static bool downedClockworkGod = false;

        public override void OnWorldLoad()
        {
            downedClockworkGod = false;
        }

        public override void OnWorldUnload()
        {
            downedClockworkGod = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedClockworkGod) tag["downedClockworkGod"] = true;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedClockworkGod = tag.ContainsKey("downedClockworkGod");
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedClockworkGod;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedClockworkGod = flags[0];
        }
    }

}
