﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;

using Game.States;
using Game.Input;
using Game.Shoot;
using Game.RTS;

using API.Ent;

namespace Game.CharacSystem
{
    public class CharacMgr : CharacterMgr
    {
        private const string SINBAD_MESH = "Sinbad.mesh";
        private const string ROBOT_MESH = "robot.mesh";

        private readonly List<VanillaCharacter>[] mCharacters;
        private readonly StateManager             mStateMgr;
        private readonly Controller               mController;
        private readonly API.Geo.World            mWorld;
        private readonly BulletManager            mBulletMgr;
        private readonly User                     mUser;

        public StateManager  StateMgr   { get { return this.mStateMgr; } }
        public SceneManager  SceneMgr   { get { return this.mStateMgr.SceneMgr; } }
        public Controller    Controller { get { return this.mController; } }
        public API.Geo.World World      { get { return this.mWorld; } }
        public VanillaPlayer MainPlayer { get; private set; }
        public BulletManager BulletMgr  { get { return this.mBulletMgr; } }

        public CharacMgr(StateManager stateMgr, API.Geo.World world, User user, BulletManager bulletMgr = null)
        {
            this.mStateMgr = stateMgr;
            this.mController = stateMgr.Controller;
            this.mWorld = world;
            this.mUser = user;
            this.mBulletMgr = bulletMgr;
            this.mCharacters = new List<VanillaCharacter>[Enum.GetValues(typeof(Faction)).Length];
            for(int i = 0; i < this.mCharacters.Length; i++)
                this.mCharacters[i] = new List<VanillaCharacter>();
        }

        public void AddCharacter(CharacterInfo info)
        {
            if (info.RTS == null)
            {
                if (info.Faction == Faction.Red)
                    info.RTS = this.mStateMgr.MainState.RTSMgr.AIRTS;
                else
                    info.RTS = this.mStateMgr.MainState.RTSMgr.PlayerRTS;
            }
            
            if (info.IsPlayer)
            {
                if (this.mStateMgr.GameInfo.IsInEditorMode)
                    throw new Exception("Can't add player in the story editor");
                
                VanillaPlayer player = new VanillaPlayer(this, SINBAD_MESH, info);
                this.mCharacters[(int)info.Faction].Add(player);
                if (this.MainPlayer == null)
                {
                    player.MakeHimMainPlayer(this.mUser);
                    this.MainPlayer = player;
                }
            }
            else { this.mCharacters[(int)info.Faction].Add(new VanillaNonPlayer(this, ROBOT_MESH, info)); }

            LogManager.Singleton.DefaultLog.LogMessage("Character " + info.Name + " added");
        }

        public List<VanillaCharacter> GetFactionCharacters(Faction faction) { return this.mCharacters[(int)faction]; }
        public VanillaCharacter GetCharacterById(int id)
        {
            return this.mCharacters.Select(characList => characList.Find(charac => charac.Info.Id == id)).FirstOrDefault(charac => charac != null);
        }

        public void Update(float frameTime)
        {
            foreach (List<VanillaCharacter> characList in this.mCharacters)
            {
                List<VanillaCharacter> needToRemove = new List<VanillaCharacter>();
                for (int i = 0; i < characList.Count; i++)
                {
                    if (characList[i].WaitForRemove)
                    {
                        if (characList[i] is VanillaPlayer)
                            this.mStateMgr.RequestStatePush(typeof(DeathMenuState));
                        else
                        {
                            VanillaCharacter charac = characList[i];
                            needToRemove.Add(charac);
                            this.mStateMgr.MainState.User.RemoveSelectedAlly((VanillaNonPlayer)charac);
                            charac.Dispose();
                            continue;
                        }
                    }
                    characList[i].Update(frameTime);
                }

                foreach (VanillaCharacter charac in needToRemove)
                    characList.Remove(charac);
            }
        }

        public void Dispose()
        {
            foreach (List<VanillaCharacter> characList in this.mCharacters)
            {
                while (characList.Count > 0)
                {
                    characList[0].Dispose(false);
                    characList.RemoveAt(0);
                }
            }

            CharacterInfo.ResetID();
        }
    }
}
