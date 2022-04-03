using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Temporal.Content.Projectiles.Pendulums
{
    internal abstract class BasePendulum : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            
        }

        internal virtual void SafeSetDefaults()
        {

        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.maxPenetrate = 4;
            SafeSetDefaults();
        }


        public override void AI()
        {
            float num = 250f;
            float scaleFactor = 0.1f;
            float num2 = 15;
            float num3= 12;

            
            // Extend String Length
            if(Main.player[Projectile.owner].yoyoString)
            {
                num += num * 0.25f + 10f;
            }
            // Kill if Owner Dead
            if(Main.player[Projectile.owner].dead)
            {
                Kill(1);
                return;
            }
            // Change Direction
            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            Main.player[Projectile.owner].SetDummyItemTime(2);
            if (Projectile.position.X + (float)(Projectile.width / 2) > Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2))
            {
                Main.player[Projectile.owner].ChangeDir(1);
                Projectile.direction = 1;
            } else
            {
                Main.player[Projectile.owner].ChangeDir(-1);
                Projectile.direction = -1;
            }


            if(Projectile.ai[0] == 0f || Projectile.ai[0] == 1f)
            {
                if(Projectile.ai[0] == 1f)
                {
                    num *= 0.75f;
                }

                bool flag3 = false;
                Vector2 value = Main.player[Projectile.owner].Center - Projectile.Center;
                if(value.Length() > (double)num * 0.9)
                {
                    flag3 = true;
                }

                if(value.Length() > num)
                {
                    float num7 = value.Length() - num;
                    Vector2 vector = new Vector2(value.X, value.Y);
                    value.Normalize();
                    value *= num;
                    Projectile.position = Main.player[Projectile.owner].Center - value;
                    Projectile.position.X -= Projectile.width * 0.5f;
                    Projectile.position.Y -= Projectile.height * 0.5f;
                    float num8 = Projectile.velocity.Length();
                    Vector2 vector2 = Projectile.Center;
                    Vector2 vector3 = Main.player[Projectile.owner].Center;
                    if(vector2.Y < vector3.Y) vector.Y = Math.Abs(vector.Y);
                    else if(vector2.Y > vector3.Y) vector.Y = 0f - Math.Abs(vector.Y);
                    if (vector2.X < vector3.X) vector.X = Math.Abs(vector.X);
                    else if(vector2.X > vector3.X) vector.X = 0f - Math.Abs(vector.X);
                    vector.Normalize();
                    vector *= Projectile.velocity.Length();
                    if(Math.Abs(Projectile.velocity.X) > Math.Abs(Projectile.velocity.Y))
                    {
                        Vector2 velocity = Projectile.velocity;
                        velocity.Y += vector.Y;
                        velocity.Normalize();
                        velocity *= Projectile.velocity.Length();
                        if(Math.Abs(vector.X) < 0.1 || Math.Abs(vector.Y) < 0.1)
                        {
                            Projectile.velocity = velocity;
                        } else
                        {
                            Projectile.velocity = (velocity + Projectile.velocity * 2f) / 3f;
                        }
                    } else
                    {
                        Vector2 velocity2 = Projectile.velocity;
                        velocity2.X += vector2.X;
                        velocity2.Normalize();
                        velocity2 *= Projectile.velocity.Length();
                        if(Math.Abs(vector.X) < 0.2 || Math.Abs(vector.Y) < 0.2)
                        {
                            Projectile.velocity = velocity2;
                        } else
                        {
                            Projectile.velocity = (velocity2 + Projectile.velocity * 2f) / 3f;
                        }
                    }
                }

                if(Projectile.velocity.Length() > num2)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= num2;
                }
                if(Projectile.velocity.Length() < num3)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= num3;
                }

                // Run Custom AI
                CustomAI();
                return;
            }

            // Dunno
            Projectile.tileCollide = false;
            Vector2 vector4 = Main.player[Projectile.owner].Center - Projectile.Center;
            float num4 = vector4.Length();
            if(num4 < 40f || vector4.HasNaNs() || num4 > 2000f)
            {
                Kill(0);
                return;
            }

            float num5 = num2 * 1.5f;

            float num6 = 12f;
            vector4.Normalize();
            vector4 *= num5;
            Projectile.velocity = (Projectile.velocity * (num6 - 1f) + vector4) / num6;
        }

        internal virtual void CustomAI()
        {
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.MountedCenter;

            vector.Y += player.gfxOffY;

            float num1 = Projectile.Center.X - vector.X;
            float num2 = Projectile.Center.Y - vector.Y;

            float num3 = (float)Math.Atan2(num2, num1) - 1.75f;

            bool flag = true;
            if(num1 == 0f && num2 == 0f)
            {
                flag = false;
            } else
            {
                float num4 = (float)Math.Sqrt(num1 * num1 + num2 * num2); ;
                num4 = 12f / num4;
                num1 *= num4;
                num2 *= num4;
                vector.X -= num1 * 0.1f;
                vector.Y -= num2 * 0.1f;
                num1 = Projectile.position.X + (float)Projectile.width * 0.5f - vector.X;
                num2 = Projectile.position.Y + (float)Projectile.height * 0.5f - vector.Y;
            }
            Color colour = default;
            while(flag)
            {
                float num5 = 12f;
                float num6 = (float)Math.Sqrt(num1 * num1 + num2 * num2);
                float num7 = num6;
                if(float.IsNaN(num6) || float.IsNaN(num7))
                {
                    flag = false;
                    continue;
                }
                if(num6 < 20f)
                {
                    num5 = num6 - 6f;
                    flag = false;
                }
                num6 = 12f / num6;
                num1 *= num6;
                num2 *= num6;
                vector.X += num1;
                vector.Y += num2;
                num1 = Projectile.position.X + (float)Projectile.width * 0.5f - vector.X;
                num2 = Projectile.position.Y + (float)Projectile.height * 0.1f - vector.Y;
                if(num7 > 12f)
                {
                    float num8 = .3f;
                    float num9 = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
                    if (num9 > 16f) num9 = 16f;
                    num9 = 1f - num9 / 16f;
                    num8 *= num9;
                    num9 = num7 / 80f;
                    if (num9 > 1f) num9 = 1f;
                    num8 *= num9;
                    if (num8 < 0f) num8 = 0f;
                    num8 *= num9;
                    num8 *= 0.5f;
                    if(num2 > 0f)
                    {
                        num2 *= 1f + num8;
                        num1 *= 1f - num8;
                    } else
                    {
                        num9 = Math.Abs(Projectile.velocity.X) / 3f;
                        if (num9 > 1f) num9 = 1f;
                        num9 -= 0.5f;
                        num8 *= num9;
                        if(num8 > 0f)
                        {
                            num8 *= 2f;
                        }
                        num2 *= 1f + num8;
                        num1 *= 1f - num8;
                    }
                }
                num3 = (float)Math.Atan2(num2, num1) - 1.57f;
                Color white = Color.White;
                white.A = ((byte)((float)(int)(white).A * 0.4f));
                white = TemporalUtils.TryApplyingPlayerStringColour(player.stringColor, white);
                float num10 = 0.5f;
                white = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16f), white);
                colour = new Color(
                    (int)(byte)(white.R * num10),
                    (int)(byte)(white.G * num10),
                    (int)(byte)(white.B * num10),
                    (int)(byte)(white.A * num10)
                );
                Main.EntitySpriteDraw(
                    TextureAssets.FishingLine.Value,
                    new Vector2(
                        vector.X - Main.screenPosition.X + (float)TextureAssets.FishingLine.Width() * 0.5f,
                        vector.Y - Main.screenPosition.Y + (float)TextureAssets.FishingLine.Height() * 0.5f
                    ) - new Vector2(6f, 0f),
                    (Rectangle?)new Rectangle(0, 0, TextureAssets.FishingLine.Width(), (int)num5),
                    colour,
                    num3,
                    new Vector2((float)TextureAssets.FishingLine.Width() * 0.5f, 0f),
                    1f,
                    (SpriteEffects)0,
                    0)
               ;
            }

            return base.PreDraw(ref lightColor);
        }



    }
}
