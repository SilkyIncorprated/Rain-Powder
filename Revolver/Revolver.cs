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
    class Revolver : GunClass
    {

        public override bool HeavyWeapon => false;

        public RevolverAbstract abstractGun
        {
            get
            {
                return this.abstractPhysicalObject as RevolverAbstract;
            }
        }

        public Revolver(RevolverAbstract abstr, World world) : base(abstr, world)
        {

            this.Maxammo = 6;
            this.ammo = 6;
            this.recoil = 13.5f;
            this.reloadTime = 2f;
            this.bulletdamage = 1.5f;
            this.twohanded = false;
            this.recoilpunishamount = 0.9f;
            this.RPM = 475;
            this.RealSprites = 1;

        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[this.RealSprites + this.Maxammo];

            sLeaser.sprites[0] = new FSprite("Revolver_NormalState");

            for (int x = 1; x < sLeaser.sprites.Length; x++)
            {
                sLeaser.sprites[x] = new FSprite("Circle20");
                sLeaser.sprites[x].y = 8f;
                sLeaser.sprites[x].x = 25f * (this.Maxammo - Mathf.Round(this.Maxammo / 2));
            }

            AddToContainer(sLeaser, rCam, null);
        }

    }
}
