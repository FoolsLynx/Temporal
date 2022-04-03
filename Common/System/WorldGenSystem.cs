using System.Collections.Generic;
using Temporal.Common.System.Generation;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Temporal.Common.System
{
    internal class WorldGenSystem : ModSystem
    {

        internal static int towerX;
        internal static int towerY;

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int towerIndex = tasks.FindIndex(x => x.Name.Equals("Pyramids"));
            if(towerIndex != -1)
            {
                tasks.Insert(towerIndex + 1, new TemporalTowerGenPass().OnComplete(delegate(GenPass pass)
                {
                    TemporalTowerGenPass towerPass = pass as TemporalTowerGenPass;
                    towerX = towerPass.towerX;
                    towerY = towerPass.towerY;
                }));
            }
        }
    }
}
