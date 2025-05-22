using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common.Entities;
using System;

[assembly: ModInfo("Dynamic Maze", "dynamicmazemod", Version = "1.0.0", Authors = new[] { "MelliRunH" })]

namespace DynamicMazeMod
{
    public class DynamicMazeModSystem : ModSystem
    {
        ICoreServerAPI api;

        public static MazeConfig Config;

        public override void StartServerSide(ICoreServerAPI api)
        {
            this.api = api;

            LoadConfig();

            if (Config.allowCommandActivation)
            {
                api.ChatCommands.Create("laberinto")
                    .WithDescription("Inicia el laberinto dinámico en tu posición.")
                    .RequiresPrivilege("gamemode")
                    .HandleWith(OnLaberintoCommand);
            }
            else
            {
                api.Server.Logger.Notification("[dynamicmazemod] El comando '/laberinto' está deshabilitado por configuración.");
            }
        }

        private void LoadConfig()
        {
            string filename = "config.json";
            Config = api.LoadModConfig<MazeConfig>(filename);
            if (Config == null)
            {
                api.Server.Logger.Warning("[dynamicmazemod] No se encontró config.json, se crea uno por defecto.");
                Config = new MazeConfig();
                api.StoreModConfig(Config, filename);
            }
            else
            {
                api.Server.Logger.Notification("[dynamicmazemod] Configuración cargada correctamente.");
            }
        }

        private TextCommandResult OnLaberintoCommand(TextCommandCallingArgs args)
        {
            if (!Config.allowCommandActivation)
            {
                return TextCommandResult.Error("Este comando está desactivado por configuración.");
            }

            IPlayer player = args.Caller.Player;
            BlockPos playerPos = player.Entity.Pos.AsBlockPos;

            BlockFacing facing = GetFacingFromYaw(player.Entity.Pos.Yaw);

            BlockPos pedestalPos = playerPos.AddCopy((int)facing.Normalf.X, 0, (int)facing.Normalf.Z);

            Block pedestalBlock = api.World.GetBlock(new AssetLocation(Config.pedestalBlock));
            if (pedestalBlock == null || pedestalBlock.BlockId <= 0)
            {
                (player as IServerPlayer)?.SendMessage(GlobalConstants.GeneralChatGroup,
                    $"Error: Bloque '{Config.pedestalBlock}' no encontrado o inválido.",
                    EnumChatType.CommandError);
                return TextCommandResult.Error("Bloque pedestal inválido.");
            }

            api.World.BlockAccessor.BreakBlock(pedestalPos, null);
            api.World.BlockAccessor.SetBlock(pedestalBlock.BlockId, pedestalPos);
            api.World.BlockAccessor.MarkBlockDirty(pedestalPos);

            BlockPos mazeOrigin = pedestalPos.AddCopy((int)(facing.Normalf.X * 2), 0, (int)(facing.Normalf.Z * 2));

            int mazeSize = Config.defaultSize switch
            {
                "pequeño" => 25,
                "mediano" => 50,
                "grande" => 150,
                _ => 150
            };

            string[] wallBlocks = Config.wallBlockTypes ?? new[] { "game:claybricks-uneven-four-soldier-fire" };
            string wallBlock = wallBlocks[0];

            string floorBlock = Config.floorBlock ?? "game:mudbrick-light";
            string trapBlock = Config.trapBlock ?? "game:trap-spike";
            string rewardBlock = Config.rewardBlock ?? "game:chest-east";

            MazeGenerator generator = new MazeGenerator(
                api,
                mazeSize,
                mazeSize,
                Config.mazeHeight,
                wallBlock,
                floorBlock,
                trapBlock,
                rewardBlock,
                Config.trapChance,
                Config.rewardChance
            );

            generator.Generate(mazeOrigin);

            (player as IServerPlayer)?.SendMessage(GlobalConstants.GeneralChatGroup,
                Config.startMessage + " Apareció un frente a ti.",
                EnumChatType.CommandSuccess);

            return TextCommandResult.Success("Laberinto iniciado.");
        }

        private static BlockFacing GetFacingFromYaw(float yaw)
        {
            yaw = yaw % 360;
            if (yaw < 0) yaw += 360;

            if (yaw >= 315 || yaw < 45)
                return BlockFacing.NORTH;
            if (yaw >= 45 && yaw < 135)
                return BlockFacing.EAST;
            if (yaw >= 135 && yaw < 225)
                return BlockFacing.SOUTH;
            return BlockFacing.WEST;
        }
    }

    public class MazeConfig
    {
        public string defaultSize { get; set; } = "grande";
        public string pedestalBlock { get; set; } = "game:mudbrick-light";
        public int mazeHeight { get; set; } = 11;
        public double trapChance { get; set; } = 0.25;
        public double rewardChance { get; set; } = 0.15;
        public int mazeDurationSeconds { get; set; } = 1600;
        public bool allowCommandActivation { get; set; } = true;
        public bool announceCoordinates { get; set; } = true;
        public int maxPlayers { get; set; } = 20;
        public string mazeDifficulty { get; set; } = "difícil";
        public bool enableRanking { get; set; } = true;
        public int eventIntervalSeconds { get; set; } = 60;
        public string[] wallBlockTypes { get; set; } = new string[] { "game:claybricks-uneven-four-soldier-fire", "game:diamond-stone-clean" };
        public string floorBlock { get; set; } = "game:mudbrick-light";
        public string rewardBlock { get; set; } = "game:chest-east";
        public string trapBlock { get; set; } = "game:trap-spike";
        public string startMessage { get; set; } = "¡El laberinto dinámico ha comenzado!";
        public string successMessage { get; set; } = "¡Felicidades, has completado el laberinto!";
        public string failMessage { get; set; } = "Has fallado en el laberinto.";
        public string[] possibleRewards { get; set; } = new[] { "game:metalbit-steel", "game:charcoal" };
        public bool autoRegenerate { get; set; } = true;
        public int regenerateIntervalMinutes { get; set; } = 30;
        public bool allowBlockBreaking { get; set; } = false;
        public bool allowBlockPlacement { get; set; } = false;
        public bool enableDebugLogging { get; set; } = true;
        public int penaltyFreezeSeconds { get; set; } = 15;
    }
}
