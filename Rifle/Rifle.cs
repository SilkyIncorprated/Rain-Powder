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
            this.reloadTime = 48;
            this.recoilpunishamount = 1.1f;
            this.recoilPattern = new Vector2(0, 2);
            this.RPM = 400;
            this.RealSprites = 2;

        }

        public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            base.InitiateSprites(sLeaser, rCam);

            sLeaser.sprites[0] = new FSprite("HuntingRifle_NormalState");
            sLeaser.sprites[1] = new FSprite("HuntingRifle_ReloadingState");
        }

        public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            base.DrawSprites(sLeaser, rCam, timeStacker, camPos);

            sLeaser.sprites[1].rotation = sLeaser.sprites[0].rotation;

            if (this.reloading > 0)
            {
                sLeaser.sprites[0].isVisible = false;
                sLeaser.sprites[1].isVisible = true;
            }
            else
            {
                sLeaser.sprites[0].isVisible = true;
                sLeaser.sprites[1].isVisible = false;
            }


        }

    }
}
