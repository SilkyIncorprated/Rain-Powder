using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Core;

namespace GunTest.RevolverStuff
{
    sealed class RevolverAbstract : AbstractPhysicalObject
    {
        public RevolverAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, RevolverFisobs.AbstrRevolver, null, pos, ID)
        {
        }

        public override void Realize()
        {
            base.Realize();
            if (realizedObject == null)
                realizedObject = new Revolver(this, world);
        }

    }
}