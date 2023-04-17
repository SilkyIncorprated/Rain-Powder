using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using Fisobs;
using Fisobs.Core;
using RWCustom;
using UnityEngine;
using MonoMod;
using System.Collections;
namespace GunTest
{
    [BepInPlugin("silky.gunworld", "Rain Powder", "1.2.1")]
    public class Plugin : BaseUnityPlugin
    {

        private bool init = false;

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            Content.Register(new RevolverStuff.RevolverFisobs());
            Content.Register(new Rifle.RifleFisobs());
            Content.Register(new Shotgun.ShotgunFisobs());
            Content.Register(new Mac10.Mac10Fisobs());
            Content.Register(new BulletProjectile.BulletProjectileFisobs());
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig.Invoke(self);

            if (init) { return; }

            init = true;

            Futile.atlasManager.LoadAtlas("atlases/Revolver");
            Futile.atlasManager.LoadAtlas("atlases/HuntingRifle");
            Futile.atlasManager.LoadAtlas("atlases/Shotgun");
            Futile.atlasManager.LoadAtlas("atlases/Mac10");
            Futile.atlasManager.LoadAtlas("atlases/AmmoBox");

            MachineConnector.SetRegisteredOI("silky.gunworld", new GunOptions());

            On.Player.ThrowObject += Player_ThrowObject;

        }

        private void Player_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
        {
            //Logger.LogInfo(grasp);

            try
            {

                if (self.grasps[grasp].grabbed is GunClass)
                {

                    GunClass gun = (self.grasps[grasp].grabbed as GunClass);

                    int othergrasp = 0;

                    if (grasp == 0)
                    {
                        othergrasp = 1;
                    }
                    else
                    {
                        othergrasp = 0;
                    }

                    IntVector2 intVector = new IntVector2(self.ThrowDirection, 0);
                    Vector2 vector = self.firstChunk.pos + intVector.ToVector2() * 10f + new Vector2(0f, 4f);
                    if (self.room.GetTile(vector).Solid)
                    {
                        vector = self.mainBodyChunk.pos;
                    }

                    if (gun.ammo <= 0)
                    {
                        if (gun.reloading <= 0)
                        {
                            if (self.grasps[othergrasp].grabbed != null && gun.IsObjectVaild(self.grasps[othergrasp].grabbed))
                            {
                                Weapon tehrock = self.grasps[othergrasp].grabbed as Weapon;
                                BodyChunk mainBodyChunk = self.mainBodyChunk;
                                mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
                                self.room.PlaySound(SoundID.Gate_Clamp_Lock, self.mainBodyChunk, false, 0.5f, 3f + UnityEngine.Random.value);
                                AbstractPhysicalObject abstractPhysicalObject = self.grasps[othergrasp].grabbed.abstractPhysicalObject;
                                self.ReleaseGrasp(1);
                                abstractPhysicalObject.realizedObject.RemoveFromRoom();
                                abstractPhysicalObject.Room.RemoveEntity(abstractPhysicalObject);
                                StartCoroutine(GunReload(gun));
                            }
                            else if (self.grasps[othergrasp].grabbed == null | !gun.IsObjectVaild(self.grasps[othergrasp].grabbed))
                            {
                                gun.Thrown(self, vector, new Vector2?(self.mainBodyChunk.pos - intVector.ToVector2() * 10f), intVector, Mathf.Lerp(1f, 1.5f, self.Adrenaline), eu);
                            }
                        }
                    }
                    else
                    {
                        gun.Thrown(self, vector, new Vector2?(self.mainBodyChunk.pos - intVector.ToVector2() * 10f), intVector, Mathf.Lerp(1f, 1.5f, self.Adrenaline), eu);
                    }

                }
                else
                {
                    orig.Invoke(self, grasp, eu);
                }
            }
            catch(Exception ex)
            {
                Logger.LogWarning(ex);
            }
        }

        IEnumerator GunReload(GunClass gun, float reloadMult = 1f, int AmmoFill = -1)
        {

            if (AmmoFill == -1)
            {
                AmmoFill = gun.Maxammo;
            }

            gun.reloading = 1f;

            yield return new WaitForSecondsRealtime(gun.reloadTime* reloadMult);

            gun.reloading = 0f;
            gun.ammo = AmmoFill;

            BodyChunk mainBodyChunk = gun.grabbedBy[0].grabber.mainBodyChunk;
            mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
            gun.room.PlaySound(gun.ReloadClickID, gun.firstChunk);

        }

    }
}
