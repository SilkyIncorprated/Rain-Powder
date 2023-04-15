using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Core;

namespace GunTest.Rifle
{
    sealed class RifleAbstract : AbstractPhysicalObject
    {
        public RifleAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, RifleFisobs.AbstrRifle, null, pos, ID)
        {
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
                realizedObject = new Rifle(this, world);
        }

    }
}