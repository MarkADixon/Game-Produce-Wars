using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using EasyStorage;

namespace ParallaxEngine
{

    //class that contains a list of sourcetextures in use by the parallaxengine in the current level and/or editor project
    //the class maintains the indexing of tiled textures and can return a tile texture with a call contiaining the texture ID and index number  
    //Sprite draw calls functions in this class

    public static class LevelDataManager
    {
        #region DECLARATIONS
        public static List<TextureData> levelTextures;
        public static List<TextureData> effectTextures;
        public static Dictionary<string,Texture2D> UItextures;
        public static ContentManager levelContent;
        public static ContentManager effectsContent;
        public static ContentManager UIcontent;
        public static ContentManager tempContent;
        public const int texturePadding = 2;
        public static Random rand;
        public static int world = -1;
        public static int nextworld = 0;
        public static int nextlevel = 0;
        public static int level = -1;
        public static bool worldChanged = false;
        public static bool levelChanged = false;
        public static string texturePack;
        public static ParallaxManager parallaxEngine;
        public static LevelData[,] levelData = new LevelData[7, 16];


        // A generic EasyStorage save device
        public static IAsyncSaveDevice SaveDevice;
        //We can set up different file names for different things we may save.
        public static string fileName_options = "ProduceWars_Options";
        public static string fileName_game = "ProduceWars_Game";
        //public static string fileName_awards = "YourGame_Awards";

        //This is the name of the save file you'll find if you go into your memory
        //options on the Xbox. If you name it something like 'MyGameSave' then
        //people will have no idea what it's for and might delete your save.
        //YOU SHOULD ONLY HAVE ONE OF THESE
        public static string containerName = "ProduceWars_Save";

        #endregion

        #region CONSTRUCTOR
        public static void SplashPreInitialize(Game game)
        {

            if (UIcontent == null)
            {
                UIcontent = new ContentManager(game.Services, "Content");
                UItextures = new Dictionary<string, Texture2D>();

                //load textures
                Texture2D controls = UIcontent.Load<Texture2D>(@"MenuUI\Controls");
                UItextures.Add("Controls", controls);
                Texture2D LevelNoStar = UIcontent.Load<Texture2D>(@"MenuUI\LevelNoStar");
                UItextures.Add("LevelNoStar", LevelNoStar);
                Texture2D LevelWithStar = UIcontent.Load<Texture2D>(@"MenuUI\LevelWithStar");
                UItextures.Add("LevelWithStar", LevelWithStar);
                Texture2D MainCheat = UIcontent.Load<Texture2D>(@"MenuUI\MainCheat");
                UItextures.Add("MainCheat", MainCheat);
                Texture2D MainOption = UIcontent.Load<Texture2D>(@"MenuUI\MainOption");
                UItextures.Add("MainOption", MainOption);
                Texture2D MainQuit = UIcontent.Load<Texture2D>(@"MenuUI\MainQuit");
                UItextures.Add("MainQuit", MainQuit);
                Texture2D MainSingle = UIcontent.Load<Texture2D>(@"MenuUI\MainSingle");
                UItextures.Add("MainSingle", MainSingle);
                Texture2D MainSound = UIcontent.Load<Texture2D>(@"MenuUI\MainSound");
                UItextures.Add("MainSound", MainSound);
                Texture2D Waterflame = UIcontent.Load<Texture2D>(@"MenuUI\Waterflame");
                UItextures.Add("Waterflame", Waterflame);
                Texture2D MenuBack = UIcontent.Load<Texture2D>(@"MenuUI\MenuBack");
                UItextures.Add("MenuBack", MenuBack);
                Texture2D NumberLock = UIcontent.Load<Texture2D>(@"MenuUI\Number_Lock");
                UItextures.Add("NumberLock", NumberLock);
                Texture2D Numbers = UIcontent.Load<Texture2D>(@"MenuUI\numbers");
                UItextures.Add("Numbers", Numbers);
                Texture2D Star = UIcontent.Load<Texture2D>(@"Textures\Powerups\Star");
                UItextures.Add("Star", Star);
                Texture2D Pixel = UIcontent.Load<Texture2D>(@"MenuUI\testPixel");
                UItextures.Add("Pixel", Pixel);
                Texture2D WPixel = UIcontent.Load<Texture2D>(@"Textures\WorldAll\pixel");
                UItextures.Add("WPixel", WPixel);
                Texture2D Title = UIcontent.Load<Texture2D>(@"MenuUI\Title");
                UItextures.Add("Title", Title);
                Texture2D TutApple = UIcontent.Load<Texture2D>(@"MenuUI\TutApple");
                UItextures.Add("TutApple", TutApple);
                Texture2D TutBanana = UIcontent.Load<Texture2D>(@"MenuUI\TutBanana");
                UItextures.Add("TutBanana", TutBanana);
                Texture2D TutCherries = UIcontent.Load<Texture2D>(@"MenuUI\TutCherries");
                UItextures.Add("TutCherries", TutCherries);
                Texture2D TutLemon = UIcontent.Load<Texture2D>(@"MenuUI\TutLemon");
                UItextures.Add("TutLemon", TutLemon);
                Texture2D TutOrange = UIcontent.Load<Texture2D>(@"MenuUI\TutOrange");
                UItextures.Add("TutOrange", TutOrange);
                Texture2D TutStraw = UIcontent.Load<Texture2D>(@"MenuUI\TutStraw");
                UItextures.Add("TutStraw", TutStraw);
                Texture2D TutWater = UIcontent.Load<Texture2D>(@"MenuUI\TutWater");
                UItextures.Add("TutWater", TutWater);
                Texture2D World0 = UIcontent.Load<Texture2D>(@"MenuUI\World0");
                UItextures.Add("World0", World0);
                Texture2D World1 = UIcontent.Load<Texture2D>(@"MenuUI\World1");
                UItextures.Add("World1", World1);
                Texture2D World2 = UIcontent.Load<Texture2D>(@"MenuUI\World2");
                UItextures.Add("World2", World2);
                Texture2D World3 = UIcontent.Load<Texture2D>(@"MenuUI\World3");
                UItextures.Add("World3", World3);
                Texture2D World4 = UIcontent.Load<Texture2D>(@"MenuUI\World4");
                UItextures.Add("World4", World4);
                Texture2D World5 = UIcontent.Load<Texture2D>(@"MenuUI\World5");
                UItextures.Add("World5", World5);
                Texture2D World6 = UIcontent.Load<Texture2D>(@"MenuUI\World6");
                UItextures.Add("World6", World6);
                Texture2D A = UIcontent.Load<Texture2D>(@"Buttons\A");
                UItextures.Add("A", A);
                Texture2D B = UIcontent.Load<Texture2D>(@"Buttons\B");
                UItextures.Add("B", B);
                Texture2D LB = UIcontent.Load<Texture2D>(@"Buttons\LB");
                UItextures.Add("LB", LB);
                Texture2D LS = UIcontent.Load<Texture2D>(@"Buttons\LS");
                UItextures.Add("LS", LS);
                Texture2D LT = UIcontent.Load<Texture2D>(@"Buttons\LT");
                UItextures.Add("LT", LT);
                Texture2D RB = UIcontent.Load<Texture2D>(@"Buttons\RB");
                UItextures.Add("RB", RB);
                Texture2D RS = UIcontent.Load<Texture2D>(@"Buttons\RS");
                UItextures.Add("RS", RS);
                Texture2D RT = UIcontent.Load<Texture2D>(@"Buttons\RT");
                UItextures.Add("RT", RT);
                Texture2D X = UIcontent.Load<Texture2D>(@"Buttons\X");
                UItextures.Add("X", X);
                Texture2D Y = UIcontent.Load<Texture2D>(@"Buttons\Y");
                UItextures.Add("Y", Y);
                Texture2D BigBronzeMedal = UIcontent.Load<Texture2D>(@"GameUI\BigBronzeMedal");
                UItextures.Add("BigBronzeMedal", BigBronzeMedal);
                Texture2D BigSilverMedal = UIcontent.Load<Texture2D>(@"GameUI\BigSilverMedal");
                UItextures.Add("BigSilverMedal", BigSilverMedal);
                Texture2D BigGoldMedal = UIcontent.Load<Texture2D>(@"GameUI\BigGoldMedal");
                UItextures.Add("BigGoldMedal", BigGoldMedal);
                Texture2D MedalBronze = UIcontent.Load<Texture2D>(@"GameUI\MedalBronze");
                UItextures.Add("MedalBronze", MedalBronze);
                Texture2D MedalSilver = UIcontent.Load<Texture2D>(@"GameUI\MedalSilver");
                UItextures.Add("MedalSilver", MedalSilver);
                Texture2D MedalGold = UIcontent.Load<Texture2D>(@"GameUI\MedalGold");
                UItextures.Add("MedalGold", MedalGold);
                Texture2D NoMedal = UIcontent.Load<Texture2D>(@"GameUI\NoMedal");
                UItextures.Add("NoMedal", NoMedal);
                Texture2D Pear = UIcontent.Load<Texture2D>(@"GameUI\KingPear");
                UItextures.Add("Pear", Pear);
                Texture2D PearStache = UIcontent.Load<Texture2D>(@"GameUI\pearstache");
                UItextures.Add("PearStache", PearStache);
                Texture2D Broc = UIcontent.Load<Texture2D>(@"GameUI\broc1");
                UItextures.Add("Broc", Broc);
                Texture2D Cursor = UIcontent.Load<Texture2D>(@"GameUI\cursor");
                UItextures.Add("Cursor", Cursor);
                Texture2D UnlockBanana = UIcontent.Load<Texture2D>(@"GameUI\BananaUnlock");
                UItextures.Add("UnlockBanana", UnlockBanana);
                Texture2D UnlockBonus = UIcontent.Load<Texture2D>(@"GameUI\BonusUnlock");
                UItextures.Add("UnlockBonus", UnlockBonus);
                Texture2D UnlockCheat = UIcontent.Load<Texture2D>(@"GameUI\CheatUnlock");
                UItextures.Add("UnlockCheat", UnlockCheat);
                Texture2D UnlockCherry = UIcontent.Load<Texture2D>(@"GameUI\CherryUnlock");
                UItextures.Add("UnlockCherry", UnlockCherry);
                Texture2D UnlockLemon = UIcontent.Load<Texture2D>(@"GameUI\LemonUnlock");
                UItextures.Add("UnlockLemon", UnlockLemon);
                Texture2D UnlockMusic = UIcontent.Load<Texture2D>(@"GameUI\MusicUnlock");
                UItextures.Add("UnlockMusic", UnlockMusic);
                Texture2D UnlockOrange = UIcontent.Load<Texture2D>(@"GameUI\OrangeUnlock");
                UItextures.Add("UnlockOrange", UnlockOrange);
                Texture2D UnlockStrawberry = UIcontent.Load<Texture2D>(@"GameUI\StrawberryUnlock");
                UItextures.Add("UnlockStrawberry", UnlockStrawberry);
                Texture2D UnlockWatermelon = UIcontent.Load<Texture2D>(@"GameUI\WatermelonUnlock");
                UItextures.Add("UnlockWatermelon", UnlockWatermelon);
                Texture2D UnlockLevels = UIcontent.Load<Texture2D>(@"GameUI\LevelUnlock");
                UItextures.Add("UnlockLevels", UnlockLevels);
                Texture2D AppleJackX = UIcontent.Load<Texture2D>(@"GameUI\AppleJackX");
                UItextures.Add("AppleJackX", AppleJackX);
                Texture2D AppleJack = UIcontent.Load<Texture2D>(@"GameUI\AppleJack");
                UItextures.Add("AppleJack", AppleJack);
                Texture2D PeachPie = UIcontent.Load<Texture2D>(@"GameUI\PeachPie");
                UItextures.Add("PeachPie", PeachPie);
            }
        }
        
        public static void Initialize(Game game, ParallaxManager _parallaxEngine, int _world, int _level)
        {
            rand = new Random();
            parallaxEngine = _parallaxEngine;

            if (tempContent == null)
            {
                tempContent = new ContentManager(game.Services, "Content");
            }

            if (levelContent == null)
            {
                levelContent = new ContentManager(game.Services, "Content");
                levelTextures = new List<TextureData>();
            }

            if (effectsContent == null)
            {
                effectsContent = new ContentManager(game.Services, "Content");
                effectTextures = new List<TextureData>();
                //load effects
                LoadTextures("Content\\Textures\\Effects\\EffectTextures.pck");
            }

            //reinitialize UI content if it is empty somehow
            if (UIcontent == null) SplashPreInitialize(game);
            
            if (world != _world)
            {
                UnloadLevelData();
                worldChanged = true; //flag for the load world function
            }
            else worldChanged = false;

            if (level != _level)
            {
                levelChanged = true; //flag for the soundManager music function
            }
            else levelChanged = false;

            world = _world;
            level = _level;

            LoadMap("Content\\Levels\\" + world + "_" + level + ".lvl");
        }
        #endregion



        #region TEXTURE METHODS

        //sprites or tiles in given row on the sprite sheet texture
        public static int SpritesInRow (Sprite sprite)
        {
            if (sprite.IsEffect) return effectTextures[sprite.TextureID].SpritesInRow;
            else return levelTextures[sprite.TextureID].SpritesInRow;
        }

        public static int SpriteWidth(Sprite sprite)
        {
            if (sprite.IsEffect) return effectTextures[sprite.TextureID].TileWidth;
            else return levelTextures[sprite.TextureID].TileWidth;
        }

        public static int SpriteHeight(Sprite sprite)
        {
            if (sprite.IsEffect) return effectTextures[sprite.TextureID].TileHeight;
            else return levelTextures[sprite.TextureID].TileHeight;
        }

        //returns a rectangle to draw tile from given the index number for a particular tile
        public static Rectangle GetSourceRect(Sprite sprite)
        {
            int index = sprite.TextureIndex + sprite.CurrentFrame;
            if (sprite.IsEffect)
            {
                if (!effectTextures[sprite.TextureID].IsTiled)
                {
                    return new Rectangle(0, 0, SpriteWidth(sprite), SpriteHeight(sprite));
                }
                else
                {
                    return new Rectangle(
                    texturePadding + (int)(index % SpritesInRow(sprite)) * (SpriteWidth(sprite) + (2 * texturePadding)),
                    texturePadding + (int)(index / SpritesInRow(sprite)) * (SpriteHeight(sprite) + (2 * texturePadding)),
                    SpriteWidth(sprite),
                    SpriteHeight(sprite));
                }
            }
            else
            {
                if (!levelTextures[sprite.TextureID].IsTiled)
                {
                    return new Rectangle(0, 0, SpriteWidth(sprite), SpriteHeight(sprite));
                }
                else
                {
                    return new Rectangle(
                    texturePadding + (int)(index % SpritesInRow(sprite)) * (SpriteWidth(sprite) + (2 * texturePadding)),
                    texturePadding + (int)(index / SpritesInRow(sprite)) * (SpriteHeight(sprite) + (2 * texturePadding)),
                    SpriteWidth(sprite),
                    SpriteHeight(sprite));
                }
            }
        }

        public static Texture2D GetSourceTexture(Sprite sprite)
        {
            if (sprite.IsEffect) return effectTextures[sprite.TextureID].Texture;
            else return levelTextures[sprite.TextureID].Texture;
        }

        public static Sprite.Type GetSpriteTypeFromTextureData(int ID)
        {
            Sprite.Type spriteType = Sprite.Type.None;
            switch (levelTextures[ID].SpriteType)
            {
                case "NONE":
                    {
                        break;
                    }
                case "TERRAIN":
                    {
                        spriteType = Sprite.Type.Terrain;
                        break;
                    }
                case "BLOCK":
                    {
                        spriteType = Sprite.Type.Block;
                        break;
                    }
                case "FLAME":
                    {
                        spriteType = Sprite.Type.Flame;
                        break;
                    }
                case "DECO":
                    {
                        spriteType = Sprite.Type.Deco;
                        break;
                    }
                case "STAR":
                    {
                        spriteType = Sprite.Type.Star;
                        break;
                    }
                case "FRUIT":
                    {
                        spriteType = Sprite.Type.Fruit;
                        break;
                    }
                case "VEGGIE":
                    {
                        spriteType = Sprite.Type.Veggie;
                        break;
                    }
                case "POWERUP":
                    {
                        spriteType = Sprite.Type.PowerUp;
                        break;
                    }

                case "CREATURE":
                    {
                        spriteType = Sprite.Type.Creature;
                        break;
                    }
                case "FAN":
                    {
                        spriteType = Sprite.Type.Fan;
                        break;
                    }
                case "SMASHER":
                    {
                        spriteType = Sprite.Type.Smasher;
                        break;
                    }
                case "SAW":
                    {
                        spriteType = Sprite.Type.Saw;
                        break;
                    }
                case "WINDMILL":
                    {
                        spriteType = Sprite.Type.Windmill;
                        break;
                    }
                case "TNT":
                    {
                        spriteType = Sprite.Type.Explosive;
                        break;
                    }
                case "SWITCH":
                    {
                        spriteType = Sprite.Type.Switch;
                        break;
                    }
                case "SPIKE":
                    {
                        spriteType = Sprite.Type.Spike;
                        break;
                    }
                case "SPRING":
                    {
                        spriteType = Sprite.Type.Spring;
                        break;
                    }
                case "BOSS":
                    {
                        spriteType = Sprite.Type.Boss;
                        break;
                    }
                case "TOWER":
                    {
                        spriteType = Sprite.Type.Tower;
                        break;
                    }
                default:
                    break;
            }
            return spriteType;
        }
       
        public static string GetFileNameByTextureID(int ID)
        {
            foreach (TextureData texture in levelTextures)
            {
                if (texture.TextureID == ID) return texture.FileName;
            }
            return "no texture by that ID";
        }
#endregion


        #region EDITOR ONLY METHODS (not used in game code)
        //used only by editor
        //public static int GetIDbyName(string texturetype)
        //{
        //    int ID = -1;  
        //    foreach (TextureData texture in levelTextures)
        //    {
        //        if (texture.SpriteType == texturetype)
        //            return texture.TextureID;
        //    }
        //    return ID;
        //}

        //used by editor
        //public static string GetFileByID(int id)
        //{
        //    foreach (TextureData texture in levelTextures)
        //    {
        //        if (texture.TextureID == id)
        //            return texture.FileName;
        //    }

        //    return "";
        //}

//This collision method only used by editor
        //public static bool[,] GetCollisionData(int _id, int _index)
        //{ 
        //    if (!levelTextures[_id].IsTiled) return levelTextures[_id].GetCollisionData;
        //    else
        //    {
        //        bool[,] tileCollisionData = new bool[SpriteWidth(_id),SpriteHeight(_id)];
        //        Rectangle tileRectangle = GetSourceRect(_id,_index);
        //        for (int y = 0; y < tileRectangle.Height; y++)
        //        {
        //            for (int x = 0; x < tileRectangle.Width; x++)
        //            {
        //                tileCollisionData [x,y] = levelTextures[_id].GetCollisionData[x+tileRectangle.X,y+tileRectangle.Y];
        //            }
        //        }
        //        return tileCollisionData;
        //    }
        //}

        //asks the level manager to load a texture, returns Texture ID or -1 if the ID
        //public static int Load (string filepath)
        //{
        //    int loadedTextureID = -1;
        //    TextureData newTexture = new TextureData();
        //    newTexture.FilePath = filepath;
        //    string[] seperators = new string[] { @"\" };
        //    string[] result2 = filepath.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
        //    newTexture.FileName = result2[result2.Length - 1];

            //if the texture in the filepath is already loaded, return the ID
        //    loadedTextureID = IsTextureLoaded(newTexture.FileName); 
        //    if (loadedTextureID != -1) return loadedTextureID;
            
            //if it wasnt already loaded, load it and generate an ID, and return the new ID
        //    newTexture.IsTiled = false; 
        //    newTexture.Texture = content.Load<Texture2D>(newTexture.FilePath);
        //    if (newTexture.TextureID < 0) CreateID(newTexture); 
        //    LevelDataManager.levelTextures.Add(newTexture);
        //    return newTexture.TextureID;
        //}

        //public static int Load(string filepath, bool isTiled, int tileWidth, int tileHeight)
        //{
        //    int loadedTextureID = -1;
        //    TextureData newTexture = new TextureData();
        //    newTexture.FilePath = filepath;
        //    string[] seperators = new string[] { @"\" };
        //    string[] result2 = filepath.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
        //    newTexture.FileName = result2[result2.Length - 1];

            //if the texture in the filepath is already loaded, return the ID
        //    loadedTextureID = IsTextureLoaded(newTexture.FileName);
        //    if (loadedTextureID != -1)
        //    {
        //        int loadedIndex = GetIndexByID(loadedTextureID);
        //        levelTextures[loadedIndex].IsTiled = isTiled;
        //        levelTextures[loadedIndex].TileWidth = tileWidth;
        //        levelTextures[loadedIndex].TileHeight = tileHeight;
        //        return loadedTextureID;
        //    }

            //if it wasnt already loaded, load it and generate an ID, and return the new ID
        //    else
        //    {
        //        newTexture.IsTiled = isTiled;
        //        newTexture.TileWidth = tileWidth;
        //        newTexture.TileHeight = tileHeight;
        //        newTexture.Texture = content.Load<Texture2D>(newTexture.FilePath);
        //        if (newTexture.TextureID < 0) CreateID(newTexture);
        //        LevelDataManager.levelTextures.Add(newTexture);
        //        return newTexture.TextureID;
        //    }
        //}
        //private static void CreateID(TextureData newTexture)
        //{
        //    newTexture.TextureID += 1;         
        //        foreach (TextureData loadedTextures in levelTextures)
        //        {
        //            if (loadedTextures.TextureID == newTexture.TextureID) CreateID(newTexture);
        //        }
        //    return;
        //}
        //check by filename to see if a texture has already been loaded
        //public static int IsTextureLoaded(string filename)
        //{
        //    int loadedID = -1;
        //    foreach (TextureData texture in levelTextures)
        //    {
        //        if (texture.FileName == filename) return texture.TextureID;
        //    }
        //    return loadedID;
        //}
        //private static int GetIndexByID(int ID)
        //{
        //    for( int i = 0; i < levelTextures.Count; i++)
        //    {
        //        if (levelTextures[i].TextureID == ID) return i;
        //    }
        //    return -1;
        //}

        //public static void SetSpriteType(int ID, string type)
        //{
        //    levelTextures[ID].SpriteType = type;
        //}

        //public static void SetAnimatedFlag(int ID, bool flag)
        //{
        //    levelTextures[ID].IsAnimated = flag;
        //}

        //public static void SetTiledFlag(int ID, bool flag)
        //{
        //    levelTextures[ID].IsTiled = flag;
        //}
#endregion

        #region LOAD
        //pass initialized TextureData, for loading of levels in mapeditor and game
        public static void Load(TextureData newTexture)
        {

            if (newTexture.SpriteType == "EFFECT")
            {
                newTexture.Texture = effectsContent.Load<Texture2D>(newTexture.FilePath);
                LevelDataManager.effectTextures.Add(newTexture);
            }
            else
            {
                newTexture.Texture = levelContent.Load<Texture2D>(newTexture.FilePath);
                LevelDataManager.levelTextures.Add(newTexture);
            }
        }
        #endregion

        #region LOAD MAP
        public static void LoadMap(string _loadFilePath)
        {
            LoadWorld(_loadFilePath);

            if (worldChanged)
            {
                LoadTextures(levelContent.RootDirectory + texturePack);
                worldChanged = false;
            }

            FileStream fs = new FileStream(_loadFilePath, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(fs);
            string data = read.ReadLine();
            do
            {
                switch (data)
                {
                    case "<LAYER>":
                        LoadLayerData(read);
                        break;
                    case "<SPRITE>":
                        LoadSpriteData(read);
                        break;
                    default:
                        break;
                }

                data = read.ReadLine();
            } while (data != "<ENDFILE>");


            read.Close();
            fs.Close();

            return;

        }

        public static void LoadWorld(string _loadFilePath)
        {
            FileStream fs = new FileStream(_loadFilePath, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(fs);
            string data = read.ReadLine();
            bool isLoadWorldDone = false;
            do
            {
                switch (data)
                {
                    case "<WORLD>":
                        isLoadWorldDone = LoadWorldData(read);
                        break;
                    default:
                        break;
                }

                data = read.ReadLine();
            } while (isLoadWorldDone == false);


            read.Close();
            fs.Close();
            return;
        }

        public static void LoadTextures(string _loadFilePath)
        {
            FileStream fs = new FileStream(_loadFilePath, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(fs);
            string data = read.ReadLine();
            do
            {
                switch (data)
                {
                    case "<TEXTUREDATA>":
                        LoadTextureData(read);
                        break;
                    default:
                        break;
                }

                data = read.ReadLine();
            } while (data != "<ENDFILE>");
            read.Close();
            fs.Close();
            return;
        }

        public static void LoadLevelFile(string _loadFilePath)
        {
            FileStream fs = new FileStream(_loadFilePath, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(fs);
            string data = read.ReadLine();
            do
            {
                switch (data)
                {
                    case "<DATA>":
                        LoadLevelData(read);
                        break;
                    default:
                        break;
                }

                data = read.ReadLine();
            } while (data != "<ENDFILE>");
            read.Close();
            fs.Close();
            return;
        }

        public static void LoadLevelData(StreamReader read)
        {
            //variables to initialize new LevelData when read in
            int world = 0;
            int level = 0;
            string name = null;
            int bronze = 0;
            int silver = 0;
            int gold = 0;
            bool safety = false;
            bool isIntro = false;
            List<string> textBox = new List<string>();
            List<int> speaker = new List<int>();
            List<Vector2> cursor = new List<Vector2>();
            List<Vector2> look = new List<Vector2>();

            string line = read.ReadLine();
            string elementName = null;

            do
            {
                string[] stringSeparators = new string[] { ":" };
                string[] result = line.Split(stringSeparators,
                StringSplitOptions.RemoveEmptyEntries);
                elementName = result[0];

                switch (elementName)
                {
                    case "World":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) world = value;
                            break;
                        }
                    case "Level":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) level = value;
                            break;
                        }
                    case "Name":
                        {
                            name = result[1];
                            break;
                        }
                    case "B":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) bronze = value;
                            break;
                        }
                    case "S":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) silver = value;
                            break;
                        }
                    case "G":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) gold = value;
                            break;
                        }
                    case "Safety":
                        {
                            bool value;
                            if (bool.TryParse(result[1], out value)) safety = value;
                            break;
                        }
                    case "Intro":
                        {
                            bool value;
                            if (bool.TryParse(result[1], out value)) isIntro = value;
                            break;
                        }
                    case "Text":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) speaker.Add(value);
                            textBox.Add(result[2]);
                            break;
                        }
                    case "Cursor":
                        {
                            int value;
                            bool bvalue = false;
                            Vector2 cursorLoc = Vector2.Zero;
                            if (bool.TryParse(result[1], out bvalue))
                            {
                                if (bvalue) if (int.TryParse(result[2], out value)) cursorLoc = new Vector2(value, cursorLoc.Y);
                                if (bvalue) if (int.TryParse(result[3], out value)) cursorLoc = new Vector2(cursorLoc.X, value);
                            }
                            cursor.Add(cursorLoc);
                            break;
                        }
                    case "Look":
                        {
                            int value;
                            bool bvalue = false;
                            Vector2 lookLoc = Vector2.Zero;
                            if (bool.TryParse(result[1], out bvalue))
                            {
                                if (bvalue) if (int.TryParse(result[2], out value)) lookLoc = new Vector2(value, lookLoc.Y);
                                if (bvalue) if (int.TryParse(result[3], out value)) lookLoc = new Vector2(lookLoc.X, value);
                            }
                            look.Add(lookLoc);
                            break;
                        }

                    default:
                        break;
                }
                line = read.ReadLine();
            } while (line != "<ENDDATA>");

            LevelData templevelData = new LevelData(world, level, name, bronze, silver, gold, safety, isIntro,textBox,speaker,cursor,look);
            levelData[world, level] = templevelData;
            return;
        }

        public static bool LoadWorldData(StreamReader read)
        {
            string line = read.ReadLine();
            string elementName = null;
            int worldWidth = 0;
            int worldHeight = 0;
            int worldGridTileWidth = 0;
            int worldGridTileHeight = 0;
            int worldGridMapWidth = 0;
            int worldGridMapHeight = 0;
            do
            {
                string[] stringSeparators = new string[] { ":" };
                string[] result = line.Split(stringSeparators,
                StringSplitOptions.RemoveEmptyEntries);
                elementName = result[0];

                switch (elementName)
                {
                    case "WorldGridTileWidth":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) worldGridTileWidth = value;
                            break;
                        }
                    case "WorldGridTileHeight":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) worldGridTileHeight = value;
                            break;
                        }
                    case "WorldGridMapWidth":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) worldGridMapWidth = value;
                            break;
                        }
                    case "WorldGridMapHeight":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) worldGridMapHeight = value;
                            break;
                        }
                    case "TexturePack":
                        {
                            texturePack = result[1];
                            break;
                        }
                    default:
                        break;
                }
                line = read.ReadLine();
            } while (line != "<ENDWORLD>");

            worldWidth = worldGridTileWidth * worldGridMapWidth;
            worldHeight = worldGridTileHeight * worldGridMapHeight;
            Camera.WorldRectangle = new Rectangle(0, 0, worldWidth, worldHeight);
            Camera.Position = new Vector2(0, (worldHeight - Camera.ViewportHeight));
            return true;
        }

        public static void LoadTextureData(StreamReader read)
        {
            //variables to initialize new TextureData when read in
            string filePath = null;
            string fileName = null;
            int textureID = -1;
            string spriteType = "";
            bool isTiled = false;
            bool isAnimated = false;
            int tileWidth = 0;
            int tileHeight = 0;

            string line = read.ReadLine();
            string elementName = null;

            do
            {
                string[] stringSeparators = new string[] { ":" };
                string[] result = line.Split(stringSeparators,
                StringSplitOptions.RemoveEmptyEntries);
                elementName = result[0];

                switch (elementName)
                {
                    case "FilePath":
                        {
                            filePath = result[1];
                            break;
                        }
                    case "FileName":
                        {
                            fileName = result[1];
                            break;
                        }
                    case "TextureID":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) textureID = value;
                            break;
                        }
                    case "SpriteType":
                        {
                            if (result.Length > 1) spriteType = result[1];
                            break;
                        }
                    case "IsTiled":
                        {
                            bool value;
                            if (bool.TryParse(result[1], out value)) isTiled = value;
                            break;
                        }
                    case "IsAnimated":
                        {
                            bool value;
                            if (bool.TryParse(result[1], out value)) isAnimated = value;
                            break;
                        }
                    case "TileWidth":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) tileWidth = value;
                            break;
                        }
                    case "TileHeight":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) tileHeight = value;
                            break;
                        }
                    default:
                        break;
                }
                line = read.ReadLine();
            } while (line != "<ENDTEXTUREDATA>");

            TextureData textureData = new TextureData(filePath, fileName, textureID, spriteType, isTiled, isAnimated, tileWidth, tileHeight);
            LevelDataManager.Load(textureData);

            return;
        }

        public static void LoadLayerData(StreamReader read)
        {
            //variables to initialize new Layer when read in
            string layerName = "";
            Vector2 layerParallax = new Vector2(1f, 0f);
            bool isAwake = true;
            bool isVisible = true;
            bool isLayerMotion = false;
            float layerVelocity = 0.0f;
            Vector2 layerVDirection = Vector2.Zero;
            bool isLayerGravity = false;
            float layerAcceleration = 0.0f;
            Vector2 layerADirection = Vector2.UnitY;

            string line = read.ReadLine();
            string elementName = null;

            do
            {
                string[] stringSeparators = new string[] { ":" };
                string[] result = line.Split(stringSeparators,
                StringSplitOptions.RemoveEmptyEntries);
                elementName = result[0];

                switch (elementName)
                {
                    case "LayerName":
                        {
                            layerName = result[1];
                            break;
                        }
                    case "LayerParallax":
                        {
                            float x = 1.0f;
                            float y = 1.0f;
                            if (result[1] == "{X")
                            {
                                float value;
                                string temp;
                                temp = result[2].Replace(" Y", null);
                                if (float.TryParse(temp, out value)) x = value;
                                temp = result[3].Replace("}", null);
                                if (float.TryParse(temp, out value)) y = value;
                            }
                            layerParallax = new Vector2(x, y);
                            break;
                        }
                    //case "IsAwake":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isAwake = value;
                    //        break;
                    //    }
                    //case "IsVisible":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isVisible = value;
                    //        break;
                    //    }
                    //case "IsLayerMotion":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isLayerMotion = value;
                    //        break;
                    //    }
                    //case "LayerVelocity":
                    //    {
                    //        float value;
                    //        if (float.TryParse(result[1], out value)) layerVelocity = value;
                    //        break;
                    //    }
                    //case "LayerVDirection":
                    //    {
                    //        float x = 1.0f;
                    //        float y = 1.0f;
                    //        if (result[1] == "{X")
                    //        {
                    //            float value;
                    //            string temp;
                    //            temp = result[2].Replace(" Y", null);
                    //            if (float.TryParse(temp, out value)) x = value;
                    //            temp = result[3].Replace("}", null);
                    //            if (float.TryParse(temp, out value)) y = value;
                    //        }
                    //        layerVDirection = new Vector2(x, y);
                    //        break;
                    //    }
                    //case "IsLayerGravity":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isLayerGravity = value;
                    //        break;
                    //    }
                    //case "LayerAcceleration":
                    //    {
                    //        float value;
                    //        if (float.TryParse(result[1], out value)) layerAcceleration = value;
                    //        break;
                    //    }
                    //case "LayerADirection":
                    //    {
                    //        float x = 1.0f;
                    //        float y = 1.0f;
                    //        if (result[1] == "{X")
                    //        {
                    //            float value;
                    //            string temp;
                    //            temp = result[2].Replace(" Y", null);
                    //            if (float.TryParse(temp, out value)) x = value;
                    //            temp = result[3].Replace("}", null);
                    //            if (float.TryParse(temp, out value)) y = value;
                    //        }
                    //        layerADirection = new Vector2(x, y);
                    //        break;
                    //    }
                    default:
                        break;
                }
                line = read.ReadLine();
            } while (line != "<ENDLAYER>");

            Layer newLayer = new Layer(layerName, layerParallax, isAwake, isVisible, isLayerMotion, layerVelocity, layerVDirection, isLayerGravity, layerAcceleration, layerADirection);
            parallaxEngine.AddLayerToWorld(newLayer);

            return;
        }

        public static void LoadSpriteData(StreamReader read)
        {
            //variables to initialize new Sprite, defaluts changed when read in
            Sprite.Type spriteType = Sprite.Type.Unassigned;
            int textureID = 0;
            int textureIndex = 0;
            Rectangle spriteRectangle = new Rectangle(0, 0, 1, 1); //calls property to also set location
            Color tintColor = Color.White;
            bool isFlippedHorizontally = false;
            bool isFlippedVertically = false;
            //bool isAwake = false;
            //bool isVisible = true;
            //bool isCollidable = false;
            int hitPoints = 0;
            //bool isMobile = false;
            //float velocity = 0.0f;
            //Vector2 direction = Vector2.Zero;
            //bool isRotating = false;
            //float rotationSpeed = 0.0f;
            float totalRotation = 0.0f;
            float scale = 1.0f;
            //bool isAnimated = false;
            //bool isAnimatedWhileStopped = false;
            //bool isBounceAnimated = false;
            //float animationFPS = 0.0f;
            Sprite.Pathing pathingType = Sprite.Pathing.None;
            int pathingX = 0;
            int pathingY = 0;
            int pathingSpeed = 0;
            int pathingStart = 0;
            bool isPathingInertia = false;
            float timeDelay = 0f;

            string line = read.ReadLine();
            string elementName = null;

            do
            {
                string[] stringSeparators = new string[] { ":" };
                string[] result = line.Split(stringSeparators,
                StringSplitOptions.RemoveEmptyEntries);
                elementName = result[0];

                switch (elementName)
                {
                    //case "SpriteType":
                    //    {
                    //        string textType;
                    //        if (result.Length > 1)
                    //        {
                    //            textType = result[1];
                    //            switch (textType)
                    //            {
                    //                case "NONE":
                    //                    {
                    //                        spriteType = Sprite.Type.None;
                    //                        break;
                    //                    }
                    //               case "COIN":
                    //                    {
                    //                        spriteType = Sprite.Type.Coin;
                    //                        break;
                    //                    }
                    //                case "TERRAIN":
                    //                    {
                    //                        spriteType = Sprite.Type.Terrain;
                    //                        break;
                    //                    }
                    //                case "FRUIT":
                    //                    {
                    //                        spriteType = Sprite.Type.Fruit;
                    //                        break;
                    //                    }
                    //                case "VEGGIE":
                    //                    {
                    //                        spriteType = Sprite.Type.Veggie;
                    //                        break;
                    //                    }
                    //                case "POWERUP":
                    //                    {
                    //                        spriteType = Sprite.Type.PowerUp;
                    //                        break;
                    //                    }
                    //                case "CANNON":
                    //                    {
                    //                        spriteType = Sprite.Type.Cannon;
                    //                        break;
                    //                   }
                    //                case "BLOCK":
                    //                    {
                    //                        spriteType = Sprite.Type.Block;
                    //                        break;
                    //                    }
                    //                case "HAZARD":
                    //                    {
                    //                        spriteType = Sprite.Type.Hazard;
                    //                        break;
                    //                    }
                    //                case "CREATURE":
                    //                    {
                    //                        spriteType = Sprite.Type.Creature;
                    //                        break;
                    //                    }
                    //                case "FAN":
                    //                    {
                    //                        spriteType = Sprite.Type.Fan;
                    //                        break;
                    //                    }
                    //                case "SAW":
                    //                    {
                    //                        spriteType = Sprite.Type.Saw;
                    //                        break;
                    //                    }
                    //                case "WINDMILL":
                    //                    {
                    //                        spriteType = Sprite.Type.Windmill;
                    //                        break;
                    //                    }
                    //                case "TNT":
                    //                    {
                    //                        spriteType = Sprite.Type.Explosive;
                    //                        break;
                    //                    }
                    //                default:
                    //                    break;
                    //            }
                    //        }
                    //        break;
                    //    }
                    case "ID":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) textureID = value;
                            break;
                        }
                    case "Index":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) textureIndex = value;
                            break;
                        }
                    case "Rect":
                        {
                            int x = 1;
                            int y = 1;
                            int width = 2;
                            int height = 2;
                            string temp;
                            if (result[1] == "{X")
                            {
                                int value = 1;
                                temp = result[2].Replace(" Y", null);
                                if (int.TryParse(temp, out value)) x = value;
                                temp = result[3].Replace(" Width", null);
                                if (int.TryParse(temp, out value)) y = value;
                                temp = result[4].Replace(" Height", null);
                                if (int.TryParse(temp, out value)) width = value;
                                temp = result[5].Replace("}", null);
                                if (int.TryParse(temp, out value)) height = value;
                            }
                            spriteRectangle = new Rectangle(x, y, width, height);
                            break;
                        }
                    case "Tint":
                        {
                            float r = 255;
                            float g = 255;
                            float b = 255;
                            float a = 255;
                            string temp;
                            if (result[1] == "{R")
                            {
                                float value = 255;
                                temp = result[2].Replace(" G", null);
                                if (float.TryParse(temp, out value)) r = value;
                                temp = result[3].Replace(" B", null);
                                if (float.TryParse(temp, out value)) g = value;
                                temp = result[4].Replace(" A", null);
                                if (float.TryParse(temp, out value)) b = value;
                                temp = result[5].Replace("}", null);
                                if (float.TryParse(temp, out value)) a = value;
                            }
                            a = a / 255; r = r / 255; g = g / 255; b = b / 255;
                            tintColor = new Color(r, g, b, a);
                            break;
                        }
                    case "FlipH":
                        {
                            bool value;
                            if (bool.TryParse(result[1], out value)) isFlippedHorizontally = value;
                            break;
                        }
                    case "FlipV":
                        {
                            bool value;
                            if (bool.TryParse(result[1], out value)) isFlippedVertically = value;
                            break;
                        }
                    //case "IsAwake":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isAwake = value;
                    //        break;
                    //    }
                    //case "IsVisible":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isVisible = value;
                    //        break;
                    //    }
                    //case "IsCollidable":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isCollidable = value;
                    //        break;
                    //    }
                    case "HP":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) hitPoints = value;
                            break;
                        }
                    //case "IsMobile":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isMobile = value;
                    //        break;
                    //    }
                    //case "Velocity":
                    //    {
                    //        float value;
                    //        if (float.TryParse(result[1], out value)) velocity = value;
                    //        break;
                    //    }
                    //case "Direction":
                    //    {
                    //        float x = 0.0f;
                    //        float y = 0.0f;
                    //        if (result[1] == "{X")
                    //        {
                    //            float value;
                    //            string temp;
                    //            temp = result[2].Replace(" Y", null);
                    //            if (float.TryParse(temp, out value)) x = value;
                    //            temp = result[3].Replace("}", null);
                    //            if (float.TryParse(temp, out value)) y = value;
                    //        }
                    //        direction = new Vector2(x, y);
                    //        break;
                    //    }
                    //case "IsRotating":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isRotating = value;
                    //        break;
                    //    }
                    //case "RotationSpeed":
                    //    {
                    //        float value;
                    //        if (float.TryParse(result[1], out value)) rotationSpeed = value;
                    //        break;
                    //    }
                    case "Rot":
                        {
                            float value;
                            if (float.TryParse(result[1], out value)) totalRotation = value;
                            break;
                        }
                    case "Scale:":
                        {
                            float value;
                            if (float.TryParse(result[1], out value)) scale = value;
                            break;
                        }
                    //case "IsAnimated":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isAnimated = value;
                    //        break;
                    //    }
                    //case "IsAnimatedWhileStopped":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isAnimatedWhileStopped = value;
                    //        break;
                    //    }
                    //case "IsBounceAnimated":
                    //    {
                    //        bool value;
                    //        if (bool.TryParse(result[1], out value)) isBounceAnimated = value;
                    //        break;
                    //   }
                    //case "AnimationFPS":
                    //    {
                    //        float value;
                    //        if (float.TryParse(result[1], out value)) animationFPS = value;
                    //        break;
                    //    }
                    case "Path":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) pathingType = (Sprite.Pathing)value;
                            break;
                        }
                    case "PathX":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) pathingX = value;
                            break;
                        }
                    case "PathY":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) pathingY = value;
                            break;
                        }
                    case "PathSp":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) pathingSpeed = value;
                            break;
                        }
                    case "Path%":
                        {
                            int value;
                            if (int.TryParse(result[1], out value)) pathingStart = value;
                            break;
                        }
                    case "PathI":
                        {
                            bool value;
                            if (bool.TryParse(result[1], out value)) isPathingInertia = value;
                            break;
                        }
                    case "Timer":
                        {
                            float value;
                            if (float.TryParse(result[1], out value)) timeDelay = value;
                            break;
                        }
                    default:
                        break;
                }
                line = read.ReadLine();
            } while (line != "<ENDSPRITE>");

            //if sprite type was not in the file, derive it from the textureID
            if (spriteType == Sprite.Type.Unassigned) spriteType = LevelDataManager.GetSpriteTypeFromTextureData(textureID);

            //Sprite newSprite = new Sprite(spriteType, textureID, textureIndex, spriteRectangle, tintColor, isFlippedHorizontally, isFlippedVertically,
            //                              isAwake, isVisible, isCollidable, hitPoints, isMobile, velocity, direction,
            //                              isRotating, rotationSpeed, totalRotation, scale, isAnimated, isAnimatedWhileStopped, isBounceAnimated, animationFPS,
            //                              pathingType, pathingX, pathingY, pathingSpeed, pathingStart, isPathingInertia, timeDelay);

            Sprite newSprite = new Sprite(spriteType, textureID, textureIndex, spriteRectangle, tintColor, isFlippedHorizontally, isFlippedVertically,
                               hitPoints, totalRotation, scale, pathingType, pathingX, pathingY, pathingSpeed, pathingStart, isPathingInertia, timeDelay);

            //set up pathing
            newSprite.InitPathingPoint();
            if (newSprite.pathing != Sprite.Pathing.None)
            {
                newSprite.InitializePathing();
                newSprite.IsAwake = true;
            }

            //translate sprite location in parralax for the difference in size of viewport (195) between editor and game
            //newSprite.Location = newSprite.Location + new Vector2(0, (195.0f * (1.0f - parallaxEngine.worldLayers[parallaxEngine.worldLayers.Count - 1].LayerParallax.Y)));
            parallaxEngine.worldLayers[parallaxEngine.worldLayers.Count - 1].AddSpriteToLayer(newSprite);

            return;
        }

        #endregion

        #region UNLOAD 
        //call when changing levels
        public static void UnloadLevelData()
        {
            levelTextures = new List<TextureData>();
            if (levelContent != null) levelContent.Unload();
        }

        public static void UnloadEffectsData()
        {
            effectTextures = new List<TextureData>();
            if (effectsContent != null) effectsContent.Unload();
        }

        #endregion

       public static string RootDir()
       {
            return levelContent.RootDirectory;
       }

       public static void ReadSaveGameData()
       {
           LoadLevelFile("Content\\Levels\\leveldata.lvl");

           //attempt to load options
           if (LevelDataManager.SaveDevice.FileExists(LevelDataManager.containerName, LevelDataManager.fileName_options))
           {
               try
               {
                   LevelDataManager.SaveDevice.Load(
                       LevelDataManager.containerName,
                       LevelDataManager.fileName_game,
                       stream =>
                       {
                           using (StreamReader reader = new StreamReader(stream))
                           {
                               for (int i = 0; i < 7; i++)
                               {
                                   for (int j = 1; j < 16; j++)
                                   {
                                           levelData[i, j].unlocked = bool.Parse(reader.ReadLine());
                                           levelData[i, j].starCollected = bool.Parse(reader.ReadLine());
                                           levelData[i, j].shots = int.Parse(reader.ReadLine());
                                           levelData[i, j].bestScore = int.Parse(reader.ReadLine());
                                   }
                               }
                           }
                       });
               }
               catch
               {
               }
           }

           LockSetup();

           return;
       }

       public static void LockSetup()
       {
           //ensure training unocked for game start
           for (int i = 1; i<15; i++)
           {
             if (i < 11) levelData[0, i].unlocked = true;
             else levelData[0, i].unlocked = false;
           }

           levelData[1, 1].unlocked = true;
           levelData[1, 2].unlocked = true;
           levelData[1, 3].unlocked = true;
           levelData[1, 4].unlocked = true;
           levelData[1, 5].unlocked = true;

#if XBOX
           if (Guide.IsTrialMode)
           {
               levelData[2, 1].unlocked = true;
               levelData[2, 2].unlocked = true;
               levelData[2, 3].unlocked = true;
               levelData[2, 4].unlocked = true;
               levelData[2, 5].unlocked = true;
               levelData[3, 1].unlocked = true;
               levelData[3, 2].unlocked = true;
               levelData[3, 3].unlocked = true;
               levelData[3, 4].unlocked = true;
               levelData[3, 5].unlocked = true;
               levelData[4, 1].unlocked = true;
               levelData[4, 2].unlocked = true;
               levelData[4, 3].unlocked = true;
               levelData[4, 4].unlocked = true;
               levelData[4, 5].unlocked = true;
               levelData[5, 1].unlocked = true;
               levelData[5, 2].unlocked = true;
               levelData[5, 3].unlocked = true;
               levelData[5, 4].unlocked = true;
               levelData[5, 5].unlocked = true;
           }
#endif 

       }

       public static void WriteSaveGameData()
       {
#if XBOX
           if (Guide.IsTrialMode) return;
#endif

            // make sure the device is ready
            if (LevelDataManager.SaveDevice.IsReady)
            {
                try
                {
                    // save a file asynchronously. this will trigger IsBusy to return true
                    // for the duration of the save process.
                    LevelDataManager.SaveDevice.SaveAsync(
                        LevelDataManager.containerName,
                        LevelDataManager.fileName_game,
                        stream =>
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                for (int i = 0; i < 7; i++)
                                {
                                    for (int j = 1; j < 16; j++)
                                    {
                                            writer.WriteLine(levelData[i, j].unlocked);
                                            writer.WriteLine(levelData[i, j].starCollected);
                                            writer.WriteLine(levelData[i, j].shots);
                                            writer.WriteLine(levelData[i, j].bestScore);
                                    }
                                }
                            }
                        });
                }
                catch
                {
                }
            }
           return;
       }

       public static int EarnedGold()
       {
           int earnedMedals = 0;

           //count total gold medals earned
           for (int i = 0; i < 7; i++)
           {
               for (int j = 1; j < 16; j++)
               {
                   if (LevelDataManager.levelData[i, j].IsGold()) earnedMedals += 1;
               }
           }
           return earnedMedals;
       }

       public static int EarnedStars()
       {
           int starsEarned = 0;

           //count total gold medals earned
           for (int i = 0; i < 7; i++)
           {
               for (int j = 1; j < 16; j++)
               {
                   if (LevelDataManager.levelData[i, j].starCollected) starsEarned += 1;
               }
           }
           return starsEarned;
       }
    }
}
