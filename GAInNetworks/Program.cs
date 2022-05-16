using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GAInNetworks
{
    class Program
    {
        public static Random r { get; private set; }
        public static List<Individual> Population { get; set; }
        public static int TournamentSize { get; set; } = 2;
        public static int Iterations { get; set; } = 300;
        public static double MutationRate { get; set; } = 0.4;
        public static Individual Best { get; set; }
        public static Report ProjectReport { get; set; } = new Report();

        static void Main(string[] args)
        {
            
            r = new Random();
            Stopwatch stopwatch = new Stopwatch();
            string fileName = @"C:\Users\Maciej\source\repos\GAInNetworks\InitialPopulation.json";
            string jsonString = File.ReadAllText(fileName);
            Population = JsonSerializer.Deserialize<List<Individual>>(jsonString);//Matrix.CreatePopulation();
            Best = Population[0];
            //ReportGenerator.PrintInitialPopulation(Population);
            ReportGenerator.PrintNetworkMatrix(Matrix.matrix);
            ProjectReport.ReportName = "TS-TPC-IM";

            for(int j=0; j < 20; j++)
            {
                Population = JsonSerializer.Deserialize<List<Individual>>(jsonString);
                Best = Population[0];
                stopwatch.Start();
                for (int i = 0; i < Iterations; i++)
                {
                    var StochasticUniversalSampling = Evolution.StochasticUniversalSampling(Population);
                    var Parent1 = Evolution.tournamentSelection(Population, 2);//StochasticUniversalSampling.Parent1;//Evolution.rouletteSelection(Population); //.tournamentSelection(Population, 2);
                    var Parent2 = Evolution.tournamentSelection(Population, 2);//Evolution.rouletteSelection(Population); //.tournamentSelection(Population, 2);



                    var Offspring = Evolution.TwoPointCrossover(Parent1, Parent2);

                    var Child1 = Offspring.Child1;//Evolution.TwoPointCrossover(Parent1, Parent2);//Evolution.UniformCrossover(Parent1, Parent2);//.TwoPointCrossover(Parent1, Parent2);
                    var Child2 = Offspring.Child2;//Evolution.TwoPointCrossover(Parent1, Parent2);//Evolution.UniformCrossover(Parent1, Parent2);//.TwoPointCrossover(Parent1, Parent2);

                    Child1 = Evolution.inversionMutation(Child1);//.scrumbleMutation(Child1);
                    Child2 = Evolution.inversionMutation(Child2);//.scrumbleMutation(Child2);

                    Child1.CalculateFitness();
                    Child2.CalculateFitness();

                    Population.Add(Child1);
                    Population.Add(Child2);

                    TerminateWeakest();
                    ShowStrongest(i, stopwatch);
                }

                stopwatch.Stop();
                stopwatch.Reset();
            }


            ProjectReport.Data.ForEach(item =>
            {
                item.MaxFitness = item.MaxFitness / 20;
                item.MinFitness = item.MinFitness / 20;
                item.AvgOfFitness = item.AvgOfFitness / 20;
                item.TimeOfIteration = item.TimeOfIteration / 20;
            });

            ReportGenerator.GenerateRaport(ProjectReport);
        }

        static void TerminateWeakest()
        {
            for(int i = 0; i < 2; i++)
            {
                var maxFit = Population.Max(x => x.Fitness);
                var indiv = Population.FirstOrDefault(x => x.Fitness == maxFit);
                Population.Remove(indiv);
            }
        }

        static void ShowStrongest(int iteration, Stopwatch stopwatch)
        {
            var minFit = Population.Min(x => x.Fitness);
            var indivmin = Population.FirstOrDefault(x => x.Fitness == minFit);

            var maxFit = Population.Max(x => x.Fitness);
            var indivmax = Population.FirstOrDefault(x => x.Fitness == maxFit);

            if (Best.Fitness > minFit)
            {
                Best = indivmin;

            }

            if (ProjectReport.Data.Exists(x => x.Iteration == iteration) == false)
            {
                ProjectReport.Data.Add(new ReportData()
                {
                    Iteration = iteration,
                    MaxFitness = 0,
                    MinFitness = 0,
                    TimeOfIteration = 0,
                    AvgOfFitness = 0
                });
            }

            ProjectReport.Data[iteration].MaxFitness += maxFit;
            ProjectReport.Data[iteration].MinFitness += minFit;
            ProjectReport.Data[iteration].TimeOfIteration += stopwatch.ElapsedMilliseconds;
            ProjectReport.Data[iteration].AvgOfFitness += Population.Average(x => x.Fitness);


            Console.WriteLine("TimeElapsed: "+ stopwatch.ElapsedMilliseconds + "ms | Iter:" + iteration + " MIN Fitenss: " + Best.Fitness + " " + "MAX Fitenss: " + indivmax.Fitness + " ");
            for (int i = 0; i < Best.Genes.Count; i++)
            {
                Console.Write(Best.Genes[i] + " -> ");
            }
            Console.WriteLine();
        }
    }
}
