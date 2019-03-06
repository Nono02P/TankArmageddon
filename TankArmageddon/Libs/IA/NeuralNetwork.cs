using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IA
{
    [DataContract]
    public class NeuralNetwork
    {
        private Random _rnd;

        [DataMember]
        private List<Matrix> _neurons;
        [DataMember]
        private List<Matrix> _weights;
        [DataMember]
        private List<Matrix> _bias;

        [DataMember]
        public ActivationFunctions.eActivationFunction ActivationFunction { get; set; }
        [DataMember]
        public float LearningRate { get; set; } = 0.2f;

        public NeuralNetwork(int NbInputs, int[] NbHiddens, int NbOutputs, ActivationFunctions.eActivationFunction pActivationFunction = ActivationFunctions.eActivationFunction.Sigmoid, bool pAllowNegative = false)
        {
            _rnd = new Random();
            ActivationFunction = pActivationFunction;
            _weights = new List<Matrix>();
            _neurons = new List<Matrix>();
            _bias = new List<Matrix>();

            Matrix Weights_IH = new Matrix(NbHiddens[0], NbInputs);
            Matrix Bias = new Matrix(NbHiddens[0], 1);
            Weights_IH.Randomize(_rnd, pAllowNegative);
            Bias.Randomize(_rnd, pAllowNegative);
            _weights.Add(Weights_IH);
            _bias.Add(Bias);

            for (int i = 0; i < NbHiddens.Length; i++)
            {
                Matrix w;
                Matrix b;
                if (i < NbHiddens.Length - 1)
                {
                    w = new Matrix(NbHiddens[i + 1], NbHiddens[i]);
                    b = new Matrix(NbHiddens[i + 1], 1);
                }
                else
                {
                    w = new Matrix(NbOutputs, NbHiddens[i]);
                    b = new Matrix(NbOutputs, 1);
                }
                // Poids
                w.Randomize(_rnd, pAllowNegative);
                _weights.Add(w);
                // Biais
                b.Randomize(_rnd, pAllowNegative);
                _bias.Add(b);
            }
        }

        /// <summary>
        /// Exécutes le réseau de neurones afin d'évaluer la sortie en fonction des entrées.
        /// </summary>
        /// <param name="pInputs">Entrées à évaluer.</param>
        /// <returns>Sorties.</returns>
        public float[] FeedForward(float[] pInputs)
        {
            _neurons.Clear();
            // Sauvegarde les entrées dans _neurons
            Matrix O = Matrix.FromArray(pInputs);
            _neurons.Add(O);

            // Calcule la valeur des neurones de chaque couche puis sauvegarde ces valeurs dans _neurons.
            for (int i = 0; i < _weights.Count; i++)
            {
                Matrix w = _weights[i];
                Matrix b = _bias[i];
                O = Matrix.DotProduct(w, O);
                O.Add(b);
                O.ExecuteOnMatrix(ActivationFunctions.GetActivationFunction(ActivationFunction));
                _neurons.Add(O);
            }
            return O.Data;
        }

        /// <summary>
        /// Entraine le réseau en exécutant le réseau pour en évaluer les sorties.
        /// Puis compare les sorties obtenus par rapport aux sorties désirées, et ajuste les poids afin de se rapprocher des sorties désirées.
        /// </summary>
        /// <param name="pInputs">Entrées à évaluer.</param>
        /// <param name="pTarget">Sorties qui devraient être renvoyées par le réseau (réponses)</param>
        /// <returns>Item1 = Sorties
        /// Item2 = Error</returns>
        public Tuple<float[], float[]> Train(float[] pInputs, float[] pTarget)
        {
            // - O => Outputs
            // - T => Targets
            FeedForward(pInputs);
            Matrix O = _neurons[_neurons.Count - 1];
            Matrix T = Matrix.FromArray(pTarget);

            // Calcule l'erreur entre la sortie et la consigne.
            Matrix error = Matrix.Subtract(T, O);

            for (int i = _weights.Count - 1; i >= 0; i--)
            {
                // Calcule le gradient dans la direction 
                // de la couche de neurones de sortie vers la couche de neurones de l'entrée.
                //
                // Le gradient est la dérivée (de la fonction utilisée) de la sortie qu'elle a généré.
                Matrix gradient = Matrix.ExecuteOnMatrix(_neurons[i + 1], ActivationFunctions.GetDerivedActivationFunction(ActivationFunction));
                gradient.Multiply(error);
                gradient.Multiply(LearningRate);

                // Calcule le delta de poids entre deux couches puis l'ajoute au poids actuel.
                // * Ouputs <-- Hidden[n].
                // * Hidden[n] <-- Hidden[n - 1].
                // * Hidden[n - 1] <-- Inputs
                Matrix previous_Transpose = Matrix.Transpose(_neurons[i]);
                Matrix weight_Delta = Matrix.DotProduct(gradient, previous_Transpose);
                _weights[i].Add(weight_Delta);

                // Ajuste le Biais de sortie par rapport à son gradient.
                _bias[i].Add(gradient);

                // Calcule l'erreur à appliquer sur la couche précédente.
                Matrix weight_Transpose = Matrix.Transpose(_weights[i]);
                error = Matrix.DotProduct(weight_Transpose, error);
            }
            return new Tuple<float[], float[]> (O.Data, error.Data);
        }
    }
}