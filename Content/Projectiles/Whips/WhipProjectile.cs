using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace Temporal.Content.Projectiles.Whips
{
    internal abstract class WhipProjectile : ModProjectile
    {
        public int segments = 20;
        public float rangeMultiplier = 1f;

        public virtual void SafeSetDefaults() { }
        
        public virtual void CustomAI() { }

        public sealed override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            SafeSetDefaults();
        }

        public sealed override void PostAI()
        {
            Projectile.GetWhipSettings(Projectile, out var timeToFlyOut, out var _, out var _);
            if (Projectile.ai[0] == (float)(int)(timeToFlyOut / 2f))
            {
                Projectile.WhipPointsForCollision.Clear();
                FillWhipControlPoints(Projectile.WhipPointsForCollision);
                Vector2 position = Projectile.WhipPointsForCollision[Projectile.WhipPointsForCollision.Count - 1];
                SoundEngine.PlaySound(SoundID.Item153, position);
            }

            CustomAI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Handle Sprite Effects
            SpriteEffects spriteEffects = 0;
            if (Projectile.spriteDirection == 1)
            {
                spriteEffects = (SpriteEffects)((int)spriteEffects ^ 1);
            }
            // Get the Whip Texture
            Texture2D whipTexture = TextureAssets.Projectile[Projectile.type].Value;
            // Create a Rectangle holding the details for frame size
            Rectangle rectangle = whipTexture.Frame(1, 5);
            // Get the Height of a frame
            int height = rectangle.Height;
            // Decrease Rectangle Size (Fixes issues with graphics if not adjusted)
            rectangle.Height -= 2;
            // Get Center Point
            Vector2 center = rectangle.Size() / 2f;
            // Get a List of Control Points
            List<Vector2> controlPoints = new();
            FillWhipControlPoints(controlPoints); // Use New Custom Method

            // Get First point
            Vector2 currentPosition = controlPoints[0];
            // Loop Through Each Point
            for (int i = 0; i < controlPoints.Count - 1; i++)
            {
                // Get the Origin
                Vector2 origin = center;
                // Set the Default Scale
                float scale = 1f;
                // Change Origin on first index
                if (i == 0)
                {
                    origin.Y -= 4f;
                }
                else
                {
                    // Set frame based on segments.
                    int frame = 1;
                    if (i > 10)
                    {
                        frame = 2;
                    }
                    if (i > 30)
                    {
                        frame = 3;
                    }
                    // Adjust the Rectange Y based on the frame index
                    rectangle.Y = height * frame;
                }
                // If on the final point
                if (i == controlPoints.Count - 2)
                {
                    // Change frame to the Tip
                    rectangle.Y = height * 4;
                    // Get the timeToFlyOut variable
                    Projectile.GetWhipSettings(Projectile, out var timeToFlyOut, out _, out _);
                    // Get Scale from Time
                    float t = Projectile.ai[0] / timeToFlyOut;
                    float amount = Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true);
                    scale = MathHelper.Lerp(0.5f, 1.5f, amount);
                }
                // Get Current Point Position
                Vector2 currentPoint = controlPoints[i];
                // Get Offset from Next Point
                Vector2 offsetPoint = controlPoints[i + 1] - currentPoint;
                // Get the Rotation
                float rotation = offsetPoint.ToRotation() - MathHelper.PiOver2;
                // Get Lighting Colour based on Tile
                Color color = Lighting.GetColor(currentPoint.ToTileCoordinates());
                // Draw the Whip
                Main.spriteBatch.Draw(
                    whipTexture,
                    currentPosition - Main.screenPosition,
                    (Rectangle?)rectangle,
                    color,
                    rotation,
                    origin,
                    scale,
                    spriteEffects,
                    0f
                );
                // Increase Current Position by Offset
                currentPosition += offsetPoint;
            }
            // Prevent default Whip drawing from happening
            return false;
        }

        private void FillWhipControlPoints(List<Vector2> list)
        {
            // Get Default Whip Settings
            Projectile.GetWhipSettings(Projectile, out var timeToFlyOut, out var _, out var _);
            // Needs to be understood better
            float num = Projectile.ai[0] / timeToFlyOut;
            float num10 = 0.5f;
            float num11 = 1f + num10;
            float num12 = (float)Math.PI * 10f * (1f - num * num11) * (float)(-Projectile.spriteDirection) / (float)segments;
            float num13 = num * num11;
            float num14 = 0f;
            if (num13 > 1f)
            {
                num14 = (num13 - 1f) / num10;
                num13 = MathHelper.Lerp(1f, 0f, num14);
            }
            float num15 = Projectile.ai[0] - 1f;
            Player player = Main.player[Projectile.owner];
            Item heldItem = Main.player[Projectile.owner].HeldItem;
            num15 = (float)(ContentSamples.ItemsByType[heldItem.type].useAnimation * 2) * num * player.whipRangeMultiplier;
            float num16 = Projectile.velocity.Length() * num15 * num13 * rangeMultiplier / (float)segments;
            float num17 = 1f;
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);
            Vector2 vector = playerArmPosition;
            float num2 = -(float)Math.PI / 2f;
            Vector2 value = vector;
            float num3 = (float)Math.PI / 2f + (float)Math.PI / 2f * (float)Projectile.spriteDirection;
            Vector2 value2 = vector;
            float num4 = (float)Math.PI / 2f;
            list.Add(playerArmPosition);
            for (int i = 0; i < segments; i++)
            {
                float num5 = (float)i / (float)segments;
                float num6 = num12 * num5 * num17;
                Vector2 vector2 = vector + num2.ToRotationVector2() * num16;
                Vector2 vector3 = value2 + num4.ToRotationVector2() * (num16 * 2f);
                Vector2 val = value + num3.ToRotationVector2() * (num16 * 2f);
                float num7 = 1f - num13;
                float num8 = 1f - num7 * num7;
                Vector2 value3 = Vector2.Lerp(vector3, vector2, num8 * 0.9f + 0.1f);
                Vector2 value4 = Vector2.Lerp(val, value3, num8 * 0.7f + 0.3f);
                Vector2 spinningpoint = playerArmPosition + (value4 - playerArmPosition) * new Vector2(1f, num11);
                float num9 = num14;
                num9 *= num9;
                Vector2 item = spinningpoint.RotatedBy(Projectile.rotation + 4.712389f * num9 * (float)Projectile.spriteDirection, playerArmPosition);
                list.Add(item);
                num2 += num6;
                num4 += num6;
                num3 += num6;
                vector = vector2;
                value2 = vector3;
                value = val;
            }
        }
    }
}
