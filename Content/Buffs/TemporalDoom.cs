using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temporal.Common.Player;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Temporal.Content.Buffs
{
    internal class TemporalDoom : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            TemporalBuffPlayer buffPlayer = player.GetModPlayer<TemporalBuffPlayer>();
            if(!buffPlayer.doomBuff)
            {
                buffPlayer.ApplyDoom();
            } else
            {
                buffPlayer.DecreaseDoomCount();
            }
            buffPlayer.doomCount--;
            return false;
        }
    }
}
