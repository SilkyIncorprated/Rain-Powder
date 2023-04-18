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

namespace GunTest.Shotgun
{
    class Shotgun : GunClass
    {

        public override bool HeavyWeapon => true;

        public ShotgunAbstract abstractGun
        {
            get
            {
                return this.abstractPhysicalObject as ShotgunAbstract;
            }
        }
        
        public override bool IsObjectVaild(PhysicalObject Obj)
        {
            return (Obj is FirecrackerPlant);
        }

        public Shotgun(ShotgunAbstract abstr, World world) : base(abstr, world)
        {

            this.Maxammo = 2;
            this.ammo = 2;
            this.recoil = 30f;
            this.reloadTime = 60;
            this.bullets = 10;
            this.bulletdamage = .6f;
            this.FireID = SoundID.Bomb_Explode;
            this.recoilPattern = new Vector2(0, 2);
            this.RPM = 195;

        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1 + this.Maxammo];

            sLeaser.sprites[0] = new FSprite("Shotgun_NormalState");

            for (int x = 1; x < sLeaser.sprites.Length; x++)
            {
                sLeaser.sprites[x] = new FSprite("Circle20");
                sLeaser.sprites[x].y = 8f;
                sLeaser.sprites[x].x = 25f * (this.Maxammo - Mathf.Round(this.Maxammo/2));
            }

            AddToContainer(sLeaser, rCam, null);
        }

    }
}
