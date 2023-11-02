using System.Collections.Generic;
using System.Linq;
using ProjBobcat.Class.Helper;

namespace CMFL.MVVM.Class.Helper.Game
{
    public static class GameIconHelper
    {
        private static readonly List<string> Icons = new List<string>
        {
            "Furnace_On", "Log_Jungle", "Sandstone", "Cake", "Planks_Birch", "Bedrock", "Planks_DarkOak", "Clay",
            "Stone_Diorite", "Hardened_Clay", "Emerald_Block", "Planks_Oak", "Obsidian", "Creeper_Head", "Log_Birch",
            "Dirt", "Farmland", "Redstone_Ore", "Leaves_Oak", "Planks_Spruce", "Lapis_Ore", "Dirt_Snow", "Lectern_Book",
            "Log_Acacia", "Coal_Ore", "Coal_Block", "Stone_Andesite", "Gold_Block", "Gravel", "Quartz_Ore", "Chest",
            "Snow", "Cobblestone", "Iron_Ore", "Furnace", "Glazed_Terracotta_White", "Leaves_Jungle", "Red_Sandstone",
            "Gold_Ore", "Ice_Packed", "Carved_Pumpkin", "Iron_Block", "Grass", "Stone", "TNT",
            "Glazed_Terracotta_Light_Blue", "End_Stone", "Emerald_Ore", "Wool", "Dirt_Podzol", "Water", "Log_Oak",
            "Diamond_Block", "Leaves_Birch", "Log_DarkOak", "Redstone_Block", "Planks_Acacia", "Log_Spruce",
            "Soul_Sand", "Enchanting_Table", "Red_Sand", "Glowstone", "Leaves_Spruce", "Crafting_Table", "Netherrack",
            "Glass", "Sand", "Glazed_Terracotta_Orange", "Stone_Granite", "Mycelium", "Skeleton_Skull", "Nether_Brick",
            "Planks_Jungle", "Bookshelf", "Brick", "Diamond_Ore"
        };

        public static string GetIconByIndex(int index)
        {
            return index > Icons.Count - 1 ? RandomIcon(true) : Icons[index];
        }

        public static int GetIconIndex(string icon)
        {
            return Icons.IndexOf(icon);
        }

        public static Dictionary<string, string> GetAllIcons()
        {
            return Icons.ToDictionary(i => i, i => $"/Assets/Images/Icons/{i}.png");
        }

        public static string GetIcon(string name)
        {
            return Icons.Contains(name) ? $"/Assets/Images/Icons/{name}.png" : RandomIcon(false);
        }

        public static string RandomIcon(bool returnName)
        {
            var icon = Icons.RandomSample();
            return returnName ? icon : $"/Assets/Images/Icons/{icon}.png";
        }
    }
}