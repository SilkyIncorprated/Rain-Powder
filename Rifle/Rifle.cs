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

namespace GunTest.Rifle
{
    class Rifle : GunClass
    {

        public override bool HeavyWeapon => true;

        public RifleAbstract abstractGun
        {
            get
            {
                return this.abstractPhysicalObject as RifleAbstract;
            }
        }

        public override bool IsObjectVaild(PhysicalObject Obj)
        {
            return (Obj is Spear);
        }

        public Rifle(RifleAbstract abstr, World world) : base(abstr, world)
        {

            this.Maxammo = 4;
            this.ammo = 4;
            this.bulletdamage = 2.25f;
            this.recoil = 9.5f;
            this.reloadTime = 2.75f;
            this.recoilpunishamount = 1.1f;
            this.recoilPattern = new Vector2(0, 2);
            this.RPM = 400;

        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1 + this.Maxammo];

            sLeaser.sprites[0] = new FSprite("HuntingRifle_NormalState");

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
