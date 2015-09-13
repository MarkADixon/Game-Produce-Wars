using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using ParallaxEngine;
using ProduceWars.Managers;

namespace ProduceWars
{
    public class VictoryScreen : GameScreen
    {
        KeyboardState keyboardState;
        GamePadState gamePadState;
        PlayerIndex controllingPlayer;
        PlayerIndex responsePlayer;
        Texture2D medal, produce, stache,pixel,levelStar,noMedal,newUnlock;
        List<Texture2D> unlocks;
        float unlockTransition = 0f;
        bool unlockTranOn = false;
        bool unlockTranOff = false;
        float starRotation = 0f;
        bool isUnlockLevels = false;

        bool isVictory = false; //if true its a victory screen with medal awarded
        bool starCollected = false;
        int shotsTaken = 0;
        int prevBest = 0;
        int destructionPoints = 0;
        int bestScore = 0;
        int prevGold, nowGold = 0;
        int prevStars, nowStars = 0;
        bool prevOrange, prevStrawberry, prevCherry, prevBanana, prevLemon, prevWatermelon = false;
        bool trackGrass1, trackGrass2, trackForest1, trackForest2, trackDesert1, trackDesert2, trackSnow1, trackSnow2, trackFactory1, trackFactory2, trackBoss = false;

        string shots = "Shots Score:";
        string best = "Shots Best:";
        string destruction = "Damage Score:";
        string dbest = "Damage Best:";
        string shotsN = "   ";
        string bestN = "   ";
        string destructionN = "   ";
        string dbestN = "   ";
        string victory = "VICTORY!";
   

        Vector2 shotsM, bestM, destructionM, dbestM, shotsNM,bestNM,destructionNM,dbestNM = Vector2.Zero;

        Color backColor;
        Color barColor;


        public VictoryScreen(bool _starCollected, int _shotsTaken)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(1);
            starCollected = _starCollected;
            shotsTaken = _shotsTaken;
            unlocks = new List<Texture2D>();
        }

        public override void LoadContent(ContentManager _content)
        {
            prevGold = LevelDataManager.EarnedGold();
            prevStars = LevelDataManager.EarnedStars();
            prevBest = LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].shots;
            destructionPoints = Camera.Score;
            bestScore = LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].bestScore;

            if (LevelDataManager.levelData[1, 6].unlocked)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if(LevelDataManager.levelData[1, 6+i].IsBronze()) count++;
                }
                if (count >= 2) prevOrange = true;
            }
            if (LevelDataManager.levelData[2, 1].unlocked) prevStrawberry = true;
            if (LevelDataManager.levelData[2, 6].unlocked)
            {
                int count = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (LevelDataManager.levelData[2, 6 + i].IsBronze()) count++;
                }
                if (count >= 2) prevCherry = true;
            } 
            if (LevelDataManager.levelData[3, 1].unlocked) prevBanana = true;
            if (LevelDataManager.levelData[4, 1].unlocked) prevLemon = true;
            if (LevelDataManager.levelData[5, 1].unlocked) prevWatermelon = true;
            if (LevelDataManager.levelData[1, 6].unlocked) trackGrass1 = true;
            if (LevelDataManager.levelData[1, 11].unlocked) trackGrass2 = true;
            if (LevelDataManager.levelData[2, 6].unlocked) trackForest1= true;
            if (LevelDataManager.levelData[2, 11].unlocked) trackForest2 = true;
            if (LevelDataManager.levelData[3, 6].unlocked) trackDesert1 = true;
            if (LevelDataManager.levelData[3, 11].unlocked) trackDesert2 = true;
            if (LevelDataManager.levelData[4, 6].unlocked) trackSnow1 = true;
            if (LevelDataManager.levelData[4, 11].unlocked) trackSnow2 = true;
            if (LevelDataManager.levelData[5, 1].unlocked) trackFactory1 = true;
            if (LevelDataManager.levelData[5, 6].unlocked) trackFactory2 = true;
            if (LevelDataManager.levelData[5, 11].unlocked) trackBoss = true;


            //update and save data
            if (destructionPoints > bestScore)
            {
                LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].bestScore = destructionPoints;
            }

            if (shotsTaken < LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].shots  ||
                LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].shots == 0)
            {
                if (!GameSettings.Cheating)
                {
                    LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].shots = shotsTaken;
                }
            }

            if (shotsTaken <= LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].bronze && !GameSettings.Cheating)
            {
                if (starCollected)
                {
                    LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].starCollected = true;

                    //test if that was a new star unlocked
                    nowStars = LevelDataManager.EarnedStars();
                    if (nowStars > prevStars)
                    {
                        int testStars = nowStars / 5;
                        for (int i = 1; i < 16; i++)
                        {
                            if (testStars >= i) LevelDataManager.levelData[6, i].unlocked = true;
                            else LevelDataManager.levelData[6, i].unlocked = false;
                        }

                        if (testStars * 5 == nowStars)  //will only pass if there was no remainder (it is a multiple of 5)
                        {
                            LevelDataManager.UItextures.TryGetValue("UnlockBonus", out newUnlock);
                            unlocks.Add(newUnlock);
                        }
                    }
                }


                #region LEVEL UNLOCK CHECK
                

                if (LevelDataManager.world < 6 && LevelDataManager.world > 0)
                {
                    int currentRow = 0;
                    if (LevelDataManager.level >= 6) currentRow = 1;
                    if (LevelDataManager.level >= 11) currentRow = 2;


                    int count = 1;
                    for (int i = 1; i < 6; i++)
                    {
                        if (LevelDataManager.level != i + (currentRow * 5)) //dont self count, count starts at one because bronze already checked
                        {
                            if (LevelDataManager.levelData[LevelDataManager.world, i + (currentRow * 5)].IsBronze()) count++;
                        }
                    }

#if XBOX
                    if (Guide.IsTrialMode) count = 1;
#endif

                    if (count >= 4)
                    {
                        if (currentRow == 0)
                        {
                            if (LevelDataManager.levelData[LevelDataManager.world, 6].unlocked == false)
                            {
                                isUnlockLevels = true;
                                LevelDataManager.UItextures.TryGetValue("UnlockLevels", out newUnlock);
                                unlocks.Add(newUnlock);
                                for (int i = 0; i < 5; i++)
                                {
                                    LevelDataManager.levelData[LevelDataManager.world, 6 + i].unlocked = true;
                                }
                            }
                        }
                        if (currentRow == 1)
                        {
                            if (LevelDataManager.levelData[LevelDataManager.world, 11].unlocked == false)
                            {
                                isUnlockLevels = true;
                                LevelDataManager.UItextures.TryGetValue("UnlockLevels", out newUnlock);
                                unlocks.Add(newUnlock);
                                for (int i = 0; i < 5; i++)
                                {
                                    LevelDataManager.levelData[LevelDataManager.world, 11 + i].unlocked = true;
                                }
                            }
                        }
                        if (currentRow == 2 && LevelDataManager.world < 5)
                        {
                            if (LevelDataManager.levelData[LevelDataManager.world+1, 1].unlocked == false)
                            {
                                isUnlockLevels = true;
                                LevelDataManager.UItextures.TryGetValue("UnlockLevels", out newUnlock);
                                unlocks.Add(newUnlock);
                                for (int i = 0; i < 5; i++)
                                {
                                    LevelDataManager.levelData[LevelDataManager.world + 1, 1 + i].unlocked = true;
                                }
                            }
                        }
                    }
                }

                #endregion

                //if (LevelDataManager.level != 15 && LevelDataManager.world < 6) LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level + 1].unlocked = true;
                //else if (LevelDataManager.world+1 <6 ) LevelDataManager.levelData[LevelDataManager.world + 1, 1].unlocked = true;

                //if (shotsTaken <= LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].silver) //unlock 2 for silver
                //{
                //    if (LevelDataManager.level <= 13 && LevelDataManager.world < 6) LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level + 2].unlocked = true;
                //    if (LevelDataManager.level == 14 && LevelDataManager.world+1 < 6) LevelDataManager.levelData[LevelDataManager.world + 1, 1].unlocked = true;
                //    if (LevelDataManager.level == 15 && LevelDataManager.world+1 < 6) LevelDataManager.levelData[LevelDataManager.world + 1, 2].unlocked = true;
                //}

                if (shotsTaken <= LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].gold) //unlock 3 for gold
                {
                    //if (LevelDataManager.level <= 12 && LevelDataManager.world < 6) LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level + 3].unlocked = true;
                    //if (LevelDataManager.level == 13 && LevelDataManager.world+1 < 6) LevelDataManager.levelData[LevelDataManager.world + 1, 1].unlocked = true;
                    //if (LevelDataManager.level == 14 && LevelDataManager.world+1 < 6) LevelDataManager.levelData[LevelDataManager.world + 1, 2].unlocked = true;
                    //if (LevelDataManager.level == 15 && LevelDataManager.world+1 < 6) LevelDataManager.levelData[LevelDataManager.world + 1, 3].unlocked = true;
                    
                    //test if its a new gold unlocked 
                    nowGold = LevelDataManager.EarnedGold();
                    if (nowGold > prevGold)
                    {
                        int testGold = nowGold / 5;
                        if (testGold * 5 == nowGold)  //will only pass if there was no remainder (it is a multiple of 5)
                        {
                            LevelDataManager.UItextures.TryGetValue("UnlockCheat", out newUnlock);
                            unlocks.Add(newUnlock);
                        }
                    }
                }
            }
            LevelDataManager.WriteSaveGameData();



            LevelDataManager.UItextures.TryGetValue("Pixel", out pixel);
            LevelDataManager.UItextures.TryGetValue("Star", out levelStar);
            LevelDataManager.UItextures.TryGetValue("NoMedal", out noMedal);

            if (shotsTaken > LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].silver)
            {
                LevelDataManager.UItextures.TryGetValue("BigBronzeMedal", out medal);
                if (shotsTaken <= LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].bronze) isVictory = true;
            }
            if (shotsTaken <= LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].silver &&
                shotsTaken > LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].gold)
            {
                LevelDataManager.UItextures.TryGetValue("BigSilverMedal", out medal);
                isVictory = true;
            }
            if (shotsTaken <= LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].gold)
            {
                LevelDataManager.UItextures.TryGetValue("BigGoldMedal", out medal);
                isVictory = true;
            }

            #region UNLOCK FRUIT OR MUSIC TRACK CHECK
            if (!prevOrange)
            {
                bool isUnlocked = false;
                if (LevelDataManager.levelData[1, 6].unlocked)
                {
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (LevelDataManager.levelData[1, 6 + i].IsBronze()) count++;
                    }
                    if (count >= 2) isUnlocked = true;
                }
                if (isUnlocked)
                {
                    LevelDataManager.UItextures.TryGetValue("UnlockOrange", out newUnlock);
                    unlocks.Add(newUnlock);
                    LevelDataManager.UItextures.TryGetValue("TutOrange", out newUnlock);
                    unlocks.Add(newUnlock);
                }
            }
            if (LevelDataManager.levelData[2, 1].unlocked && !prevStrawberry)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockStrawberry", out newUnlock);
                unlocks.Add(newUnlock);
                LevelDataManager.UItextures.TryGetValue("TutStraw", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (!prevCherry)
            {
                bool isUnlocked = false;
                if (LevelDataManager.levelData[2, 6].unlocked)
                {
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (LevelDataManager.levelData[2, 6 + i].IsBronze()) count++;
                    }
                    if (count >= 2) isUnlocked = true;
                }
                if (isUnlocked)
                {
                    LevelDataManager.UItextures.TryGetValue("UnlockCherry", out newUnlock);
                    unlocks.Add(newUnlock);
                    LevelDataManager.UItextures.TryGetValue("TutCherries", out newUnlock);
                    unlocks.Add(newUnlock);
                }
            }
            if (LevelDataManager.levelData[3, 1].unlocked && !prevBanana)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockBanana", out newUnlock);
                unlocks.Add(newUnlock);
                LevelDataManager.UItextures.TryGetValue("TutBanana", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[4, 1].unlocked && !prevLemon)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockLemon", out newUnlock);
                unlocks.Add(newUnlock);
                LevelDataManager.UItextures.TryGetValue("TutLemon", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[5, 1].unlocked && !prevWatermelon)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockWatermelon", out newUnlock);
                unlocks.Add(newUnlock);
                LevelDataManager.UItextures.TryGetValue("TutWater", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[1, 6].unlocked && !trackGrass1)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[1, 11].unlocked && !trackGrass2)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[2, 6].unlocked && !trackForest1)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[2, 11].unlocked && !trackForest2)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[3, 6].unlocked && !trackDesert1)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[3, 11].unlocked && !trackDesert2)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[4, 6].unlocked && !trackSnow1)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[4, 11].unlocked && !trackSnow2)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[5, 1].unlocked && !trackFactory1)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[5, 6].unlocked && !trackFactory2)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            if (LevelDataManager.levelData[5, 11].unlocked && !trackBoss)
            {
                LevelDataManager.UItextures.TryGetValue("UnlockMusic", out newUnlock);
                unlocks.Add(newUnlock);
            }
            #endregion
            if (unlocks.Count != 0) unlockTranOn = true;



            if (!isVictory)
            {
                if (GameSettings.isBoss)
                {
                    victory = "THE PEACH HAS BEEN PIED!";
                    LevelDataManager.UItextures.TryGetValue("PeachPie", out produce);
                    SoundManager.Play(SoundManager.Sound.FanfareDefeat, false, false);
                    SoundManager.Play(SoundManager.Sound.pw_docbroc2, false, false);
                }
                else
                {
                    victory = "FAIL!";
                    LevelDataManager.UItextures.TryGetValue("Broc", out produce);
                    SoundManager.Play(SoundManager.Sound.FanfareDefeat, false, false);
                    SoundManager.Play(SoundManager.Sound.pw_docbroc2, false, false);
                }
            }
            else
            {
                LevelDataManager.UItextures.TryGetValue("Pear", out produce);
                LevelDataManager.UItextures.TryGetValue("PearStache", out stache);
                SoundManager.Play(SoundManager.Sound.FanfareVictory, false, false);
            }

            if (GameSettings.Cheating) victory = "Cheater!";

            shotsM = ScreenManager.buxtonFont.MeasureString(shots);
            bestM = ScreenManager.buxtonFont.MeasureString(best);
            destructionM = ScreenManager.buxtonFont.MeasureString(destruction);
            dbestM = ScreenManager.buxtonFont.MeasureString(dbest);
            shotsN += shotsTaken.ToString();
            if (prevBest != 0) bestN += prevBest.ToString();
            destructionN += destructionPoints.ToString();
            if (bestScore != 0) dbestN += bestScore.ToString();
            shotsNM = ScreenManager.buxtonFont.MeasureString(shotsN);
            bestNM = ScreenManager.buxtonFont.MeasureString(bestN);
            destructionNM = ScreenManager.buxtonFont.MeasureString(destructionN);
            dbestNM = ScreenManager.buxtonFont.MeasureString(dbestN);

            base.LoadContent(_content);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);


        }


        #region Handle Input
        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            #region GET STATE OF INPUTS
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            controllingPlayer = ControllingPlayer.Value;
#if WINDOWS
            keyboardState = input.CurrentKeyboardStates[(int)controllingPlayer];
#endif
            gamePadState = input.CurrentGamePadStates[(int)controllingPlayer];
            #endregion

            if (unlocks.Count != 0)
            {
                if (input.IsAnyFourPressed(ControllingPlayer, out responsePlayer))
                {
                    if (!unlockTranOn && !unlockTranOff) unlockTranOff = true;
                }
                return;
            }

         
            #region "A" NEXT LEVEL
            if (input.IsAPressed(ControllingPlayer, out responsePlayer) && !GameSettings.isBoss)
            {
                
                if (LevelDataManager.world == 6)
                {
                    LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen(), new LevelSelectScreen());
                    return;
                }

                if ((LevelDataManager.level != 15 && LevelDataManager.world > 0) || (LevelDataManager.world == 0 && LevelDataManager.level != 10))
                {
                    if (LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level + 1].unlocked)
                    {
                        LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new GameplayScreen(LevelDataManager.world, LevelDataManager.level + 1));
                    }
                    else
                    {
                        LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen(), new LevelSelectScreen());
                    }
                }
                else
                {
                    if (LevelDataManager.nextworld != 6)
                    {
                        LevelDataManager.nextworld += 1;
                        LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen(), new LevelSelectScreen());
                    }     
                }
            }
            #endregion

            #region "X" GOTO LEVEL SELECT
            if (input.IsXPressed(ControllingPlayer, out responsePlayer) || input.IsBPressed(ControllingPlayer, out responsePlayer))
            {
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen(), new LevelSelectScreen());
            }
            #endregion

            #region "Y" RESTART LEVEL
            if (input.IsYPressed(ControllingPlayer, out responsePlayer))
            {
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new GameplayScreen(LevelDataManager.world, LevelDataManager.level));
            }
            #endregion

        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            backColor = Color.White * TransitionAlpha;
            barColor = new Color(0, 0, 0, 128) * TransitionAlpha;

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(pixel, new Rectangle(Camera.ViewportWidth/2 - 280, 0 , 560, Camera.ViewportHeight ), barColor);
            if (isVictory) ScreenManager.SpriteBatch.Draw(produce, new Rectangle((int)((Camera.ViewportWidth *0.5f) - (produce.Width*0.5f)), 36, produce.Width, produce.Height), backColor);
            else ScreenManager.SpriteBatch.Draw(produce, new Rectangle((int)((Camera.ViewportWidth * 0.5f) - (produce.Width * 0.5f)), 36, produce.Width, produce.Height), null, backColor,0f,Vector2.Zero,SpriteEffects.FlipHorizontally,1f);
            if (isVictory) ScreenManager.SpriteBatch.Draw(stache, new Rectangle((int)((Camera.ViewportWidth * 0.5f) - (produce.Width * 0.5f)), 36, stache.Width, stache.Height), backColor);

            if (!GameSettings.Cheating)
            {
                if (!GameSettings.isBoss) DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.largeFont, victory, new Vector2(Camera.ViewportWidth * 0.5f - (ScreenManager.largeFont.MeasureString(victory).X * 0.5f), 260), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
                else DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.largeFont, victory, new Vector2(Camera.ViewportWidth * 0.5f - (ScreenManager.largeFont.MeasureString(victory).X * 0.5f), 480), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
            }
            else
            {
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.largeFont, victory, new Vector2(Camera.ViewportWidth * 0.5f - (ScreenManager.largeFont.MeasureString(victory).X * 0.5f), 260), Color.Red * TransitionAlpha, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
            }

            if (!GameSettings.isBoss)
            {
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, shots, new Vector2((int)(Camera.ViewportWidth * 0.5f - shotsM.X), 330), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, shotsN, new Vector2((int)(Camera.ViewportWidth * 0.5f), 330), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, best, new Vector2((int)(Camera.ViewportWidth * 0.5f - bestM.X), 370), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, bestN, new Vector2((int)(Camera.ViewportWidth * 0.5f), 370), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, destruction, new Vector2((int)(Camera.ViewportWidth * 0.5f - destructionM.X), 410), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, destructionN, new Vector2((int)(Camera.ViewportWidth * 0.5f), 410), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, dbest, new Vector2((int)(Camera.ViewportWidth * 0.5f - dbestM.X), 450), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.buxtonFont, dbestN, new Vector2((int)(Camera.ViewportWidth * 0.5f), 450), backColor, Vector2.Zero, 1.0f, SpriteEffects.None, TransitionAlpha);

                //draw medal portion of screen
                ScreenManager.SpriteBatch.Draw(medal, new Rectangle((int)(Camera.ViewportWidth * 0.5f - medal.Width * 0.5f), 486, medal.Width, medal.Height), backColor);
                starRotation += 0.04f;
                if (starCollected && isVictory && (!GameSettings.Cheating)) ScreenManager.SpriteBatch.Draw(levelStar, new Rectangle((int)(Camera.ViewportWidth * 0.5f + 55), 520, levelStar.Width, levelStar.Height),
                                            new Rectangle(0, 0, levelStar.Width, levelStar.Height), backColor, starRotation, new Vector2(levelStar.Width * 0.5f, levelStar.Height * 0.5f), SpriteEffects.None, 0f);
                if (!isVictory || GameSettings.Cheating) ScreenManager.SpriteBatch.Draw(noMedal, new Rectangle((int)(Camera.ViewportWidth * 0.5f - noMedal.Width * 0.5f), 486, noMedal.Width, noMedal.Height), backColor);
            }

            //draw buttons
            if (!GameSettings.isBoss)
            {
                ScreenManager.SpriteBatch.Draw(A, new Rectangle(382, 610, 48, 48), backColor);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "Next", new Vector2(432, 618), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
            }
            ScreenManager.SpriteBatch.Draw(B, new Rectangle(532, 610, 48, 48), backColor);
            ScreenManager.SpriteBatch.Draw(X, new Rectangle(574, 610, 48, 48), backColor);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "Menu", new Vector2(624, 618), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
            ScreenManager.SpriteBatch.Draw(Y, new Rectangle(730, 610, 48, 48), backColor);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "Replay", new Vector2(780, 618), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);

            #region draw unlocks
            if (unlocks.Count != 0)
            {
                if (TransitionAlpha >= 1f)
                {
                    if (unlocks[0] != null) ScreenManager.SpriteBatch.Draw(unlocks[0], new Rectangle((int)((Camera.Origin.X) - (unlocks[0].Width*0.5f)), (int)((Camera.Origin.Y) - (unlocks[0].Height*0.5f)),unlocks[0].Width,unlocks[0].Height ), Color.White*unlockTransition);

                    if (unlockTranOn)
                    {
                        unlockTransition += 3f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (unlockTransition >= 1)
                        {
                            unlockTransition = 1;
                            unlockTranOn = false;
                        }
                    }
                    if (unlockTranOff) unlockTransition -= 3f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    {
                        if (unlockTransition <= 0)
                        {
                            unlockTransition = 0;
                            unlockTranOff = false;
                            if (unlocks.Count > 1) unlockTranOn = true;
                            unlocks.RemoveAt(0);
                        }
                    }
                }
            }
            #endregion


            ScreenManager.SpriteBatch.End();
        }

    }
}
