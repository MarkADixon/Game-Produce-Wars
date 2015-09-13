using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using ParallaxEngine;

namespace ProduceWars.Managers
{
    public static class SoundManager
    {
        private static List<SoundEffectInstance> soundChannel;
        private static int totalSoundChannels = 20;
        private static List<SoundEffect> soundEffects;
        private static Song BGMa;
        private static Song BGMb;
        private static int thisTrack = 0;

        //private static List<Vector2> soundQueue;
        private static int currentMusicSet = int.MaxValue;

        private static ContactListener contactListener;

       

        public enum Sound
        {
            AnimalBee, //
            AnimalBird, //
            AnimalFishChomp,
            AnimalSnowman1, //
            AnimalSnowman2, //
            AppleSpin, //
            BlockHit1, //
            BlockHit2,  //
            BreakCheese,  //
            BreakIce, //
            BreakStone, //
            BreakStone2, //
            BreakWood, //
            CannonBallHit1,
            CannonBallHit2,
            CannonShot, //
            CannonShot2, //
            Chicken,
            Click, //
            CursorMove, //
            CursorSelect, //
            DecoLava,
            DecoWater, //
            //EffectBirdsLoop,
            EffectCricketsLoop, //
            EffectLeafRustle,
            EffectRainLoop, //
            FanfareDefeat,
            FanfareVictory,
            FruitBanana, //
            FruitHit1,  //
            FruitHit2,  //
            FruitHit3,  //
            FruitLemon, //
            //HazardFlamethrower,
            HazardSaw, //
            HazardSaw2, //
            HazardSmasher,
            Lightning,
            PerfectShot, //
            PowerupFireHit, //
            PowerupFireShot, //
            PowerupIceHit, //
            PowerupIceShot, //
            PowerupLightningHit, //
            PowerupLightningShot, //
            pw_docbroc,
            pw_docbroc2,
            pw_docbroc3,
            pw_pear,
            Spring,
            StarCollect,//
            TNT, //
            TNT2 //
        }

        private static List<int> soundVolume = new List<int> //default volume of the sound file 0 to 100 (converted 0.0f to 1.0f when played
        {
            #region sound volumes
            100, //AnimalBee,
            50, //AnimalBird,
            100, //AnimalFishChomp,
            30, //AnimalSnowman1,
            30, //AnimalSnowman2,
            50, //applespin
            100, //BlockHit1, 
            100, //BlockHit2,  
            100, //BreakCheese,  
            100, //BreakIce, 
            100, //BreakStone, 
            100, //BreakStone2, 
            100, //BreakWood, 
            100, //CannonBallHit1,
            100, //CannonBallHit2,
            100, //CannonShot, 
            100, //CannonShot2, 
            100, //Chicken,
            100, //Click, 
            100, //CursorMove, 
            40, //CursorSelect, 
            100, //DecoLava
            50, //DecoWater
            //100, //EffectBirdsLoop,
            30, //EffectCricketsLoop,
            100, //EffectLeafRustle,
            40, //EffectRainLoop,
            100, //FanfareDefeat,
            100, //FanfareVictory,
            100, //FruitBanana, 
            100, //FruitHit1,  
            100, //FruitHit2,  
            100, //FruitHit3,  
            100, //FruitLemon, 
            //100, //HazardFlamethrower,
            100, //HazardSaw,
            100, //HazardSaw2, 
            100, //HazardSmasher,
            100, //Lightning,
            50, //PerfectShot, 
            100, //PowerupFireHit,
            100, //PowerupFireShot,
            100, //PowerupIceHit,
            100, //PowerupIceShot,
            100, //PowerupLightningHit,
            100, //PowerupLightningShot,
            100, //pw_docbroc,
            100, //pw_docbroc2,
            100, //pw_docbroc3,
            100, //pw_pear,
            100, //Spring
            100, //StarCollect,
            100, //TNT, 
            100, //TNT2 
            #endregion
        };

        private static List<string> soundFiles = new List<string>
        {
            #region filename list
            "AnimalBee",
            "AnimalBird",
            "AnimalFishChomp",
            "AnimalSnowman1",
            "AnimalSnowman2",
            "AppleSpin",
            "BlockHit1",
            "BlockHit2",
            "BreakCheese",
            "BreakIce",
            "BreakStone",
            "BreakStone2",
            "BreakWood",
            "CannonBallHit1",
            "CannonBallHit2",
            "CannonShot",
            "CannonShot2",
            "Chicken",
            "Click",
            "CursorMove",
            "CursorSelect",
            "DecoLava",
            "DecoWater",
            //"EffectBirdsLoop",
            "EffectCricketsLoop",
            "EffectLeafRustle",
            "EffectRainLoop",
            "FanfareDefeat",
            "FanfareVictory",
            "FruitBanana",
            "FruitHit1",
            "FruitHit2",
            "FruitHit3",
            "FruitLemon",
            //"HazardFlamethrower",
            "HazardSaw",
            "HazardSaw2",
            "HazardSmasher",
            "Lightning",
            "PerfectShot",
            "PowerupFireHit",
            "PowerupFireShot",
            "PowerupIceHit",
            "PowerupIceShot",
            "PowerupLightningHit",
            "PowerupLightningShot",
            "pw_docbroc",
            "pw_docbroc2",
            "pw_docbroc3",
            "pw_pear",
            "Spring",
            "StarCollect",
            "TNT",
            "TNT2"
            #endregion
        };


        //initialized in game1 before anything else loads
        public static void Initialize(ContentManager content)
        {
            MediaPlayer.Volume = GameSettings.MusicVolume * 0.01f;
            soundEffects = new List<SoundEffect>();
            soundChannel = new List<SoundEffectInstance>();

            for (int i = 0; i < soundFiles.Count; i++)
            {
                soundEffects.Add(content.Load<SoundEffect>(@"Audio\" + soundFiles[i]));
            }

            MediaPlayer.MediaStateChanged += new EventHandler<EventArgs>(MediaPlayer_MediaStateChanged);
            return;
        }

        static void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.State != MediaState.Stopped) return;

            if (thisTrack == 1)
            {
                if (BGMb != null)
                {
                    thisTrack = 2;
                    MediaPlayer.Play(BGMb);
                }
                else MediaPlayer.Play(BGMa);
            }
            else
            {
                if (thisTrack == 2)
                {
                    thisTrack = 1;
                    MediaPlayer.Play(BGMa);
                }
            }

            if (!GameSettings.MusicOn) PauseMusic(GameSettings.MusicOn);
        }

        public static void LoadMusic(ContentManager levelContent, int nextMusicSet)
        {
            MediaPlayer.Volume = GameSettings.MusicVolume * 0.01f;
            if (GameSettings.isBoss) nextMusicSet = 7;
            if (nextMusicSet == currentMusicSet) return;
            currentMusicSet = nextMusicSet;
            MediaPlayer.IsRepeating = false;
            BGMa = null;
            BGMb = null;

            if (currentMusicSet == 6) //bonus world
            {
                switch (LevelDataManager.level)
                {
                    case 1:
                    case 2:
                    case 4:
                        {
                            currentMusicSet = 1; //play grass
                            break;
                        }
                    case 5:
                    case 13:
                        {
                            currentMusicSet = 2; //forest
                            break;
                        }
                    case 6:
                    case 7:
                    case 8:
                        {
                            currentMusicSet = 3; //desert
                            break;
                        }
                    case 3:
                    case 14:
                        {
                            currentMusicSet = 4; //snow
                            break;
                        }
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 15:
                        {
                            currentMusicSet = 5; //factory
                            break;
                        }
                    default:
                        {
                            currentMusicSet = -1; //menu (did we miss one?)
                            break;
                        }
                }
            }


            switch (currentMusicSet)
            {

                case -1:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Title");
                        break;
                    }
                case 0:
                case 6:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Training");
                        break;
                    }
                case 1:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Grass1");
                        BGMb = levelContent.Load<Song>(@"OST\Grass2");
                        break;
                    }
                case 2:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Forest1");
                        BGMb = levelContent.Load<Song>(@"OST\Forest2");
                        break;
                    }
                case 3:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Desert1");
                        BGMb = levelContent.Load<Song>(@"OST\Desert2");
                        break;
                    }
                case 4:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Snow1");
                        BGMb = levelContent.Load<Song>(@"OST\Snow2");
                        break;
                    }
                case 5:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Factory1");
                        BGMb = levelContent.Load<Song>(@"OST\Factory2");
                        break;
                    }
                case 7:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\FinalBoss1");
                        break;
                    }
                case 8:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Credits");
                        break;
                    }
                default:
                    return;
            }

            thisTrack = LevelDataManager.rand.Next(1, 3);

            if (thisTrack == 1) MediaPlayer.Play(BGMa);
            if (thisTrack == 2)
            {
                if (BGMb != null) MediaPlayer.Play(BGMb);
                else MediaPlayer.Play(BGMa);
            }

            if(!GameSettings.MusicOn)PauseMusic(GameSettings.MusicOn);
            return;
        }

        public static void MusicSoundTest(int song)
        {
            MediaPlayer.Volume = 100;
            ContentManager levelContent = LevelDataManager.levelContent;
            MediaPlayer.IsRepeating = true;
       
            switch (song)
            {
                case 0:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Title");
                        break;
                    }
                case 1:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Credits");
                        break;
                    }
                case 2:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Training");
                        break;
                    }
                case 3:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Grass1");
                        break;
                    }
                case 4:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Grass2");
                        break;
                    }
                case 5:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Forest1");
                        break;
                    }
                case 6:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Forest2");
                        break;
                    }
                case 7:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Desert1");
                        break;
                    }
                case 8:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Desert2");
                        break;
                    }
                case 9:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Snow1");
                        break;
                    }
                case 10:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Snow2");
                        break;
                    }
                case 11:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Factory1");
                        break;
                    }
                case 12:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\Factory2");
                        break;
                    }
                case 13:
                    {
                        BGMa = levelContent.Load<Song>(@"OST\FinalBoss1");
                        break;
                    }
                default:
                    break;
            }

            MediaPlayer.Play(BGMa);
            if (!GameSettings.MusicOn) GameSettings.MusicOn=true;
            return;
        }


        public static void PauseMusic(bool isPlaying)
        {
            if (isPlaying) MediaPlayer.Resume();
            else MediaPlayer.Pause();
            return;
        }

        public static void StopMusic()
        {
            MediaPlayer.Pause();
        }

        public static void ClearSounds()
        {
            if (soundChannel.Count > 0)
            {
                for (int i = soundChannel.Count - 1; i >= 0; i--)
                {
                    soundChannel[i].Stop();
                    soundChannel[i].Dispose();
                    soundChannel.Remove(soundChannel[i]);
                }
            }
        }

        public static void Play(Sound NextSound, bool looped, bool pitchvary)
        {
            if (soundChannel.Count > 0)
            {
                for (int i = soundChannel.Count - 1; i >= 0; i--)
                {
                    if (soundChannel[i].State == SoundState.Stopped)
                    {
                        soundChannel[i].Dispose();
                        soundChannel.Remove(soundChannel[i]);
                    }
                }
            }

            if (soundChannel.Count <= totalSoundChannels)
            {
            soundChannel.Add(soundEffects[(int)NextSound].CreateInstance());
            soundChannel[soundChannel.Count - 1].IsLooped = looped;
            if (pitchvary) soundChannel[soundChannel.Count - 1].Pitch = ((float)LevelDataManager.rand.Next(-50, 51)) * 0.01f;
            soundChannel[soundChannel.Count - 1].Volume = soundVolume[(int)NextSound]*GameSettings.EffectVolume*0.0001f;
            soundChannel[soundChannel.Count - 1].Play();
            }
            return;
        }

        public static void Play(Sound sound1, Sound sound2, bool looped, bool pitch)
        {
            int rand = LevelDataManager.rand.Next(0, 2);
            if (rand == 0) Play(sound1, looped, pitch);
            if (rand == 1) Play(sound2, looped, pitch);
            return;
        }

        public static void Play(Sound sound1, Sound sound2, Sound sound3, bool looped, bool pitch)
        {
            int rand = LevelDataManager.rand.Next(0, 3);
            if (rand == 0) Play(sound1, looped, pitch);
            if (rand == 1) Play(sound2, looped, pitch);
            if (rand == 2) Play(sound3, looped, pitch);
            return;
        }


        public static void InitializeSoundEvents(ContactListener _contactListener)
        {
            contactListener = _contactListener;

            #region SUBSCRIBE TO SOUND EVENTS
            contactListener.StarCollected += new ContactListener.EffectEventHandler(contactListener_StarCollected);
            contactListener.SawCutting += new ContactListener.EffectEventHandler(contactListener_SawCutting);
            contactListener.BeeDeflection += new ContactListener.EffectEventHandler(contactListener_BeeDeflection);
            contactListener.SpringDeflection += new ContactListener.EffectEventHandler(contactListener_SpringDeflection);
            contactListener.FruitExploded += new ContactListener.EffectEventHandler(contactListener_FruitExploded);
            contactListener.VeggieExploded += new ContactListener.EffectEventHandler(contactListener_VeggieExploded);
            contactListener.CreatureExploded += new ContactListener.EffectEventHandler(contactListener_CreatureExploded);
            contactListener.FireShotExploded += new ContactListener.EffectEventHandler(contactListener_FireShotExploded);
            contactListener.IceShotExploded += new ContactListener.EffectEventHandler(contactListener_IceShotExploded);
            contactListener.LitShotExploded += new ContactListener.EffectEventHandler(contactListener_LitShotExploded);
            contactListener.BombExploded += new ContactListener.EffectEventHandler(contactListener_BombExploded);
            contactListener.ShotFired += new ContactListener.EffectEventHandler(contactListener_ShotFired);
            contactListener.SnowballThrown += new ContactListener.EffectEventHandler(contactListener_SnowballThrown);
            contactListener.PerfectShot += new ContactListener.EffectEventHandler(contactListener_PerfectShot);
            contactListener.BananaActivated += new ContactListener.EffectEventHandler(contactListener_BananaActivated);
            contactListener.LemonActivated += new ContactListener.EffectEventHandler(contactListener_LemonActivated);
            contactListener.WaterSplash += new ContactListener.EffectEventHandler(contactListener_WaterSplash);
            contactListener.LavaSplash += new ContactListener.EffectEventHandler(contactListener_LavaSplash);

            #endregion

            return;
        }

        static void contactListener_StarCollected(object sender, EffectEventArgs e)
        {
            Play(Sound.StarCollect, false, false);
        }
        static void contactListener_SawCutting(object sender, EffectEventArgs e)
        {
            if (GameSettings.CheatInvincibility) return;

            Play(Sound.HazardSaw2, Sound.HazardSaw, false, true);
        }
        static void contactListener_BeeDeflection(object sender, EffectEventArgs e)
        {
            Play(Sound.AnimalBee, false, true);
        }
        static void contactListener_SpringDeflection(object sender, EffectEventArgs e)
        {
            Play(Sound.Spring, false, true);
        }
        static void contactListener_FruitExploded(object sender, EffectEventArgs e)
        {
            Play(Sound.FruitHit1, Sound.FruitHit2, Sound.FruitHit3, false, true);
        }
        static void contactListener_VeggieExploded(object sender, EffectEventArgs e)
        {
            if (GameSettings.CheatGoodToBeBad) return;
            else
            {
                if (e.spriteA.TextureIndex != 98) Play(Sound.FruitHit1, Sound.FruitHit2, Sound.FruitHit3, false, true);
                else Play(Sound.CannonBallHit1, Sound.CannonBallHit2, false, true);
            }
        }
        static void contactListener_CreatureExploded(object sender, EffectEventArgs e)
        {
            Play(Sound.FruitHit1, Sound.FruitHit2, Sound.FruitHit3, false, true);
        }
        static void contactListener_FireShotExploded(object sender, EffectEventArgs e)
        {
            Play(Sound.PowerupFireHit, false, false);
        }
        static void contactListener_IceShotExploded(object sender, EffectEventArgs e)
        {
            Play(Sound.PowerupIceHit, false, false);
        }
        static void contactListener_LitShotExploded(object sender, EffectEventArgs e)
        {
            Play(Sound.PowerupLightningHit, false, false);
        }
        static void contactListener_BombExploded(object sender, EffectEventArgs e)
        {
            Play(Sound.TNT, Sound.TNT2, false, true);
        }
        static void contactListener_ShotFired(object sender, EffectEventArgs e)
        {
            if (e.spriteA.TextureIndex == 1)
            {
                Play(Sound.PowerupFireShot, false, false);
                return;
            }
            if (e.spriteA.TextureIndex == 2)
            {
                Play(Sound.PowerupIceShot, false, false);
                return;
            }
            if (e.spriteA.TextureIndex == 3)
            {
                Play(Sound.PowerupLightningShot, false, false);
                return;
            }

            Play(Sound.CannonShot, Sound.CannonShot2, false, true);
        }
        static void contactListener_SnowballThrown(object sender, EffectEventArgs e)
        {
            if (Camera.IsObjectVisible(e.spriteA.SpriteRectangle, new Vector2(1f, 1f)))
            {
                Play(Sound.AnimalSnowman1, Sound.AnimalSnowman2, false, true);
            }
        }
        static void contactListener_PerfectShot(object sender, EffectEventArgs e)
        {
            Play(Sound.PerfectShot, false, false);
        }
        static void contactListener_BananaActivated(object sender, EffectEventArgs e)
        {
            Play(Sound.FruitBanana, false, true);
        }
        static void contactListener_LemonActivated(object sender, EffectEventArgs e)
        {
            Play(Sound.FruitLemon, false, true);
        }
        static void contactListener_WaterSplash(object sender, EffectEventArgs e)
        {
            SoundManager.Play(SoundManager.Sound.DecoWater, false, true);
        }
        static void contactListener_LavaSplash(object sender, EffectEventArgs e)
        {
            SoundManager.Play(SoundManager.Sound.DecoLava, false, true);
        }

        public static void SetVolume(float volume)
        {
            MediaPlayer.Volume = GameSettings.MusicVolume * 0.01f * volume;
        }


    }
}
