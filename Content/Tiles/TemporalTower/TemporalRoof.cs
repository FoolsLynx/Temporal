using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Temporal.Content.Tiles.TemporalTower
{
    internal class TemporalRoof : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(75, 153, 93));
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }
    }
}
