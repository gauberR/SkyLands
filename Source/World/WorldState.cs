﻿using System;
using System.Collections.Generic;
using Mogre;

using Game.CharacSystem;
using Game.BaseApp;

namespace Game {

    public partial class World
    {
        public override void Hide() { }
        public override void Show() { }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_F1))
            {
                this.mIsDebugMode = !this.mIsDebugMode;

                ((Player)this.mCharacMgr.GetCharacter()).IsDebugMode = this.mIsDebugMode;
                
                if (this.mIsDebugMode)
                {
                    Camera cam = this.mCharacMgr.MainPlayerCam.Camera;
                    Vector3 position = cam.RealPosition;
                    Quaternion orientation = cam.RealOrientation;
                    cam.DetachFromParent();
                    cam.Position = position;
                    cam.Orientation = orientation;
                    
                    this.mCameraMan = new CameraMan(cam);
                }
                else 
                    this.mCharacMgr.MainPlayerCam.InitCamera();
            }

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_F2))
                Console.WriteLine(this.mCaelumSystem.Sun.Node.Position);

            float delta = 150;
            if (this.mStateMgr.Input.MouseMoveZ > 0) { this.mCaelumSystem.TimeScale += delta; }
            else if (this.mStateMgr.Input.MouseMoveZ < 0) { this.mCaelumSystem.TimeScale -= delta; }

            this.mCharacMgr.Update(frameTime);

            if (!this.mCharacMgr.GetCharacter().IsMoving)
            {
                MoisManager input = ((Player)this.mCharacMgr.GetCharacter()).Input;
                this.mCameraMan.MouseMovement(input.MouseMoveX, input.MouseMoveY);
                this.mCameraMan.UpdateCamera(frameTime, input);
            }

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE)) { this.mStateMgr.RequestStatePop(); }    // Return to the MenuState
        }

        public override void Shutdown() {
            if (this.mStateMgr == null) { return; }

            this.mStateMgr.Window.PreViewportUpdate -= this.mCaelumSystem.PreViewportUpdate;
            this.mCaelumSystem.Shutdown(true);
            this.mStateMgr.SceneManager.ClearScene();

            this.mStateMgr = null;
            this.mIsStartedUp = false;
        }
    }
}
