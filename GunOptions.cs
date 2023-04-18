using System;
using System.Collections.Generic;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace GunTest
{
    class GunOptions : OptionInterface
    {

		public GunOptions()
		{
			GunOptions.UsesMouseAim = this.config.Bind<bool>("usemouseaimoption", false, info: null);
			GunOptions.SufferRecoilPunish = this.config.Bind<bool>("gunrecoilpunishtoggle", true, info: null);
			GunOptions.AmmoCounterSytle = this.config.Bind<string>("gunammocounterstyle", "Circles", info: null);

			// Funnys

			GunOptions.RecoilMultiplyer = this.config.Bind<float>("gunrecoilmultipler", 1f, info: null);
			GunOptions.RealGunMode = this.config.Bind<bool>("realgunmodegun", false, info: null);
			GunOptions.ToyGunMode = this.config.Bind<bool>("toygunmodegun", false, info: null);
			GunOptions.DemomanMode = this.config.Bind<bool>("demomangunmode", false, info: null);
		}

		public static string BPTranslate(string t) // stole this from Rtound world code :troll:
		{
			return OptionInterface.Translate(t);
		}

        public override void Update()
        {
            base.Update();



        }

        public override void Initialize()
        {
            base.Initialize();

            this.Tabs = new OpTab[]
            {
                new OpTab(this, "Settings"),
				new OpTab(this, "Funnines")
			};

			OpCheckBox opMouseAim = new OpCheckBox(GunOptions.UsesMouseAim, new Vector2(15f, 530f));
			OpComboBox opStyleAmmo = new OpComboBox(GunOptions.AmmoCounterSytle, new Vector2(15f, 450f), 60f, new List<ListItem>
			{
			
				new ListItem("Circles"),
				new ListItem("Boxes"),
				new ListItem("None")

			});
			OpCheckBox opGunPunishToggle = new OpCheckBox(GunOptions.SufferRecoilPunish, new Vector2(15f, 490f));
			this.Tabs[0].AddItems(new UIelement[]
			{
				opMouseAim,
				new OpLabel(45f, 530f, GunOptions.BPTranslate("Use Mouse Aim"), false)
				{
					bumpBehav = opMouseAim.bumpBehav,
					description = GunOptions.BPTranslate("If the guns uses the mosue to aim.")
				},
				opGunPunishToggle,
				new OpLabel(45f, 490f, GunOptions.BPTranslate("Suffer Recoil"), false)
				{
					bumpBehav = opGunPunishToggle.bumpBehav,
					description = GunOptions.BPTranslate("Toggle being stunned by firing too much.")
				},
				opStyleAmmo,
				new OpLabel(45f + 60f, 450f, GunOptions.BPTranslate("Ammo counter style"), false)
				{
					bumpBehav = opStyleAmmo.bumpBehav,
					description = GunOptions.BPTranslate("How the guns ammo is shown.")
				}
			});

			// funnys

			OpCheckBox opRealGunMode = new OpCheckBox(GunOptions.RealGunMode, new Vector2(15f, 530f));
			OpCheckBox opToyGunMode = new OpCheckBox(GunOptions.ToyGunMode, new Vector2(15f, 450f));
			OpFloatSlider opRecoilMult = new OpFloatSlider(GunOptions.RecoilMultiplyer, new Vector2(15f, 490f), 200, 1, false)
			{
				min = -10f,
				max = 10f,
			};
			OpCheckBox opDemomanMode = new OpCheckBox(GunOptions.DemomanMode, new Vector2(15f, 410f));
			this.Tabs[1].AddItems(new UIelement[]
			{
				opRecoilMult,
				new OpLabel(45f + 200f, 490f, GunOptions.BPTranslate("Recoil Strength"), false)
				{
					bumpBehav = opRecoilMult.bumpBehav,
					description = GunOptions.BPTranslate("The guns recoil Strength")
				},
				opRealGunMode,
				new OpLabel(45f, 530f, GunOptions.BPTranslate("Real gun mode"), false)
				{
					bumpBehav = opMouseAim.bumpBehav,
					description = GunOptions.BPTranslate("Toggle if the guns are true amercain.")
				},
				opToyGunMode,
				new OpLabel(45f, 450f, GunOptions.BPTranslate("Toy gun mode"), false)
				{
					bumpBehav = opMouseAim.bumpBehav,
					description = GunOptions.BPTranslate("Toggle if the guns are from walmart.")
				},
				opDemomanMode,
				new OpLabel(45f, 410f, GunOptions.BPTranslate("Demoman Mode"), false)
				{
					bumpBehav = opDemomanMode.bumpBehav,
					description = GunOptions.BPTranslate("What makes me a good demoman? If I were a bad demoman, I wouldn't be sittin' here discussin' it with you, now would I?!")
				}
			});

		}

        public static Configurable<bool> UsesMouseAim;
		public static Configurable<string> AmmoCounterSytle;
		public static Configurable<bool> RealGunMode;
		public static Configurable<float> RecoilMultiplyer;
		public static Configurable<bool> SufferRecoilPunish;
		public static Configurable<bool> ToyGunMode;
		public static Configurable<bool> DemomanMode;

	}   
}
