using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Core;

namespace GunTest.Mac10
{
    sealed class Mac10Abstract : AbstractPhysicalObject
    {
        public Mac10Abstract(World world, WorldCoordinate pos, EntityID ID) : base(world, Mac10Fisobs.AbstrRevolver, null, pos, ID)
        {
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
                realizedObject = new Mac10(this, world);
        }

    }
}