using System;

namespace IA
{
    public static class ActivationFunctions
    {
        public enum eActivationFunction : byte
        {
            Sigmoid,
            TanH,
            ReLU,
            ELU,
        }

        /// <summary>
        /// Permet de retourner la fonction d'activation.
        /// </summary>
        /// <param name="pActivationFunction">Fonction d'activation désirée.</param>
        /// <returns>Fonction d'activation au format de FunctionOnMatrix(float value, int idxRow, int idxColumn).</returns>
        public static FunctionOnMatrix GetActivationFunction(eActivationFunction pActivationFunction)
        {
            switch (pActivationFunction)
            {
                case eActivationFunction.Sigmoid:
                    return Sigmoid;
                case eActivationFunction.TanH:
                    return TanH;
                case eActivationFunction.ReLU:
                    return ReLU;
                case eActivationFunction.ELU:
                    return ELU;
                default:
                    throw new Exception("This activation function isn't implemented.");
            }
        }

        /// <summary>
        /// Permet de retourner la dérivée de la fonction d'activation.
        /// </summary>
        /// /// <param name="pActivationFunction">Fonction d'activation désirée.</param>
        /// <returns>Dérivée de la fonction d'activation au format de FunctionOnMatrix(float value, int idxRow, int idxColumn).</returns>
        public static FunctionOnMatrix GetDerivedActivationFunction(eActivationFunction pActivationFunction)
        {
            switch (pActivationFunction)
            {
                case eActivationFunction.Sigmoid:
                    return DerivedSigmoid;
                case eActivationFunction.TanH:
                    return DerivedTanH;
                case eActivationFunction.ReLU:
                    return DerivedReLU;
                case eActivationFunction.ELU:
                    return DerivedELU;
                default:
                    throw new Exception("This activation function isn't implemented.");
            }
        }

        /// <summary>
        /// Fonction d'activation Sigmoid.
        /// </summary>
        private static float Sigmoid(float value, int idxRow, int idxColumn)
        {
            return (float)(1 / (1 + Math.Exp(-value)));
        }

        /// <summary>
        /// Dérivée de la fonction d'activation Sigmoid.
        /// </summary>
        private static float DerivedSigmoid(float value, int idxRow, int idxColumn)
        {
            return value * (1 - value);
        }

        /// <summary>
        /// Fonction d'activation TanH.
        /// </summary>
        private static float TanH(float value, int idxRow, int idxColumn)
        {
            return (float)Math.Tanh(value);
        }

        /// <summary>
        /// Dérivée de la fonction d'activation TanH.
        /// </summary>
        private static float DerivedTanH(float value, int idxRow, int idxColumn)
        {
            return 1 - value * value;
        }

        /// <summary>
        /// Fonction d'activation ReLU.
        /// </summary>
        private static float ReLU(float value, int idxRow, int idxColumn)
        {
            return Math.Max(0, value);
        }

        /// <summary>
        /// Dérivée de la fonction d'activation ReLU.
        /// </summary>
        private static float DerivedReLU(float value, int idxRow, int idxColumn)
        {
            float result = 0;
            if (value >= 0)
                result = 1;
            return result;
        }

        /// <summary>
        /// Fonction d'activation ELU.
        /// </summary>
        private static float ELU(float value, int idxRow, int idxColumn)
        {
            float result = 0;
            float alpha = 1;
            if (value >= 0)
            {
                result = value;
            }
            else
            {
                result = (float)(alpha * (Math.Exp(value) - 1));
            }
            return result;
        }

        /// <summary>
        /// Dérivée de la fonction d'activation ELU.
        /// </summary>
        private static float DerivedELU(float value, int idxRow, int idxColumn)
        {
            float result = 0;
            float alpha = 1;
            if (value >= 0)
            {
                result = 1;
            }
            else
            {
                result = ELU(value, idxRow, idxColumn) + alpha;
            }
            return result;
        }
    }
}
