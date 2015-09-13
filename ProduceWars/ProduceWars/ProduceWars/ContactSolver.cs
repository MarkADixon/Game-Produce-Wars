using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ParallaxEngine;
using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using ProduceWars.Managers;

namespace ProduceWars
{
    public class ContactSolver //solves for damages on contacts thrown by contact listener and throws follow up "result" events fro the effects and damages categories 
    {
        private ContactListener contactListener;
        private World physicsWorld;


        public ContactSolver(World _world, ContactListener _contactListener)
        {
            physicsWorld = _world;
            contactListener = _contactListener;

            #region SUBSCRIBE TO CONTACT EVENTS
            contactListener.StarContact += new ContactListener.BeginContactEventHandler(contactListener_StarContact);
            contactListener.SawContact += new ContactListener.BeginContactEventHandler(contactListener_SawContact);
            contactListener.SmasherContact += new ContactListener.BeginContactEventHandler(contactListener_SmasherContact);
            contactListener.SpikeContact += new ContactListener.BeginContactEventHandler(contactListener_SpikeContact);
            contactListener.SpringContact += new ContactListener.BeginContactEventHandler(contactListener_SpringContact);
            contactListener.AcidContact += new ContactListener.BeginContactEventHandler(contactListener_AcidContact);
            contactListener.ExplosionContact += new ContactListener.BeginContactEventHandler(contactListener_ExplosionContact);
            contactListener.FanWindContact += new ContactListener.BeginContactEventHandler(contactListener_FanWindContact);
            contactListener.PowerUpContact += new ContactListener.BeginContactEventHandler(contactListener_PowerUpContact);
            contactListener.BeehiveContact += new ContactListener.BeginContactEventHandler(contactListener_BeehiveContact);
            contactListener.FireBooContact += new ContactListener.BeginContactEventHandler(contactListener_FireBooContact);
            contactListener.SawShotContact += new ContactListener.BeginContactEventHandler(contactListener_SawShotContact);
            contactListener.FireballShotContact += new ContactListener.BeginContactEventHandler(contactListener_FireballShotContact);
            contactListener.FireballBlastContact += new ContactListener.BeginContactEventHandler(contactListener_FireballBlastContact);
            contactListener.IceShotContact += new ContactListener.BeginContactEventHandler(contactListener_IceShotContact);
            contactListener.IceBlastContact += new ContactListener.BeginContactEventHandler(contactListener_IceBlastContact);
            contactListener.LightningShotContact += new ContactListener.BeginContactEventHandler(contactListener_LightningShotContact);
            contactListener.LightningBlastContact += new ContactListener.BeginContactEventHandler(contactListener_LightningBlastContact);
            contactListener.BossContact += new ContactListener.BeginContactEventHandler(contactListener_BossContact);
            #endregion

            #region SUBSCRIBE TO COLLISION EVENTS
            contactListener.TerrainCollision += new ContactListener.PostContactEventHandler(contactListener_TerrainCollision);
            contactListener.BlockCollision += new ContactListener.PostContactEventHandler(contactListener_BlockCollision);
            contactListener.StaticBlockCollision += new ContactListener.PostContactEventHandler(contactListener_StaticBlockCollision);
            contactListener.CannonBallCollision += new ContactListener.PostContactEventHandler(contactListener_CannonBallCollision);
            contactListener.FruitCollision += new ContactListener.PostContactEventHandler(contactListener_FruitCollision);
            contactListener.VeggieCollision += new ContactListener.PostContactEventHandler(contactListener_VeggieCollision);
            contactListener.TNTCollision += new ContactListener.PostContactEventHandler(contactListener_TNTCollision);
            contactListener.SwitchCollision += new ContactListener.PostContactEventHandler(contactListener_SwitchCollision);
            contactListener.FanCollision += new ContactListener.PostContactEventHandler(contactListener_FanCollision);
            contactListener.GremlinCollision += new ContactListener.PostContactEventHandler(contactListener_GremlinCollision);
            contactListener.BirdCollision += new ContactListener.PostContactEventHandler(contactListener_BirdCollision);
            contactListener.BatCollision += new ContactListener.PostContactEventHandler(contactListener_BatCollision);
            contactListener.FishCollision += new ContactListener.PostContactEventHandler(contactListener_FishCollision);
            contactListener.SpiderCollision += new ContactListener.PostContactEventHandler(contactListener_SpiderCollision);
            contactListener.SnowmanCollision += new ContactListener.PostContactEventHandler(contactListener_SnowmanCollision);
            contactListener.SnowballCollision += new ContactListener.PostContactEventHandler(contactListener_SnowballCollision);
            contactListener.FruitShotCollision += new ContactListener.PostContactEventHandler(contactListener_FruitShotCollision);
            contactListener.VeggieShotCollision += new ContactListener.PostContactEventHandler(contactListener_VeggieShotCollision);
            contactListener.ExplosiveShotCollision += new ContactListener.PostContactEventHandler(contactListener_ExplosiveShotCollision);
            contactListener.CannonBallShotCollision += new ContactListener.PostContactEventHandler(contactListener_CannonBallShotCollision);
            contactListener.BossCollision += new ContactListener.PostContactEventHandler(contactListener_BossCollision);
            #endregion
        }





        #region CONTACT EVENTS (precollision resolution, contact on sensor objects)
        //for xxxContact Events, sprite A is xxx and sprite B is the object contacted 
        void contactListener_StarContact(object sender, BeginContactEventArgs e)
        {
            // spriteA is Star, spriteB is Object
            switch (e.spriteB.SpriteType)
            {
                //if the object is the player shot, collect the star
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.FireballShot:
                case Sprite.Type.IceShot:
                case Sprite.Type.LightningShot:
                case Sprite.Type.ExplosiveShot:
                case Sprite.Type.CannonballShot:
                case Sprite.Type.SawShot:
                    {
                        contactListener.CollectStar(e.spriteA);
                        break;
                    }
                default:
                    break;
            }
            return;
        }  //setup done
        void contactListener_SawContact(object sender, BeginContactEventArgs e)
        {
            // spriteA Saw, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.LightningShot:
                case Sprite.Type.LightningBlast:
                    {
                        contactListener.ExplodeLitShot(e.spriteB);
                        contactListener.DamageToSaw(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                
                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_SmasherContact(object sender, BeginContactEventArgs e) //setup done
        {
            // spriteA smasher, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.LightningShot:
                case Sprite.Type.LightningBlast:
                    {
                        contactListener.ExplodeLitShot(e.spriteB);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }

                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_SpikeContact(object sender, BeginContactEventArgs e)
        {
            // spriteA spike, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                    {
                        contactListener.DamageToShot(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_SpringContact(object sender, BeginContactEventArgs e) //setup done
        {
            //spriteA spring, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                    {
                        contactListener.DamageToShot(e.spriteB, 1, e.spriteA.SpriteType);
                        contactListener.ActivateSpring(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, 1, e.spriteA.SpriteType);
                        contactListener.ActivateSpring(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, 1, e.spriteA.SpriteType);
                        contactListener.ActivateSpring(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.ExplosiveShot:
                case Sprite.Type.CannonballShot:
                case Sprite.Type.Explosive:
                case Sprite.Type.Snowball:
                case Sprite.Type.Block:
                case Sprite.Type.CannonBall:
                    {
                        contactListener.ActivateSpring(e.spriteA, e.spriteB);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_AcidContact(object sender, BeginContactEventArgs e)
        {
            //acid hitpoints is set as time passed between frames in ms, scale damage for slower frames, x60 /1000ms should give value of 1 for 60fps and more for less fps
            int damage = (int)((((float)e.spriteB.HitPoints * 0.1f) + 3) * (float)e.spriteA.HitPoints * 0.06f); //e.damage % of object hit points (min 1) per tick (will leave all objects severely weakened)

            // spriteA acid, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                    {
                        if (e.spriteB.spriteBody.IsSensor) break; //dont kill inside barrel for acid
                        contactListener.DamageToShot(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        //cheese, wood and stone are index 0,1,2
                        if (e.spriteB.TextureIndex < 3) contactListener.DamageToBlock(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done, contains acid damamge calculations
        void contactListener_ExplosionContact(object sender, BeginContactEventArgs e)
        {
            //the explosion object is the "Kill radius" explosion object
            // spriteA is explosion, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }

                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_FanWindContact(object sender, BeginContactEventArgs e)
        { 
            //spriteA WInd, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                case Sprite.Type.CannonballShot:
                case Sprite.Type.Fruit:
                case Sprite.Type.Veggie:
                case Sprite.Type.Explosive:
                case Sprite.Type.Snowball:
                case Sprite.Type.Block:
                case Sprite.Type.CannonBall:
                    {
                        contactListener.ActivateWindDeflection(e.spriteA, e.spriteB);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_PowerUpContact(object sender, BeginContactEventArgs e)
        {
            // spriteA powerup Barrel, spriteB object
            switch (e.spriteB.SpriteType)
            {
                //if the object is a shot, call next event
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                case Sprite.Type.CannonballShot:
                case Sprite.Type.SawShot:
                    {
                        contactListener.ActivatePowerUp(e.spriteB, e.spriteA);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_BeehiveContact(object sender, BeginContactEventArgs e)
        {
            // spriteA beehive, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                    {
                        contactListener.DamageToShot(e.spriteB, 1, e.spriteA.SpriteType);
                        contactListener.ActivateBee(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.FireballShot:
                case Sprite.Type.IceShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, 1, e.spriteA.SpriteType);
                        contactListener.ActivateBee(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, 1, e.spriteA.SpriteType);
                        contactListener.ActivateBee(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.ExplosiveShot:
                case Sprite.Type.Explosive:
                case Sprite.Type.Snowball:
                    {
                        contactListener.ActivateBee(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Saw:
                case Sprite.Type.SawShot:
                case Sprite.Type.CannonBall:
                case Sprite.Type.CannonballShot:
                case Sprite.Type.LightningShot:
                case Sprite.Type.Explosion:
                case Sprite.Type.FireballBlast:
                case Sprite.Type.LightningBlast:
                    {
                        contactListener.DamageToBee(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_FireBooContact(object sender, BeginContactEventArgs e)
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                    {
                        contactListener.DamageToShot(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.IceShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Beehive:
                    {
                        contactListener.DamageToBee(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        if (e.spriteB.TextureIndex != 2) //stone is immume
                        {
                            contactListener.DamageToBlock(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        }
                        break;
                    }
                default:
                    break;
            }
        }  //setup done
        void contactListener_SawShotContact(object sender, BeginContactEventArgs e)
        {
            // spriteA sawshot, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }

                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        contactListener.SawIsCutting(e.spriteA, e.spriteB);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_FireballShotContact(object sender, BeginContactEventArgs e)
        {
            //sprite A fireball shot, sprite b object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Star:
                    {
                        contactListener.CollectStar(e.spriteB);
                        break;
                    }
                case Sprite.Type.Terrain:
                case Sprite.Type.Fruit:
                case Sprite.Type.Veggie:
                case Sprite.Type.StaticBlock:
                case Sprite.Type.CannonBall:
                case Sprite.Type.Switch:
                case Sprite.Type.Explosive:
                case Sprite.Type.Fan:
                case Sprite.Type.Beehive:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                    {
                        e.spriteA.IsHit = true;
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB,1,Sprite.Type.FireballShot);
                        break;
                    }
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, int.MaxValue, Sprite.Type.FireballShot);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        //go through ice and kill it, other blocks explode the shot
                        if (e.spriteB.TextureIndex == 3) contactListener.DamageToBlock(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        else e.spriteA.IsHit = true;
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done  
        void contactListener_FireballBlastContact(object sender, BeginContactEventArgs e)
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                    {
                        contactListener.DamageToShot(e.spriteB, 10, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.IceShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, 10, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, 10, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Beehive:
                    {
                        contactListener.DamageToBee(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        if (e.spriteB.TextureIndex != 2) //stone is immume
                        {
                            contactListener.DamageToBlock(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        }
                        break;
                    }
                default:
                    break;
            }
        }
        void contactListener_IceShotContact(object sender, BeginContactEventArgs e) //setup done
        {
            //sprite A ice shot, sprite b object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Star:
                    {
                        contactListener.CollectStar(e.spriteB);
                        break;
                    }
                case Sprite.Type.Terrain:
                case Sprite.Type.Fruit:
                case Sprite.Type.Veggie:
                case Sprite.Type.Block:
                case Sprite.Type.StaticBlock:
                case Sprite.Type.CannonBall:
                case Sprite.Type.Switch:
                case Sprite.Type.Explosive:
                case Sprite.Type.Fan:
                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.FireballBlast:
                    {
                        e.spriteA.IsHit = true;
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_IceBlastContact(object sender, BeginContactEventArgs e)
        {
            
        }
        void contactListener_LightningShotContact(object sender, BeginContactEventArgs e)
        {
            int damage = 9;
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Star:
                    {
                        contactListener.CollectStar(e.spriteB);
                        break;
                    }
                case Sprite.Type.Terrain:
                    {
                        e.spriteA.IsHit = true;
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        if (e.spriteA.TextureIndex == 2) //stone triggers blast
                        {
                            e.spriteA.IsHit = true;
                        }
                        else
                        {
                            contactListener.DamageToBlock(e.spriteB, damage, e.spriteA.SpriteType);
                        }
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Saw:
                    {
                        contactListener.DamageToSaw(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToFan(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_LightningBlastContact(object sender, BeginContactEventArgs e)
        {
            int damage = 3;
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Block:
                    {
                        if (e.spriteA.TextureIndex != 2) //stone immune
                        {
                            contactListener.DamageToBlock(e.spriteB, damage, e.spriteA.SpriteType);
                        }
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Saw:
                    {
                        contactListener.DamageToSaw(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToFan(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
        } //setup done
        void contactListener_BossContact(object sender, BeginContactEventArgs e)
        {
            if (e.spriteB.SpriteType == Sprite.Type.FruitShot)
            {
                contactListener.DamageToShot(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                contactListener.DamageToBoss(e.spriteA, 1, e.spriteB.SpriteType);
            }
        }
        #endregion

        #region COLLISION EVENTS
        // for xxxCollision Events, xxx is sprite A and sprite B is the object it collided with
        void contactListener_TerrainCollision(object sender, PostContactEventArgs e)
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        if (e.spriteB.spriteBody.LinearVelocity.Length() > GameSettings.Vdust)
                        {
                            Vector2 poofOffset = 32f * Vector2.Normalize(e.spriteA.SpriteCenterInWorld - e.spriteB.SpriteCenterInWorld);
                            contactListener.DoSmallPoof((e.spriteB.SpriteCenterInWorld + poofOffset), e.spriteB);
                        }
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                case Sprite.Type.LightningShot:
                case Sprite.Type.IceShot:
                case Sprite.Type.SawShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        if (e.spriteB.spriteBody.LinearVelocity.Length() > GameSettings.Vdust)
                        {
                            Vector2 poofOffset = 32f * Vector2.Normalize(e.spriteA.SpriteCenterInWorld - e.spriteB.SpriteCenterInWorld);
                            contactListener.DoSmallPoof((e.spriteB.SpriteCenterInWorld + poofOffset), e.spriteB);
                        }
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_BlockCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        contactListener.DamageToBlock(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        //go through ice and kill it, other blocks explode the shot
                        if (e.spriteA.TextureIndex == 3) contactListener.DamageToBlock(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        else e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.CannonballShot:
                case Sprite.Type.CannonBall:
                case Sprite.Type.StaticBlock:
                case Sprite.Type.Switch:
                case Sprite.Type.Terrain:
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToBlock(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToBlock(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToBlock(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToBlock(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        if (e.spriteA.TextureIndex != 2) //stone is immume
                        {
                            contactListener.DamageToBlock(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        }
                        break;
                    }
                default:
                    break;
            }
            return;
        }  
        void contactListener_StaticBlockCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_CannonBallCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_FruitCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToFruit(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.VeggieShot:
                    {
                        contactListener.DamageToFruit(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToFruit(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToFruit(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Terrain:
                case Sprite.Type.StaticBlock:
                case Sprite.Type.CannonBall:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToFruit(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToFruit(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToFruit(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToFruit(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToFruit(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToFruit(e.spriteA, 10, e.spriteB.SpriteType);
                        break;
                    }

                default:
                    break;
            }
            return;
        }
        void contactListener_VeggieCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToVeggie(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FruitShot:
                    {
                        contactListener.DamageToVeggie(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToVeggie(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToVeggie(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Terrain:
                case Sprite.Type.StaticBlock:
                case Sprite.Type.CannonBall:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToVeggie(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToVeggie(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToVeggie(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToVeggie(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToVeggie(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToVeggie(e.spriteA, 10, e.spriteB.SpriteType);
                        break;
                    }

                default:
                    break;
            }
            return;
        }
        void contactListener_TNTCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        contactListener.DamageToExplosive(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.CannonballShot:
                case Sprite.Type.CannonBall:
                case Sprite.Type.StaticBlock:
                case Sprite.Type.Switch:
                case Sprite.Type.Terrain:
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToExplosive(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToExplosive(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToExplosive(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToExplosive(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToExplosive(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        e.spriteA.IsHit = true;
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_SwitchCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                    {
                        contactListener.DamageToSwitch(e.spriteA, 1, e.spriteB.SpriteType);
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToSwitch(e.spriteA, 1, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToSwitch(e.spriteA, 1, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToSwitch(e.spriteA, 1, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSwitch(e.spriteA, 1, e.spriteB.SpriteType);
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToSwitch(e.spriteA, 1, e.spriteB.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_FanCollision(object sender, PostContactEventArgs e)
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_GremlinCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_BirdCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_BatCollision(object sender, PostContactEventArgs e)  //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_FishCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_SpiderCollision(object sender, PostContactEventArgs e)
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                    {
                        e.spriteB.IsHit = true;
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        } //setup done
        void contactListener_SnowmanCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                case Sprite.Type.FireballShot:
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToCreature(e.spriteA, int.MaxValue, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_SnowballCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                case Sprite.Type.VeggieShot:
                case Sprite.Type.ExplosiveShot:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballShot:
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToSnowball(e.spriteA, 1, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Terrain:
                case Sprite.Type.StaticBlock:
                case Sprite.Type.CannonBall:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSnowball(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }

                default:
                    break;
            }
            return;
        }
        void contactListener_FruitShotCollision(object sender, PostContactEventArgs e) //setup done
        {
            
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Terrain:
                    {
                        if (e.spriteA.spriteBody.LinearVelocity.Length() > GameSettings.Vdust)
                        {
                            Vector2 poofOffset = 32f * Vector2.Normalize(e.spriteB.SpriteCenterInWorld - e.spriteA.SpriteCenterInWorld);
                            contactListener.DoSmallPoof((e.spriteA.SpriteCenterInWorld + poofOffset), e.spriteA);
                        }
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.StaticBlock:
                case Sprite.Type.CannonBall:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                case Sprite.Type.Fan:
                case Sprite.Type.Boss:
                case Sprite.Type.Tower:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }

                case Sprite.Type.Block:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToShot(e.spriteA, 10, e.spriteB.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_VeggieShotCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, e.damage , e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Terrain:
                    {
                        if (e.spriteA.spriteBody.LinearVelocity.Length() > GameSettings.Vdust)
                        {
                            Vector2 poofOffset = 32f * Vector2.Normalize(e.spriteB.SpriteCenterInWorld - e.spriteA.SpriteCenterInWorld);
                            contactListener.DoSmallPoof((e.spriteA.SpriteCenterInWorld + poofOffset), e.spriteA);
                        }
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.StaticBlock:
                case Sprite.Type.CannonBall:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }

                case Sprite.Type.Block:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                    {
                        contactListener.DamageToShot(e.spriteA, 10, e.spriteB.SpriteType);
                        break;
                    }

                default:
                    break;
            }
            return;
        }
        void contactListener_ExplosiveShotCollision(object sender, PostContactEventArgs e) //setup done
        {
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Terrain:
                    {
                        if (e.spriteA.spriteBody.LinearVelocity.Length() > GameSettings.Vdust)
                        {
                            Vector2 poofOffset = 32f * Vector2.Normalize(e.spriteB.SpriteCenterInWorld - e.spriteA.SpriteCenterInWorld);
                            contactListener.DoSmallPoof((e.spriteA.SpriteCenterInWorld + poofOffset), e.spriteA);
                        }
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }
                case Sprite.Type.StaticBlock:
                case Sprite.Type.CannonBall:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                case Sprite.Type.Fan:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        break;
                    }

                case Sprite.Type.Block:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSnowball(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToShot(e.spriteA, e.damage, e.spriteB.SpriteType);
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.FireballBlast:
                case Sprite.Type.Tower:
                case Sprite.Type.Boss:
                    {
                        e.spriteA.IsHit = true;
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_CannonBallShotCollision(object sender, PostContactEventArgs e) //setup done
        {
            // spriteA cannonball shot, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.Block:
                    {
                        contactListener.DamageToBlock(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        contactListener.DamageToFruit(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        contactListener.DamageToVeggie(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        contactListener.DamageToExplosive(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Terrain:
                    {
                        if (e.spriteA.spriteBody.LinearVelocity.Length() > GameSettings.Vdust)
                        {
                            Vector2 poofOffset = 32f * Vector2.Normalize(e.spriteB.SpriteCenterInWorld - e.spriteA.SpriteCenterInWorld);
                            contactListener.DoSmallPoof((e.spriteA.SpriteCenterInWorld + poofOffset), e.spriteA);
                        }
                        break;
                    }
                case Sprite.Type.Beehive:
                case Sprite.Type.Gremlin:
                case Sprite.Type.Bird:
                case Sprite.Type.Bat:
                case Sprite.Type.Fish:
                case Sprite.Type.Spider:
                case Sprite.Type.Snowman:
                    {
                        contactListener.DamageToCreature(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Snowball:
                    {
                        contactListener.DamageToSnowball(e.spriteB, int.MaxValue, e.spriteA.SpriteType);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        contactListener.DamageToSwitch(e.spriteB, 1, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
            return;
        }
        void contactListener_BossCollision(object sender, PostContactEventArgs e)
        {
            // spriteA boss, spriteB object
            switch (e.spriteB.SpriteType)
            {
                case Sprite.Type.FruitShot:
                    {
                        contactListener.DamageToShot(e.spriteB, e.damage, e.spriteA.SpriteType);
                        break;
                    }
                default:
                    break;
            }
        }

        #endregion

    }
}
