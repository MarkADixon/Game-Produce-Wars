#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ParallaxEngine;
using ProduceWars.Managers;
#endregion

namespace ProduceWars
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization
        Texture2D MainSingle, MainOption, MainCheat, MainSound, MainQuit;
        //ContentManager contentInstance;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {
        }

        public override void LoadContent(ContentManager _content)
        {
            

            //commented, instances its own content manager
            //content = _content;
            //if (contentInstance == null)
            //    contentInstance = new ContentManager(ScreenManager.Game.Services, "Content");

            LevelDataManager.UItextures.TryGetValue("MainSingle", out MainSingle);
            LevelDataManager.UItextures.TryGetValue("MainOption", out MainOption);
            LevelDataManager.UItextures.TryGetValue("MainCheat", out MainCheat);
            LevelDataManager.UItextures.TryGetValue("MainSound", out MainSound);
            LevelDataManager.UItextures.TryGetValue("MainQuit", out MainQuit);
            LevelDataManager.UItextures.TryGetValue("Title", out menuTitleGraphic);
            isMenuTitleGraphic = true;
            menuTitleGraphicScale = 1.0f;

            //create textured menu entries
            MenuEntry playGameMenuEntry = new MenuEntry(MainSingle);
            MenuEntry optionsMenuEntry = new MenuEntry(MainOption);
            MenuEntry cheatMenuEntry = new MenuEntry(MainCheat);
            MenuEntry soundMenuEntry = new MenuEntry(MainSound);
            MenuEntry exitMenuEntry = new MenuEntry(MainQuit);

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            cheatMenuEntry.Selected += CheatMenuEntrySelected;
            soundMenuEntry.Selected += SoundMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(cheatMenuEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            SoundManager.LoadMusic(LevelDataManager.levelContent, -1);
        }




        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
        }

        #endregion



        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new WorldSelectScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
            ScreenManager.AddScreen(new MainOptionScreen(false), e.PlayerIndex);
        }

        void CheatMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CheatMenuScreen(), e.PlayerIndex);
        }

        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new SoundMenuScreen(), e.PlayerIndex);
        }
        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }

        #endregion
    }
}
