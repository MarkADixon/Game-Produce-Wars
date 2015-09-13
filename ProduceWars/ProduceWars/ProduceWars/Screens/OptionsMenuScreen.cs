#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.IO;
using Microsoft.Xna.Framework;
using ParallaxEngine;
using ProduceWars.Managers;
#endregion

//
//  THIS IS A TESTING AND DEBUG OPTIONS SCREEN!!
//
namespace ProduceWars
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry isMusicMenuEntry;
        MenuEntry resetSaveMenuEntry;
        MenuEntry locksMenuEntry;
        MenuEntry minFiringPowerMenuEntry;
        MenuEntry maxFiringPowerMenuEntry;
        MenuEntry gravityMenuEntry;
        MenuEntry physicsScaleMenuEntry;
        MenuEntry explosiveForceMenuEntry;
        MenuEntry explosiveRadiusMenuEntry;
        MenuEntry fruitHPMenuEntry;
        MenuEntry fruitMassMenuEntry;
        MenuEntry fruitFrictionMenuEntry;
        MenuEntry fruitRestitutionMenuEntry;
        MenuEntry veggieHPMenuEntry;
        MenuEntry veggieMassMenuEntry;
        MenuEntry veggieFrictionMenuEntry;
        MenuEntry veggieRestitutionMenuEntry;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {

            // Create our menu entries.
            isMusicMenuEntry = new MenuEntry(string.Empty);
            resetSaveMenuEntry = new MenuEntry("Reset Save Data");
            locksMenuEntry = new MenuEntry("Level/Fruit Locked : ");
            minFiringPowerMenuEntry = new MenuEntry(string.Empty);
            maxFiringPowerMenuEntry = new MenuEntry(string.Empty);
            gravityMenuEntry = new MenuEntry(string.Empty);
            physicsScaleMenuEntry = new MenuEntry(string.Empty);
            explosiveForceMenuEntry = new MenuEntry(string.Empty);
            explosiveRadiusMenuEntry = new MenuEntry(string.Empty);
            fruitHPMenuEntry = new MenuEntry(string.Empty);
            fruitMassMenuEntry = new MenuEntry(string.Empty);
            fruitFrictionMenuEntry = new MenuEntry(string.Empty);
            fruitRestitutionMenuEntry = new MenuEntry(string.Empty);
            veggieHPMenuEntry = new MenuEntry(string.Empty);
            veggieMassMenuEntry = new MenuEntry(string.Empty);
            veggieFrictionMenuEntry = new MenuEntry(string.Empty);
            veggieRestitutionMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            isMusicMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(isMusicMenuEntry_Selected);
            resetSaveMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(resetSaveMenuEntry_Selected);
            locksMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(locksMenuEntry_Selected);
            minFiringPowerMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(minFiringPowerMenuEntry_Plus);
            minFiringPowerMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(minFiringPowerMenuEntry_Minus);
            maxFiringPowerMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(maxFiringPowerMenuEntry_Plus);
            maxFiringPowerMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(maxFiringPowerMenuEntry_Minus);
            gravityMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(gravityMenuEntry_Plus);
            gravityMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(gravityMenuEntry_Minus);
            physicsScaleMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(physicsScaleMenuEntry_Plus);
            physicsScaleMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(physicsScaleMenuEntry_Minus);
            explosiveForceMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(explosiveForceMenuEntry_Plus);
            explosiveForceMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(explosiveForceMenuEntry_Minus);
            explosiveRadiusMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(explosiveRadiusMenuEntry_Plus);
            explosiveRadiusMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(explosiveRadiusMenuEntry_Minus);

            fruitHPMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(fruitHPMenuEntry_Plus);
            fruitHPMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(fruitHPMenuEntry_Minus);
            fruitMassMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(fruitMassMenuEntry_Plus);
            fruitMassMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(fruitMassMenuEntry_Minus);
            fruitFrictionMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(fruitFrictionMenuEntry_Plus);
            fruitFrictionMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(fruitFrictionMenuEntry_Minus);
            fruitRestitutionMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(fruitRestitutionMenuEntry_Plus);
            fruitRestitutionMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(fruitRestitutionMenuEntry_Minus);

            veggieHPMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(veggieHPMenuEntry_Plus);
            veggieHPMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(veggieHPMenuEntry_Minus);
            veggieMassMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(veggieMassMenuEntry_Plus);
            veggieMassMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(veggieMassMenuEntry_Minus);
            veggieFrictionMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(veggieFrictionMenuEntry_Plus);
            veggieFrictionMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(veggieFrictionMenuEntry_Minus);
            veggieRestitutionMenuEntry.Plus += new System.EventHandler<PlayerIndexEventArgs>(veggieRestitutionMenuEntry_Plus);
            veggieRestitutionMenuEntry.Minus += new System.EventHandler<PlayerIndexEventArgs>(veggieRestitutionMenuEntry_Minus);

            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(isMusicMenuEntry);
            MenuEntries.Add(resetSaveMenuEntry);
            MenuEntries.Add(locksMenuEntry);
            MenuEntries.Add(minFiringPowerMenuEntry);
            MenuEntries.Add(maxFiringPowerMenuEntry);
            //MenuEntries.Add(gravityMenuEntry);
            //MenuEntries.Add(physicsScaleMenuEntry);
            MenuEntries.Add(explosiveForceMenuEntry);
            MenuEntries.Add(explosiveRadiusMenuEntry);
            MenuEntries.Add(fruitHPMenuEntry);
            MenuEntries.Add(fruitMassMenuEntry);
            MenuEntries.Add(fruitFrictionMenuEntry);
            MenuEntries.Add(fruitRestitutionMenuEntry);
            MenuEntries.Add(veggieHPMenuEntry);
            MenuEntries.Add(veggieMassMenuEntry);
            MenuEntries.Add(veggieFrictionMenuEntry);
            MenuEntries.Add(veggieRestitutionMenuEntry);
            MenuEntries.Add(back);
        }

        void locksMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.LocksOn = !GameSettings.LocksOn;
            locksMenuEntry.Text = "Level/Fruit Locks (Beta Testing): " + (GameSettings.LocksOn ? " On" : " Off");
        }

        void isMusicMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MusicOn = !GameSettings.MusicOn;
            isMusicMenuEntry.Text = "Music :" + (GameSettings.MusicOn ? " On" : " Off");
            SoundManager.PauseMusic(GameSettings.MusicOn);
        }

        void resetSaveMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Beta Test Feature:  Reset save data?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 1; j < 16; j++)
                {
                    LevelDataManager.levelData[i, j].unlocked = false;
                    LevelDataManager.levelData[i, j].starCollected = false;
                    LevelDataManager.levelData[i, j].shots = 0;
                    LevelDataManager.levelData[i, j].bestScore = 0;
                }
            }
            for (int i = 1; i<11; i++)
            {
                LevelDataManager.levelData[0, i].unlocked = true;
            }
            LevelDataManager.levelData[1, 1].unlocked = true;
            LevelDataManager.WriteSaveGameData();
        }
   
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            // make sure the device is ready
            if (LevelDataManager.SaveDevice.IsReady)
            {
                try
                {
                    // save a file asynchronously. this will trigger IsBusy to return true
                    // for the duration of the save process.
                    LevelDataManager.SaveDevice.SaveAsync(
                        LevelDataManager.containerName,
                        LevelDataManager.fileName_options,
                        stream =>
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                writer.WriteLine(GameSettings.MusicOn);
                                //writer.WriteLine(currentLanguage);
                                //writer.WriteLine(frobnicate);
                                //writer.WriteLine(elf);
                            }
                        });
                }
                catch
                {
                }
            }
            base.OnCancel(playerIndex);
        } //saves options

        void SetMenuEntryText()
        {
            isMusicMenuEntry.Text = "Music :" + (GameSettings.MusicOn ? " On" : " Off");
            locksMenuEntry.Text = "Level/Fruit Locks : " + (GameSettings.LocksOn ? " On" : " Off");
            minFiringPowerMenuEntry.Text = "Minimum Firing Power : " + GameSettings.MinFiringPower.ToString();
            maxFiringPowerMenuEntry.Text = "Target, Maximum Firepower! " + GameSettings.MaxFiringPower.ToString();
            gravityMenuEntry.Text = "World Gravity : " + GameSettings.Gravity;
            physicsScaleMenuEntry.Text = "Pixels Per Meter Physics Ratio : " + GameSettings.PhysicsScale;
            explosiveForceMenuEntry.Text = "Explosive force (TNTM x2, TNTL x4) : " + GameSettings.ExplosivePower;
            explosiveRadiusMenuEntry.Text = "Explosive effect radius multiplier (xTNT width): " + GameSettings.ExplosiveForceRadiusMultiplier;
            fruitHPMenuEntry.Text = "Fruit Hit Points : " + GameSettings.FruitHP.ToString();
            fruitMassMenuEntry.Text = "Fruit Mass : " + GameSettings.FruitMass.ToString();
            fruitFrictionMenuEntry.Text = "Fruit Friction : " + GameSettings.FruitFriction.ToString();
            fruitRestitutionMenuEntry.Text = "Fruit Restitution : " + GameSettings.FruitBouncy.ToString();
            veggieHPMenuEntry.Text = "Veggie Hit Points : " + GameSettings.VeggieHP.ToString();
            veggieMassMenuEntry.Text = "Veggie Mass : " + GameSettings.VeggieMass.ToString();
            veggieFrictionMenuEntry.Text = "Veggie Friction : " + GameSettings.VeggieFriction.ToString();
            veggieRestitutionMenuEntry.Text = "Veggie Restitution : " + GameSettings.VeggieBouncy.ToString();
        }
        #endregion

        #region Handle Input
        void minFiringPowerMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MinFiringPower += 25;
            if (GameSettings.MinFiringPower >= GameSettings.MaxFiringPower) GameSettings.MinFiringPower = GameSettings.MaxFiringPower;
            minFiringPowerMenuEntry.Text = "Minimum Firing Power : " + GameSettings.MinFiringPower.ToString();
        }
        void minFiringPowerMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MinFiringPower -= 25;
            if (GameSettings.MinFiringPower <= 0) GameSettings.MinFiringPower = 25;
            minFiringPowerMenuEntry.Text = "Minimum Firing Power : " + GameSettings.MinFiringPower.ToString();
        }
        void maxFiringPowerMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MaxFiringPower += 25;
            maxFiringPowerMenuEntry.Text = "Target, Maximum Firepower! " + GameSettings.MaxFiringPower.ToString();
        }
        void maxFiringPowerMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MaxFiringPower -= 25;
            if (GameSettings.MaxFiringPower <= GameSettings.MinFiringPower) GameSettings.MaxFiringPower = GameSettings.MinFiringPower;
            maxFiringPowerMenuEntry.Text = "Target, Maximum Firepower! " + GameSettings.MaxFiringPower.ToString();
        }
        void gravityMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.Gravity += 0.2f;
            gravityMenuEntry.Text = "World Gravity : " + GameSettings.Gravity;
        }
        void gravityMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.Gravity -= 0.2f;
            gravityMenuEntry.Text = "World Gravity : " + GameSettings.Gravity;
        }
        void physicsScaleMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.PhysicsScale += 2;
            physicsScaleMenuEntry.Text = "Pixels Per Meter Physics Ratio : " + GameSettings.PhysicsScale;
        }
        void physicsScaleMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.PhysicsScale -= 2;
            if (GameSettings.PhysicsScale <= 0) GameSettings.PhysicsScale = 2;
            physicsScaleMenuEntry.Text = "Pixels Per Meter Physics Ratio : " + GameSettings.PhysicsScale;
        }
        void explosiveForceMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.ExplosivePower += 0.1f;
            explosiveForceMenuEntry.Text = "Explosive force (TNTM x2, TNTL x3) : " + GameSettings.ExplosivePower;
        }
        void explosiveForceMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.ExplosivePower -= 0.1f;
            explosiveForceMenuEntry.Text = "Explosive force (TNTM x2, TNTL x3) : " + GameSettings.ExplosivePower;
        }
        void explosiveRadiusMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.ExplosiveForceRadiusMultiplier += 5f;
            explosiveRadiusMenuEntry.Text = "Explosive effect radius base: " + (int)GameSettings.ExplosiveForceRadiusMultiplier;
        }
        void explosiveRadiusMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.ExplosiveForceRadiusMultiplier -= 5f;
            explosiveRadiusMenuEntry.Text = "Explosive effect radius base: " + (int)GameSettings.ExplosiveForceRadiusMultiplier;
        }

        void fruitHPMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.FruitHP += 10;
            fruitHPMenuEntry.Text = "Fruit Hit Points : " + GameSettings.FruitHP.ToString();
        }
        void fruitHPMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.FruitHP -= 10;
            if (GameSettings.FruitHP < 0) GameSettings.FruitHP = 0;
            fruitHPMenuEntry.Text = "Fruit Hit Points : " + GameSettings.FruitHP.ToString();
        }
        void fruitMassMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.FruitMass += 0.02f;
            fruitMassMenuEntry.Text = "Fruit Mass : " + GameSettings.FruitMass.ToString();
        }
        void fruitMassMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.FruitMass -= 0.02f;
            if (GameSettings.FruitMass <= 0) GameSettings.FruitMass = 0.02f;
            fruitMassMenuEntry.Text = "Fruit Mass : " + GameSettings.FruitMass.ToString();
        }
        void veggieHPMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.VeggieHP += 10;
            veggieHPMenuEntry.Text = "Veggie Hit Points : " + GameSettings.VeggieHP.ToString();
        }
        void veggieHPMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.VeggieHP -= 10;
            if (GameSettings.VeggieHP < 0) GameSettings.VeggieHP = 0;
            veggieHPMenuEntry.Text = "Veggie Hit Points : " + GameSettings.VeggieHP.ToString();
        }
        void veggieMassMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.VeggieMass += 0.02f;
            veggieMassMenuEntry.Text = "Veggie Mass : " + GameSettings.VeggieMass.ToString();
        }
        void veggieMassMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.VeggieMass -= 0.02f;
            if (GameSettings.VeggieMass <= 0) GameSettings.VeggieMass = 0.02f;
            veggieMassMenuEntry.Text = "Veggie Mass : " + GameSettings.VeggieMass.ToString();
        }
        void fruitFrictionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.FruitFriction += 0.025f;
            fruitFrictionMenuEntry.Text = "Fruit Friction : " + GameSettings.FruitFriction.ToString();
        }
        void fruitFrictionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.FruitFriction -= 0.025f;
            if (GameSettings.FruitFriction <= 0f) GameSettings.FruitFriction = 0.025f;
            fruitFrictionMenuEntry.Text = "Fruit Friction : " + GameSettings.FruitFriction.ToString();
        }
        void fruitRestitutionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.FruitBouncy += 0.025f;
            if (GameSettings.FruitBouncy >= 1f) GameSettings.FruitBouncy = 0.975f;
            fruitRestitutionMenuEntry.Text = "Fruit Restitution : " + GameSettings.FruitBouncy.ToString();
        }
        void fruitRestitutionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.FruitBouncy -= 0.025f;
            if (GameSettings.FruitBouncy <= 0f) GameSettings.FruitBouncy = 0.025f;
            fruitRestitutionMenuEntry.Text = "Fruit Restitution : " + GameSettings.FruitBouncy.ToString();
        }
        void veggieFrictionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.VeggieFriction += 0.025f;
            veggieFrictionMenuEntry.Text = "Veggie Friction : " + GameSettings.VeggieFriction.ToString();
        }
        void veggieFrictionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.VeggieFriction -= 0.025f;
            if (GameSettings.VeggieFriction <= 0f) GameSettings.VeggieFriction = 0.025f;
            veggieFrictionMenuEntry.Text = "Veggie Friction : " + GameSettings.VeggieFriction.ToString();
        }
        void veggieRestitutionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.VeggieBouncy += 0.025f;
            if (GameSettings.VeggieBouncy >= 1f) GameSettings.VeggieBouncy = 0.975f;
            veggieRestitutionMenuEntry.Text = "Veggie Restitution : " + GameSettings.VeggieBouncy.ToString();
        }
        void veggieRestitutionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.VeggieBouncy -= 0.025f;
            if (GameSettings.VeggieBouncy <= 0f) GameSettings.VeggieBouncy = 0.025f;
            veggieRestitutionMenuEntry.Text = "Veggie Restitution : " + GameSettings.VeggieBouncy.ToString();
        }
        #endregion
    }
}
