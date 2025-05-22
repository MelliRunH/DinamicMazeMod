using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DynamicMazeMod
{
    public class MazeGenerator
    {
        private readonly ICoreAPI api;
        private readonly int width;
        private readonly int height;
        private readonly int wallHeight;
        private readonly string wallBlockCode;
        private readonly string floorBlockCode;
        private readonly string trapBlockCode;
        private readonly string rewardBlockCode;
        private readonly double trapChance;
        private readonly double rewardChance;
        private int[,] maze;
        private readonly Random rand = new Random();

        public MazeGenerator(ICoreAPI api, int width, int height, int wallHeight, string wallBlockCode, string floorBlockCode, string trapBlockCode, string rewardBlockCode, double trapChance, double rewardChance)
        {
            this.api = api;
            this.width = (width % 2 == 0) ? width + 1 : width;
            this.height = (height % 2 == 0) ? height + 1 : height;
            this.wallHeight = wallHeight;
            this.wallBlockCode = wallBlockCode;
            this.floorBlockCode = floorBlockCode;
            this.trapBlockCode = trapBlockCode;
            this.rewardBlockCode = rewardBlockCode;
            this.trapChance = trapChance;
            this.rewardChance = rewardChance;
        }

        public void Generate(BlockPos origin)
        {
            GenerateMazeData();

            int wallBlockId = api.World.GetBlock(new AssetLocation(wallBlockCode)).BlockId;
            int floorBlockId = api.World.GetBlock(new AssetLocation(floorBlockCode)).BlockId;
            int trapBlockId = api.World.GetBlock(new AssetLocation(trapBlockCode))?.BlockId ?? 0;
            int rewardBlockId = api.World.GetBlock(new AssetLocation(rewardBlockCode))?.BlockId ?? 0;

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    BlockPos pos = origin.AddCopy(x, 0, z);
                    int block = maze[x, z];

                    for (int y = 0; y < wallHeight; y++)
                    {
                        BlockPos wallPos = pos.Copy(); wallPos.Y += y;

                        if (block == 1)
                        {
                            api.World.BlockAccessor.SetBlock(wallBlockId, wallPos);
                        }
                        else if (y == 0)
                        {
                            api.World.BlockAccessor.SetBlock(floorBlockId, wallPos);

                            if ((x != 1 || z != 0) && (x != width - 2 || z != height - 1))
                            {
                                double roll = rand.NextDouble();
                                if (roll < trapChance)
                                {
                                    api.World.BlockAccessor.SetBlock(trapBlockId, wallPos.UpCopy());
                                }
                                else if (roll < trapChance + rewardChance)
                                {
                                    api.World.BlockAccessor.SetBlock(rewardBlockId, wallPos.UpCopy());
                                }
                            }
                        }
                        else
                        {
                            api.World.BlockAccessor.SetBlock(0, wallPos); // air
                        }
                    }
                }
            }
        }

        private void GenerateMazeData()
        {
            maze = new int[width, height];

            for (int x = 0; x < width; x++)
                for (int z = 0; z < height; z++)
                    maze[x, z] = 1;

            RecursiveBacktrack(1, 1);

            maze[1, 0] = 0; // entrada norte
            maze[width - 2, height - 1] = 0; // salida sur
        }

        // Cambiado para que los pasillos tengan 2 bloques de ancho:
        private void RecursiveBacktrack(int x, int z)
        {
            maze[x, z] = 0;

            int[][] directions = new int[][] {
                new[] { 0, -2 }, new[] { 2, 0 },
                new[] { 0, 2 }, new[] { -2, 0 }
            };

            Shuffle(directions);

            foreach (var dir in directions)
            {
                int nx = x + dir[0];
                int nz = z + dir[1];

                if (nx > 0 && nx < width - 1 && nz > 0 && nz < height - 1 && maze[nx, nz] == 1)
                {
                    // Dejamos libre el espacio entre x,z y nx,nz
                    maze[x + dir[0] / 2, z + dir[1] / 2] = 0;

                    // --- CAMBIO PRINCIPAL ---
                    // Añadimos también un bloque contiguo perpendicular para ampliar el pasillo a 2 bloques de ancho
                    if (dir[0] != 0) // movimiento horizontal
                    {
                        // liberamos una fila de 2 bloques de alto en z y z+1 (o z-1 si estamos en borde)
                        if (z + 1 < height - 1)
                            maze[x + dir[0] / 2, z + 1] = 0;
                        else if (z - 1 > 0)
                            maze[x + dir[0] / 2, z - 1] = 0;
                    }
                    else // movimiento vertical
                    {
                        // liberamos dos columnas de bloques en x y x+1 (o x-1 si estamos en borde)
                        if (x + 1 < width - 1)
                            maze[x + 1, z + dir[1] / 2] = 0;
                        else if (x - 1 > 0)
                            maze[x - 1, z + dir[1] / 2] = 0;
                    }

                    RecursiveBacktrack(nx, nz);
                }
            }
        }

        private void Shuffle(int[][] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1); 
                var tmp = array[i]; 
                array[i] = array[j];
                array[j] = tmp;
            }
        }
    }
}
