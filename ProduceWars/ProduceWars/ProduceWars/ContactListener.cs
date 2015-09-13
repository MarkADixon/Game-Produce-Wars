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

namespace ProduceWars
{
    public class BeginContactEventArgs : EventArgs
    {
        public Sprite spriteA;
        public Sprite spriteB;
        public Contact contact;

        public BeginContactEventArgs(Sprite _spriteA, Sprite _spriteB, Contact _contact) 
        {
		    this.spriteA = _spriteA;
		    this.spriteB = _spriteB;
            this.contact = _contact;
	    }

    }

    public class PostContactEventArgs : EventArgs
    {
        public Sprite spriteA;
        public Sprite spriteB;
        public int damage;
        public bool flag;

        public PostContactEventArgs(Sprite _spriteA, Sprite _spriteB, int _damage)
        {
            this.spriteA = _spriteA;
            this.spriteB = _spriteB;
            this.damage = _damage;
        }
        public PostContactEventArgs(Sprite _spriteA, Sprite _spriteB, int _damage, bool _flag)
        {
            this.spriteA = _spriteA;
            this.spriteB = _spriteB;
            this.damage = _damage;
            this.flag = _flag;
        }

    }

    public class EffectEventArgs : EventArgs
    {
        public Sprite spriteA;
        public Sprite spriteB;
        public Vector2 location;
        public EffectEventArgs(Sprite _spriteA)
        {
            this.spriteA = _spriteA;
        }
        public EffectEventArgs(Sprite _spriteA, Sprite _spriteB)
        {
            this.spriteA = _spriteA;
            this.spriteB = _spriteB;
        }
        public EffectEventArgs(Vector2 _location, Sprite _spriteA)
        {
            this.location = _location;
            this.spriteA = _spriteA;
        }
    }

    public class DamageEventArgs : EventArgs
    {
        public Sprite sprite;
        public int damage;
        public Sprite.Type damageType;

        public DamageEventArgs(Sprite _sprite, int _damage, Sprite.Type _damageType)
        {
            this.sprite = _sprite;
            this.damage = _damage;
            this.damageType = _damageType;
        }
    }

    public class ContactListener
    {
        World world;
        Sprite spriteA = new Sprite(19, 0, Vector2.Zero, false);
        Sprite spriteB = new Sprite(19, 0, Vector2.Zero, false);
        Vector2 relativeVelocity = Vector2.Zero;
        int damage = 0;
        
        //begin contact handlers are for sensors, they act on the object without physical collision resolution afterwards
        public delegate void BeginContactEventHandler(object sender, BeginContactEventArgs e);
        public event BeginContactEventHandler StarContact;
        public event BeginContactEventHandler SawContact;
        public event BeginContactEventHandler SmasherContact;
        public event BeginContactEventHandler SpikeContact;
        public event BeginContactEventHandler SpringContact;
        public event BeginContactEventHandler AcidContact;
        public event BeginContactEventHandler ExplosionContact;
        public event BeginContactEventHandler FanWindContact;
        public event BeginContactEventHandler PowerUpContact;
        public event BeginContactEventHandler BeehiveContact;
        public event BeginContactEventHandler FireBooContact;
        public event BeginContactEventHandler SawShotContact;
        public event BeginContactEventHandler FireballShotContact;
        public event BeginContactEventHandler FireballBlastContact;
        public event BeginContactEventHandler IceShotContact;
        public event BeginContactEventHandler IceBlastContact;
        public event BeginContactEventHandler LightningShotContact;
        public event BeginContactEventHandler LightningBlastContact;
        public event BeginContactEventHandler BossContact;

        public delegate void PostContactEventHandler(object sender, PostContactEventArgs e);
        public event PostContactEventHandler TerrainCollision;
        public event PostContactEventHandler BlockCollision;
        public event PostContactEventHandler StaticBlockCollision;
        public event PostContactEventHandler CannonBallCollision;
        public event PostContactEventHandler FruitCollision;
        public event PostContactEventHandler VeggieCollision;
        public event PostContactEventHandler TNTCollision;
        public event PostContactEventHandler SwitchCollision;
        public event PostContactEventHandler FanCollision;
        public event PostContactEventHandler GremlinCollision;
        public event PostContactEventHandler BirdCollision;
        public event PostContactEventHandler BatCollision;
        public event PostContactEventHandler FishCollision;
        public event PostContactEventHandler SpiderCollision;
        public event PostContactEventHandler SnowmanCollision;
        public event PostContactEventHandler SnowballCollision;
        public event PostContactEventHandler FruitShotCollision;
        public event PostContactEventHandler VeggieShotCollision;
        public event PostContactEventHandler ExplosiveShotCollision;
        public event PostContactEventHandler CannonBallShotCollision;
        public event PostContactEventHandler BossCollision;

        
        public delegate void EffectEventHandler(object sender, EffectEventArgs e);
        public event EffectEventHandler PowerUpActivated;
        public event EffectEventHandler FruitExploded;
        public event EffectEventHandler VeggieExploded; 
        public event EffectEventHandler BombExploded;
        public event EffectEventHandler FireShotExploded;
        public event EffectEventHandler IceShotExploded;
        public event EffectEventHandler LitShotExploded;
        public event EffectEventHandler ShotFired;
        public event EffectEventHandler FanActivated;
        public event EffectEventHandler CreatureExploded;
        public event EffectEventHandler BlockExploded;
        public event EffectEventHandler StarCollected;
        public event EffectEventHandler SawCutting;
        public event EffectEventHandler BeeDeflection;
        public event EffectEventHandler SpringDeflection;
        public event EffectEventHandler WindDeflection;
        public event EffectEventHandler Poof;
        public event EffectEventHandler SmallPoof;
        public event EffectEventHandler ImpactPoofs;
        public event EffectEventHandler SnowballThrown;
        public event EffectEventHandler TowerSawShot;
        public event EffectEventHandler NewSpiderThread;
        public event EffectEventHandler PerfectShot;
        public event EffectEventHandler BananaActivated;
        public event EffectEventHandler LemonActivated;
        public event EffectEventHandler WaterSplash;
        public event EffectEventHandler LavaSplash;
        public event EffectEventHandler EmberCreated;
        public event EffectEventHandler DropCreated;
        public event EffectEventHandler WindParticleCreated;
        public event EffectEventHandler FruitMotionBlur;
        public event EffectEventHandler CreateTNTBarrel;
        public event EffectEventHandler AddPowerBarrel;

        public delegate void DamageEventHandler(object sender, DamageEventArgs e);
        public event DamageEventHandler DamageFruit;
        public event DamageEventHandler DamageBlock;
        public event DamageEventHandler DamageVeggie;
        public event DamageEventHandler DamageShot;
        public event DamageEventHandler DamageExplosive;
        public event DamageEventHandler DamageSwitch;
        public event DamageEventHandler DamageCreature;
        public event DamageEventHandler DamageSpike;
        public event DamageEventHandler DamageSnowball;
        public event DamageEventHandler DamageSaw;
        public event DamageEventHandler DamageFan;
        public event DamageEventHandler DamageBee;
        public event DamageEventHandler DamageBoss;

        public ContactListener (World _world)
        {
            world = _world;
            world.ContactManager.BeginContact = BeginContact; //subscribe to events for pre-contacts solve from physics engine
            world.ContactManager.PostSolve = PostSolve;  //post contact event        
        }
        
       
        private bool BeginContact(Contact contact)
        {
            //return true to continue processing contact, false to cancel it

            spriteA = (Sprite)contact.FixtureA.UserData;
            spriteB = (Sprite)contact.FixtureB.UserData;

            if (spriteA != null)
            {
                //optimization code
                if (spriteA.SpriteType == Sprite.Type.Terrain || spriteA.SpriteType == Sprite.Type.Block)
                {
                    if (spriteB.SpriteType == Sprite.Type.Terrain || spriteB.SpriteType == Sprite.Type.Block)
                    {
                        return true;
                    }
                }

                switch (spriteA.SpriteType)
                {
                    case Sprite.Type.Star:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            StarContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.Saw:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            SawContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.Smasher:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            SmasherContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.Spike:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            SpikeContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.Spring:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            SpringContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.Acid:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            AcidContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.Explosion:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            ExplosionContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.PowerUp:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            PowerUpContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.FanWind:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            FanWindContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.Beehive:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            BeehiveContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.FireBoo:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            FireBooContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.SawShot:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            SawShotContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.FireballShot:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            FireballShotContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.IceShot:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            IceShotContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.LightningShot:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            LightningShotContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.FireballBlast:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            FireballBlastContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.IceBlast:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            IceBlastContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.LightningBlast:
                        {
                            BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                            LightningBlastContact(this, beginContactArgs);
                            return false;
                        }
                    case Sprite.Type.Boss:
                        {
                            if (contact.FixtureA.IsSensor)
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteA, spriteB, contact);
                                BossContact(this, beginContactArgs);
                                return false;
                            }
                            return true;
                        }        
                    default:
                        break;
                }

                if (spriteB != null) switch (spriteB.SpriteType)
                    {
                        case Sprite.Type.Star:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                StarContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.Saw:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                SawContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.Smasher:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                SmasherContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.Spike:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                SpikeContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.Spring:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                SpringContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.Acid:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                AcidContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.Explosion:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                ExplosionContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.PowerUp:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                PowerUpContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.FanWind:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                FanWindContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.Beehive:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                BeehiveContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.FireBoo:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                FireBooContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.SawShot:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                SawShotContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.FireballShot:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                FireballShotContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.IceShot:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                IceShotContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.LightningShot:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                LightningShotContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.FireballBlast:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                FireballBlastContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.IceBlast:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                IceBlastContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.LightningBlast:
                            {
                                BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                LightningBlastContact(this, beginContactArgs);
                                return false;
                            }
                        case Sprite.Type.Boss:
                            {
                                if (contact.FixtureB.IsSensor)
                                {
                                    BeginContactEventArgs beginContactArgs = new BeginContactEventArgs(spriteB, spriteA, contact);
                                    BossContact(this, beginContactArgs);
                                    return false;
                                }
                                return true;
                            }  
                        default:
                            break;
                    }
            }
            return true; //return -- direct physics solver to continue processing as physical collision since the collision is not involving above sensors 
        }

        private void PostSolve(Contact contact, ContactConstraint impulse)
        {
            spriteA = (Sprite)contact.FixtureA.UserData;
            spriteB = (Sprite)contact.FixtureB.UserData;

            //code optimization
            if (spriteA.SpriteType == Sprite.Type.Terrain && spriteB.SpriteType == Sprite.Type.Terrain) return;

            //relativeVelocity = new Vector2(Math.Abs(contact.FixtureA.Body.LinearVelocity.X - contact.FixtureB.Body.LinearVelocity.X), Math.Abs(contact.FixtureA.Body.LinearVelocity.Y - contact.FixtureB.Body.LinearVelocity.Y));
            relativeVelocity = contact.FixtureA.Body.LinearVelocity - contact.FixtureB.Body.LinearVelocity;
            if (relativeVelocity.Length() < 1) damage = 0;
            else damage = GetDamage(impulse, spriteA, spriteB);

            switch (spriteA.SpriteType)
            {
                case Sprite.Type.Terrain:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        TerrainCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Block:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        BlockCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.StaticBlock:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        StaticBlockCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.CannonBall:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        CannonBallCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Fruit:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        FruitCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Veggie:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        VeggieCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Explosive:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        TNTCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Switch:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        SwitchCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Fan:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        FanCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Gremlin:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        GremlinCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Bird:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        BirdCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Bat:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        BatCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Fish:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        FishCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Spider:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        SpiderCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Snowman:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        SnowmanCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Snowball:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        SnowballCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.FruitShot:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        FruitShotCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.VeggieShot:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        VeggieShotCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.ExplosiveShot:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        ExplosiveShotCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage);
                        CannonBallShotCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Boss:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteA, spriteB, damage, contact.FixtureA.IsSensor);
                        BossCollision(this, postContactArgs);
                        return;
                    }
                default:
                    break;
            }

            switch (spriteB.SpriteType)
            {
                case Sprite.Type.Terrain:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        TerrainCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Block:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        BlockCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.StaticBlock:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        StaticBlockCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.CannonBall:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        CannonBallCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Fruit:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        FruitCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Veggie:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        VeggieCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Explosive:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        TNTCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Switch:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        SwitchCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Fan:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        FanCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Gremlin:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        GremlinCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Bird:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        BirdCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Bat:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        BatCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Fish:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        FishCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Spider:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        SpiderCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Snowman:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        SnowmanCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Snowball:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        SnowballCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.FruitShot:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        FruitShotCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.VeggieShot:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        VeggieShotCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.ExplosiveShot:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        ExplosiveShotCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.CannonballShot:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage);
                        CannonBallShotCollision(this, postContactArgs);
                        return;
                    }
                case Sprite.Type.Boss:
                    {
                        PostContactEventArgs postContactArgs = new PostContactEventArgs(spriteB, spriteA, damage,contact.FixtureB.IsSensor);
                        BossCollision(this, postContactArgs);
                        return;
                    }
                default:
                    break;
            }

            return;
        }

        #region EFFECTS HANDLERS
        public void ActivatePowerUp(Sprite spriteA, Sprite spriteB)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(spriteA, spriteB);
            PowerUpActivated(this, effectEventArgs);
            return;
        }
        public void ExplodeFruit(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            FruitExploded(this, effectEventArgs);
            return;
        }
        public void ExplodeVeggie(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            VeggieExploded(this, effectEventArgs);
            return;
        }
        public void ExplodeBlock(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            BlockExploded(this, effectEventArgs);
            return;
        }
        public void ExplodeCreature(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            CreatureExploded(this, effectEventArgs);
            return;
        }
        public void ExplodeBomb(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            BombExploded(this, effectEventArgs);
            return;
        }
        public void ExplodeFireShot(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            FireShotExploded(this, effectEventArgs);
            return;
        }
        public void ExplodeIceShot(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            IceShotExploded(this, effectEventArgs);
            return;
        }
        public void ExplodeLitShot(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            LitShotExploded(this, effectEventArgs);
            return;
        }
        public void FireShot(Vector2 location, Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(location, sprite);
            ShotFired(this, effectEventArgs);
            return;
        }
        public void ActivateFan(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            FanActivated(this, effectEventArgs);
            return;
        }
        public void CollectStar(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            StarCollected(this, effectEventArgs);
            return;
        }
        public void SawIsCutting(Sprite saw, Sprite beingCut)
        {
            //sprite a is the saw, sprite b is being cut.  This is not the damage event, it is for sound or visual effect triggering
            EffectEventArgs effectEventArgs = new EffectEventArgs(saw,beingCut);
            SawCutting(this, effectEventArgs);
            return;
        }
        public void ActivateBee(Sprite bee, Sprite deflectedObject)
        {
            //sprite a is the bee, sprite b is being deflected.  This is not the damage event, it is for sound and effect triggering
            EffectEventArgs effectEventArgs = new EffectEventArgs(bee, deflectedObject);
            BeeDeflection(this, effectEventArgs);
            return;
        }
        public void ActivateSpring(Sprite spring, Sprite deflectedObject)
        {
            //sprite a is the spring, sprite b is being deflected.  This is not the damage event, it is for sound and effect triggering
            EffectEventArgs effectEventArgs = new EffectEventArgs(spring, deflectedObject);
            SpringDeflection(this, effectEventArgs);
            return;
        }
        public void ActivateWindDeflection(Sprite wind, Sprite deflectedObject)
        {
            //sprite a is the wind, sprite b is being deflected.  This is not a damage event, it is for sound and effect triggering
            EffectEventArgs effectEventArgs = new EffectEventArgs(wind, deflectedObject);
            WindDeflection(this, effectEventArgs);
            return;
        }
        public void DoPoof(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            Poof(this, effectEventArgs);
            return;
        }
        public void DoSmallPoof(Vector2 location, Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(location, sprite);
            SmallPoof(this, effectEventArgs);
            return;
        }
        public void DoImpactPoofs(Vector2 location, Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(location, sprite);
            ImpactPoofs(this, effectEventArgs);
            return;
        }
        public void DoCreateTNTBarrel(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            CreateTNTBarrel(this, effectEventArgs);
            return;
        }
        public void ThrowSnowball(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            SnowballThrown(this, effectEventArgs);
            return;
        }
        public void ShootTower(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            TowerSawShot(this, effectEventArgs);
            return;
        }
        public void CreateSpiderThread(Sprite spider)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(spider);
            NewSpiderThread(this, effectEventArgs);
            return;
        }
        public void ShotWasPerfect(Sprite shot, Sprite barrel)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(shot, barrel);
            PerfectShot(this, effectEventArgs);
            return;
        }
        public void ActivateBanana(Sprite banana)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(banana);
            BananaActivated(this, effectEventArgs);
            return;
        }
        public void ActivateLemon(Sprite lemon)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(lemon);
            LemonActivated(this, effectEventArgs);
            return;
        }
        public void SplashWater(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            WaterSplash(this, effectEventArgs);
            return;
        }
        public void SplashLava(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            LavaSplash(this, effectEventArgs);
            return;
        }
        public void CreateEmber(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            EmberCreated(this, effectEventArgs);
            return;
        }
        public void CreateDrop(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            DropCreated(this, effectEventArgs);
            return;
        }
        public void CreateWindParticle(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            WindParticleCreated(this, effectEventArgs);
            return;
        }
        public void CreateFruitMotionBlur(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            FruitMotionBlur(this, effectEventArgs);
            return;
        }
        public void AddThisPowerBarrel(Sprite sprite)
        {
            EffectEventArgs effectEventArgs = new EffectEventArgs(sprite);
            AddPowerBarrel(this, effectEventArgs);
            return;
        }

        #endregion

        #region DAMAGE EVENTS
        public void DamageToFruit(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageFruit(this, damageEventArgs);
            return;
        }
        public void DamageToBlock(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageBlock(this, damageEventArgs);
            return;
        }
        public void DamageToVeggie(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageVeggie(this, damageEventArgs);
            return;
        }
        public void DamageToShot(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageShot(this, damageEventArgs);
            return;
        }
        public void DamageToExplosive(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageExplosive(this, damageEventArgs);
            return;
        }
        public void DamageToSwitch(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageSwitch(this, damageEventArgs);
            return;
        }
        public void DamageToCreature(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageCreature(this, damageEventArgs);
            return;
        }
        public void DamageToSnowball(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageSnowball(this, damageEventArgs);
            return;
        }
        public void DamageToSpike(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageSpike(this, damageEventArgs);
            return;
        }
        public void DamageToSaw(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageSaw(this, damageEventArgs);
            return;
        }
        public void DamageToFan(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageFan(this, damageEventArgs);
            return;
        }
        public void DamageToBee(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageBee(this, damageEventArgs);
            return;
        }
        public void DamageToBoss(Sprite sprite, int damage, Sprite.Type damageType)
        {
            DamageEventArgs damageEventArgs = new DamageEventArgs(sprite, damage, damageType);
            DamageBoss(this, damageEventArgs);
            return;
        }
        #endregion

        public int GetDamage(ContactConstraint impulse, Sprite spriteA, Sprite spriteB)
        {
            float maxImpulse = 0;
            int damage = 0;
            for (int i = 0; i < impulse.Manifold.PointCount; ++i)
            {
                maxImpulse = Math.Max(0, impulse.Manifold.Points[i].NormalImpulse);
            }
            damage = (int)((maxImpulse - 1f) * GameSettings.DamageMultiplier); //the -1 reduces bumping around and may need to be adjusted)

            if (damage > spriteA.HitPoints) damage = spriteA.HitPoints;
            if (damage > spriteB.HitPoints) damage = spriteB.HitPoints;

            if (damage < 0) return 0;
            else return damage;
        }
        //end class ContactListener
    }



}
