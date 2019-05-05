using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class LSystemEvolver {
    //  Member variables
    private int maxWidth;
    private int maxHeight;
    private int numRules;
    private int populationSize;
    private int mutationRate;
    private List<LSystem> population;

    //Constructor
    public LSystemEvolver(int nrules, int width, int height, int popSize, int mutRate) {
        //  Set hyperparameters
        numRules = nrules;
        maxWidth = width;
        maxHeight = height;
        populationSize = popSize;
        mutationRate = mutRate;

        //  Initialize population
        for (int i = 0; i < populationSize; i++) {
            //  Create random LSystem and add it to population
            population.Add(new LSystem(numRules, maxWidth));
        }

    }

    //Methods
    private LSystem Crossover(LSystem lparent, LSystem rparent) {
        return LSystem.Crossover(lparent, rparent);
    }

    private LSystem Mutation(LSystem l) {
        return LSystem.Mutation(l);
    }

    public List<LSystem> EvolvePopulation(double[] fitness, int mu, int lambda) {
        //  Check if length of fitness array matches population size
        if (fitness.Length != populationSize) {
            Console.WriteLine("Error: Invalid fitness size.");
            return population;
        }

        //  Check if mu + lambda equals population size.
        if (mu + lambda != populationSize) {
            Console.WriteLine("Error: mu + lambda does not equal populationSize.");
            return population;
        }
        
        //  Sett the fitness values for the population
        for (int popIndex = 0; popIndex < populationSize; popIndex++) {
            population[popIndex].SetFitness(fitness[popIndex]);
        }

        //  Sort the population in descending order by fitness value
        List<LSystem> orderedPopulation = population.OrderByDescending(l => l.fitness).ToList<LSystem>();

        //  Perform mu + lambda genetic algorithm. 
        //  Mu is the number of parents, lambda is the number of offspring.
        List<LSystem> newPopulation = orderedPopulation;

        //  Delete least fit lambda individuals
        //  Replace removed individuals with copies of mu best individuals
        //  Mutate the offspring
        for (int popIndex = mu; popIndex < lambda; popIndex++) {
            newPopulation[popIndex] = Mutation(newPopulation[popIndex % mu]);
        }

        return newPopulation;
        
    }

    


}