﻿using Game.Input;
using Game.GUIs;
using Game.BaseApp;

namespace Game.States
{
    class MenuState : State
    {
        public MenuState(StateManager stateMgr) : base(stateMgr, "Menu") { }
        
        public override void Show()
        {
            new MainMenu(this.mStateMgr);
            OgreForm.SelectBar.Visible = false;
            this.mStateMgr.Controller.CursorVisibility = true;
        }

        public override void Update(float frameTime)
        {
            if (this.mStateMgr.Controller.HasActionOccured(UserAction.Start))
                this.mStateMgr.RequestStatePop();
            if (this.mStateMgr.Controller.HasActionOccured(UserAction.Jump))
                this.mStateMgr.RequestStatePush(typeof(GameState));
        }
    }
}
