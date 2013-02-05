﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.World.Blocks
{
    class LeavesBlock : VanillaBlock
    {
        public LeavesBlock() {
            this.mName = "Leaves";
            this.mMaterial = "cube/leaves";
        }
    }
}