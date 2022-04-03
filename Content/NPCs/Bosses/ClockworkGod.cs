using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Temporal.Common.Player;
using Temporal.Common.System;
using Temporal.Content.BossBars;
using Temporal.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Temporal.Content.NPCs.Bosses
{
    internal class ClockworkGod : ModNPC
    {
        internal static int DoomBuff() { return ModContent.BuffType<TemporalDoom>(); }



        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCDebuffImmunityData debuffData = new()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Poisoned,
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 48;
            NPC.damage = 60;
            NPC.defense = 40;
            NPC.lifeMax = 20000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 50);
            NPC.SpawnWithHigherTime(30);
            NPC.boss = true;
            NPC.npcSlots = 16f;
            NPC.aiStyle = -1;
            NPC.BossBar = ModContent.GetInstance<ClockworkGodBossBar>();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            
        }

        public override void BossLoot(ref string name, ref int potionType)
        {

        }
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref BossDownedSystem.downedClockworkGod, -1);
        }

        // AI
        public override void AI()
        {
            if (DespawnAI())
            {
                return;
            }


        }

        private bool DespawnAI()
        {
            if(NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(false);
            }

            Player player = Main.player[NPC.target];
            if(player.dead)
            {
                NPC.velocity.Y -= 0.04f;
                NPC.EncourageDespawn(10);
                return true;
            }
            return false;
        }

        // Skills
        private void SkillGearsOfTime()
        {
            for(int i = 0; i < 255; i++)
            {
                Player player = Main.player[i];
                if(player.active && !player.dead)
                {
                    float distance = Vector2.Distance(NPC.Center, player.Center);
                    if(distance < 200f)
                    {
                        TemporalBuffPlayer temporalPlayer = player.GetModPlayer<TemporalBuffPlayer>();
                        if(!player.HasBuff(DoomBuff()))
                        {
                            // To Do: Replace this with a Projectile located on the Player
                            temporalPlayer.ApplyDoom(5);
                        }
                    }
                }
            }
        }

        private void SkillTimeWarp()
        {
            for(int i = 0; i < 255; i++)
            {
                Player player = Main.player[i];
                if(player.active && !player.dead)
                {
                    TemporalBuffPlayer temporalPlayer = player.GetModPlayer<TemporalBuffPlayer>();
                    if(player.HasBuff(DoomBuff()))
                    {
                        // Replace with a Projectile
                        temporalPlayer.ReApplyDoom();
                    }
                }
            }
        }

        // Freezes all targets with doom for a set amount of time
        private void SkillTimeStop()
        {

        }

        // Fully Heals the Boss. Only used once per battle
        private void SkillTimeReversal()
        {

        }

        // Spins the Clock and depending on the outcome will decide 
        // what happens
        
        private void SkillTimeRoulette()
        {
            // 1 Voral Blade
            // 2 Thousand Arrows
            // 3 Infernal Blaze
            // 4 Absolute Zero
            // 5 Tempest
            // 6 Time Stop
            // 7 Summon Minion
            // 8 ???
            // 9 ???
            // 10 ???
            // 11 ???
            // 12 Time Warp
        }

        // Deals Melee Damage 
        private void SkillVorpalBlade()
        {

        }

        // Deals Ranged Damage
        private void SkillThousandArrows()
        {

        }

        // Deals Magic Damage
        private void SkillInfernalBlaze()
        {

        }

        // Deals Magic Damage
        private void SkillAbsoluteZero()
        {

        }

        // Deals Magic Damage
        private void SkillTempest()
        {

        }

        // Summons a Minion
        private void SkillSummonMinion()
        {

        }





        // Drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            
        }


    }
}
