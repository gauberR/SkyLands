﻿using System;
using API.Generator;
using API.Geo.Cuboid;
using API.Generic;

namespace Game.World.Generator.Decorators
{
    class TreeDecorator : Decorator
    {
        public override void populate(Island curr, Random random) {
            int x = random.Next(2, (int)curr.getSize().x) * Cst.CHUNK_SIDE;
            int z = random.Next(2, (int)curr.getSize().z) * Cst.CHUNK_SIDE;
            int y = curr.getSurfaceHeight(x, z);

            while (y == -1)
            {
                x = random.Next(2, (int)curr.getSize().x) * Cst.CHUNK_SIDE;
                z = random.Next(2, (int)curr.getSize().z) * Cst.CHUNK_SIDE;
                y = curr.getSurfaceHeight(x, z);
            }

            int size = ((int)(curr.getSize().x + curr.getSize().z) / 2) / 5;

            for(int i = 0; i < size * Cst.CHUNK_SIDE; i += 5) {
                for (int j = 0, max = random.Next(4, 10); j < max; j++) {
                    double angle = 2.0 * Math.PI * random.NextDouble();

                    int treeX = (int)(x + i * Math.Cos(angle)) + (random.Next(-10, 11) / 5);
                    int treeZ = (int)(y + i * Math.Sin(angle)) + (random.Next(-10, 11) / 5);
                    int treeY = curr.getSurfaceHeight(treeX, treeZ, "Grass");

                    if(treeY != -1) { this.createTree(curr, random, treeX, treeY, treeZ); }
                }
            }
        }

        public void createTree(Island curr, Random random, int x, int y, int z) {
            for(int i = 0, height = random.Next(6, 12); i < height; i++) {
                if(i != height - 1) { curr.setBlockAt(x, y + i, z, "Wood", true); }
                else { curr.setBlockAt(x, y + i, z, "Leaves", true); }
                if(i >= height - 5 && i < height - 3) {
                    for (int j = -2; j < 3; j++) {
                        for (int k = -2; k < 3; k++) {
                            if (j == 0 && k == 0) { continue; }
                            if ((j == -2 && (k == -2 || k == 2)) || (j == 2 && (k == -2 || k == 2))) {
                                if(random.Next(0, 100) > 15) { curr.setBlockAt(x + j, y + i, z + k, "Leaves", true); }
                            } else {
                                curr.setBlockAt(x + j, y + i, z + k, "Leaves", true);
                            }
                        }
                    }
                } else if(i >= height - 3) {
                    for(int j = -1; j < 2; j++) {
                        for(int k = -1; k < 2; k++) {
                            if(j == 0 && k == 0) { continue; }
                            if(i == height - 3 && (j == -1 && (k == -1 || k == 1)) || (j == 1 && (k == -1 || k == 1))) {
                                if(random.Next(0, 100) > 75) { curr.setBlockAt(x + j, y + i, z + k, "Leaves", true); }
                            } else {
                                curr.setBlockAt(x + j, y + i, z + k, "Leaves", true);
                            }
                        }
                    }
                }

            }

        }

    }
}
