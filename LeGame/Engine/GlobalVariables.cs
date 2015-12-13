﻿namespace LeGame.Engine
{
    using System;

    public static class GlobalVariables
    {
        public static readonly Random Rng = new Random();

        public const string ContentDir = @"..\..\..\Content\";

        public const int TileWidth = 32;

        public const int TileHeight = 32;

        // private static readonly GameTime globalTime;

        public static int WindowWidth { get; } = 800;

        public static int WindowHeight { get; } = 480;

        public const float RightAngle = 1.55f;

        public const float LeftAngle = -1.55f;

        public const float UpAngle = 0f;

        public const float DownAngle = 3.15f;
    }
}
