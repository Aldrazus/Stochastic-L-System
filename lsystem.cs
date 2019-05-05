using System.Collections;
using System.Collections.Generic;
using System;
//using UnityEngine;


//  Block coordinates (x, y) indicate the center of the block.
//  Floor is at y = -3.50 . 

public class LSystem
{
    //Random number generator
    private static System.Random random = new System.Random();
    //Change list to tuple.
    private Dictionary<string, Tuple<List<string>, List<double>>> rules;

    //  Change these to private.
    public int maxHeight;

    public int maxWidth = 5;

    public int numRules = 3;

    public double fitness;

    public List<string> iterations;


    public List<double> rowStartCoordinates;

    public List<List<List<double>>> blockCoordinates;

    //Change to static
    public Dictionary<string, List<double>> blocks = new Dictionary<string, List<double>>
        {
            {"1", new List<double> {0.84, 0.84}},
            {"2", new List<double> {0.85, 0.43}},
            {"3", new List<double> {0.43, 0.85}},
            {"4", new List<double> {0.43, 0.43}},
            {"5", new List<double> {0.22, 0.22}},
            {"6", new List<double> {0.43, 0.22}},
            {"7", new List<double> {0.22, 0.43}},
            {"8", new List<double> {0.85, 0.22}},
            {"9", new List<double> {0.22, 0.85}},
            {"A", new List<double> {1.68, 0.22}},
            {"B", new List<double> {0.22, 1.68}},
            {"C", new List<double> {2.06, 0.22}},
            {"D", new List<double> {0.22, 2.06}}
        };


    //  TODO: fix issue with air blocks and height checks
    //Change to static
    public Dictionary<string, string> block_names = new Dictionary<string, string> {
        //{"0", "Air"},
        {"1", "SquareHole"},
        {"2", "RectFat"},
        {"3","RectFat"},
        {"4","SquareSmall"},
        {"5", "SquareTiny"},
        {"6", "RectTiny"},
        {"7", "RectTiny"},
        {"8", "RectSmall"},
        {"9", "RectSmall"},
        {"A", "RectMedium"},
        {"B", "RectMedium"},
        {"C", "RectBig"},
        {"D", "RectBig"}
    };

    //Constructor with pre-defined rules & axiom.
    public LSystem(Dictionary<string, Tuple<List<string>, List<double>>> r, int numRule, int maxW) {
        //  Get an axiom from a rule.
        string axiom = "A"; //  Default
        foreach (KeyValuePair<string, Tuple<List<string>, List<double>>> ruleset in r) {
            if (ruleset.Value.Item1.Count != 0) {
                axiom = ruleset.Key;
                break;
            }
        }

        maxWidth = maxW;
        numRules = numRule;
        rules = r;
        iterations = new List<string> { axiom };
        rowStartCoordinates = new List<double> { };

    }

    //Constructor with random rules & axiom.
    public LSystem(int numRule, int maxW) {

        numRules = numRule;
        maxWidth = maxW;
        //Get random rules
        rules = GenerateRandomRules(numRule, maxW);


        //Get random axiom from rule conditions
        List<string> possibleAxioms = new List<string>(rules.Keys);
        string axiom = possibleAxioms[random.Next(0, possibleAxioms.Count)];

        //Initialize LSystem
        
        iterations = new List<string> { axiom };
        rowStartCoordinates = new List<double> { };
    }

    //Rescale probabilities of rules to interval [0, 1]
    public static List<double> RescaleWeights(List<double> weights) {

        double total = 0;

        List<double> newWeights = new List<double> { };

        foreach (double weight in weights) {
            total += weight;
        }

        foreach (double weight in weights) {
            newWeights.Add(weight / total);
        }

        return newWeights;

    }

    //Generates random rules
    private Dictionary<string, Tuple<List<string>, List<double>>> GenerateRandomRules(int numRule, int maxW) {

        //  Initializing the dictionary
        Dictionary<string, Tuple<List<string>, List<double>>> newRules = new Dictionary<string, Tuple<List<string>, List<double>>>();
        foreach (string symbol in blocks.Keys) {
            newRules[symbol] = new Tuple<List<string>, List<double>>(new List<string>(), new List<double>());
        }


        for (int i = 0; i < numRule; i++) {
            List<string> symbols = new List<string>(blocks.Keys);

            //Get a condition.
            string condition = symbols[random.Next(symbols.Count)];

            //Get a successor of random width < max width.
            int succWidth = random.Next(1, maxW);
            string successor = "";
            for (int s = 0; s < succWidth; s++) {
                string nextSymbol = symbols[random.Next(symbols.Count)];
                successor += nextSymbol;
            }

            //Get a probability.
            double probability = random.Next(0, 100) / 100;

            //Update existing successors and probabilities for given condition.
            List<string> newSuccessors = newRules[condition].Item1;
            List<double> newProbabilities = newRules[condition].Item2;

            newSuccessors.Add(successor);
            newProbabilities.Add(probability);

            //Rescale probabilities to [0, 1].
            newProbabilities = RescaleWeights(newProbabilities);

            //Replace old successors and probabilities with new ones.
            newRules[condition] = new Tuple<List<string>, List<double>>(
                new List<string>(newSuccessors),
                new List<double>(newProbabilities)
            );

        }

        return newRules;

    }

    public static LSystem Crossover(LSystem lparent, LSystem rparent) {


        //  Don't really need to be equal
        if (lparent.numRules != rparent.numRules) {
            Console.WriteLine("Error: Parents do not have equal number of rules.");
        }

        //  Initializing the dictionary
        Dictionary<string, Tuple<List<string>, List<double>>> offspringRules = new Dictionary<string, Tuple<List<string>, List<double>>>();
        foreach (string symbol in lparent.blocks.Keys)
        {
            offspringRules[symbol] = new Tuple<List<string>, List<double>>(new List<string>(), new List<double>());
        }


        //  Get rules from the left parent.
        int ldictIndex = 0;
        int llistIndex = 0;
        List<string> lconditions = new List<string>(lparent.rules.Keys);
        //  TODO: change to half of numRules
        for (int i = 0; i < lparent.numRules / 2; i++) {

            //  Check if all of the rules with same condition have been seen.
            while (llistIndex >= lparent.rules[lconditions[ldictIndex]].Item1.Count)
            {
                //  Move to the next condition
                ldictIndex++;
                llistIndex = 0;
            }

            string condition = lconditions[ldictIndex];
            string successor = lparent.rules[condition].Item1[llistIndex];
            double probability = lparent.rules[condition].Item2[llistIndex];


            //Update existing successors and probabilities for given condition.
            List<string> newSuccessors = offspringRules[condition].Item1;
            List<double> newProbabilities = offspringRules[condition].Item2;

            newSuccessors.Add(successor);
            newProbabilities.Add(probability);

            //Rescale probabilities to [0, 1].
            newProbabilities = RescaleWeights(newProbabilities);

            //Replace old successors and probabilities with new ones.
            offspringRules[condition] = new Tuple<List<string>, List<double>>(
                new List<string>(newSuccessors),
                new List<double>(newProbabilities)
            );

            llistIndex++;
            
        }

        //  Get rules from right parent.
        int rdictIndex = 0;
        int rlistIndex = 0;
        List<string> rconditions = new List<string>(rparent.rules.Keys);
        //  TODO: change to half of numRules
        for (int i = rparent.numRules / 2; i < rparent.numRules; i++)
        {

            //  Check if all of the rules with same condition have been seen.
            while (rlistIndex >= rparent.rules[rconditions[rdictIndex]].Item1.Count)
            {
                //  Move to the next condition
                rdictIndex++;
                rlistIndex = 0;
            }

            string condition = rconditions[rdictIndex];
            string successor = rparent.rules[condition].Item1[rlistIndex];
            double probability = rparent.rules[condition].Item2[rlistIndex];


            //Update existing successors and probabilities for given condition.
            List<string> newSuccessors = offspringRules[condition].Item1;
            List<double> newProbabilities = offspringRules[condition].Item2;

            newSuccessors.Add(successor);
            newProbabilities.Add(probability);

            //Rescale probabilities to [0, 1].
            newProbabilities = RescaleWeights(newProbabilities);

            //Replace old successors and probabilities with new ones.
            offspringRules[condition] = new Tuple<List<string>, List<double>>(
                new List<string>(newSuccessors),
                new List<double>(newProbabilities)
            );

            rlistIndex++;

        }

        return new LSystem(offspringRules, lparent.numRules, lparent.maxWidth);



    }


    //  TODO: Change condition too?
    //  Mutates one random rule, changing the successor and probability of that rule.
    public static LSystem Mutation(LSystem l) {
        List<string> symbols = new List<string>(l.blocks.Keys);

        //Get a successor of random width < max width.
        int succWidth = random.Next(1, l.maxWidth);
        string successor = "";
        for (int s = 0; s < succWidth; s++)
        {
            string nextSymbol = symbols[random.Next(symbols.Count)];
            successor += nextSymbol;
        }

        //Get a probability.
        double probability = random.Next(0, 100) / 100;

        Dictionary<string, Tuple<List<string>, List<double>>> r = l.rules;

        foreach (KeyValuePair<string, Tuple<List<string>, List<double>>> ruleset in l.rules) {
            if (ruleset.Value.Item1.Count != 0) {
                List<string> newSuccessors = ruleset.Value.Item1;
                List<double> newProbabilities = ruleset.Value.Item2;

                //  Mutate a random rule
                int randInd = random.Next(0, newSuccessors.Count);
                newSuccessors[randInd] = successor;
                newProbabilities[randInd] = probability;

                //Rescale probabilities to [0, 1].
                newProbabilities = RescaleWeights(newProbabilities);
                r[ruleset.Key] = new Tuple<List<string>, List<double>>(new List<string>(newSuccessors), new List<double>(newProbabilities));

                return new LSystem(r, l.numRules, l.maxWidth);
            }
        }

        Console.WriteLine("Error: Empty rule dictionary.");
        return new LSystem(r, l.numRules, l.maxWidth);

    }

    //  Iterates LSystem, applying rules on axiom for numIter iterations.
    public void Iterate(int numIter) {
        iterations = new List<string> { iterations[0] };

        for (; numIter > 0; numIter--) {

            string newAxiom = "";

            foreach (char symbol in iterations[iterations.Count - 1]) {
                //  Check if there is a rule with that condition.
                if (rules.ContainsKey(symbol.ToString()) && rules[symbol.ToString()].Item1.Count != 0) {
                    WSelect wselect = new WSelect();
                    newAxiom += wselect.Select(rules[symbol.ToString()].Item1, rules[symbol.ToString()].Item2);
                }
                else {

                    newAxiom += symbol;
                }
            }
            iterations.Add(newAxiom);
        }

        //Debug.Log("-----------------------------------------");
        //foreach (string iteration in iterations)
        //{
        //    Debug.Log(iteration);
        //}
        //Debug.Log("-----------------------------------------");

        blockCoordinates = new List<List<List<double>>>();
        for (int i = 0; i < iterations.Count; i++) {
            blockCoordinates.Add(new List<List<double>>());
            for (int j = 0; j < iterations[i].Length; j++) {
                blockCoordinates[i].Add(new List<double>());
            }
        }

        GetStartCoordinates(0.0);
        GetBlockCoordinates();

    }


    //  Update the list of start coordinates.
    private void GetStartCoordinates(double structureCenterX) {

        double rowCenterX = structureCenterX;

        //  Find the start of each row (X-coordinate) by subtracting half of width from
        //  structure center.
        //  Goes from top row to bottom row.
        for (int rowIndex = 0; rowIndex < iterations.Count; rowIndex++) {

            double rowWidth = 0;


            foreach (char symbol in iterations[rowIndex]) {
                //Debug.Log(symbol.ToString());
                rowWidth += blocks[symbol.ToString()][0];
            }

            double rowStart = rowCenterX - (rowWidth / 2);

            rowStartCoordinates.Add(rowStart);

        }


    }

    private void GetBlockCoordinates() {

        for (int rowIndex = iterations.Count - 1; rowIndex > -1; rowIndex--) {

            //  Get x coordinates
            double x = rowStartCoordinates[rowIndex];
            for (int colIndex = 0; colIndex < iterations[rowIndex].Length; colIndex++) {
                x += blocks[iterations[rowIndex][colIndex].ToString()][0] / 2;
                blockCoordinates[rowIndex][colIndex].Add(x);
                x += blocks[iterations[rowIndex][colIndex].ToString()][0] / 2;
            }

            //  Get y coordinates
            if (rowIndex == iterations.Count - 1) {
                for (int colIndex = 0; colIndex < iterations[rowIndex].Length; colIndex++) {
                    blockCoordinates[rowIndex][colIndex].Add(-3.5 + blocks[iterations[rowIndex][colIndex].ToString()][1] / 2);
                }
            }
            else {

                string bottomRow = iterations[rowIndex + 1], currRow = iterations[rowIndex];
                for (int colIndex = 0; colIndex < iterations[rowIndex].Length; colIndex++) {
                    List<double> checkInterval = new List<double> {
                        blockCoordinates[rowIndex][colIndex][0] - blocks[currRow[colIndex].ToString()][0],
                        blockCoordinates[rowIndex][colIndex][0] + blocks[currRow[colIndex].ToString()][0]
                    };

                    double maxHeight = -3.5;
                    for (int axIndex = 0; axIndex < bottomRow.Length; axIndex++) {
                        string symbol = bottomRow[axIndex].ToString();
                        double blockWidth = blocks[symbol][0];
                        double blockCenterX = blockCoordinates[rowIndex + 1][axIndex][0];
                        double leftEdge = blockCenterX - (blockWidth / 2);
                        double rightEdge = blockCenterX + (blockWidth / 2);

                        if (
                        (leftEdge > checkInterval[0] && leftEdge < checkInterval[1]) ||
                        (rightEdge > checkInterval[0] && rightEdge < checkInterval[1]) ||
                        (leftEdge < checkInterval[0] && rightEdge > checkInterval[1])
                        )
                        {
                            double blockHeight = blocks[symbol][1];
                            double blockTopEdgeHeight = blockCoordinates[rowIndex + 1][axIndex][1] + blockHeight / 2;
                            if (blockTopEdgeHeight > maxHeight) {
                                maxHeight = blockTopEdgeHeight;
                            }
                        }
                    }

                    blockCoordinates[rowIndex][colIndex].Add(maxHeight + blocks[currRow[colIndex].ToString()][1] / 2);
                }
            }


        }

    }

    public string GenerateXML(int height) {
        //Generate the level by iterating.
        Iterate(height);

        return StartXML() + BlocksToXML() + EndXML();

        //Write the XML file.
        //StringToStructure.StartFile(path);
        //StringToStructure.WriteBlocksToFile(this, path);
        //StringToStructure.EndFile(path);

    }

    private string StartXML() {
        return "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n" +
            "<Level width =\"2\">\n" +
            "<Camera x=\"0\" y=\"2\" minWidth=\"20\" maxWidth=\"30\">\n" +
            "<Birds>\n" +
            "<Bird type=\"BirdRed\"/>\n" +
            "<Bird type=\"BirdRed\"/>\n" +
            "<Bird type=\"BirdRed\"/>\n" +
            "</Birds>\n" +
            "<Slingshot x=\"-8\" y=\"-2.5\">\n" +
            "<GameObjects>\n";
    }

    private string EndXML() {
        return "</GameObjects>\n" +
            "</Level>\n";
    }

    private string BlocksToXML() {
        string xmlBlocks = "";
        for (int rowIndex = 0; rowIndex < iterations.Count; rowIndex++)
        {
            for (int colIndex = 0; colIndex < iterations[rowIndex].Length; colIndex++)
            {
                string symbol = iterations[rowIndex][colIndex].ToString();
                string blockType = block_names[symbol];
                string material = "wood";
                double x = blockCoordinates[rowIndex][colIndex][0];
                double y = blockCoordinates[rowIndex][colIndex][1];
                //  TODO: include checks for rotation
                double rotation = 0;
                xmlBlocks += Xmlify(blockType, material, x, y, rotation);
            }
        }
        return xmlBlocks;
    }

    private static string Xmlify(string blockType, string material, double x, double y, double rot) {
        return String.Format(
            "<Block type=\"{0}\" material=\"{1}\" x=\"{2}\" y=\"{3}\" rotation=\"{4}\" />\n",
            blockType,
            material,
            x.ToString(),
            y.ToString(),
            rot.ToString());
    }

    public string this[int key] {
        get
        {
            return iterations[key];
        }
        set
        {
            iterations[key] = value;
        }
    }

    public void SetFitness(double fvalue) {
        fitness = fvalue;
    }

    class WSelect {

        private static System.Random random = new System.Random();
        public string Select(List<string> choices, List<double> weights) {
            double total = 100.0;
            double count = 0.0;
            double winner = random.Next(0, 100);

            for (int i = 0; i < choices.Count; i++) {
                count += weights[i] * total;
                if (winner < count) {
                    return choices[i];
                }
            }

            //Debug.Log("Error: choice was not picked.");
            return choices[0];
        }
    }
}
