using Temporal.Content.Projectiles.Whips;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Temporal.Content.Items.Weapons.Whips
{
    internal class ClockworkWhip : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<ClockworkWhipProjectile>(), 50, 0.75f, 4f);

            Item.value = Item.buyPrice(gold: 3);
            Item.rare = ItemRarityID.Red;
        }
    }
}
