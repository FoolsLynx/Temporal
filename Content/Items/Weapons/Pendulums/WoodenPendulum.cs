using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using Temporal.Content.Projectiles.Pendulums;

namespace Temporal.Content.Items.Weapons.Pendulums
{
    internal class WoodenPendulum : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;

            Item.DamageType = DamageClass.Melee;
            Item.damage = 8;
            Item.knockBack = 1.75f;
            Item.crit = 7;

            Item.value = Terraria.Item.buyPrice(silver: 1);

            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<WoodenPendulumProjectile>();

            Item.noMelee = true;
            Item.noUseGraphic = true;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 10)
                .AddIngredient(ItemID.Cobweb, 20)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
