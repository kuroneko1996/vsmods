using Vintagestory.API.Common;
using Vintagestory.API.Server;
using HarmonyLib;

namespace Morpheus
{
    public class ModConfigFile
    {
        public static ModConfigFile Current { get; set; }

        public float percent { get; set; } = 50;
    }

    public class MorpheusMod : ModSystem
    {
        Harmony harmony = new Harmony("morpheus");
        const string configFileName = "morpheus.json";

        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);

            try
            {
                var config = api.LoadModConfig<ModConfigFile>(configFileName);
                if (config == null)
                {
                    ModConfigFile.Current = new ModConfigFile();
                    api.StoreModConfig(ModConfigFile.Current, configFileName);
                } else
                {
                    ModConfigFile.Current = config;
                }
                Patch.percent = ModConfigFile.Current.percent;
            }
            catch (System.Exception e)
            {
                api.World.Logger.Log(EnumLogType.Error, "Error loading/saving morpheus.json. Check the file");
            }
            Patch.sapi = api;
            // Harmony.DEBUG = true;
            harmony.PatchAll();

            api.World.Logger.Log(EnumLogType.Debug, "Morpheus initialized, using at least " + Patch.percent + "% votes");
        }

        public override void Dispose()
        {
            base.Dispose();
            harmony.UnpatchAll("morpheus");
        }
    }
}
