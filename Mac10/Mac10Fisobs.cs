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

namespace GunTest.Mac10
{
    sealed class Mac10Fisobs : Fisob
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType AbstrMac10 = new AbstractPhysicalObject.AbstractObjectType("Mac10", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID mMac10 = new MultiplayerUnlocks.SandboxUnlockID("Mac10", true);

        public Mac10Fisobs() : base(AbstrMac10)
        {
            Icon = new SimpleIcon(spriteName: "Symbol_HellSpear", spriteColor: Color.white);

            SandboxPerformanceCost = new SandboxPerformanceCost(linear: 0.2f, 0f);

            RegisterUnlock(mMac10, parent: MultiplayerUnlocks.SandboxUnlockID.GreenLizard, data: 0);
        }

        public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock unlock)
        {
            // Crate data is just floats separated by ; characters.
            string[] p = saveData.CustomData.Split(';');

            if (p.Length < 5)
            {
                p = new string[5];
            }

            var result = new Mac10Abstract(world, saveData.Pos, saveData.ID)
            {
            };

            return result;
        }

    }
}
