using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ParallaxEngine;

//
//Cheat Menu
//Use the same template as the sound menu. Unlock one cheat per five ?stars/medals?, in this order:
//NOTE THAT THE BARREL CHEATS ARE MUTUALLY EXLCUSIVE, SO IF ONE IS TURNED ON, IT TURNS THE OTHERS OFF!
//
//-Each cheat should have a description so the player knows what it does. Put the description at the top under the "Cheat" menu header. I give you all of these below. Anything in parentheses is a design note, and not included in the description in-game. If a cheat is not unlocked, just have it greyed out, but let the player still move over them so they can see the descriptions but can't toggle from OFF to ON. <--- Gives people incentive to know what they are aiming for
//
//Invincibility 
//Fruit has unlimited hit points and is unaffected by saws, spikes, or hazards. (It still dies when leaving the screen, the player presses “B”, or it times out)
//
//TNT Barrel
//Starting barrel is a TNT barrel.
//
//Sharpshooter
//Shot always has perfect accuracy. (so, no matter where they hit (A) the second time during the shot, it is perfect accuracy)
//
//Faster than a Speeding Pomegranate
//Doubles shot velocity. (increase barrel min/max power to 1200/2400)
//
//Fire Barrel
//Starting barrel is a fireball barrel.
//
//Blindfold
//Removes the targeting line from shot barrels.
//
//Growth Barrel
//Starting barrel is a growth barrel.
//
//Funsplosions
//Doubles the radius on all explosions. (maybe double power too? will require testing/tweaking)
//
//Entire Legion of my Best Troops
//Unlimited shots with non-apple fruit. (doesn't grey out special fruit after use)
//
//Lightning Barrel
//Starting barrel is a lightning barrel.
//
//Fly Me to the Moon
//Reduces gravity to 15%. (make sure this doesn’t glitch the anti-grav bonus level, and adjusts the shot line correctly)
//
//Saw Barrel
//Starting barrel is a sawblade barrel.
//
//Hard Mode
//Stronger veggies. Stronger blocks. Stronger will to win. (multiply veggie hit points by 4. Multiply block hit points by 2. Increase accuracy penalty by 4x)
//
//Wildlife Barrel
//Starting barrel shoots animals. (randomly chooses fish, spider, bat, bird, or gremlin – no special properties?)
//
//It’s Good to be Bad
//Shoot veggies, fight fruit. (You shoot vegetables, and fight fruit. Swap out all appropriate graphics with their counterparts; Doc Broc, training, and bonus stages can remain the same)
//

namespace ProduceWars
{
    class CheatMenuScreen : MenuScreen
    {
        Texture2D pixel;
        Color backColor, barColor;
        int halfW = Camera.Viewport.Width / 2;
        int buttonHeight = 580;
        int earnedGold = 0;
        int lastEntry = -1;
        int medals = 0;
        string description1 = "";
        string description2 = "";
        bool isDenied = false;
        bool deniedColorDir = false;
        float deniedColorval = 1.0f;
        Color deniedColor = Color.White;

        MenuEntry InvincibilityMenuEntry, TNTBarrelMenuEntry, SharpshooterMenuEntry, FasterMenuEntry, FireBarrelMenuEntry, BlindfoldMenuEntry,
            GrowthBarrelMenuEntry, FunsplosionMenuEntry, LegionMenuEntry, LightningBarrelMenuEntry, MoonMenuEntry, SawBarrelMenuEntry,
            HardModeMenuEntry, CannonBarrel, GoodToBeBadMenuEntry;



        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public CheatMenuScreen()
            : base("Cheat Menu")
        {

            //count total gold medals earned
            earnedGold = LevelDataManager.EarnedGold();

            // Create our menu entries.
            InvincibilityMenuEntry = new MenuEntry("");
            TNTBarrelMenuEntry = new MenuEntry("");
            SharpshooterMenuEntry = new MenuEntry("");
            FasterMenuEntry = new MenuEntry("");
            FireBarrelMenuEntry = new MenuEntry("");
            BlindfoldMenuEntry = new MenuEntry("");
            GrowthBarrelMenuEntry = new MenuEntry("");
            FunsplosionMenuEntry = new MenuEntry("");
            LegionMenuEntry = new MenuEntry("");
            LightningBarrelMenuEntry = new MenuEntry("");
            MoonMenuEntry = new MenuEntry("");
            CannonBarrel = new MenuEntry("");
            SawBarrelMenuEntry = new MenuEntry("");
            HardModeMenuEntry = new MenuEntry("");
            GoodToBeBadMenuEntry = new MenuEntry("");

            // Hook up menu event handlers.

            // Add entries to the menu.
            MenuEntries.Add(InvincibilityMenuEntry);
            MenuEntries.Add(CannonBarrel);
            MenuEntries.Add(SharpshooterMenuEntry);
            MenuEntries.Add(FasterMenuEntry);
            MenuEntries.Add(FireBarrelMenuEntry);
            MenuEntries.Add(BlindfoldMenuEntry);
            MenuEntries.Add(GrowthBarrelMenuEntry);
            MenuEntries.Add(FunsplosionMenuEntry);
            MenuEntries.Add(LegionMenuEntry);
            MenuEntries.Add(LightningBarrelMenuEntry);
            MenuEntries.Add(MoonMenuEntry);
            MenuEntries.Add(TNTBarrelMenuEntry);
            MenuEntries.Add(SawBarrelMenuEntry);
            MenuEntries.Add(HardModeMenuEntry);
            MenuEntries.Add(GoodToBeBadMenuEntry);

            SetMenuEntryText();
        }

        private void SetMenuEntryText()
        {
            InvincibilityMenuEntry.Text = "Invincibility : " + (GameSettings.CheatInvincibility ? " On" : " Off");
            TNTBarrelMenuEntry.Text = "TNT barrel : " + (GameSettings.CheatTNTBarrel ? " On" : " Off");
            SharpshooterMenuEntry.Text = "Sharpshooter : " + (GameSettings.CheatSharpshooter ? " On" : " Off");
            FasterMenuEntry.Text = "Speed Demon : " + (GameSettings.CheatFaster ? " On" : " Off");
            FireBarrelMenuEntry.Text = "Fire barrel : " + (GameSettings.CheatFireBarrel ? " On" : " Off");
            BlindfoldMenuEntry.Text = "Blindfolded : " + (GameSettings.CheatBlindfold ? " On" : " Off");
            GrowthBarrelMenuEntry.Text = "Growth Barrel : " + (GameSettings.CheatGrowthBarrel ? " On" : " Off");
            FunsplosionMenuEntry.Text = "Funsplosions : " + (GameSettings.CheatFunsplosions ? " On" : " Off");
            LegionMenuEntry.Text = "Unlimited Fruit : " + (GameSettings.CheatLegion ? " On" : " Off");
            LightningBarrelMenuEntry.Text = "Lightning Barrel : " + (GameSettings.CheatLightningBarrel ? " On" : " Off");
            MoonMenuEntry.Text = "Fly to the Moon : " + (GameSettings.CheatMoon ? " On" : " Off");
            CannonBarrel.Text = "Cannon Barrel : " + (GameSettings.CheatCannonBarrel ? " On" : " Off");
            SawBarrelMenuEntry.Text = "Saw Barrel : " + (GameSettings.CheatSawBarrel ? " On" : " Off");
            HardModeMenuEntry.Text = "Hard Mode : " + (GameSettings.CheatHardMode ? " On" : " Off");
            GoodToBeBadMenuEntry.Text = "Now this is just evil : " + (GameSettings.CheatGoodToBeBad ? " On" : " Off"); 
        }

        private void SetDescription(int index)
        {


            switch (index)
            {
                case 0: //invincibility //
                    {
                        medals = (index+1) * 5;
                        description1 = "Fruit has unlimited hit points and is immune to hazards.";
                        description2 = "Shot will end when it comes to rest or player presses B.";
                        break;
                    }
                case 1: //cannon barrel //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Starting barrel will shoot cannonballs.";
                        description2 = "Cannonballs are heavy and indestructable.";
                        break;
                    }
                case 2: //sharpshooter //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Shot will always have perfect accuracy.";
                        description2 = "";
                        break;
                    }
                case 3: //faster shot //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Doubles shot velocity, but hard to aim.";
                        description2 = "Faster than a speeding pomegranate.";
                        break;
                    }
                case 4: //fire barrel //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Starting barrel will shoot fireballs.";
                        description2 = "Fireballs do not affect stone or metal.";
                        break;
                    }
                case 5: //blindfold //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Removes targeting line from the shot.";
                        description2 = "For an extra challenge.";
                        break;
                    }
                case 6: //growth barrel //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Starting barrel will shoot giant sized objects.";
                        description2 = "Say hello to mutant fruit!";
                        break;
                    }
                case 7: //funsplosions //
                    {
                        medals = (index + 1) * 5;
                        description1 = "All explosives have increased power.";
                        description2 = "Utter ridiculousness will ensue.";
                        break; 
                    }
                case 8: //legion //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Unlimited shots with non-apple fruits.";
                        description2 = "An entire legion of my best troops...";
                        break;
                    }
                case 9: //lightning //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Starting barrel will shoot lightning.";
                        description2 = "Lightning has some strange effects. Try it.";
                        break;
                    }
                case 10: //moon //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Gravity reduced to that of the Moon.";
                        description2 = "Attack of the lunar fruit!";
                        break;
                    }
                case 11: //tnt barrel //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Starting barrel will shoot a TNT crate.";
                        description2 = "Which explodes...naturally.  BOOM!";
                        break;
                    }
                case 12:  //saw barrel //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Starting barrel will shoot a saw blade.";
                        description2 = "Nothing will survive.";
                        break;
                    }
                case 13: //hard mode //
                    {
                        medals = (index + 1) * 5;
                        description1 = "Enemies are stronger.  Blocks are tougher.";
                        description2 = "A challenge, but your score will be higher.";
                        break;
                    }
                case 14: //exploding veggies
                    {
                        medals = (index + 1) * 5;
                        description1 = "Veggies will explode when killed.";
                        description2 = "";
                        break;
                    }
                default:
                    break;
            }

        }

        public override void LoadContent(ContentManager _content)
        {

            LevelDataManager.UItextures.TryGetValue("MainCheat", out menuTitleGraphic);
            isMenuTitleGraphic = true;

            //get other textures
            LevelDataManager.UItextures.TryGetValue("Pixel", out pixel);


            base.LoadContent(_content);
        }

        public override void HandleInput(InputState input)
        {
            if (input.IsXPressed(ControllingPlayer, out playerIndex))
            {
                GameSettings.Cheating = false;
                SetMenuEntryText();
            }

            base.HandleInput(input);
        }

        protected override void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            switch (entryIndex)
            {
                case 0:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatInvincibility = !GameSettings.CheatInvincibility;
                        else isDenied = true;
                        break;
                    }
                case 1:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatCannonBarrel = !GameSettings.CheatCannonBarrel;
                        else isDenied = true;
                        break;
                    }
                case 2:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatSharpshooter = !GameSettings.CheatSharpshooter;
                        else isDenied = true;
                        break;
                    }
                case 3:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatFaster = !GameSettings.CheatFaster;
                        else isDenied = true;
                        break;
                    }
                case 4:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatFireBarrel = !GameSettings.CheatFireBarrel;
                        else isDenied = true;
                        break;
                    }
                case 5:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatBlindfold = !GameSettings.CheatBlindfold;
                        else isDenied = true;
                        break;
                    }
                case 6:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatGrowthBarrel = !GameSettings.CheatGrowthBarrel;
                        else isDenied = true;
                        break;
                    }
                case 7:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatFunsplosions = !GameSettings.CheatFunsplosions;
                        else isDenied = true;
                        break;
                    }
                case 8:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatLegion = !GameSettings.CheatLegion;
                        else isDenied = true;
                        break;
                    }
                case 9:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatLightningBarrel = !GameSettings.CheatLightningBarrel;
                        else isDenied = true;
                        break;
                    }
                case 10:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatMoon = !GameSettings.CheatMoon;
                        else isDenied = true;
                        break;
                    }
                case 11:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatTNTBarrel = !GameSettings.CheatTNTBarrel;
                        else isDenied = true;
                        break;
                    }
                case 12:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatSawBarrel = !GameSettings.CheatSawBarrel;
                        else isDenied = true;
                        break;
                    }
                case 13:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatHardMode = !GameSettings.CheatHardMode;
                        else isDenied = true;
                        break;
                    }
                case 14:
                    {
                        if (earnedGold >= medals || !GameSettings.LocksOn) GameSettings.CheatGoodToBeBad = !GameSettings.CheatGoodToBeBad;
                        else isDenied = true;
                        break;
                    }
                default:
                    break;
            }
            SetMenuEntryText();

            base.OnSelectEntry(entryIndex, playerIndex);
        }

        public override void Draw(GameTime gameTime)
        {
            //set desciption text
            if (selectedEntry != lastEntry)
            {
                lastEntry = selectedEntry;
                SetDescription(lastEntry);
            }

            if (isDenied)
            {
                if (!deniedColorDir) deniedColorval -=  3f*(float) gameTime.ElapsedGameTime.TotalSeconds;
                else deniedColorval += 3f*(float)gameTime.ElapsedGameTime.TotalSeconds;

                if (deniedColorval <= -0.2f)
                {
                    deniedColorDir = true;
                }

                if (deniedColorval >= 1f)
                {
                    deniedColorval = 1f;
                    deniedColorDir = false;
                    isDenied = false;
                }

                deniedColor = new Color (1f, deniedColorval, deniedColorval);
            }

            backColor = Color.White * TransitionAlpha;
            barColor = new Color(0, 0, 0, 128) * TransitionAlpha;
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(pixel, new Rectangle(halfW - 280, 0, 560, Camera.ViewportHeight), barColor);

            //draw description
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, "Requires "  + medals.ToString() + " total gold medals.  You have "+ earnedGold.ToString() + " gold medals.", new Vector2(380, 490), deniedColor * TransitionAlpha, Vector2.Zero, 0.8f, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, description1, new Vector2(380, 520), backColor, Vector2.Zero, 0.8f, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, description2, new Vector2(380, 550), backColor, Vector2.Zero, 0.8f, SpriteEffects.None, TransitionAlpha);

            //draw buttons
            ScreenManager.SpriteBatch.Draw(A, new Rectangle((int)halfW - 260, buttonHeight ,48, 48), backColor);
            ScreenManager.SpriteBatch.Draw(X, new Rectangle((int)halfW - 70, buttonHeight, 48, 48), backColor);
            ScreenManager.SpriteBatch.Draw(B, new Rectangle((int)halfW + 130, buttonHeight, 48, 48), backColor);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "ENABLE", new Vector2(halfW - 210, buttonHeight + 8), backColor, Vector2.Zero, 1, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "DISABLE", new Vector2(halfW-20, buttonHeight + 8), backColor, Vector2.Zero, 1, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "BACK", new Vector2(halfW + 180, buttonHeight + 8), backColor, Vector2.Zero, 1, SpriteEffects.None, TransitionAlpha);
            
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            base.OnCancel(playerIndex);
        }

    }
}
