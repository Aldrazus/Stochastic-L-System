using System;
using System.Collections.Generic;
using System.IO;

class StringToStructure
{
    private static string Xmlify(string blockType, string material, double x, double y, double rot)
    {
        return String.Format(
            "<Block type=\"{0}\" material=\"{1}\" x=\"{2}\" y=\"{3}\" rotation=\"{4}\" />\n",
            blockType,
            material,
            x.ToString(),
            y.ToString(),
            rot.ToString());
    }

    public static void StartFile(string path)
    {
        string text = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" +
            "<Level width =\"2\">\n" +
            "<Camera x=\"0\" y=\"2\" minWidth=\"20\" maxWidth=\"30\">\n" +
            "<Birds>\n" +
            "<Bird type=\"BirdRed\"/>\n" +
            "<Bird type=\"BirdRed\"/>\n" +
            "<Bird type=\"BirdRed\"/>\n" +
            "</Birds>\n" +
            "<Slingshot x=\"-8\" y=\"-2.5\">\n" +
            "<GameObjects>\n";
        File.WriteAllText(path, text);
    }

    public static void EndFile(string path)
    {
        string text = "</GameObjects>\n" +
            "</Level>\n";
        File.AppendAllText(path, text);
    }

    public static void WriteBlocksToFile(LSystem l, string path)
    {
        string xmlBlocks = "";
        for (int rowIndex = 0; rowIndex < l.iterations.Count; rowIndex++)
        {
            for (int colIndex = 0; colIndex < l.iterations[rowIndex].Length; colIndex++)
            {
                string symbol = l.iterations[rowIndex][colIndex].ToString();
                string blockType = l.block_names[symbol];
                string material = "wood";
                double x = l.blockCoordinates[rowIndex][colIndex][0];
                double y = l.blockCoordinates[rowIndex][colIndex][1];
                //  TODO: include checks for rotation
                double rotation = 0;
                xmlBlocks += Xmlify(blockType, material, x, y, rotation);
            }
        }
        File.AppendAllText(path, xmlBlocks);

#if (false)
        for (double i = -3; i < 3.10; i += 0.8) {
            string xmlPig = String.Format(
            "<Pig type=\"BasicSmall\" material=\"\" x=\"{0}\" y=\"-3.2\" rotation=\"0\" />\n",
            i);
            File.AppendAllText(path, xmlPig);
        }
#endif

    }

#if (false)
    static void Main(string[] args)
    {
        //File path for level xml
        string path = "../../levels/level-04.xml";


        Dictionary<string, string> materialKey = new Dictionary<string, string>
        {
            {"W", "wood"},
            {"S", "stone"},
            {"I", "ice"},
            {"A", "air"}
        };

        Dictionary<string, Tuple<List<string>, List<double>>> rules = new Dictionary<string, Tuple<List<string>, List<double>>>
                {
                    {"1", new Tuple<List<string>, List<double>>(new List<string> {"1", "212"}, new List<double> {0.80, 0.20})},
                    {"4", new Tuple<List<string>, List<double>>(new List<string> {"4", "141"}, new List<double> {0.60, 0.40})},
                    {"2", new Tuple<List<string>, List<double>>(new List<string> {"2", "26262"}, new List<double> {0.20, 0.80})}
                };

        LSystem lsystem = new LSystem("4", rules);

        LSystem r = new LSystem(3, 5);

        //Iterate through L-system
        lsystem.Iterate(3);

        //Start writing in level file
        StartFile(path);
        WriteBlocksToFile(lsystem, path);
        EndFile(path);

        foreach (string axiom in lsystem.iterations)
        {
            Console.WriteLine(axiom);
        }


        Console.ReadKey();
    }
#endif
}