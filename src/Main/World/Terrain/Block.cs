﻿using System;
using System.Collections.Generic;
using Mogre;

using Material = API.Generic.Material;

namespace Game.Terrain
{    
    public class Block
    {
        private Material mType;// { get { return mType; } private set { mType = value; UpdateMaterial(); } }
        private SceneNode mNode   { get; set; }
        //public Vector3 mBlockPos { get; set; }
        private Vector3 mBlockPos;


        public Vector3 blockPos    { get { return this.mBlockPos; } }
        public Vector3 setBlockPos { set { this.mBlockPos = value; } }


        public Block(Vector3 blockPos, Material blockType = Material.AIR)
        {
            this.mType = blockType;
            this.mBlockPos = blockPos;
        }

        public bool hasSceneNode() { return (this.mNode != null); }

        public bool IsAir() { return (mType == Material.AIR); }
        
        public string getType() { return "1";}

        public void setType(Material type) { this.mType = type; }


        public override string ToString() { return this.mBlockPos.ToString(); }
    }
}