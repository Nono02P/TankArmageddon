using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using TankArmageddon;

namespace IA
{
    [DataContract]
    public class GeneticNeuralNetwork : NeuralNetwork
    {
        #region Evènements
        public event onIntChange OnFitnessScoreChange;
        #endregion

        #region Variables privées
        private int _fitnessScore;
        #endregion

        #region Propriétés
        [DataMember]
        public int FitnessScore { get => _fitnessScore; set { if (_fitnessScore != value) OnFitnessScoreChange?.Invoke(this, _fitnessScore, value); _fitnessScore = value; } }
        #endregion

        #region Constructeur
        public GeneticNeuralNetwork(int NbInputs, int[] NbHiddens, int NbOutputs, ActivationFunctions.eActivationFunction pActivationFunction = ActivationFunctions.eActivationFunction.Sigmoid, bool pAllowNegative = false) : base(NbInputs, NbHiddens, NbOutputs, pActivationFunction, pAllowNegative) { }
        #endregion

        #region Créé une instance à partir d'un fichier
        public static GeneticNeuralNetwork OpenFromFile(string pPathFile)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(GeneticNeuralNetwork));
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(pPathFile));
            return (GeneticNeuralNetwork)ser.ReadObject(stream);
        }
        #endregion

        #region Génère deux enfants à partir de deux parents
        /// <summary>
        /// Génère deux enfants à partir de deux parents
        /// </summary>
        /// <param name="pParent1"></param>
        /// <param name="pParent2"></param>
        /// <returns>Renvoie un array de deux enfants</returns>
        public static GeneticNeuralNetwork[] CreateChilds(GeneticNeuralNetwork pParent1, GeneticNeuralNetwork pParent2, Random pRnd)
        {
            GeneticNeuralNetwork[] childs = new GeneticNeuralNetwork[2];
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i] = new GeneticNeuralNetwork(pParent1.NbInputs, pParent1.NbHiddens, pParent1.NbOutputs, pParent1.ActivationFunction, pParent1._allowNegative);
                for (int w = 0; w < pParent1._weights.Count; w++)
                {
                    Matrix m1 = pParent1._weights[w];
                    Matrix m2 = pParent2._weights[w];
                    for (int d = 0; d < m1.Data.Length; d++)
                    {
                        if (pRnd.NextDouble() > 0.5f)
                        {
                            childs[i]._weights[w].Data[d] = m1.Data[d];
                        }
                        else
                        {
                            childs[i]._weights[w].Data[d] = m2.Data[d];
                        }
                    }
                }
                for (int b = 0; b < pParent1._bias.Count; b++)
                {
                    Matrix m1 = pParent1._bias[b];
                    Matrix m2 = pParent2._bias[b];
                    for (int d = 0; d < m1.Data.Length; d++)
                    {
                        if (pRnd.NextDouble() > 0.5f)
                        {
                            childs[i]._bias[b].Data[d] = m1.Data[d];
                        }
                        else
                        {
                            childs[i]._bias[b].Data[d] = m2.Data[d];
                        }
                    }
                }
            }
            return childs;
        }
        #endregion

        #region Mutation
        /// <summary>
        /// Effectue une mutation sur le génome actuel.
        /// </summary>
        /// <param name="pMutationRate">Pourcentage de chance de faire muter les poids du réseau de neurones.</param>
        /// <param name="pRnd"></param>
        public void Mutate(Random pRnd, float pMutationRate = 0.01f)
        {
            for (int w = 0; w < _weights.Count; w++)
            {
                Matrix m = _weights[w];
                for (int d = 0; d < m.Data.Length; d++)
                {
                    if (pRnd.NextDouble() <= pMutationRate)
                        _weights[w].Data[d] = pRnd.Next();
                }
            }
            for (int b = 0; b < _bias.Count; b++)
            {
                Matrix m = _bias[b];
                for (int d = 0; d < m.Data.Length; d++)
                {
                    if (pRnd.NextDouble() <= pMutationRate)
                        _bias[b].Data[d] = pRnd.Next();
                }
            }
        }
        #endregion
    }
}