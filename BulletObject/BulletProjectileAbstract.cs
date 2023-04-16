using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Core;

namespace GunTest.BulletProjectile
{
    sealed class BulletProjectileAbstract : AbstractPhysicalObject
    {
        public BulletProjectileAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, BulletProjectileFisobs.AbstrBulletProjectile, null, pos, ID)
        {
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
                realizedObject = new BulletProjectile(this, world);
        }

    }
}