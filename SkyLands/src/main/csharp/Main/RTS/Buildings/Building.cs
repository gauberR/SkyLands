﻿using System.Linq;
using System.Collections.Generic;

using Game.BaseApp;
using Game.CharacSystem;
using Game.States;
using Game.World.Generator;
using Mogre;

using Game.World.Blocks;
using API.Geo.Cuboid;

namespace Game.RTS
{
    public abstract class Building
    {
        private const string ghostBlock = "GhostBlock";

        protected StateManager mStateMgr;
        protected Island mIsland;
        protected Dictionary<byte, int> mNeededRessources;
        protected Vector3 mConsBlockPos;
        protected List<Vector3> mClearZone;
        protected byte mColoredBlock;
        protected byte[, ,] mBuilding;
        protected int mYDiff;

        public string Selection { get; set; }
        public Faction Faction { get; private set; }
        public Vector3 Position { get; protected set; }
        public Vector3 Size { get; protected set; }
        public bool RessourcesDisplayed { get; set; }
        private Vector3 RealPos { get { return this.Position - Vector3.UNIT_Y * this.mYDiff; } }
        private bool mIsCreated;
        private int mSymetricFactor = 1;

        protected Building(StateManager stateMgr, Island island, Vector3 pos, Faction fact, string selection)
        {
            this.mStateMgr = stateMgr;
            this.mIsland = island;
            this.Faction = fact;
            this.mColoredBlock = (byte) ((this.Faction == Faction.Blue) ? 32 : 31);
            this.mNeededRessources = new Dictionary<byte, int>();
            this.mClearZone = new List<Vector3>();
            this.Position = pos;
            this.Selection = selection;
            this.Init();
            this.DrawRemainingRessource();
        }

        protected abstract void Init();
        protected virtual void Create()
        {
            this.mIsCreated = true;
            if (this.mBuilding != null)
            {
                
            }
        }

        public void DrawRemainingRessource()
        {
            int i = 40;
            foreach (byte b in this.mNeededRessources.Keys)
            {
                GUI.SetBlockAt(i, VanillaChunk.staticBlock[VanillaChunk.byteToString[b]].getItemTexture(), this.mNeededRessources[b]);
                i++;
            }
            this.RessourcesDisplayed = true;
        }

        public bool IsCharactInBat(VanillaCharacter charac)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 blockPos = charac.BlockPosition + Vector3.UNIT_Y * i;
                if (this.RealPos.x <= blockPos.x && blockPos.x < this.RealPos.x + this.Size.x &&
                   this.RealPos.y <= blockPos.y && blockPos.y < this.RealPos.y + this.Size.y &&
                   this.RealPos.z <= blockPos.z && blockPos.z < this.RealPos.z + this.Size.z)
                    return true;
            }
            return false;
        }

        public void WaitForRessources()
        {
            this.Create();
            if (this.IsCharactInBat(this.mStateMgr.MainState.CharacMgr.MainPlayer))
                this.mSymetricFactor = -1;
            this.ClearScene();
            this.BuildGhost(true);
        }

        private void ClearScene()
        {
            for (int x = 0; x < this.Size.x; x++)
            {
                for (int y = 0; y < this.Size.y; y++)
                {
                    for (int z = 0; z < this.Size.z; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) { continue; }
                        Vector3 pos = this.RealPos + new Vector3(x, 0, z) * this.mSymetricFactor + Vector3.UNIT_Y * y;
                        if (this.mBuilding[x, y, z] != 0 || this.mClearZone.Contains(new Vector3(x, y, z)) &&
                            !(this.mIsland.getBlock(pos, false) is ConstructionBlock))
                            this.mIsland.removeFromScene(pos);
                    }
                }
            }
        }

        private void BuildGhost(bool create)
        {
            for (int x = 0; x < this.Size.x; x++)
            {
                for (int y = 0; y < this.Size.y; y++)
                {
                    for (int z = 0; z < this.Size.z; z++)
                    {
                        Vector3 pos = this.RealPos + new Vector3(x, 0, z) * this.mSymetricFactor + Vector3.UNIT_Y * y;
                        if (this.mBuilding[x, y, z] != 0 && !(this.mIsland.getBlock(pos, false) is ConstructionBlock))
                        {
                            if(create)
                                this.mIsland.addBlockToScene(pos, ghostBlock);
                            else
                                this.mIsland.removeFromScene(pos);
                        }
                    }
                }
            }
        }

        public void Build()
        {
            if (!this.mIsCreated) { this.Create(); }
            if (this.IsCharactInBat(this.mStateMgr.MainState.CharacMgr.MainPlayer))
                this.mSymetricFactor = -1;
            this.ClearScene();

            for (int x = 0; x < this.Size.x; x++)
            {
                for (int y = 0; y < this.Size.y; y++)
                {
                    for (int z = 0; z < this.Size.z; z++)
                    {
                        Vector3 pos = this.RealPos + new Vector3(x, 0, z) * this.mSymetricFactor + Vector3.UNIT_Y * y;
                        if (this.mBuilding[x, y, z] != 0 && !(this.mIsland.getBlock(pos, false) is ConstructionBlock))
                        {
                            string name = VanillaChunk.staticBlock[VanillaChunk.byteToString[this.mBuilding[x, y, z]]].getName();
                            this.mIsland.addBlockToScene(pos, name);
                        }
                    }
                }
            }
            this.OnBuild();
        }

        protected virtual void OnBuild()
        {
            if (this.mConsBlockPos != Vector3.ZERO)
            {
                this.mIsland.removeFromScene(this.Position);
                this.mIsland.addBlockToScene(this.Position + this.mConsBlockPos, "Construction");
            }
            this.mStateMgr.MainState.BuildingMgr.ActConstBlock = null;
            User.RequestBuilderClose = true;
        }

        public void OnDrop(int pos, int newAmount)
        {
            string imgName = GUI.GetImageAt(pos);
            byte b = VanillaChunk.textureToBlock[imgName].getId();
            this.mNeededRessources[b] = newAmount;

            if (this.mNeededRessources.All(keyValPair => keyValPair.Value <= 0))
                this.Build();
        }
    }
}
