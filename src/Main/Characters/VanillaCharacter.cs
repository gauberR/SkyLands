﻿using System;
using System.Collections.Generic;
using Mogre;

using Game.World;

namespace Game.CharacSystem
{
    public abstract class VanillaCharacter
    {
        private readonly Vector3 SCALE_CHARAC = new Vector3(9, 10, 9);
        private const float WALK_SPEED = 300.0f;
        private const float GRAVITY_ACCEL_T0 = -750;
        private const float GRAVITY_ACCEL_TMAX = -1000;
        private const float T_MAX = 4;  // Time until the character reach its max speed fall
        private const float GRAVITY_CONST_B = T_MAX * (GRAVITY_ACCEL_TMAX - 1) / (GRAVITY_ACCEL_T0 - GRAVITY_ACCEL_TMAX);
        private const float GRAVITY_CONST_A = GRAVITY_ACCEL_T0 * GRAVITY_CONST_B;

        protected CharacMgr     mCharacMgr;
        protected SceneNode     mNode;
        protected AnimationMgr  mAnimMgr;
        protected CharacterInfo mCharInfo;
        protected MovementInfo  mMovementInfo;
        protected Anim[]        mRunAnims;
        private Anim[]          mIdleAnims;
        private Vector3         mBoundingBoxSize;
        private Timer           mTimeOfFall;
        private Vector3[]       mBlocksCollision;

        public SceneNode Node         { get { return this.mNode; } }
        public float     Height       { get { return this.mBoundingBoxSize.y; } }
        public bool      IsMoving     { get { return this.mMovementInfo.IsMoving; } }
        public Vector3[] Blocks       { get { return this.mBlocksCollision; } }
        public Vector3   FeetPosition
        {
            get { return this.mNode.Position - new Vector3(0, this.Height / 2, 0); }
            set { this.mNode.SetPosition(value.x, value.y + this.Height / 2, value.z); }
        }

        protected VanillaCharacter(CharacMgr characMgr, string meshName, CharacterInfo charInfo)
        {
            this.mCharacMgr = characMgr;
            this.mCharInfo = charInfo;
            this.mMovementInfo = new MovementInfo();
            this.mTimeOfFall = new Timer();
            
            /* Create entity and node */
            SceneManager sceneMgr = characMgr.SceneMgr;
            Entity playerEnt = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            playerEnt.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            Entity swordL = sceneMgr.CreateEntity("Sword.mesh");
            playerEnt.AttachObjectToBone("Sheath.L", swordL);
            Entity swordR = sceneMgr.CreateEntity("Sword.mesh");
            playerEnt.AttachObjectToBone("Sheath.R", swordR);

            this.mNode = sceneMgr.RootSceneNode.CreateChildSceneNode("CharacterNode_" + this.mCharInfo.Id);
            this.mNode.AttachObject(playerEnt);
            this.mNode.Scale(SCALE_CHARAC);
            
            //this.mBoundingBoxSize = playerEnt.BoundingBox.Size * SCALE_CHARAC;
            this.mBoundingBoxSize = new Vector3(MainWorld.CUBE_SIDE, 2 * MainWorld.CUBE_SIDE, MainWorld.CUBE_SIDE);
            this.FeetPosition = this.mCharInfo.SpawnPoint;
            this.mBlocksCollision = this.CreateHitBlocks(this.FeetPosition);

            /* Create Animations */
            this.mIdleAnims = new Anim[] { Anim.IdleBase, Anim.IdleTop };
            this.mRunAnims  = new Anim[] { Anim.RunBase, Anim.RunTop };
            this.mAnimMgr   = new AnimationMgr(playerEnt.AllAnimationStates);
            this.mAnimMgr.SetAnims(this.mIdleAnims);
        }

        public void Update(float frameTime)
        {
            /* Apply gravity */
            Vector3 translation = new Vector3();
            if (!this.mCharacMgr.World.HasCharacCollision(this.mBlocksCollision, CubeFace.underFace))
            {
                if (!this.mAnimMgr.CurrentAnims.Contains(Anim.JumpLoop))
                {
                    this.mAnimMgr.SetAnims(Anim.JumpLoop);
                    this.mTimeOfFall.Reset();
                }

                float sec = (float)this.mTimeOfFall.Milliseconds / 1000;
                if (sec >= T_MAX) { translation += GRAVITY_ACCEL_TMAX * Vector3.UNIT_Y; }
                else { translation += (sec + GRAVITY_CONST_A) / (sec + GRAVITY_CONST_B) * Vector3.UNIT_Y; }
            }
            else if (this.mAnimMgr.CurrentAnims.Contains(Anim.JumpLoop))  // The character has reach the ground
                this.mAnimMgr.SetAnims(Anim.JumpEnd);

            /* Actualise mMovementInfo */
            if (this.mCharInfo.IsPlayer) { (this as VanillaPlayer).Update(frameTime); }
            else                         { (this as VanillaNonPlayer).Update(frameTime); }

            /* Apply mMovementInfo */
            if (this.mMovementInfo.IsMoving)
            {
                translation += WALK_SPEED * this.mMovementInfo.MoveDirection;
                this.mNode.Yaw(this.mMovementInfo.YawValue * frameTime);
            }
            this.Translate(translation, frameTime);

            /* Update animations */
            if (this.mAnimMgr.CurrentAnims.Count == 0) { this.mAnimMgr.AddAnims(this.mIdleAnims); } // By default apply idle anim
            this.mAnimMgr.Update(frameTime);
            this.mMovementInfo.ClearInfo();
        }

        private void Translate(Vector3 translate, float frameTime)
        {
            Vector3 tempPos = this.FeetPosition + translate;
            Vector3[] tempBlocks = this.CreateHitBlocks(tempPos);
            if (translate.y < 0 && this.mCharacMgr.World.HasCharacCollision(tempBlocks, CubeFace.underFace))
            {
                translate.y = this.FeetPosition.y - tempBlocks[0].y;
            }

            /* Here translate has been modified to avoid collisions */
            this.mBlocksCollision = this.CreateHitBlocks(this.FeetPosition + translate);
            this.mNode.Translate(translate * frameTime, Mogre.Node.TransformSpace.TS_LOCAL);
        }

        private Vector3[] CreateHitBlocks(Vector3 position)
        {
            Vector3[] hitBlocks = new Vector3[2];
            hitBlocks[0] = this.mCharacMgr.World.GetBlockAbsPosFromAbs(position, new Vector3());
            hitBlocks[1] = hitBlocks[0] + new Vector3(0, MainWorld.CUBE_SIDE, 0);

            return hitBlocks;
        }
    }
}