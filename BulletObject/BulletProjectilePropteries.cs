﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fisobs.Properties;

namespace GunTest.BulletProjectile
{
    sealed class BulletProjectilePropteries : ItemProperties
    {

        public override void Throwable(Player player, ref bool throwable)
        => throwable = false;
        public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 0;
        public override void ScavWeaponPickupScore(Scavenger scav, ref int score)
        => score = 0;


        private static readonly BulletProjectilePropteries properties = new BulletProjectilePropteries();

        public ItemProperties Properties(Weapon forObject)
        {
            // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
            // The Mosquitoes example from the Fisobs github demonstrates this.
            return properties;
        }

    }
}