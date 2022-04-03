using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Temporal.Content.Tiles.TemporalTower
{
    internal class TemporalBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<TemporalRoof>()] = true;
            Main.tileMerge[ModContent.TileType<TemporalRoof>()][Type] = true;

            AddMapEntry(new Color(205, 167, 132));

            
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }
    }
}
