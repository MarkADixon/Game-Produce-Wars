using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProduceWars
{
    public static class GameSettings
    {
        public const bool isTestingMode = false;

        public const float Vdust = 2.0f;
        public const float shotTimer = 5.99f;
        public static bool TitleSafe = false;
        public static bool isBoss = false;
        public static float bossDamage =1.0f;
        public static bool isAppleJump = true;

        private static bool musicOn = true;
        public static bool MusicOn
        {
            get { return musicOn; }
            set { musicOn = value; }
        }

        private static int musicVolume = 100;
        public static int MusicVolume
        {
            get { return musicVolume; }
            set { musicVolume = (int)MathHelper.Clamp(value, 0, 100); }
        }

        private static int effectVolume = 100;
        public static int EffectVolume
        {
            get { return effectVolume; }
            set { effectVolume = (int)MathHelper.Clamp(value, 0, 100); }
        }
        

        public static Color[] Colors = { Color.Red, Color.Orange, Color.Yellow, Color.Lime, Color.Blue, Color.Purple, Color.Plum };
        public static string[] ColorName = { "Cherry", "Orange", "Lemon", "Lime", "Blueberry", "Grape", "Plum"};
        public static int ShotColorIndex = 0;
        private static Color shotPathColor = Color.Red;
        public static Color ShotPathColor
        {
            get { return shotPathColor; }
            set { shotPathColor = value; }
        }

        public static string[] HelpGraphicName = { "Apple", "Orange", "Strawberry", "Cherry", "Banana", "Lemon", "Watermelon" };
        public static int HelpGraphicIndex = 0;

        #region CHEATS
        public static bool Cheating
        {
            get
            {
                return (
                cheatInvincibility ||
                cheatTNTBarrel ||
                cheatSharpshooter ||
                cheatFaster ||
                cheatFireBarrel ||
                cheatBlindfold ||
                cheatGrowthBarrel ||
                cheatFunsplosions ||
                cheatLegion ||
                cheatLightningBarrel ||
                cheatMoon ||
                cheatCannonBarrel ||
                cheatSawBarrel ||
                cheatHardMode ||
                cheatGoodToBeBad);
            }
            set
            {
                cheatInvincibility = value;
                cheatTNTBarrel = value;
                cheatSharpshooter = value;
                cheatFaster = value;
                cheatFireBarrel = value;
                cheatBlindfold = value;
                cheatGrowthBarrel = value;
                cheatFunsplosions = value;
                cheatLegion = value;
                cheatLightningBarrel = value;
                cheatMoon = value;
                cheatCannonBarrel = value;
                cheatSawBarrel = value;
                cheatHardMode = value;
                cheatGoodToBeBad = value;
            }
        }

        private static bool cheatInvincibility = false;
        public static bool CheatInvincibility
        {
            get { return cheatInvincibility; }
            set { cheatInvincibility = value; }
        }

        private static bool cheatTNTBarrel = false;
        public static bool CheatTNTBarrel
        {
            get { return cheatTNTBarrel; }
            set 
            { 
                if (value)
                {
                    cheatTNTBarrel = false;
                    cheatFireBarrel = false;
                    cheatGrowthBarrel = false;
                    cheatLightningBarrel = false;
                    cheatCannonBarrel = false;
                    cheatSawBarrel = false;
                }
                cheatTNTBarrel = value;
            }
        }

        private static bool cheatSharpshooter = false;
        public static bool CheatSharpshooter
        {
            get { return cheatSharpshooter; }
            set { cheatSharpshooter = value; }
        }

        private static bool cheatFaster = false;
        public static bool CheatFaster
        {
            get { return cheatFaster; }
            set { cheatFaster = value; }
        }

        private static bool cheatFireBarrel = false;
        public static bool CheatFireBarrel
        {
            get { return cheatFireBarrel; }
            set 
            {
                if (value)
                {
                    cheatTNTBarrel = false;
                    cheatFireBarrel = false;
                    cheatGrowthBarrel = false;
                    cheatLightningBarrel = false;
                    cheatCannonBarrel = false;
                    cheatSawBarrel = false;
                } 
                cheatFireBarrel = value;
            }
        }

        private static bool cheatBlindfold = false;
        public static bool CheatBlindfold
        {
            get { return cheatBlindfold; }
            set { cheatBlindfold = value; }
        }

        private static bool cheatGrowthBarrel = false;
        public static bool CheatGrowthBarrel
        {
            get { return cheatGrowthBarrel; }
            set
            {
                if (value)
                {
                    cheatTNTBarrel = false;
                    cheatFireBarrel = false;
                    cheatGrowthBarrel = false;
                    cheatLightningBarrel = false;
                    cheatCannonBarrel = false;
                    cheatSawBarrel = false;
                }
                cheatGrowthBarrel = value;
            }
        }

        private static bool cheatFunsplosions = false;
        public static bool CheatFunsplosions
        {
            get { return cheatFunsplosions; }
            set { cheatFunsplosions = value; }
        }

        private static bool cheatLegion = false;
        public static bool CheatLegion
        {
            get { return cheatLegion; }
            set { cheatLegion = value; }
        }

        private static bool cheatLightningBarrel = false;
        public static bool CheatLightningBarrel
        {
            get { return cheatLightningBarrel; }
            set
            {
                if (value)
                {
                    cheatTNTBarrel = false;
                    cheatFireBarrel = false;
                    cheatGrowthBarrel = false;
                    cheatLightningBarrel = false;
                    cheatCannonBarrel = false;
                    cheatSawBarrel = false;
                }
                cheatLightningBarrel = value;
            }
        }

        private static bool cheatMoon = false;
        public static bool CheatMoon
        {
            get { return cheatMoon; }
            set { cheatMoon = value; }
        }

        private static bool cheatCannonBarrel = false;
        public static bool CheatCannonBarrel
        {
            get { return cheatCannonBarrel; }
            set
            {
                if (value)
                {
                    cheatTNTBarrel = false;
                    cheatFireBarrel = false;
                    cheatGrowthBarrel = false;
                    cheatLightningBarrel = false;
                    cheatCannonBarrel = false;
                    cheatSawBarrel = false;
                }
                cheatCannonBarrel = value;
            }
        }

        private static bool cheatSawBarrel = false;
        public static bool CheatSawBarrel
        {
            get { return cheatSawBarrel; }
            set
            {
                if (value)
                {
                    cheatTNTBarrel = false;
                    cheatFireBarrel = false;
                    cheatGrowthBarrel = false;
                    cheatLightningBarrel = false;
                    cheatCannonBarrel = false;
                    cheatSawBarrel = false;
                } 
                cheatSawBarrel = value;
            }
        }

        private static bool cheatHardMode = false;
        public static bool CheatHardMode
        {
            get { return cheatHardMode; }
            set { cheatHardMode = value; }
        }

        private static bool cheatGoodToBeBad = false;
        public static bool CheatGoodToBeBad 
        {
            get { return cheatGoodToBeBad; }
            set { cheatGoodToBeBad = value; }
        }

        #endregion


        private static bool multiSampling = true;
        public static bool MultiSampling
        {
            get { return multiSampling; }
            set { multiSampling = value; }
        }

        private static bool locksOn = false;
        public static bool LocksOn
        {
            get { return locksOn; }
            set { locksOn = value; }
        }

        private static bool cheatBarrelOn = false;
        public static bool CheatBarrelOn
        {
            get { return cheatBarrelOn; }
            set { cheatBarrelOn = value; }
        }

        private static bool debugViewEnabled = false;
        public static bool DebugViewEnabled
        {
            get { return debugViewEnabled; }
            set { debugViewEnabled = value; }
        }

        private static bool showControlsEnabled = false;
        public static bool ShowControlsEnabled
        {
            get { return showControlsEnabled; }
            set { showControlsEnabled = value; }
        }

        private static int damageMultiplier = 10; //multiplies the impact force so higher, less abstract HP values can be set
        public static int DamageMultiplier { get { return damageMultiplier; } }

        private static int minFiringPower = 600;
        public static int MinFiringPower
        {
            get { return minFiringPower; }
            set { minFiringPower = value; }
        }

        private static int maxFiringPower = 1200;
        public static int MaxFiringPower
        {
            get { return maxFiringPower; }
            set { maxFiringPower = value; }
        }

        private static float explosivePower = 1f;
        public static float ExplosivePower
        {
            get { return explosivePower; }
            set { explosivePower = value; }
        }

        private static float explosiveForceRadiusMultiplier = 200f;
        public static float ExplosiveForceRadiusMultiplier
        {
            get { return explosiveForceRadiusMultiplier; }
            set { explosiveForceRadiusMultiplier = value; }
        }

        private static float gravity = 10f;
        public static float Gravity
        {
            get {
                if (CheatMoon) return gravity * 0.2f;
                return gravity; 
            }
            set { gravity = value; }
        }

        private static int physicsScale = 50;
        public static int PhysicsScale 
        {
            get { return physicsScale; }
            set { physicsScale = value; }
        }

        private static int fruitHP = 250;
        public static int FruitHP
        {
            get { return fruitHP; }
            set { fruitHP = value; }
        }

        private static float fruitMass = 0.516f;
        public static float FruitMass
        {
            get { return fruitMass; }
            set { fruitMass = value; }
        }

        private static float fruitFriction = 0.5f;
        public static float FruitFriction
        {
            get { return fruitFriction; }
            set { fruitFriction = value; }
        }

        private static float fruitBouncy = 0.5f;
        public static float FruitBouncy
        {
            get { return fruitBouncy; }
            set { fruitBouncy = value; }
        }


        private static int veggieHP = 40;
        public static int VeggieHP
        {
            get {
                if (CheatHardMode) return veggieHP * 4;
                return veggieHP; }
            set { veggieHP = value; }
        }

        private static float veggieMass = 0.516f;
        public static float VeggieMass
        {
            get { return veggieMass; }
            set { veggieMass = value; }
        }

        private static float veggieFriction = 0.5f;
        public static float VeggieFriction
        {
            get { return veggieFriction; }
            set { veggieFriction = value; }
        }

        private static float veggieBouncy = 0.5f;
        public static float VeggieBouncy
        {
            get { return veggieBouncy; }
            set { veggieBouncy = value; }
        }

        private static bool blockDeathEnabled = true;
        public static bool BlockDeathEnabled
        {
            get { return blockDeathEnabled; }
            set { blockDeathEnabled = value; }
        }

        private static int blockBaseHP = 6;
        public static int BlockBaseHP
        {
            get {
                if (CheatHardMode) return blockBaseHP * 2; 
                return blockBaseHP;
            }
            set { blockBaseHP = value; }
        }

        private static int cheeseHPMultiplier = 2;
        public static int CheeseHPMultiplier
        {
            get { return cheeseHPMultiplier; }
            set { cheeseHPMultiplier = value; }
        }

        private static float cheeseDensity = 0.4f; //0.2 is density of foam..for foam cheese, cheese is actually a little over 1.0
        public static float CheeseDensity
        {
            get { return cheeseDensity; }
            set { cheeseDensity = value; }
        }

        private static float cheeseFriction = 0.5f;
        public static float CheeseFriction
        {
            get { return cheeseFriction; }
            set { cheeseFriction = value; }
        }

        private static float cheeseRestitution = 0.7f;
        public static float CheeseRestitution
        {
            get { return cheeseRestitution; }
            set { cheeseRestitution = value; }
        }

        private static float woodDensity = 1.0f;
        public static float WoodDensity
        {
            get { return woodDensity; }
            set { woodDensity = value; }
        }

        private static int woodHPMultiplier = 4;
        public static int WoodHPMultiplier
        {
            get { return woodHPMultiplier; }
            set { woodHPMultiplier = value; }
        }

        private static float woodFriction = 0.4f;
        public static float WoodFriction
        {
            get { return woodFriction; }
            set { woodFriction = value; }
        }

        private static float woodRestitution = 0.6f;
        public static float WoodRestitution
        {
            get { return woodRestitution; }
            set { woodRestitution = value; }
        }

        private static float iceDensity = 1.0f;
        public static float IceDensity
        {
            get { return iceDensity; }
            set { iceDensity = value; }
        }

        private static int iceHPMultiplier = 12;
        public static int IceHPMultiplier
        {
            get { return iceHPMultiplier; }
            set { iceHPMultiplier = value; }
        }

        private static float iceFriction = 0.05f;
        public static float IceFriction
        {
            get { return iceFriction; }
            set { iceFriction = value; }
        }

        private static float iceRestitution = 0.5f;
        public static float IceRestitution
        {
            get { return iceRestitution; }
            set { iceRestitution = value; }
        }

        private static float stoneDensity = 3.0f;
        public static float StoneDensity
        {
            get { return stoneDensity; }
            set { stoneDensity = value; }
        }

        private static int stoneHPMultiplier = 16;
        public static int StoneHPMultiplier
        {
            get { return stoneHPMultiplier; }
            set { stoneHPMultiplier = value; }
        }

        private static float stoneFriction = 0.7f;
        public static float StoneFriction
        {
            get { return stoneFriction; }
            set { stoneFriction = value; }
        }

        private static float stoneRestitution = 0.4f;
        public static float StoneRestitution
        {
            get { return stoneRestitution; }
            set { stoneRestitution = value; }
        }

        private static float metalDensity = 3.2f;
        public static float MetalDensity
        {
            get { return metalDensity; }
            set { metalDensity = value; }
        }

        private static int metalHPMultiplier = 50;
        public static int MetalHPMultiplier
        {
            get { return metalHPMultiplier; }
            set { metalHPMultiplier = value; }
        }

        private static float metalFriction = 0.6f;
        public static float MetalFriction
        {
            get { return metalFriction; }
            set { metalFriction = value; }
        }

        private static float metalRestitution = 0.35f;
        public static float MetalRestitution
        {
            get { return metalRestitution; }
            set { metalRestitution = value; }
        }

    }


}
