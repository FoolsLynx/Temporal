using Terraria.ModLoader;

namespace Temporal
{
	public class TemporalMod : Mod
	{
		internal static TemporalMod Instance;

        public override void Load()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
        }
    }
}