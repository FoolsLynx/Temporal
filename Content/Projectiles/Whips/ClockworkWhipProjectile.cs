using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temporal.Content.Projectiles.Whips
{
    internal class ClockworkWhipProjectile : WhipProjectile
    {
        public override void SafeSetDefaults()
        {
            segments = 30;
            rangeMultiplier = 0.85f;
        }

        public override void CustomAI()
        {
            
        }
    }
}
