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
    [BepInPlugin("silky.gunworld", "Rain Powder", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {

        private bool init = false;

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            Content.Register(new RevolverStuff.RevolverFisobs());
            Content.Register(new Rifle.RifleFisobs());
            Content.Register(new Shotgun.ShotgunFisobs());
        }

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig.Invoke(self);

            if (init) { return; }

            init = true;

            Futile.atlasManager.LoadAtlas("atlases/Revolver");
            Futile.atlasManager.LoadAtlas("atlases/HuntingRifle");
            Futile.atlasManager.LoadAtlas("atlases/Shotgun");

            On.Player.ThrowObject += Player_ThrowObject;

        }

        private void Player_ThrowObject(On.Player.orig_ThrowObject orig, Player self, int grasp, bool eu)
        {
            //Logger.LogInfo(grasp);

            try
            {

                if (self.grasps[0].grabbed is RevolverStuff.Revolver)
                {

                    RevolverStuff.Revolver gun = (self.grasps[0].grabbed as RevolverStuff.Revolver);

              

                    if (gun.ammo == 0)
                    {
                        if (gun.reloading <= 0)
                        {
                            if (self.grasps[1].grabbed != null && self.grasps[1].grabbed is Spear)
                            {
                                Spear tehrock = self.grasps[1].grabbed as Spear;
                                BodyChunk mainBodyChunk = self.mainBodyChunk;
                                mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
                                self.room.PlaySound(SoundID.Gate_Clamp_Lock, self.mainBodyChunk, false, 0.5f, 3f + UnityEngine.Random.value);
                                AbstractPhysicalObject abstractPhysicalObject = self.grasps[1].grabbed.abstractPhysicalObject;
                                self.ReleaseGrasp(1);
                                abstractPhysicalObject.realizedObject.RemoveFromRoom();
                                abstractPhysicalObject.Room.RemoveEntity(abstractPhysicalObject);
                                StartCoroutine(RevolverReload(gun));
                            }
                            else
                            {
                                gun.room.PlaySound(SoundID.Gate_Bolt, gun.bodyChunks[0], false, 0.5f, 6.5f);
                            }
                        }
                    }

                    IntVector2 intVector = new IntVector2(self.ThrowDirection, 0);
                    Vector2 vector = self.firstChunk.pos + intVector.ToVector2() * 10f + new Vector2(0f, 4f);
                    if (self.room.GetTile(vector).Solid)
                    {
                        vector = self.mainBodyChunk.pos;
                    }

                    gun.Thrown(self, vector, new Vector2?(self.mainBodyChunk.pos - intVector.ToVector2() * 10f), intVector, Mathf.Lerp(1f, 1.5f, self.Adrenaline), eu);

                }
                else if (self.grasps[0].grabbed is Rifle.Rifle)
                {

                    Rifle.Rifle gun = (self.grasps[0].grabbed as Rifle.Rifle);



                    if (gun.ammo == 0)
                    {
                        if (gun.reloading <= 0)
                        {
                            if (self.grasps[1].grabbed != null && self.grasps[1].grabbed is Spear)
                            {
                                Spear tehrock = self.grasps[1].grabbed as Spear;
                                BodyChunk mainBodyChunk = self.mainBodyChunk;
                                mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
                                self.room.PlaySound(SoundID.Gate_Clamp_Lock, self.mainBodyChunk, false, 0.5f, 3f + UnityEngine.Random.value);
                                AbstractPhysicalObject abstractPhysicalObject = self.grasps[1].grabbed.abstractPhysicalObject;
                                self.ReleaseGrasp(1);
                                abstractPhysicalObject.realizedObject.RemoveFromRoom();
                                abstractPhysicalObject.Room.RemoveEntity(abstractPhysicalObject);
                                StartCoroutine(RifleReload(gun));
                            }
                            else
                            {
                                gun.room.PlaySound(SoundID.Gate_Bolt, gun.bodyChunks[0], false, 0.5f, 6.5f);
                            }
                        }
                    }

                    IntVector2 intVector = new IntVector2(self.ThrowDirection, 0);
                    Vector2 vector = self.firstChunk.pos + intVector.ToVector2() * 10f + new Vector2(0f, 4f);
                    if (self.room.GetTile(vector).Solid)
                    {
                        vector = self.mainBodyChunk.pos;
                    }

                    gun.Thrown(self, vector, new Vector2?(self.mainBodyChunk.pos - intVector.ToVector2() * 10f), intVector, Mathf.Lerp(1f, 1.5f, self.Adrenaline), eu);

                }
                else if (self.grasps[0].grabbed is Shotgun.Shotgun)
                {

                    Shotgun.Shotgun gun = (self.grasps[0].grabbed as Shotgun.Shotgun);



                    if (gun.ammo == 0)
                    {
                        if (gun.reloading <= 0)
                        {
                            if (self.grasps[1].grabbed != null && self.grasps[1].grabbed is FirecrackerPlant)
                            {
                                Spear tehrock = self.grasps[1].grabbed as Spear;
                                BodyChunk mainBodyChunk = self.mainBodyChunk;
                                mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
                                self.room.PlaySound(SoundID.Gate_Clamp_Lock, self.mainBodyChunk, false, 0.5f, 3f + UnityEngine.Random.value);
                                AbstractPhysicalObject abstractPhysicalObject = self.grasps[1].grabbed.abstractPhysicalObject;
                                self.ReleaseGrasp(1);
                                abstractPhysicalObject.realizedObject.RemoveFromRoom();
                                abstractPhysicalObject.Room.RemoveEntity(abstractPhysicalObject);
                                StartCoroutine(ShotgunReload(gun));
                            }
                            else
                            {
                                gun.room.PlaySound(SoundID.Gate_Bolt, gun.bodyChunks[0], false, 0.5f, 6.5f);
                            }
                        }
                    }

                    IntVector2 intVector = new IntVector2(self.ThrowDirection, 0);
                    Vector2 vector = self.firstChunk.pos + intVector.ToVector2() * 10f + new Vector2(0f, 4f);
                    if (self.room.GetTile(vector).Solid)
                    {
                        vector = self.mainBodyChunk.pos;
                    }

                    gun.Thrown(self, vector, new Vector2?(self.mainBodyChunk.pos - intVector.ToVector2() * 10f), intVector, Mathf.Lerp(1f, 1.5f, self.Adrenaline), eu);

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

        IEnumerator RevolverReload(RevolverStuff.Revolver gun, float reloadMult = 1f, int AmmoFill = -1)
        {

            if (AmmoFill == -1)
            {
                AmmoFill = gun.Maxammo;
            }

            gun.reloading = 1f;
            gun.aimoverride = true;

            yield return new WaitForSecondsRealtime(gun.reloadTime* reloadMult);

            gun.reloading = 0f;
            gun.aimoverride = false;
            gun.ammo = AmmoFill;

            BodyChunk mainBodyChunk = gun.grabbedBy[0].grabber.mainBodyChunk;
            mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
            gun.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, gun.firstChunk, false, 0.5f, 8f + UnityEngine.Random.value);

        }

        IEnumerator RifleReload(Rifle.Rifle gun, float reloadMult = 1f, int AmmoFill = -1)
        {

            if (AmmoFill == -1)
            {
                AmmoFill = gun.Maxammo;
            }

            gun.reloading = 1f;
            gun.aimoverride = true;

            yield return new WaitForSecondsRealtime(gun.reloadTime * reloadMult);

            gun.reloading = 0f;
            gun.aimoverride = false;
            gun.ammo = AmmoFill;

            BodyChunk mainBodyChunk = gun.grabbedBy[0].grabber.mainBodyChunk;
            mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
            gun.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, gun.firstChunk, false, 0.5f, 8f + UnityEngine.Random.value);

        }

        IEnumerator ShotgunReload(Shotgun.Shotgun gun, float reloadMult = 1f, int AmmoFill = -1)
        {

            if (AmmoFill == -1)
            {
                AmmoFill = gun.Maxammo;
            }

            gun.reloading = 1f;
            gun.aimoverride = true;

            yield return new WaitForSecondsRealtime(gun.reloadTime * reloadMult);

            gun.reloading = 0f;
            gun.aimoverride = false;
            gun.ammo = AmmoFill;

            BodyChunk mainBodyChunk = gun.grabbedBy[0].grabber.mainBodyChunk;
            mainBodyChunk.vel.y = mainBodyChunk.vel.y + 4f;
            gun.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, gun.firstChunk, false, 0.5f, 8f + UnityEngine.Random.value);

        }
    }
}
