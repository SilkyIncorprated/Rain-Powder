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

namespace GunTest.Rifle
{
    sealed class RifleFisobs : Fisob
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType AbstrRifle = new AbstractPhysicalObject.AbstractObjectType("Rifle", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID mRifle = new MultiplayerUnlocks.SandboxUnlockID("Rifle", true);

        public RifleFisobs() : base(AbstrRifle)
        {
            Icon = new SimpleIcon(spriteName: "Spear", spriteColor: Color.white);

            SandboxPerformanceCost = new SandboxPerformanceCost(linear: 0.2f, 0f);

            RegisterUnlock(mRifle, parent: MultiplayerUnlocks.SandboxUnlockID.GreenLizard, data: 0);
        }

        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock unlock)
        {
            // Crate data is just floats separated by ; characters.
            string[] p = saveData.CustomData.Split(';');

            if (p.Length < 5)
            {
                p = new string[5];
            }

            var result = new RifleAbstract(world, saveData.Pos, saveData.ID)
            {
            };

            return result;
        }

    }
}
