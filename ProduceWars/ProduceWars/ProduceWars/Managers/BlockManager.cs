using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParallaxEngine;
using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace ProduceWars.Managers
{
    public class BlockManager
    {
        private List<Sprite> BlockSprites;
        private List<Sprite> StaticBlockSprites; //floats, metal and switch blocks (all immune to dying)
        private World physicsWorld;
        private ContactListener contactListener;
        private int counter = 0; //used to check blocks for out of bounds one per frame to distribute load

        public BlockManager(World _world, ContactListener _contactListener)
        {
            BlockSprites = new List<Sprite>();
            StaticBlockSprites = new List<Sprite>();
            physicsWorld = _world;
            contactListener = _contactListener;
            contactListener.DamageBlock += new ContactListener.DamageEventHandler(contactListener_DamageBlock);
            contactListener.DamageSwitch += new ContactListener.DamageEventHandler(contactListener_DamageSwitch);
            contactListener.BlockExploded += new ContactListener.EffectEventHandler(contactListener_BlockExploded);
        }

        void contactListener_BlockExploded(object sender, EffectEventArgs e)
        {
            physicsWorld.RemoveBody(e.spriteA.spriteBody);
            e.spriteA.IsExpired = true;
            BlockSprites.Remove(e.spriteA);
            return;
        }

        void contactListener_DamageBlock(object sender, DamageEventArgs e)
        {
            if (e.damage <= 0) return;
            if (e.damage >= e.sprite.HitPoints)
            {
                Camera.AddScore(e.sprite.HitPoints);
                e.sprite.HitPoints = 0;
                e.sprite.IsHit = true;
            }
            else
            {
                Camera.AddScore(e.damage);
                e.sprite.HitPoints -= e.damage;
            }


            if (e.sprite.HitPoints <= 0)
            {
                if (e.sprite.TextureIndex == 0) SoundManager.Play(SoundManager.Sound.BreakCheese, false, true);
                if (e.sprite.TextureIndex == 1) SoundManager.Play(SoundManager.Sound.BreakWood, false, true);
                if (e.sprite.TextureIndex == 2) SoundManager.Play(SoundManager.Sound.BreakStone, SoundManager.Sound.BreakStone2, false,true);
                if (e.sprite.TextureIndex == 3) SoundManager.Play(SoundManager.Sound.BreakIce, false,true);
                return;
            }
            else
            {
                if (e.damageType != Sprite.Type.Explosion && e.damageType != Sprite.Type.Acid && e.damageType != Sprite.Type.FireballBlast)
                {
                    SoundManager.Play(SoundManager.Sound.BlockHit1, SoundManager.Sound.BlockHit2, false, true);
                }
            }

            //changes blocks to damaged texture if they are undamaged and HP less than half (animation frame precise set to damage threshold since blocks not animated)
            if (e.sprite.TextureID < 33)
            {
                if (e.sprite.TextureIndex < 4) //cannot visually damage anything except indexes 0-3
                {
                    if (e.sprite.HitPoints <= e.sprite.AnimationFramePrecise)
                    {
                        e.sprite.TextureID += 37; //switch to damaged texture if HP below threshold
                        if (e.sprite.TextureIndex == 0) SoundManager.Play(SoundManager.Sound.BreakCheese, false,true);
                        if (e.sprite.TextureIndex == 1) SoundManager.Play(SoundManager.Sound.BreakWood, false,true);
                        if (e.sprite.TextureIndex == 2) SoundManager.Play(SoundManager.Sound.BreakStone, SoundManager.Sound.BreakStone2, false,true);
                        if (e.sprite.TextureIndex == 3) SoundManager.Play(SoundManager.Sound.BreakIce, false,true);
                        contactListener.DoPoof(e.sprite);
                    }
                }
            }

            return;
        }

        void contactListener_DamageSwitch(object sender, DamageEventArgs e)
        {
            //the switch has already been hit this shot if it is displaying frame 2
            if (e.sprite.CurrentFrame == 2 && e.sprite.Timer != 0) return;

            e.sprite.CurrentFrame = 2;
            e.sprite.Timer = 0;

            foreach (Sprite sprite in StaticBlockSprites)
            {
                if (sprite.TextureIndex == (e.sprite.TextureIndex/3) + 6 )
                {
                if (sprite.IsHit) sprite.IsHit = false;
                else sprite.IsHit = true;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (BlockSprites.Count > 0)
            {
                //check for out of bounds
                counter += 1;
                if (counter >= BlockSprites.Count) counter = 0;
                if (!Camera.WorldRectangleBuffered.Contains(BlockSprites[counter].SpriteRectangle) && BlockSprites[counter].pathing == Sprite.Pathing.None)
                {
                    Camera.AddScore(BlockSprites[counter].HitPoints);
                    BlockSprites[counter].IsHit = true;
                }   


                for (int i = BlockSprites.Count - 1; i >= 0; i--)
                {
                    BlockSprites[i].Update(gameTime);
                    if (BlockSprites[i].IsHit && GameSettings.BlockDeathEnabled) contactListener.ExplodeBlock(BlockSprites[i]);
                }
             
            }

            if (StaticBlockSprites.Count > 0)
            {
                for (int i = StaticBlockSprites.Count - 1; i >= 0; i--)
                {
                    StaticBlockSprites[i].Update(gameTime);
                    if (StaticBlockSprites[i].TextureIndex >= 6 && StaticBlockSprites[i].TextureIndex <= 9) UpdateFadeAnimation(gameTime, StaticBlockSprites[i]); //hit static blocks are for switches, fade them out or in, hit is out, not hit is in
                }
            }
            return;
        }

        private void UpdateFadeAnimation(GameTime gameTime, Sprite sprite)
        {
            if (sprite.IsHit && sprite.spriteBody.IsSensor) return; //switch block is off
            if (!sprite.IsHit && !sprite.spriteBody.IsSensor) return; //switch block is on
            if (sprite.IsHit && !sprite.spriteBody.IsSensor) //switch block is turning off
            {
                int alpha = sprite.TintColor.A;
                alpha -= (int)gameTime.ElapsedGameTime.TotalMilliseconds/2;
                if(alpha < 50)
                { 
                    alpha = 50;
                    sprite.spriteBody.IsSensor = true;
                }
                sprite.TintColor = new Color (alpha,alpha,alpha,alpha);
            }
            if (!sprite.IsHit && sprite.spriteBody.IsSensor) //switch block coming on
            {
                int alpha = sprite.TintColor.A;
                alpha += (int)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
                if (alpha > 255)
                {
                    alpha = 255;
                    sprite.spriteBody.IsSensor = false;
                }
                sprite.TintColor = new Color(alpha, alpha, alpha, alpha);
            }
            return;
        }

        public int SpriteCount
        {
            get { return BlockSprites.Count; }
        }



        public void CreateBlock(Sprite sprite, World physicsWorld)
        {
            sprite.SpriteType = Sprite.Type.Block; //unless changed ot static when initialized, ensure Block is set

            if (sprite.TextureIndex != 6 && sprite.TextureIndex != 7 && sprite.TextureIndex != 8 && sprite.TextureIndex != 9) //IF it is Not a switch block
            {
                sprite.HitPoints = GameSettings.BlockBaseHP;
            }

            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            int sizeMultiplier = 1;

            switch (sprite.TextureID)
            {
                case 22: //10x1 square
                case 59:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 10;
                        break;
                    }
                case 23: //1x1 sqaure
                case 60:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 1;
                        break;
                    }
                case 24: //2x1 square
                case 61:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 2;
                        break;
                    }
                case 25: //2x2 square
                case 62:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 4;
                        break;
                    }
                case 26: //2x2 sphere
                case 63:
                    {
                        float radius = ConvertUnits.ToSimUnits(sprite.SpriteRectWidth / 2.0f);
                        sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, radius, radius, 10, 1.0f, position, sprite);
                        sizeMultiplier = 4;
                        break;
                    }
                case 27: //4x1 square
                case 64:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 4;
                        break;
                    }
                case 28: //4x2 square
                case 65:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 8;
                        break;
                    }
                case 29: //4x4 square
                case 66:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 16;
                        break;
                    }
                case 30: //4x4 sphere
                case 67:
                    {
                        float radius = ConvertUnits.ToSimUnits(sprite.SpriteRectWidth / 2.0f);
                        sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, radius, radius, 10, 1.0f, position, sprite);
                        sizeMultiplier = 16;
                        break;
                    }
                case 31: //6x6 square
                case 68:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 36;
                        break;
                    }
                case 32: //8x1 square
                case 69:
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sizeMultiplier = 8;
                        break;
                    }
                default:
                    break;
            }

            sprite.HitPoints = sprite.HitPoints * sizeMultiplier;
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            
            switch (sprite.TextureIndex)
            {
                    case 0:
                        {
                            SetCheesePhysics(sprite);
                            break;
                        }
                    case 1:
                        {
                            SetWoodPhysics(sprite);
                            break;
                        }
                    case 2:
                        {
                            SetStonePhysics(sprite);
                            break;
                        }
                    case 3:
                        {
                            SetIcePhysics(sprite);
                            break;
                        }
                    case 4:
                        {
                            SetMetalPhysics(sprite);
                            break;
                        }
                    case 5:
                        {
                            SetFloatPhysics(sprite);
                            break;
                        }
                    case 6:
                    case 7:
                    case 8:
                    case 9: //color switch blocks, off unless 
                        {
                            if (sprite.HitPoints == 0)
                            {
                                sprite.TintColor = new Color(50, 50, 50, 50);
                                sprite.IsHit = true;
                                sprite.spriteBody.IsSensor = true;
                            }
                            SetSwitchBlockPhysics(sprite);
                            break;
                        }
                    default:
                        break;
                
            }
            
            sprite.AnimationFramePrecise = (float)sprite.HitPoints / 2f; //since blocsk not animated, animationframe is variable representing damaged threshold
            if (sprite.TextureID > 58) sprite.HitPoints = (int)sprite.AnimationFramePrecise; //if block started with damaged texture, give it damaged HP

            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            if (sprite.pathing != Sprite.Pathing.None)
            {
                sprite.spriteBody.IgnoreGravity = true;
            }

            if (sprite.SpriteType == Sprite.Type.Block) BlockSprites.Add(sprite);
            if (sprite.SpriteType == Sprite.Type.StaticBlock || sprite.SpriteType == Sprite.Type.CannonBall) StaticBlockSprites.Add(sprite);


            return;
        }

        private void SetCheesePhysics(Sprite cheese)
        {
            cheese.spriteBody.BodyType = BodyType.Dynamic;
            cheese.spriteBody.IgnoreGravity = false;
            cheese.spriteBody.Restitution = GameSettings.CheeseRestitution;
            cheese.spriteBody.Friction = GameSettings.CheeseFriction;
            cheese.spriteBody.Mass = cheese.spriteBody.Mass * GameSettings.CheeseDensity;
            cheese.HitPoints = (int)(cheese.HitPoints * GameSettings.CheeseHPMultiplier);
            return;
        }

        private void SetWoodPhysics(Sprite wood)
        {
            wood.spriteBody.BodyType = BodyType.Dynamic;
            wood.spriteBody.IgnoreGravity = false;
            wood.spriteBody.Restitution = GameSettings.WoodRestitution;
            wood.spriteBody.Friction = GameSettings.WoodFriction;
            wood.spriteBody.Mass = wood.spriteBody.Mass * GameSettings.WoodDensity;
            wood.HitPoints = (int)(wood.HitPoints * GameSettings.WoodHPMultiplier);
            return;
        }

        private void SetStonePhysics(Sprite stone)
        {
            stone.spriteBody.BodyType = BodyType.Dynamic;
            stone.spriteBody.IgnoreGravity = false;
            stone.spriteBody.Restitution = GameSettings.StoneRestitution;
            stone.spriteBody.Friction = GameSettings.StoneFriction;
            stone.spriteBody.Mass = stone.spriteBody.Mass * GameSettings.StoneDensity;
            stone.HitPoints = (int)(stone.HitPoints * GameSettings.StoneHPMultiplier);
            return;
        }

        private void SetIcePhysics(Sprite ice)
        {
            ice.spriteBody.BodyType = BodyType.Dynamic;
            ice.spriteBody.IgnoreGravity = false;
            ice.spriteBody.Restitution = GameSettings.IceRestitution;
            ice.spriteBody.Friction = GameSettings.IceFriction;
            ice.spriteBody.Mass = ice.spriteBody.Mass * GameSettings.IceDensity;
            ice.HitPoints = (int)(ice.HitPoints * GameSettings.IceHPMultiplier);
            return;
        }

        private void SetMetalPhysics(Sprite metal)
        {
            if (metal.TextureID == 26 || metal.TextureID == 30) //metal spheres are mobile objects (cannonballs)
            {
                metal.spriteBody.BodyType = BodyType.Dynamic;
                metal.spriteBody.IgnoreGravity = false;
                metal.SpriteType = Sprite.Type.CannonBall; 
            }
            else //metal blocks are static floaters
            {
                metal.spriteBody.BodyType = BodyType.Static;
                metal.spriteBody.IgnoreGravity = true;
                metal.SpriteType = Sprite.Type.StaticBlock;
            }

            metal.spriteBody.Restitution = GameSettings.MetalRestitution;
            metal.spriteBody.Friction = GameSettings.MetalFriction;
            metal.spriteBody.Mass = metal.spriteBody.Mass * GameSettings.MetalDensity;
            metal.HitPoints = (int)(metal.HitPoints * GameSettings.MetalHPMultiplier);
            return;
        }

        private void SetFloatPhysics(Sprite floater)
        {
            floater.SpriteType = Sprite.Type.StaticBlock;
            floater.spriteBody.BodyType = BodyType.Kinematic;
            floater.spriteBody.IgnoreGravity = true;
            floater.spriteBody.Restitution = 0.3f;
            floater.spriteBody.Friction = 5.0f;
            floater.spriteBody.Mass = floater.spriteBody.Mass * GameSettings.MetalDensity;
            floater.HitPoints = int.MaxValue;
            return;
        }

        private void SetSwitchBlockPhysics(Sprite switchBlock)
        {
            switchBlock.SpriteType = Sprite.Type.StaticBlock;
            switchBlock.spriteBody.BodyType = BodyType.Kinematic;
            switchBlock.spriteBody.IgnoreGravity = true;
            switchBlock.spriteBody.Restitution = 0.5f;
            switchBlock.spriteBody.Friction = 1.0f;
            switchBlock.spriteBody.Mass = switchBlock.spriteBody.Mass * GameSettings.MetalDensity;
            switchBlock.HitPoints = int.MaxValue;
            return;
        }

    }
}
