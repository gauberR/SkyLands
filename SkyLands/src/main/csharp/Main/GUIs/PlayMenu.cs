﻿using System;
using Awesomium.Core;
using Mogre;

using Game.States;
using System.Windows.Forms;

using Game.BaseApp;

namespace Game.GUIs {
    public class PlayMenu : GUI {
        private readonly StateManager mStateMgr;

        public PlayMenu(StateManager stateMgr)
            : base(new Vector2(0, 0), new Vector2(404, 404), "play.html", DockStyle.Fill) {
            this.mStateMgr = stateMgr;
        }

        public override void onDocumentReady(object sender, UrlEventArgs e) {
            base.onDocumentReady(sender, e);
            JSObject j = OgreForm.webView.CreateGlobalJavascriptObject("PlayMenuJSObject");
            j.Bind("back", false, HideAndPlay);
        }

        private void HideAndPlay(object sender, EventArgs e) {
            Visible = false;
            new MainMenu(this.mStateMgr);
        }

    }
}