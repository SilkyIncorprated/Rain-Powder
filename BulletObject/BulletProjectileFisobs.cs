using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;
using UnityEngine;

namespace GunTest.BulletProjectile
{
    sealed class BulletProjectileFisobs : Fisob
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType AbstrBulletProjectile = new AbstractPhysicalObject.AbstractObjectType("BulletProjectile", true);

        public BulletProjectileFisobs() : base(AbstrBulletProjectile)
        {
            Icon = new SimpleIcon(spriteName: "Spear", spriteColor: Color.white);
        }

        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock unlock)
        {
            // Crate data is just floats separated by ; characters.
            string[] p = saveData.CustomData.Split(';');

            if (p.Length < 5)
            {
                p = new string[5];
            }

            var result = new BulletProjectileAbstract(world, saveData.Pos, saveData.ID)
            {
            };

            return result;
        }

    }
}
