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

namespace GunTest.BulletProjectile
{
    class BulletProjectile : Weapon, IDrawable
    {

        public BulletProjectile(AbstractPhysicalObject abstr, World world) : base(abstr, world)
        {
            float mass = 0.1f;
            var positions = new List<Vector2>();

            positions.Add(Vector2.zero);

            bodyChunks = new BodyChunk[positions.Count];

            // Create all body chunks
            for (int i = 0; i < bodyChunks.Length; i++)
            {
                bodyChunks[i] = new BodyChunk(this, i, Vector2.zero, 0.5f, mass / bodyChunks.Length);
            }

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

            airFriction = 1f;
            gravity = .5f;
            bounce = 0f;
            surfaceFriction = 0f;
            collisionLayer = 2;
            waterFriction = 0f;
            buoyancy = 0f;
            GoThroughFloors = false;
            base.firstChunk.loudness = 9f;
            this.tailPos = base.firstChunk.pos;

        }

        public BulletProjectileAbstract abstractBullet
        {
            get
            {
                return this.abstractPhysicalObject as BulletProjectileAbstract;
            }
        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[2];
            UnityEngine.Random.State state = UnityEngine.Random.state;
            UnityEngine.Random.InitState(this.abstractPhysicalObject.ID.RandomSeed);
            sLeaser.sprites[0] = new FSprite("Circle4", true);
            UnityEngine.Random.state = state;
            TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[]
            {
                new TriangleMesh.Triangle(0, 1, 2)
            };
            TriangleMesh triangleMesh = new TriangleMesh("Futile_White", tris, false, false);
            sLeaser.sprites[1] = triangleMesh;
            sLeaser.sprites[1].isVisible = false;   
            this.AddToContainer(sLeaser, rCam, null);
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
            sLeaser.sprites[0].x = vector.x - camPos.x;
            sLeaser.sprites[0].y = vector.y - camPos.y;
            Vector3 v = Vector3.Slerp(this.lastRotation, this.rotation, timeStacker);
            sLeaser.sprites[0].rotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), v);
            
            Vector2 vector2 = Vector2.Lerp(this.tailPos, base.firstChunk.lastPos, timeStacker);
            Vector2 a = Custom.PerpendicularVector((vector - vector2).normalized);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(0, vector + a * 2f - camPos);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(1, vector - a * 2f - camPos);
            (sLeaser.sprites[1] as TriangleMesh).MoveVertice(2, vector2 - camPos);
            sLeaser.sprites[1].isVisible = true;
            if (base.slatedForDeletetion || this.room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        

        public override void HitWall()
        {
            if (this.room.BeingViewed)
            {
                for (int i = 0; i < 7; i++)
                {
                    this.room.AddObject(new Spark(base.firstChunk.pos + this.throwDir.ToVector2() * (base.firstChunk.rad - 1f), Custom.DegToVec(UnityEngine.Random.value * 360f) * 10f * UnityEngine.Random.value + -this.throwDir.ToVector2() * 10f, new Color(1f, 1f, 1f), null, 2, 4));
                }
            }
            this.room.ScreenMovement(new Vector2?(base.firstChunk.pos), this.throwDir.ToVector2() * this.damage, 0f);
            this.room.PlaySound(SoundID.Rock_Hit_Wall, base.firstChunk);
            this.SetRandomSpin();
            this.ChangeMode(Weapon.Mode.Free);
        }

        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);
            this.Destroy();
        }

        public override void Update(bool eu)
        {
            base.Update(eu);
            base.forbiddenToPlayer = 10;
        }

        public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
        {
            if (result.obj == null)
            {
                return false;
            }
            if (result.obj == this.thrownBy)
            {
                return false;
            }
            if (result.obj is BulletProjectile)
            {
                return false;
            }
            this.ChangeMode(Weapon.Mode.Free);
            if (result.obj is Creature)
            {
                BodyChunk firstChunk = base.firstChunk;
                (result.obj as Creature).Violence(firstChunk, new Vector2?(base.firstChunk.vel * base.firstChunk.mass * this.damage), result.chunk, result.onAppendagePos, Creature.DamageType.Stab, this.damage, 15);
            }

            return true;
        }

        public override void PickedUp(Creature upPicker)
        {
            this.Destroy();
        }

        public float damage = 1f;

    }
}
