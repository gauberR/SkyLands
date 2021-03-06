﻿using Mogre;
using CaelumSharp;

using Game.BaseApp;
using Game.States;

namespace Game.Sky
{
    public class SkyMgr
    {
        private StateManager mStateMgr;
        private CaelumSystem mCaelumSystem;
        private RootLstn mListener;

        public SkyMgr(StateManager stateMgr)
        {
            this.mStateMgr = stateMgr;
            this.mCaelumSystem = new CaelumSystem(this.mStateMgr.Root, this.mStateMgr.SceneMgr, CaelumSystem.CaelumComponent.None);
            this.mListener = new RootLstn(TypeLstn.FrameStarted, mCaelumSystem.FrameStarted);

            this.CreateSky(); this.AddListeners();
        }

        private void CreateSky()
        {
            this.mCaelumSystem.AttachViewport(this.mStateMgr.Viewport);

            this.mCaelumSystem.GetUniversalClock().SetGregorianDateTime(2012, 12, 21, 12, 0, 0);
            this.mCaelumSystem.TimeScale = 25;
            //this.mCaelumSystem.EnsureSingleLightSource = true;
            //this.mCaelumSystem.EnsureSingleShadowSource = true;
            //this.mCaelumSystem.AutoNotifyCameraChanged = false;

            /* Sky */
            this.mCaelumSystem.SkyDome = new SkyDome(this.mStateMgr.SceneMgr, this.mCaelumSystem.GetCaelumCameraNode());

            /* Sun */
            this.mCaelumSystem.ManageAmbientLight = false;
            this.mCaelumSystem.MinimumAmbientLight = new ColourValue(0.1f, 0.1f, 0.1f);
            this.mCaelumSystem.Sun = new SpriteSun(this.mStateMgr.SceneMgr, this.mCaelumSystem.GetCaelumCameraNode(), "Custom_sun_disc.png", 4);
            this.mCaelumSystem.Sun.AmbientMultiplier  = new ColourValue(0.8f, 0.8f, 0.8f);
            this.mCaelumSystem.Sun.DiffuseMultiplier  = new ColourValue(3, 3, 2.7f);
            this.mCaelumSystem.Sun.SpecularMultiplier = new ColourValue(5, 5, 5);
            this.mCaelumSystem.Sun.AutoDisable = true;
            this.mCaelumSystem.Sun.AutoDisableThreshold = 0.05f;

            /* Moon */
            this.mCaelumSystem.Moon = new Moon(this.mStateMgr.SceneMgr, this.mCaelumSystem.GetCaelumCameraNode());
            this.mCaelumSystem.Moon.DiffuseMultiplier  = new ColourValue(2, 2, 1.7f);
            this.mCaelumSystem.Moon.SpecularMultiplier = new ColourValue(4, 4, 4);

            /* Stars */
            this.mCaelumSystem.PointStarfield = new PointStarfield(this.mStateMgr.SceneMgr, this.mCaelumSystem.GetCaelumCameraNode());
            this.mCaelumSystem.PointStarfield.MagnitudeScale = 1.05f;

            /* Fog */
            this.mCaelumSystem.SceneFogDensityMultiplier = 0.0f;
            //this.mCaelumSystem.SceneFogColourMultiplier  = new ColourValue(0.3f, 0.3f, 0.3f);
            this.mCaelumSystem.ManageSceneFog = true;
        }

        public void Update()
        {
            float delta = 0;
            if (this.mStateMgr.Controller.IsKeyDown(MOIS.KeyCode.KC_PGUP)) { delta += 40; }
            if (this.mStateMgr.Controller.IsKeyDown(MOIS.KeyCode.KC_PGDOWN)) { delta -= 40; }
            this.mCaelumSystem.TimeScale += delta;
        }

        public void AddListeners()
        {
            this.mStateMgr.Window.PreViewportUpdate += mCaelumSystem.PreViewportUpdate;
            this.mStateMgr.AddFrameLstn(this.mListener);
        }

        public void RemoveListeners()
        {
            this.mStateMgr.Window.PreViewportUpdate -= mCaelumSystem.PreViewportUpdate;
            this.mStateMgr.RemoveFrameLstn(this.mListener);
        }

        public void Shutdown()
        {
            this.RemoveListeners();
            this.mCaelumSystem.Shutdown(true);
        }
    }
}
