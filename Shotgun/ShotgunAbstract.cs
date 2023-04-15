using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Core;

namespace GunTest.Shotgun
{
    sealed class ShotgunAbstract : AbstractPhysicalObject
    {
        public ShotgunAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, ShotgunFisobs.AbstrRifle, null, pos, ID)
        {
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
                realizedObject = new Shotgun(this, world);
        }

    }
}