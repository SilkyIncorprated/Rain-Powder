using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using On;
using UnityEngine;
using BepInEx;
using System.Security;
using System.Security.Permissions;
using System.Collections;
using RWCustom;
using MonoMod;
using Fisobs;

namespace GunTest
{
    class GunClass : Weapon, IDrawable
    {

        public GunClass(AbstractPhysicalObject abstr, World world) : base(abstr, world)
        {
            float mass = 0.1f;
            var positions = new List<Vector2>();

            positions.Add(Vector2.zero);
            positions.Add(new Vector2(1.5f, 1));

            bodyChunks = new BodyChunk[positions.Count];

            // Create all body chunks
            for (int i = 0; i < bodyChunks.Length; i++)
            {
                bodyChunks[i] = new BodyChunk(this, i, Vector2.zero, 0.1f, mass / bodyChunks.Length);
            }

            bodyChunks[0].rad = 1f;

            bodyChunkConnections = new BodyChunkConnection[bodyChunks.Length * (bodyChunks.Length - 1) / 2];
            int connection = 0;

            // Create all chunk connections

            for (int x = 0; x < bodyChunks.Length; x++)
            {
                for (int y = x + 1; y < bodyChunks.Length; y++)
                {
                    bodyChunkConnections[connection] = new BodyChunkConnection(bodyChunks[x], bodyChunks[y], Vector2.Distance(positions[x], positions[y]), BodyChunkConnection.Type.Normal, 0.5f, -1f);
                    connection++;
                }
            }

            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.1f;
            surfaceFriction = 0.4f;
            collisionLayer = 1;
            waterFriction = 0.92f;
            buoyancy = 0.3f;
            GoThroughFloors = false;

        }

        public override void Grabbed(Creature.Grasp grasp)
        {
            base.Grabbed(grasp);
            this.grabbedBy.Add(grasp);
        }

        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);

            Vector2 center = placeRoom.MiddleOfTile(abstractPhysicalObject.pos);
            bodyChunks[0].HardSetPosition(new Vector2(0, 0) * 20f + center);
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1 + this.Maxammo];

            sLeaser.sprites[0] = new FSprite("Shotgun_NormalState");

            for (int x = 1; x < sLeaser.sprites.Length; x++)
            {
                sLeaser.sprites[x] = new FSprite("Circle20");
                sLeaser.sprites[x].y = 8f;
                sLeaser.sprites[x].x = 25f * (this.Maxammo - Mathf.Round(this.Maxammo / 2));
            }

            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            var gunspr = sLeaser.sprites[0];
            float size = bodyChunks[0].rad / this.sprSize;
            gunspr.SetPosition(Vector2.Lerp(bodyChunks[0].lastPos, bodyChunks[0].pos, timeStacker) - camPos);
            //gunspr.anchorX = 1f/100000;
            gunspr.scale = size;
            for (int i = 1; i < sLeaser.sprites.Length; i++)
            {
                var spr = sLeaser.sprites[i];

                if (this.grabbedBy.Count > 0)
                {
                    if (this.grabbedBy[0].grabber is Player)
                    {
                        spr.isVisible = true;
                        spr.SetPosition(Vector2.Lerp(this.grabbedBy[0].grabber.mainBodyChunk.lastPos, this.grabbedBy[0].grabber.mainBodyChunk.pos, timeStacker) - camPos);
                    }
                    else
                    {
                        spr.isVisible = false;
                        spr.SetPosition(Vector2.Lerp(bodyChunks[0].lastPos, bodyChunks[0].pos, timeStacker) - camPos);
                    }
                }
                else
                {
                    spr.SetPosition(Vector2.Lerp(bodyChunks[0].lastPos, bodyChunks[0].pos, timeStacker) - camPos);
                    spr.isVisible = false;
                }

                if (i > this.ammo)
                {
                    spr.scale = Mathf.Lerp(spr.scale, .125f, Time.deltaTime*25);
                    spr.color = Color.gray;
                }
                else
                {
                    spr.scale = Mathf.Lerp(spr.scale, .25f, Time.deltaTime * 25);
                    spr.color = Color.white;
                }

                spr.y += 35f;
                spr.x += (8.5f * -(this.Maxammo / 2)) + (8.5f * i);

            }

            gunspr.rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), this.aimDir) - 90f;

            if (ModManager.MSC)
            {
                if (gunspr.rotation < -90f)
                {
                    gunspr.scaleY = -size;
                }
                else
                {
                    gunspr.scaleY = size;
                }
            }

            if (slatedForDeletetion || room != rCam.room)
                sLeaser.CleanSpritesAndRemove();
        }

        public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
        {
            newContainer = rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
                newContainer.AddChild(fsprite);
        }

        public override void Update(bool eu)
        {

            recoilaimDir = new Vector2(Mathf.Lerp(recoilaimDir.x, 0, Time.deltaTime * 25), Mathf.Lerp(recoilaimDir.y, 0, Time.deltaTime * 25));

            if (this.grabbedBy.Count > 0)
            {

                Vector2 vector = new Vector2(Input.mousePosition.x + this.room.game.cameras[0].pos.x, Input.mousePosition.y + this.room.game.cameras[0].pos.y) - thrownPos;

                this.aimDir = Custom.DirVec(this.grabbedBy[0].grabber.mainBodyChunk.pos, vector + recoilaimDir);

                if (this.grabbedBy[0].grabber is Player)
                {
                    Player player = (this.grabbedBy[0].grabber as Player);
                    Vector2 vector2 = new Vector2(player.mainBodyChunk.pos.x + (this.aimDir.x * 50), player.mainBodyChunk.pos.y + (this.aimDir.y * 50));
                    int otherGrasp;

                    if (this.grabbedBy[0].graspUsed == 0)
                    {
                        otherGrasp = 1;
                    }
                    else
                    {
                        otherGrasp = 0;
                    }

                    (player.graphicsModule as PlayerGraphics).hands[this.grabbedBy[0].graspUsed].reachingForObject = true;

                    if (this.twohanded)
                    {
                        (player.graphicsModule as PlayerGraphics).hands[otherGrasp].reachingForObject = true;

                        vector2 = new Vector2(player.mainBodyChunk.pos.x + (this.aimDir.x * 15), (player.mainBodyChunk.pos.y - 4) + (this.aimDir.y * 15));
                        Vector2 vector3 = new Vector2(player.mainBodyChunk.pos.x - (this.aimDir.x * 0.5f), (player.mainBodyChunk.pos.y - 4) - (this.aimDir.y * 0.5f));
                        if (this.reloading > 0.1f)
                        {
                            (player.graphicsModule as PlayerGraphics).hands[this.grabbedBy[0].graspUsed].absoluteHuntPos = vector3 - new Vector2(0, 10);
                            (player.graphicsModule as PlayerGraphics).hands[otherGrasp].absoluteHuntPos = vector2 - new Vector2(0, 10);
                        }
                        else
                        {
                            (player.graphicsModule as PlayerGraphics).hands[this.grabbedBy[0].graspUsed].absoluteHuntPos = vector3;
                            (player.graphicsModule as PlayerGraphics).hands[otherGrasp].absoluteHuntPos = vector2;
                        }
                    }
                    else
                    {
                        if (this.reloading > 0.1f)
                        {
                            (player.graphicsModule as PlayerGraphics).hands[this.grabbedBy[0].graspUsed].absoluteHuntPos = player.mainBodyChunk.pos - new Vector2(0, 10);
                        }
                        else
                        {
                            (player.graphicsModule as PlayerGraphics).hands[this.grabbedBy[0].graspUsed].absoluteHuntPos = vector2;
                        }
                    }

                    if (player.input[0].thrw && this.auto)
                    {
                        if (Time.time - this.lastFire >= (60f/ this.RPM))
                        {
                            this.Shoot(player, eu);
                        }
                    }

                }

            }

            if (this.recoilpunish > .1f)
            {
                this.recoilpunish -= Time.deltaTime;
            }

            base.Update(eu);
        }

        public AbstractPhysicalObject GetBullet()
        {
            AbstractPhysicalObject bullet = new BulletProjectile.BulletProjectileAbstract(this.room.world, this.room.GetWorldCoordinate(thrownPos), this.room.game.GetNewID())
            {

            };

            return bullet;
        }

        public void Shoot(Creature thrownBy, bool eu)
        {
            if (this.ammo == 0 | this.reloading > 0.1f | Time.time - this.lastFire < (60f/this.RPM))
            {
                if (!(Time.time - this.lastFire < (60f / this.RPM)))
                {
                    this.room.PlaySound(SoundID.Gate_Bolt, this.bodyChunks[0], false, 0.5f, 6f);
                }
            }
            else
            {

                Vector2 gunRecoil = this.aimDir * recoil;
                for (int i = 1; i <= bullets; i++)
                {

                    Vector2 fireDir = (this.aimDir * 2) + (new Vector2(UnityEngine.Random.Range(-this.spread, this.spread), UnityEngine.Random.Range(-this.spread, this.spread)));

                    AbstractPhysicalObject bullet = GetBullet();

                    this.room.abstractRoom.AddEntity(bullet);
                    bullet.RealizeInRoom();

                    Creature shotBy = this.grabbedBy[0].grabber;

                    Vector2 firePos = this.firstChunk.pos + (this.aimDir * 5);

                    (bullet.realizedObject as BulletProjectile.BulletProjectile).damage = this.bulletdamage;
                    (bullet.realizedObject as BulletProjectile.BulletProjectile).setPosAndTail(firePos);
                    (bullet.realizedObject as BulletProjectile.BulletProjectile).Shoot(shotBy, firePos, fireDir, 1.5f, eu);
                    (bullet.realizedObject as BulletProjectile.BulletProjectile).changeDirCounter = 0;
                    this.room.AddObject(new Spark(firePos, Custom.RNV(), Color.white, null, 40, 60));

                }

                thrownBy.mainBodyChunk.vel -= gunRecoil;
                this.firstChunk.vel -= new Vector2(gunRecoil.x, gunRecoil.y * 2);
                this.room.PlaySound(this.FireID, this.bodyChunks[1]);
                this.room.AddObject(new ExplosionSpikes(this.room, this.bodyChunks[1].pos, 4, 15f, 10f, 15f, 24f, color: Color.white));

                this.ammo -= 1;

                //this.recoilaimDir += this.recoilPattern * recoil;

                if (this.recoilpunish > this.recoilthreshold)
                {
                    this.grabbedBy[0].grabber.Stun(50);
                    this.AllGraspsLetGoOfThisObject(true);
                    this.bodyChunks[0].vel = gunRecoil;
                }

                this.recoilpunish += this.recoilpunishamount;

                this.lastFire = Time.time;

            }
        }

        public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
        {

            this.Shoot(thrownBy, eu);

            //base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        }

        public float recoil = 5f;
        public bool auto = false;
        public Vector2 aimDir;
        public Vector2 recoilaimDir = new Vector2(0, 0);
        public float reloading = 0;
        public float reloadTime = 1f;
        public int ammo = 30;
        public int Maxammo = 30;
        public float bulletdamage = 1f;
        public float spread = 0f;
        public int bullets = 1;
        public float recoilpunish = 0;
        public float recoilthreshold = 3;
        public float recoilpunishamount = 1.5f;
        public bool twohanded = true;
        public SoundID FireID = SoundID.Firecracker_Bang;
        public Vector2 recoilPattern = new Vector2(0, 1f);
        public int RPM = 500;
        public float lastFire = Time.time;
        public float sprSize = 2;

        public virtual bool IsObjectVaild(PhysicalObject Obj)
        {
            return (Obj is Rock);
        }

    }
}