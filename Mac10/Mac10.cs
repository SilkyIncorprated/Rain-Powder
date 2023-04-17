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

namespace GunTest.Mac10
{
    class Mac10 : GunClass
    {

        public override bool HeavyWeapon => false;

        public Mac10Abstract abstractGun
        {
            get
            {
                return this.abstractPhysicalObject as Mac10Abstract;
            }
        }

        public Mac10(Mac10Abstract abstr, World world) : base(abstr, world)
        {

            this.Maxammo = 30;
            this.ammo = 30;
            this.recoil = 3.5f;
            this.reloadTime = 2.25f;
            this.bulletdamage = 0.45f;
            this.twohanded = false;
            this.recoilpunishamount = 0.15f;
            this.spread = .05f;
            this.auto = true;
            this.sprSize = 2.5f;
            this.RPM = 950;

        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[1 + this.Maxammo];

            sLeaser.sprites[0] = new FSprite("Mac10");

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
