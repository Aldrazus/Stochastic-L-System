using System;
using System.Collections.Generic;
using System.IO;

class StringToStructure
{
    private static string Xmlify(string blockType, string material, double x, double y, double rot)
    {
        //  Check for pigs
        if (blockType == "BasicSmall") {
            return String.Format(
                "<Pig type=\"{0}\" material=\"{1}\" x=\"{2}\" y=\"{3}\" rotation=\"{4}\" />\n",
                blockType,
                material,
                x.ToString(),
                y.ToString(),
                rot.ToString());
        }

        //  Check for TNT
        if (blockType == "TNT") {
            return String.Format(
                "<TNT type=\"{0}\" material=\"{1}\" x=\"{2}\" y=\"{3}\" rotation=\"{4}\" />\n",
                "",
                material,
                x.ToString(),
                y.ToString(),
                rot.ToString());
        }

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
                //  Check for pigs and TNT
                if (symbol == "%" || symbol == "&") {
                    string blockType = LSystem.block_names[symbol];
                    string material = "";
                    double x = l.blockCoordinates[rowIndex][colIndex][0];
                    double y = l.blockCoordinates[rowIndex][colIndex][1];
                    double rotation = 0;

                    xmlBlocks += Xmlify(blockType, material, x, y, rotation);
                }
                else {
                    string[] blockAndMaterial = LSystem.block_names[symbol].Split(' ');
                    string blockType = blockAndMaterial[0];
                    string material = blockAndMaterial[1];
                    double x = l.blockCoordinates[rowIndex][colIndex][0];
                    double y = l.blockCoordinates[rowIndex][colIndex][1];
                    double rotation = 0;

                    //  Check if it's a rotated block.
                    if ("379BDGKMOQTXZ@$".Contains(symbol))
                    {
                        rotation = 90;
                    }

                    xmlBlocks += Xmlify(blockType, material, x, y, rotation);
                }
                
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

#if (true)
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

        Dictionary<string, Tuple<List<string>, List<double>>> rules1 = new Dictionary<string, Tuple<List<string>, List<double>>>
                {
                    {"1", new Tuple<List<string>, List<double>>(new List<string> {"1", "212"}, new List<double> {0.80, 0.20})},
                    {"4", new Tuple<List<string>, List<double>>(new List<string> {"4", "141"}, new List<double> {0.60, 0.40})},
                    {"2", new Tuple<List<string>, List<double>>(new List<string> {"2", "26262"}, new List<double> {0.20, 0.80})}
                };

        Dictionary<string, Tuple<List<string>, List<double>>> rules2 = new Dictionary<string, Tuple<List<string>, List<double>>>
                {
                    {"2", new Tuple<List<string>, List<double>>(new List<string> {"2", "234"}, new List<double> {0.80, 0.20})},
                    {"4", new Tuple<List<string>, List<double>>(new List<string> {"654", "34652"}, new List<double> {0.60, 0.40})},
                    {"6", new Tuple<List<string>, List<double>>(new List<string> {"6", "64574"}, new List<double> {0.20, 0.80})}
                };

        //LSystem r1 = new LSystem(rules1, 3, 5);
        LSystem r1 = new LSystem(6, 10);
        //LSystem r2 = new LSystem(rules2, 3, 5);
        LSystem r2 = new LSystem(6, 10);

        LSystem r3 = LSystem.Crossover(r1, r2);

        //Iterate through L-system
        r3.Iterate(3);

        //Start writing in level file
        StartFile(path);
        WriteBlocksToFile(r3, path);
        EndFile(path);

        foreach (string axiom in r3.iterations)
        {
            Console.WriteLine(axiom);
        }

        string encodedString = LSystem.Encode(r3);
        Console.WriteLine(encodedString);
        LSystem decodedLSystem = LSystem.Decode(encodedString);
        Console.WriteLine(LSystem.Encode(decodedLSystem));


        Console.ReadKey();
    }
#endif
}