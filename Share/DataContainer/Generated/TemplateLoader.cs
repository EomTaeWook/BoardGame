using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DataContainer;

namespace DataContainer.Generated
{
    public partial class TemplateLoader
    {
        public static void Load(string path)
        {
            TemplateContainer<ConstantTemplate>.Load(path, "Constant.json");
            TemplateContainer<StringTemplate>.Load(path, "String.json");
            TemplateContainer<WallGoBoardTile7x7Template>.Load(path, "WallGoBoardTile7x7.json");
            TemplateContainer<WallGoBoardTileWall7x7Template>.Load(path, "WallGoBoardTileWall7x7.json");
        }
        public static void Load(Func<string, string> funcLoadJson)
        {
            TemplateContainer<ConstantTemplate>.Load("Constant.json", funcLoadJson);
            TemplateContainer<StringTemplate>.Load("String.json", funcLoadJson);
            TemplateContainer<WallGoBoardTile7x7Template>.Load("WallGoBoardTile7x7.json", funcLoadJson);
            TemplateContainer<WallGoBoardTileWall7x7Template>.Load("WallGoBoardTileWall7x7.json", funcLoadJson);
        }
        public static void MakeRefTemplate()
        {
            TemplateContainer<ConstantTemplate>.MakeRefTemplate();
            TemplateContainer<StringTemplate>.MakeRefTemplate();
            TemplateContainer<WallGoBoardTile7x7Template>.MakeRefTemplate();
            TemplateContainer<WallGoBoardTileWall7x7Template>.MakeRefTemplate();
            
            TemplateContainer<ConstantTemplate>.Combine();
            TemplateContainer<StringTemplate>.Combine();
            TemplateContainer<WallGoBoardTile7x7Template>.Combine();
            TemplateContainer<WallGoBoardTileWall7x7Template>.Combine();
        }
    }
}
