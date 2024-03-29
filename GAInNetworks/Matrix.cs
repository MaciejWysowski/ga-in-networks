﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GAInNetworks
{
    public class Matrix
    {
        public static int[,] matrix = new int[20, 20] { 
            { 1000, 52, 61,8, 16, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 52, 1000, 1000,1000, 1000, 78, 41, 6, 92, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 61, 1000, 1000,1000, 1000, 84, 63, 2, 99, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 8, 1000, 1000, 1000, 1000, 71, 48, 223, 73, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 16, 1000, 1000,1000, 1000, 63, 55, 44, 88, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 1000, 78, 84, 71, 63, 1000, 1000, 1000, 1000, 11, 22, 33, 44, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 1000, 41, 63, 48, 55, 1000, 1000, 1000, 1000, 21, 32, 43, 54, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 1000, 6, 2, 223, 44, 1000, 1000, 1000, 1000, 74, 85, 96, 14, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 1000, 92, 99, 73, 88, 1000, 1000, 1000, 1000, 46, 64, 75, 35, 1000, 1000, 1000, 1000, 1000, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 11, 21, 74, 46, 1000, 1000, 1000, 1000, 66, 55, 44, 11, 1000, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 22, 32, 85, 64, 1000, 1000, 1000, 1000, 91, 97, 73, 19, 1000, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 33, 43, 96, 75, 1000, 1000, 1000, 1000, 45, 65, 25, 85, 1000, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 44, 54, 14, 35, 1000, 1000, 1000, 1000, 73, 37, 87, 16, 1000, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 66, 91, 45, 73, 1000, 1000, 1000, 1000, 86, 84, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 55, 97, 65, 37, 1000, 1000, 1000, 1000, 74, 76, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 44, 73, 25, 87, 1000, 1000, 1000, 1000, 2, 6, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 11, 19, 85, 16, 1000, 1000, 1000, 1000, 7, 9, 1000, 1000},
            { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 86, 74, 2, 7, 1000, 1000, 52},
            { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 84, 76, 6, 9, 1000, 1000, 25},
            { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000, 52, 25, 1000},
        };

        public static List<Individual> CreatePopulation()
        {
            var r = new Random();
            var rowIndex = 0;
            var pop = new List<Individual>();
            for(int i = 0; i < 50; i++)
            {
                
                var indiv = new Individual();
                r = new Random();
                indiv.Genes.Add(0);
                var maxInter = 0;
                while (rowIndex != 19)
                {
                    var validColumn = false;
                    while (!validColumn)
                    {
                        var column = r.Next(0, 20);
                        if(matrix[rowIndex, column] != 1000 && !indiv.Genes.Contains(column))
                        {
                            indiv.Genes.Add(column);
                            indiv.Fitness += matrix[rowIndex, column];
                            validColumn = true;
                            rowIndex = column;
                        }
                        
                        if(maxInter > 2000)
                        {
                            indiv.Genes.Add(19);
                            indiv.Fitness += matrix[rowIndex, 19];
                            validColumn = true;
                            rowIndex = 19;
                        }

                        maxInter++;
                    }
                    
                    
                }
                pop.Add(indiv);
                rowIndex = 0;
            }

            string json = JsonSerializer.Serialize(pop);
            File.WriteAllText(@"C:\Users\Maciej\source\repos\GAInNetworks\InitialPopulation.json", json);

            return pop;
        }
    }
}
