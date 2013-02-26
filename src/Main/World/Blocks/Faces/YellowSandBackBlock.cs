﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using API.Generic;

using Mogre;

namespace Game.World.Blocks
{
    class YellowSandBackBlock : VanillaBlock
    {
        public YellowSandBackBlock() {
            this.mName = "YellowSand back";
            this.mMaterial = "cube/YellowSand";
        }

        public override BlockFace[] getFaces() { 
            return new BlockFace[] {
                BlockFace.backFace,
            };
        }
    }
}
