using System;
using System.Collections.Generic;
using System.Text;

namespace GAInNetworks
{
    public class Individual
    {
        public List<int> Genes { get; set; } = new List<int>();
        public int Fitness { get; set; }

        public Individual() { }
        public Individual(List<int> genes)
        {
            Genes = genes;
        }

        public void CalculateFitness()
        {
            for(int i = 0; i < Genes.Count; i++)
            {
                if(i+1 < Genes.Count)
                {
                    Fitness += Matrix.matrix[Genes[i], Genes[i + 1]];
                }
            }
        }
    }
}
