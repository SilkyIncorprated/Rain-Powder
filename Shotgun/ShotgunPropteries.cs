using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Properties;

namespace GunTest.Shotgun
{
    sealed class ShotgunPropteries : ItemProperties
    {

        public override void Throwable(Player player, ref bool throwable)
        => throwable = false;
        public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 7;
        public override void ScavWeaponPickupScore(Scavenger scav, ref int score)
        => score = 6;


        private static readonly ShotgunPropteries properties = new ShotgunPropteries();

        public ItemProperties Properties(Weapon forObject)
        {
            // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
            // The Mosquitoes example from the Fisobs github demonstrates this.
            return properties;
        }

    }
}
