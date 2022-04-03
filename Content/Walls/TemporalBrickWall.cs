using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Temporal.Content.Walls
{
    internal class TemporalBrickWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(130, 122, 110));
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }
    }
}
