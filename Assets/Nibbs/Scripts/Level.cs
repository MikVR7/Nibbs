using System.Collections.Generic;

namespace Nibbs
{
    internal enum NibbColor
    {
        Red = 0,
        Yellow = 1,
        Blue = 2,
        Green = 3,
    }

    internal class LevelData
    {
        internal int LevelNr { get; set; } = 0;
        internal List<NibbColor> NibbsColors { get; set; } = new List<NibbColor>();
        internal int ColumnCount { get; set; } = 0;
        internal int DefaultColumnHeight { get; set; } = 0;
        internal float LevelRadius { get; set; } = 0f;
        internal float NibbDefaultScaling { get; set; } = 0f;
        internal float HeightStartOffset { get; set; } = 0f;
    }

    internal class Level
    {
        internal LevelData VarOut_GetLevel(int level)
        {
            LevelData levelData = new LevelData();
            levelData.LevelNr = level;
            switch (level)
            {
                case 1:
                    levelData.NibbsColors = new List<NibbColor>() { NibbColor.Red, NibbColor.Yellow };
                    levelData.ColumnCount = 20;
                    levelData.DefaultColumnHeight = 5;
                    levelData.LevelRadius = 1.6f;
                    levelData.NibbDefaultScaling = 0.5f;
                    levelData.HeightStartOffset = 10f;
                    break;
                case 2:
                    levelData.NibbsColors = new List<NibbColor>() { NibbColor.Blue, NibbColor.Yellow };
                    levelData.ColumnCount = 20;
                    levelData.DefaultColumnHeight = 7;
                    levelData.LevelRadius = 1.6f;
                    levelData.NibbDefaultScaling = 0.5f;
                    levelData.HeightStartOffset = 10f;
                    break;
                case 3:
                    levelData.NibbsColors = new List<NibbColor>() { NibbColor.Red, NibbColor.Yellow, NibbColor.Green};
                    levelData.ColumnCount = 20;
                    levelData.DefaultColumnHeight = 5;
                    levelData.LevelRadius = 1.6f;
                    levelData.NibbDefaultScaling = 0.5f;
                    levelData.HeightStartOffset = 10f;
                    break;
            }
            return levelData;
        }
    }
}