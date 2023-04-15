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

namespace GunTest.RevolverStuff
{
    class Revolver : Weapon, IDrawable
    {

        public Revolver(RevolverAbstract abstr, World world) : base(abstr, world)
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

            sLeaser.sprites[0] = new FSprite("Revolver_NormalState");

            for (int x = 1; x < sLeaser.sprites.Length; x++)
            {
                sLeaser.sprites[x] = new FSprite("Circle20");
                sLeaser.sprites[x].y = 8f;
                sLeaser.sprites[x].x = 25f * (this.Maxammo - Mathf.Round(this.Maxammo/2));
            }

            AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            var gunspr = sLeaser.sprites[0];
            float size = bodyChunks[0].rad/2;
            gunspr.SetPosition(Vector2.Lerp(bodyChunks[0].lastPos, bodyChunks[0].pos, timeStacker) - camPos);
            gunspr.anchorX = 0;
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
                    spr.scale = .125f;
                    spr.color = Color.gray;
                }
                else
                {
                    spr.scale = .25f;
                    spr.color = Color.white;
                }

                spr.y += 35f;
                spr.x += (8.5f * -(this.Maxammo/2)) + (8.5f * i);

            }

            if (this.aimoverride)
            {
                gunspr.rotation = OverrideAngle;
            }
            else
            {
                gunspr.rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), this.aimDir) - 90f;
            }

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
            if (this.grabbedBy.Count > 0)
            {

                Vector2 vector = new Vector2(Input.mousePosition.x + this.room.game.cameras[0].pos.x, Input.mousePosition.y + this.room.game.cameras[0].pos.y) - thrownPos;

                this.aimDir = Custom.DirVec(this.firstChunk.pos, vector);
            }

            if (this.recoilpunish > .1f)
            {
                this.recoilpunish -= Time.deltaTime;
            }

            base.Update(eu);
        }

        public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
        {

            if (this.ammo == 0 | this.reloading > 0.1f)
            {
                this.room.PlaySound(SoundID.Gate_Bolt, this.bodyChunks[0], false, 0.5f, 6f);
            }
            else
            {

                Vector2 gunRecoil = this.aimDir * recoil;

                this.room.AddObject(new Spark(thrownPos, this.aimDir * 800f, Color.white, null, 30, 35));

                thrownBy.mainBodyChunk.vel -= gunRecoil;
                this.firstChunk.vel -= new Vector2(gunRecoil.x, gunRecoil.y * 2);
                this.room.PlaySound(SoundID.Firecracker_Bang, this.bodyChunks[1]);
                this.room.AddObject(new ExplosionSpikes(this.room, this.bodyChunks[1].pos, 4, 15f, 10f, 15f, 24f, color: Color.white));

                Vector2 wantedVector = this.firstChunk.pos + (this.aimDir * 2500);

                SharedPhysics.CollisionResult result = SharedPhysics.TraceProjectileAgainstBodyChunks(this, this.room, this.bodyChunks[1].pos, ref wantedVector, 5f, 1, this.grabbedBy[0].grabber, false);

                if (result.chunk != null)
                {
                    if (result.chunk.owner is Creature)
                    {
                        this.room.PlaySound(SoundID.Spear_Stick_In_Creature, result.chunk);
                        this.room.AddObject(new ExplosionSpikes(this.room, result.chunk.pos, 8, 15f, 7.5f, 24f, 60f, color: Color.white));
                        (result.chunk.owner as Creature).Violence(result.chunk, this.aimDir * -this.recoil, this.bodyChunks[1], result.onAppendagePos, Creature.DamageType.Stab, this.bulletdamage, 15f);
                    }
                }

                this.ammo -= 1;

                if (this.recoilpunish > 3f)
                {
                    this.grabbedBy[0].grabber.Stun(50);
                    this.AllGraspsLetGoOfThisObject(true);
                    this.bodyChunks[0].vel = gunRecoil;
                }

                this.recoilpunish += 1.5f;

            }

            //base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        }

        public RevolverAbstract abstractGun
        {
            get
            {
                return this.abstractPhysicalObject as RevolverAbstract;
            }
        }

        public float recoil = 13.5f;
        public Vector2 aimDir;
        private float OverrideAngle = 45f;
        public bool aimoverride = false;
        public float reloading = 0;
        public float reloadTime = 4.5f;
        public int ammo = 6;
        public int Maxammo = 6;
        public float bulletdamage = 1.5f;
        private float recoilpunish = 0;

    }
}
