using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

//
//TESTING AND DEBUG OPTIONS SCREEN
//
namespace ProduceWars
{
    class BlockOptionsMenuScreen : MenuScreen
    {
        //declare menu entries
        MenuEntry blockBaseHPMenuEntry;
        MenuEntry cheeseHPMultiplierMenuEntry;
        MenuEntry cheeseDensityMenuEntry;
        MenuEntry cheeseFrictionMenuEntry;
        MenuEntry cheeseRestitutionMenuEntry;
        MenuEntry woodHPMultiplierMenuEntry;
        MenuEntry woodDensityMenuEntry;
        MenuEntry woodFrictionMenuEntry;
        MenuEntry woodRestitutionMenuEntry;
        MenuEntry iceHPMultiplierMenuEntry;
        MenuEntry iceDensityMenuEntry;
        MenuEntry iceFrictionMenuEntry;
        MenuEntry iceRestitutionMenuEntry;
        MenuEntry stoneHPMultiplierMenuEntry;
        MenuEntry stoneDensityMenuEntry;
        MenuEntry stoneFrictionMenuEntry;
        MenuEntry stoneRestitutionMenuEntry;
        MenuEntry metalHPMultiplierMenuEntry;
        MenuEntry metalDensityMenuEntry;
        MenuEntry metalFrictionMenuEntry;
        MenuEntry metalRestitutionMenuEntry;

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public BlockOptionsMenuScreen()
            : base("Block Options")
        {
            // Create our menu entries.
            blockBaseHPMenuEntry = new MenuEntry(string.Empty);
            cheeseHPMultiplierMenuEntry = new MenuEntry(string.Empty);
            cheeseDensityMenuEntry = new MenuEntry(string.Empty);
            cheeseFrictionMenuEntry = new MenuEntry(string.Empty);
            cheeseRestitutionMenuEntry = new MenuEntry(string.Empty);
            woodHPMultiplierMenuEntry = new MenuEntry(string.Empty);
            woodDensityMenuEntry = new MenuEntry(string.Empty);
            woodFrictionMenuEntry = new MenuEntry(string.Empty);
            woodRestitutionMenuEntry = new MenuEntry(string.Empty);
            iceHPMultiplierMenuEntry = new MenuEntry(string.Empty);
            iceDensityMenuEntry = new MenuEntry(string.Empty);
            iceFrictionMenuEntry = new MenuEntry(string.Empty);
            iceRestitutionMenuEntry = new MenuEntry(string.Empty);
            stoneHPMultiplierMenuEntry = new MenuEntry(string.Empty);
            stoneDensityMenuEntry = new MenuEntry(string.Empty);
            stoneFrictionMenuEntry = new MenuEntry(string.Empty);
            stoneRestitutionMenuEntry = new MenuEntry(string.Empty);
            metalHPMultiplierMenuEntry = new MenuEntry(string.Empty);
            metalDensityMenuEntry = new MenuEntry(string.Empty);
            metalFrictionMenuEntry = new MenuEntry(string.Empty);
            metalRestitutionMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            blockBaseHPMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(blockBaseHPMenuEntry_Plus);
            blockBaseHPMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(blockBaseHPMenuEntry_Minus);
            cheeseHPMultiplierMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(cheeseHPMultiplierMenuEntry_Plus);
            cheeseHPMultiplierMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(cheeseHPMultiplierMenuEntry_Minus);
            cheeseDensityMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(cheeseDensityMenuEntry_Plus);
            cheeseDensityMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(cheeseDensityMenuEntry_Minus);
            cheeseFrictionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(cheeseFrictionMenuEntry_Plus);
            cheeseFrictionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(cheeseFrictionMenuEntry_Minus);
            cheeseRestitutionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(cheeseRestitutionMenuEntry_Plus);
            cheeseRestitutionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(cheeseRestitutionMenuEntry_Minus);
            woodHPMultiplierMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(woodHPMultiplierMenuEntry_Plus);
            woodHPMultiplierMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(woodHPMultiplierMenuEntry_Minus);
            woodDensityMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(woodDensityMenuEntry_Plus);
            woodDensityMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(woodDensityMenuEntry_Minus);
            woodFrictionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(woodFrictionMenuEntry_Plus);
            woodFrictionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(woodFrictionMenuEntry_Minus);
            woodRestitutionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(woodRestitutionMenuEntry_Plus);
            woodRestitutionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(woodRestitutionMenuEntry_Minus);
            iceHPMultiplierMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(iceHPMultiplierMenuEntry_Plus);
            iceHPMultiplierMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(iceHPMultiplierMenuEntry_Minus);
            iceDensityMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(iceDensityMenuEntry_Plus);
            iceDensityMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(iceDensityMenuEntry_Minus);
            iceFrictionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(iceFrictionMenuEntry_Plus);
            iceFrictionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(iceFrictionMenuEntry_Minus);
            iceRestitutionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(iceRestitutionMenuEntry_Plus);
            iceRestitutionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(iceRestitutionMenuEntry_Minus);
            stoneHPMultiplierMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(stoneHPMultiplierMenuEntry_Plus);
            stoneHPMultiplierMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(stoneHPMultiplierMenuEntry_Minus);
            stoneDensityMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(stoneDensityMenuEntry_Plus);
            stoneDensityMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(stoneDensityMenuEntry_Minus);
            stoneFrictionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(stoneFrictionMenuEntry_Plus);
            stoneFrictionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(stoneFrictionMenuEntry_Minus);
            stoneRestitutionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(stoneRestitutionMenuEntry_Plus);
            stoneRestitutionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(stoneRestitutionMenuEntry_Minus);
            metalHPMultiplierMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(metalHPMultiplierMenuEntry_Plus);
            metalHPMultiplierMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(metalHPMultiplierMenuEntry_Minus);
            metalDensityMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(metalDensityMenuEntry_Plus);
            metalDensityMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(metalDensityMenuEntry_Minus);
            metalFrictionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(metalFrictionMenuEntry_Plus);
            metalFrictionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(metalFrictionMenuEntry_Minus);
            metalRestitutionMenuEntry.Plus += new EventHandler<PlayerIndexEventArgs>(metalRestitutionMenuEntry_Plus);
            metalRestitutionMenuEntry.Minus += new EventHandler<PlayerIndexEventArgs>(metalRestitutionMenuEntry_Minus);

            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(blockBaseHPMenuEntry);
            MenuEntries.Add(cheeseHPMultiplierMenuEntry);
            MenuEntries.Add(cheeseDensityMenuEntry);
            MenuEntries.Add(cheeseFrictionMenuEntry);
            MenuEntries.Add(cheeseRestitutionMenuEntry);
            MenuEntries.Add(woodHPMultiplierMenuEntry);
            MenuEntries.Add(woodDensityMenuEntry);
            MenuEntries.Add(woodFrictionMenuEntry);
            MenuEntries.Add(woodRestitutionMenuEntry);
            MenuEntries.Add(iceHPMultiplierMenuEntry);
            MenuEntries.Add(iceDensityMenuEntry);
            MenuEntries.Add(iceFrictionMenuEntry);
            MenuEntries.Add(iceRestitutionMenuEntry);
            MenuEntries.Add(stoneHPMultiplierMenuEntry);
            MenuEntries.Add(stoneDensityMenuEntry);
            MenuEntries.Add(stoneFrictionMenuEntry);
            MenuEntries.Add(stoneRestitutionMenuEntry);
            MenuEntries.Add(metalHPMultiplierMenuEntry);
            MenuEntries.Add(metalDensityMenuEntry);
            MenuEntries.Add(metalFrictionMenuEntry);
            MenuEntries.Add(metalRestitutionMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            blockBaseHPMenuEntry.Text = "Block Base HP (per 1x1) : " + GameSettings.BlockBaseHP;
            cheeseHPMultiplierMenuEntry.Text = "Cheese HP Multiplier : " + GameSettings.CheeseHPMultiplier;
            cheeseDensityMenuEntry.Text = "Cheese Density : " + GameSettings.CheeseDensity;
            cheeseFrictionMenuEntry.Text = "Cheese Friction : " + GameSettings.CheeseFriction;
            cheeseRestitutionMenuEntry.Text = "Cheese Restitution : " + GameSettings.CheeseRestitution;
            woodHPMultiplierMenuEntry.Text = "Wood HP Multiplier : " + GameSettings.WoodHPMultiplier;
            woodDensityMenuEntry.Text = "Wood Density : " + GameSettings.WoodDensity;
            woodFrictionMenuEntry.Text = "Wood Friction : " + GameSettings.WoodFriction;
            woodRestitutionMenuEntry.Text = "Wood Restitution : " + GameSettings.WoodRestitution;
            iceHPMultiplierMenuEntry.Text = "Ice HP Multiplier : " + GameSettings.IceHPMultiplier;
            iceDensityMenuEntry.Text = "Ice Density : " + GameSettings.IceDensity;
            iceFrictionMenuEntry.Text = "Ice Friction : " + GameSettings.IceFriction;
            iceRestitutionMenuEntry.Text = "Ice Restitution : " + GameSettings.IceRestitution;
            stoneHPMultiplierMenuEntry.Text = "Stone HP Multiplier : " + GameSettings.StoneHPMultiplier;
            stoneDensityMenuEntry.Text = "Stone Density : " + GameSettings.StoneDensity;
            stoneFrictionMenuEntry.Text = "Stone Friction : " + GameSettings.StoneFriction;
            stoneRestitutionMenuEntry.Text = "Stone Restitution : " + GameSettings.StoneRestitution;
            metalHPMultiplierMenuEntry.Text = "Metal HP Multiplier : " + GameSettings.MetalHPMultiplier;
            metalDensityMenuEntry.Text = "Metal Density : " + GameSettings.MetalDensity;
            metalFrictionMenuEntry.Text = "Metal Friction : " + GameSettings.MetalFriction;
            metalRestitutionMenuEntry.Text = "Metal Restitution : " + GameSettings.MetalRestitution;
        }
        #endregion

        #region handle input
        void blockBaseHPMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.BlockBaseHP += 1;
            blockBaseHPMenuEntry.Text = "Block Base HP (per 1x1) : " + GameSettings.BlockBaseHP;
        }
        void blockBaseHPMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.BlockBaseHP -= 1;
            if (GameSettings.BlockBaseHP <= 0) GameSettings.BlockBaseHP = 1;
            blockBaseHPMenuEntry.Text = "Block Base HP (per 1x1) : " + GameSettings.BlockBaseHP;
        }
        void cheeseHPMultiplierMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.CheeseHPMultiplier += 1;
            cheeseHPMultiplierMenuEntry.Text = "Cheese HP Multiplier : " + GameSettings.CheeseHPMultiplier;
        }
        void cheeseHPMultiplierMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.CheeseHPMultiplier -= 1;
            if (GameSettings.CheeseHPMultiplier <= 0) GameSettings.CheeseHPMultiplier = 1;
            cheeseHPMultiplierMenuEntry.Text = "Cheese HP Multiplier : " + GameSettings.CheeseHPMultiplier;
        }
        void cheeseDensityMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.CheeseDensity += 0.025f;
            cheeseDensityMenuEntry.Text = "Cheese Density : " + GameSettings.CheeseDensity;
        }
        void cheeseDensityMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.CheeseDensity -= 0.025f;
            if (GameSettings.CheeseDensity <= 0) GameSettings.CheeseDensity = 0.025f;
            cheeseDensityMenuEntry.Text = "Cheese Density : " + GameSettings.CheeseDensity;
        }
        void cheeseFrictionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.CheeseFriction += 0.025f;
            cheeseFrictionMenuEntry.Text = "Cheese Friction : " + GameSettings.CheeseFriction;
        }
        void cheeseFrictionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.CheeseFriction -= 0.025f;
            if (GameSettings.CheeseFriction <= 0) GameSettings.CheeseFriction = 0.025f;
            cheeseFrictionMenuEntry.Text = "Cheese Friction : " + GameSettings.CheeseFriction;
        }
        void cheeseRestitutionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.CheeseRestitution += 0.025f;
            if (GameSettings.CheeseRestitution >= 1) GameSettings.CheeseRestitution = 0.975f;
            cheeseRestitutionMenuEntry.Text = "Cheese Restitution : " + GameSettings.CheeseRestitution;
        }
        void cheeseRestitutionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.CheeseRestitution -= 0.025f;
            if (GameSettings.CheeseRestitution <= 0) GameSettings.CheeseRestitution = 0.025f;
            cheeseRestitutionMenuEntry.Text = "Cheese Restitution : " + GameSettings.CheeseRestitution;
        }
        void woodHPMultiplierMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.WoodHPMultiplier += 1;
            woodHPMultiplierMenuEntry.Text = "Wood HP Multiplier : " + GameSettings.WoodHPMultiplier;
        }
        void woodHPMultiplierMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.WoodHPMultiplier -= 1;
            if (GameSettings.WoodHPMultiplier <= 0) GameSettings.WoodHPMultiplier = 1;
            woodHPMultiplierMenuEntry.Text = "Wood HP Multiplier : " + GameSettings.WoodHPMultiplier;
        }
        void woodDensityMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.WoodDensity += 0.025f;
            woodDensityMenuEntry.Text = "Wood Density : " + GameSettings.WoodDensity;
        }
        void woodDensityMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.WoodDensity -= 0.025f;
            if (GameSettings.WoodDensity <= 0) GameSettings.WoodDensity = 0.025f;
            woodDensityMenuEntry.Text = "Wood Density : " + GameSettings.WoodDensity;
        }
        void woodFrictionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.WoodFriction += 0.025f;
            woodFrictionMenuEntry.Text = "Wood Friction : " + GameSettings.WoodFriction;
        }
        void woodFrictionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.WoodFriction -= 0.025f;
            if (GameSettings.WoodFriction <= 0) GameSettings.WoodFriction = 0.025f;
            woodFrictionMenuEntry.Text = "Wood Friction : " + GameSettings.WoodFriction;
        }
        void woodRestitutionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.WoodRestitution += 0.025f;
            if (GameSettings.WoodRestitution >= 1) GameSettings.WoodRestitution = 0.975f;
            woodRestitutionMenuEntry.Text = "Wood Restitution : " + GameSettings.WoodRestitution;
        }
        void woodRestitutionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.WoodRestitution -= 0.025f;
            if (GameSettings.WoodRestitution <= 0) GameSettings.WoodRestitution = 0.025f;
            woodRestitutionMenuEntry.Text = "Wood Restitution : " + GameSettings.WoodRestitution;
        }
        void iceHPMultiplierMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.IceHPMultiplier += 1;
            iceHPMultiplierMenuEntry.Text = "Ice HP Multiplier : " + GameSettings.IceHPMultiplier;
        }
        void iceHPMultiplierMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.IceHPMultiplier -= 1;
            if (GameSettings.IceHPMultiplier <= 0) GameSettings.IceHPMultiplier = 1;
            iceHPMultiplierMenuEntry.Text = "Ice HP Multiplier : " + GameSettings.IceHPMultiplier;
        }
        void iceDensityMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.IceDensity += 0.025f;
            iceDensityMenuEntry.Text = "Ice Density : " + GameSettings.IceDensity;
        }
        void iceDensityMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.IceDensity -= 0.025f;
            if (GameSettings.IceDensity <= 0) GameSettings.IceDensity = 0.025f;
            iceDensityMenuEntry.Text = "Ice Density : " + GameSettings.IceDensity;
        }
        void iceFrictionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.IceFriction += 0.025f;
            iceFrictionMenuEntry.Text = "Ice Friction : " + GameSettings.IceFriction;
        }
        void iceFrictionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.IceFriction -= 0.025f;
            if (GameSettings.IceFriction <= 0) GameSettings.IceFriction = 0.025f;
            iceFrictionMenuEntry.Text = "Ice Friction : " + GameSettings.IceFriction;
        }
        void iceRestitutionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.IceRestitution += 0.025f;
            if (GameSettings.IceRestitution >= 1) GameSettings.IceRestitution = 0.975f;
            iceRestitutionMenuEntry.Text = "Ice Restitution : " + GameSettings.IceRestitution;
        }
        void iceRestitutionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.IceRestitution -= 0.025f;
            if (GameSettings.IceRestitution <= 0) GameSettings.IceRestitution = 0.025f;
            iceRestitutionMenuEntry.Text = "Ice Restitution : " + GameSettings.IceRestitution;
        }
        void stoneHPMultiplierMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.StoneHPMultiplier += 1;
            stoneHPMultiplierMenuEntry.Text = "Stone HP Multiplier : " + GameSettings.StoneHPMultiplier;
        }
        void stoneHPMultiplierMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.StoneHPMultiplier -= 1;
            if (GameSettings.StoneHPMultiplier <= 0) GameSettings.StoneHPMultiplier = 1;
            stoneHPMultiplierMenuEntry.Text = "Stone HP Multiplier : " + GameSettings.StoneHPMultiplier;
        }
        void stoneDensityMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.StoneDensity += 0.025f;
            stoneDensityMenuEntry.Text = "Stone Density : " + GameSettings.StoneDensity;
        }
        void stoneDensityMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.StoneDensity -= 0.025f;
            if (GameSettings.StoneDensity <= 0) GameSettings.StoneDensity = 0.025f;
            stoneDensityMenuEntry.Text = "Stone Density : " + GameSettings.StoneDensity;
        }
        void stoneFrictionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.StoneFriction += 0.025f;
            stoneFrictionMenuEntry.Text = "Stone Friction : " + GameSettings.StoneFriction;
        }
        void stoneFrictionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.StoneFriction -= 0.025f;
            if (GameSettings.StoneFriction <= 0) GameSettings.StoneFriction = 0.025f;
            stoneFrictionMenuEntry.Text = "Stone Friction : " + GameSettings.StoneFriction;
        }
        void stoneRestitutionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.StoneRestitution += 0.025f;
            if (GameSettings.StoneRestitution >= 1) GameSettings.StoneRestitution = 0.975f;
            stoneRestitutionMenuEntry.Text = "Stone Restitution : " + GameSettings.StoneRestitution;
        }
        void stoneRestitutionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.StoneRestitution -= 0.025f;
            if (GameSettings.StoneRestitution <= 0) GameSettings.StoneRestitution = 0.025f;
            stoneRestitutionMenuEntry.Text = "Stone Restitution : " + GameSettings.StoneRestitution;
        }
        void metalHPMultiplierMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MetalHPMultiplier += 1;
            metalHPMultiplierMenuEntry.Text = "Metal HP Multiplier : " + GameSettings.MetalHPMultiplier;
        }
        void metalHPMultiplierMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MetalHPMultiplier -= 1;
            if (GameSettings.MetalHPMultiplier <= 0) GameSettings.MetalHPMultiplier = 1;
            metalHPMultiplierMenuEntry.Text = "Metal HP Multiplier : " + GameSettings.MetalHPMultiplier;
        }
        void metalDensityMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MetalDensity += 0.025f;
            metalDensityMenuEntry.Text = "Metal Density : " + GameSettings.MetalDensity;
        }
        void metalDensityMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MetalDensity -= 0.025f;
            if (GameSettings.MetalDensity <= 0) GameSettings.MetalDensity = 0.025f;
            metalDensityMenuEntry.Text = "Metal Density : " + GameSettings.MetalDensity;
        }
        void metalFrictionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MetalFriction += 0.025f;
            metalFrictionMenuEntry.Text = "Metal Friction : " + GameSettings.MetalFriction;
        }
        void metalFrictionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MetalFriction -= 0.025f;
            if (GameSettings.MetalFriction <= 0) GameSettings.MetalFriction = 0.025f;
            metalFrictionMenuEntry.Text = "Metal Friction : " + GameSettings.MetalFriction;
        }
        void metalRestitutionMenuEntry_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MetalRestitution += 0.025f;
            if (GameSettings.MetalRestitution >= 1) GameSettings.MetalRestitution = 0.975f;
            metalRestitutionMenuEntry.Text = "Metal Restitution : " + GameSettings.MetalRestitution;
        }
        void metalRestitutionMenuEntry_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MetalRestitution -= 0.025f;
            if (GameSettings.MetalRestitution <= 0) GameSettings.MetalRestitution = 0.025f;
            metalRestitutionMenuEntry.Text = "Metal Restitution : " + GameSettings.MetalRestitution;
        }
        #endregion
    }
}
