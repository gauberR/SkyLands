﻿using Game.RTS;
using Mogre;

using API.Geo.Cuboid;
using API.Generic;

namespace Game.World.Blocks
{
    public abstract class VanillaBlock : Block
    {
        protected string mName;
        protected string mMaterial = "";
        protected byte   mId = 255;     //Warning do not set 255 as a value
        protected int    mMeshType = 0; //For custum mesh

        protected static int CUBE_SIDE = Cst.CUBE_SIDE;

        public override int    getMeshType()    { return this.mMeshType; }
        public override string getName()        { return this.mName; }
        public override string getMaterial()    { return this.mMaterial; }
        public override byte   getId()          { return this.mId; }
        public override string getItemTexture() { return this.mItemTexture; }

        public override string getFace(int i) { return this.mMaterial; }

        public override void onCreation(Vector3 position) { }
        public override void onDeletion() { }

        public override bool onRightClick() { return true; }
        public override bool onLeftClick()  { return true; }

        public override void onBlockLeave(API.Ent.Entity e, Vector3 position) { }
        public override void onBlockEnter(API.Ent.Entity e, Vector3 position) { e.updateTargets(); }

        public static float getBlockOnRay(Island island, Ray ray, float distMax, float distMin)
        {
            Vector3 relBlockPos;
            Block actBlock;
            return getBlockOnRay(island, ray, distMax, distMin, out relBlockPos, out actBlock);
        }
        
        public static float getBlockOnRay(Island island, Ray ray, float distMax, float minDist,
                                          out Vector3 relBlockPos, out Block actBlock)
        {
            float distance = minDist;
            relBlockPos = MainWorld.AbsToRelative(ray.GetPoint(distance));

            do
            {
                Vector3 prevRelBlockPos = relBlockPos;
                while (prevRelBlockPos == relBlockPos)
                {
                    distance += 3;
                    relBlockPos = MainWorld.AbsToRelative(ray.GetPoint(distance));
                }
                actBlock = island.getBlock(relBlockPos, false);
            } while (actBlock is Air && distance < distMax);

            return distance;
        }
    }
}
