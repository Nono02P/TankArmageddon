using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace IA
{
    public class PopulationManagerEventArgs : EventArgs
    {
        public List<GeneticNeuralNetwork> Genomes { get; private set; }

        public PopulationManagerEventArgs(List<GeneticNeuralNetwork> pGenomes)
        {
            Genomes = pGenomes;
        }
    }

    [DataContract]
    public class Population
    {
        #region Delegate
        public delegate void PopulationManagerHandler(object sender, PopulationManagerEventArgs e);
        #endregion

        #region Evènements
        public event PopulationManagerHandler OnGenomesChanged;
        #endregion

        #region Variables privées
        [DataMember]
        private Random _rnd;

        private DataContractJsonSerializer _ser;
        #endregion

        #region Propriétés
        [DataMember]
        public List<GeneticNeuralNetwork> Genomes { get; private set; }

        [DataMember]
        public float MutationRate { get; set; } = 0.01f;

        [DataMember]
        public ushort MinimumAcceptableFitness { get; set; } = 1;
        public int Generation { get; private set; }
        #endregion

        #region Constructeur
        public Population()
        {
            Genomes = new List<GeneticNeuralNetwork>();
            _rnd = new Random();
            _ser = new DataContractJsonSerializer(typeof(Population));
        }
        #endregion

        #region Créé une instance à partir d'un fichier
        public static Population OpenFromFile(string pPathFile = "Population")
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Population));
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(pPathFile + ".json"));
            return (Population)ser.ReadObject(stream);
        }
        #endregion

        #region Sérialize l'instance
        public void Export(string pPathFile = "Population")
        {
            string path = pPathFile;
            if (path == "")
            {
                path = "Population";
            }
            FileStream stream = File.Create(path + ".json");
            _ser.WriteObject(stream, this);
            stream.Close();
        }
        #endregion

        #region Nouvelle génération
        /// <summary>
        /// Créé une nouvelle génération de la population.
        /// </summary>
        /// <param name="pSavePopulation">Sauvegarde la population avant de générer une nouvelle.</param>
        /// <param name="pPathFile">Chemin du fichier de destination.</param>
        public void NextGeneration(bool pSavePopulation = false, string pPathFile = "Population")
        {
            int populationNumber = Genomes.Count;

            #region Evaluation du score total
            int totalFitness = 0;
            int maxFitness = 0;
            for (int i = 0; i < Genomes.Count; i++)
            {
                GeneticNeuralNetwork g = Genomes[i];
                if (g.FitnessScore > 0)
                    totalFitness += g.FitnessScore;
                if (g.FitnessScore > maxFitness)
                    maxFitness = g.FitnessScore;
            }
            #endregion

            #region Sauvegarde la génération actuelle
            if (pSavePopulation)
            {
                Export(totalFitness + "_" + pPathFile);
            }
            #endregion

            // Si le Fitness max est supérieur au minimum acceptable, 
            // conserve le nombre de couches cachées du réseau de neurones.
            // Sinon, génère un nouveau schéma de réseau de neurones.
            if (maxFitness > MinimumAcceptableFitness)
            {
                #region Création d'un sac de parents (pour gérer un pourcentage de chance par rapport au score)
                List<GeneticNeuralNetwork> parentBag = new List<GeneticNeuralNetwork>();
                for (int i = 0; i < Genomes.Count; i++)
                {
                    GeneticNeuralNetwork g = Genomes[i];
                    for (int j = 0; j < (g.FitnessScore / (float)totalFitness) * 100; j++)
                    {
                        parentBag.Add(g);
                    }
                }
                #endregion

                #region Génère des enfants
                List<GeneticNeuralNetwork> nextPopulation = new List<GeneticNeuralNetwork>();
                while (nextPopulation.Count < populationNumber)
                {
                    #region Sélectionne deux parents et les retire du sac
                    int indexP1 = _rnd.Next(parentBag.Count);
                    GeneticNeuralNetwork p1 = parentBag[indexP1];
                    parentBag.RemoveAt(indexP1);

                    int indexP2 = _rnd.Next(parentBag.Count);
                    GeneticNeuralNetwork p2 = parentBag[indexP2];
                    parentBag.RemoveAt(indexP2);
                    #endregion

                    GeneticNeuralNetwork[] childs = GeneticNeuralNetwork.CreateChilds(p1, p2, _rnd);

                    if (nextPopulation.Count == populationNumber - 1)
                        nextPopulation.Add(p1);
                    else
                        nextPopulation.AddRange(childs);
                }
                #endregion

                #region Effectue une mutation sur les enfants
                for (int i = 0; i < nextPopulation.Count; i++)
                {
                    GeneticNeuralNetwork c = nextPopulation[i];
                    c.Mutate(_rnd, MutationRate);
                }
                #endregion

                Genomes = nextPopulation;
            }
            else
            {
                GeneticNeuralNetwork g = Genomes[0];
                int nbInputs = g.NbInputs;
                int nbOutputs = g.NbOutputs;
                int[] hidden = new int[_rnd.Next(1, 20)];
                ActivationFunctions.eActivationFunction function = g.ActivationFunction;
                for (int i = 0; i < hidden.Length; i++)
                {
                    hidden[i] = _rnd.Next(nbOutputs, nbInputs + nbOutputs);
                }
                Genomes = new List<GeneticNeuralNetwork>();
                for (int i = 0; i < populationNumber; i++)
                {
                    Genomes.Add(new GeneticNeuralNetwork(nbInputs, hidden, nbOutputs, function, true));
                }
            }
            OnGenomesChanged?.Invoke(this, new PopulationManagerEventArgs(Genomes));
            Generation++;
        }
        #endregion
    }
}