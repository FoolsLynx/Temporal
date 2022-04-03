using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temporal.Content.Buffs;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Temporal.Common.Player
{
    internal class TemporalBuffPlayer : ModPlayer
    {
        public bool doomBuff = false;
        public int doomCount = 0;


        public override void ResetEffects()
        {
            doomBuff = false;
            doomCount = 0;
        }

        public override void UpdateDead()
        {
            ResetEffects();
        }

        public void ApplyDoom(int count = 9)
        {
            Player.AddBuff(ModContent.BuffType<TemporalDoom>(), 100000, true, false);
            doomBuff = true;
            doomCount = count;
        }

        public void ReApplyDoom()
        {
            Player.AddBuff(ModContent.BuffType<TemporalDoom>(), 100000);
        }

        public void DecreaseDoomCount()
        {
            doomCount--;
            if(doomCount <= 0)
            {
                doomCount = 0;
                doomBuff = false;
                Kill(0, 1, false, PlayerDeathReason.ByCustomReason("Let their timer hit 0"));
            }
        }
    }
}
