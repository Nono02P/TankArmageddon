using System;
using System.Runtime.Serialization;

namespace IA
{
    public delegate float FunctionOnMatrix(float value, int idxRow, int idxColumn);

    [DataContract]
    public struct Matrix
    {
        public enum eDirection
        {
            Vertical,
            Horizontal,
        }

        public enum eRotation
        {
            Rotation90,
            Rotation180,
            Rotation270,
        }

        [DataMember]
        public int Rows { get; private set; }
        [DataMember]
        public int Columns { get; private set; }
        [DataMember]
        public float[] Data { get; private set; }

        /// <summary>
        /// Créé une matrice du nombre de ligne et de colonnes spécifié.
        /// </summary>
        /// <param name="pNbRows">Nombre de lignes.</param>
        /// <param name="pNbColumns">Nombre de colonnes.</param>
        public Matrix(int pNbRows, int pNbColumns)
        {
            Rows = pNbRows;
            Columns = pNbColumns;
            Data = new float[Rows * Columns];
        }

        /// <summary>
        /// Retourne une nouvelle matrice qui est le résultat de l'addition de deux matrices.
        /// </summary>
        /// <param name="pMatrix1"></param>
        /// <param name="pMatrix2"></param>
        /// <returns>Nouvelle matrice correspondant au résultat de l'opération.</returns>
        public static Matrix Add(Matrix pMatrix1, Matrix pMatrix2)
        {
            Matrix result = pMatrix1.Copy();
            result.Add(pMatrix2);
            return result;
        }

        /// <summary>
        /// Additionne à la matrice les valeurs de la matrice passée en paramètre.
        /// </summary>
        /// <param name="pMatrix">Matrice qui doit être ajoutée.</param>
        public void Add(Matrix pMatrix)
        {
            if (Rows == pMatrix.Rows && Columns == pMatrix.Columns)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        Data[i * Columns + j] += pMatrix.Data[i * pMatrix.Columns + j];
                    }
                }
            }
        }

        /// <summary>
        /// Additionne à la matrice les valeurs de la matrice passée en paramètre à l'emplacement spécifié en colonne/ligne.
        /// </summary>
        /// <param name="pMatrix">Matrice à additionner sur la portion de matrice.</param>
        /// <param name="pRow">Ligne à laquelle doit s'ajouter la matrice passée en paramètre.</param>
        /// <param name="pColumn">Colone à laquelle doit s'ajouter la matrice passée en paramètre.</param>
        public void Add(Matrix pMatrix, int pRow, int pColumn)
        {
            for (int i = 0; i < pMatrix.Rows; i++)
            {
                for (int j = 0; j < pMatrix.Columns; j++)
                {
                    if ((pRow + i) >= 0 && (pRow + i) < Rows
                        && (pColumn + j) >= 0 && (pColumn + j) < Columns)
                        Data[(pRow + i) * Columns + (pColumn + j)] += pMatrix.Data[i * pMatrix.Columns + j];
                }
            }
        }

        /// <summary>
        /// Additionne une valeur à chacune des valeurs de la matrice.
        /// </summary>
        /// <param name="pValue">Valeur à ajouter à la matrice.</param>
        public void Add(float pValue)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Data[i * Columns + j] += pValue;
                }
            }
        }

        /// <summary>
        /// Retourne une nouvelle matrice qui est le résultat de la soustraction de deux matrices.
        /// </summary>
        /// <param name="pMatrix1">Matrice soustraite.</param>
        /// <param name="pMatrix2">Matrice soustractrice.</param>
        /// <returns>Nouvelle matrice correspondant au résultat de l'opération.</returns>
        public static Matrix Subtract(Matrix pMatrix1, Matrix pMatrix2)
        {
            Matrix result = pMatrix1.Copy();
            result.Subtract(pMatrix2);
            return result;
        }

        /// <summary>
        /// Soustrait à la matrice les valeurs de la matrice passée en paramètre.
        /// </summary>
        /// <param name="pMatrix">Matrice soustractrice.</param>
        public void Subtract(Matrix pMatrix)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    float val = 0;
                    if (pMatrix.Rows > i && pMatrix.Columns > j)
                    {
                        val = pMatrix.Data[i * pMatrix.Columns + j];
                    }
                    Data[i * pMatrix.Columns + j] -= val;
                }
            }
        }

        /// <summary>
        /// Soustrait une valeur à chacune des valeurs de la matrice.
        /// </summary>
        /// <param name="pValue">Valeur à soustraire à la matrice.</param>
        public void Subtract(int pValue)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Data[i * Columns + j] -= pValue;
                }
            }
        }

        /// <summary>
        /// Multiplie deux matrices et renvoies le résultat dans une nouvelle matrice de la taille de m1.
        /// </summary>
        /// <param name="m1">Première matrice.</param>
        /// <param name="m2">Deuxième matrice.</param>
        /// <returns>Matrice de taille identique à m1 et résultante de la multiplication de m1 et m2 (valeur par valeur).</returns>
        public static Matrix Multiply(Matrix m1, Matrix m2)
        {
            Matrix result = m1.Copy();
            result.Multiply(m2);
            return result;
        }

        /// <summary>
        /// Multiplication de chaque valeurs à chaque coordonnées de la matrice (Produit d'Hadamard).
        /// </summary>
        /// <param name="pMatrix">Matrice qui va être multiplié (valeur par valeur).</param>
        public void Multiply(Matrix pMatrix)
        {
            if (Columns == pMatrix.Columns && Rows == pMatrix.Rows)
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        float val = 0;
                        if (pMatrix.Rows > i && pMatrix.Columns > j)
                        {
                            val = pMatrix.Data[i * pMatrix.Columns + j];
                        }
                        Data[i * Columns + j] *= val;
                    }
                }
            }
            else
            {
                throw new Exception("The Matrix size must match with the size of sent pMatrix.");
            }
        }

        /// <summary>
        /// Multiplie une valeur à chacune des valeurs de la matrice.
        /// </summary>
        /// <param name="pValue">Valeur à multiplier à la matrice.</param>
        public void Multiply(float pValue)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Data[i * Columns + j] *= pValue;
                }
            }
        }

        /// <summary>
        /// Division de chaque valeurs à chaque coordonnées de la matrice.
        /// </summary>
        /// <param name="pMatrix">Matrice qui va servir de diviseur (valeur par valeur).</param>
        public void Divide(Matrix pMatrix)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    float val = 0;
                    if (pMatrix.Rows > i && pMatrix.Columns > j)
                    {
                        val = pMatrix.Data[i * pMatrix.Columns + j];
                    }
                    Data[i * Columns + j] /= val;
                }
            }
        }

        /// <summary>
        /// Diviser chacune des valeurs de la matrice à par une valeur.
        /// </summary>
        /// <param name="pValue">Diviseur de la matrice.</param>
        public void Divide(float pValue)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Data[i * Columns + j] /= pValue;
                }
            }
        }

        /// <summary>
        /// Retourne une nouvelle matrice correspondant au produit matriciel des deux matrices.
        /// </summary>
        /// <param name="m1">Matrice qui doit avoir autant de colones que ce que m2 a de lignes.</param>
        /// <param name="m2">Matrice qui doit avoir autant de lignes que ce que m1 a de colones.</param>
        /// <returns>Nouvelle matrice correspondant au résultat de l'opération.</returns>
        public static Matrix DotProduct(Matrix m1, Matrix m2)
        {
            if (m1.Columns == m2.Rows)
            {
                Matrix matrix = new Matrix(m1.Rows, m2.Columns);
                for (int i = 0; i < m1.Rows; i++)
                {
                    for (int j = 0; j < m2.Columns; j++)
                    {
                        float sum = 0;
                        for (int k = 0; k < m1.Columns; k++)
                        {
                            sum += m1.Data[i * m1.Columns + k] * m2.Data[k * m2.Columns + j];
                        }
                        matrix.Data[i * matrix.Columns + j] = sum;
                    }
                }
                return matrix;
            }
            else
            {
                throw new Exception("Columns of the Matrix must match with Rows of sent pMatrix.");
            }
        }

        /// <summary>
        /// Crée une nouvelle matrice à partir d'un array.
        /// </summary>
        /// <param name="pArray">Array contenant les données de la matrice.</param>
        /// <param name="pWidth">Nombre de colones qui composera la matrice</param>
        /// <returns>Matrice crée à partir des données.</returns>
        public static Matrix FromArray(float[] pArray, int pWidth = 1)
        {
            Matrix m = new Matrix(pArray.Length / pWidth, pWidth);
            for (int i = 0; i < pArray.Length; i++)
            {
                m.Data[i] = pArray[i];
            }
            return m;
        }

        /// <summary>
        /// Rempli le Matrix avec des valeurs au hasard.
        /// </summary>
        public void Randomize(bool pAllowNegative = false)
        {
            Random rnd = new Random();
            Randomize(rnd, pAllowNegative);
        }

        /// <summary>
        /// Rempli le Matrix avec des valeurs au hasard à partir du Random passé en paramètre.
        /// </summary>
        public void Randomize(Random rnd, bool pAllowNegative = false)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (pAllowNegative)
                        Data[i * Columns + j] += (float)rnd.NextDouble() * 2 - 1;
                    else
                        Data[i * Columns + j] += (float)rnd.NextDouble();
                }
            }
        }

        /// <summary>
        /// Retourne une nouvelle matrice qui correspond à la transposition de la matrice passée en paramètre (inverse les lignes et les colones).
        /// </summary>
        /// <param name="pMatrix">Matrice contenant les données à transposer.</param>
        /// <returns>Matrice résultante de la transposition de la matrice passée en paramètre.</returns>
        public static Matrix Transpose(Matrix pMatrix)
        {
            Matrix result = pMatrix.Copy();
            result.Transpose();
            return result;
        }

        /// <summary>
        /// Transpose la matrice (inverse les lignes et les colones).
        /// </summary>
        public void Transpose()
        {
            Matrix result = new Matrix(Columns, Rows);
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Columns; j++)
                {
                    result.Data[i * result.Columns + j] = Data[j * Columns + i];
                }
            }
            Data = result.Data;
            Rows = result.Rows;
            Columns = result.Columns;
        }

        /// <summary>
        /// Retourne une nouvelle matrice correspondant au résultat de la réflection (symétrie) de la matrice passée en paramètre.
        /// </summary>
        /// <param name="pMatrix">Matrice contenant les données sur lesquels doivent être fait la symétrie.</param>
        /// <param name="direction">Type de symétrie.</param>
        /// <returns>Matrice résultante de la réflection (symétrie) de la matrice passée en paramètre.</returns>
        public static Matrix Reflection(Matrix pMatrix, eDirection direction)
        {
            Matrix result = pMatrix.Copy();
            result.Reflection(direction);
            return result;
        }

        /// <summary>
        /// Effectue une symétrie sur la matrice.
        /// </summary>
        /// <param name="direction">Sens de la symétrie à appliquer.</param>
        public void Reflection(eDirection direction)
        {
            float[] newData = new float[Rows * Columns];
            switch (direction)
            {
                case eDirection.Vertical:
                    for (int i = 0; i < Rows; i++)
                    {
                        for (int j = 0; j < Columns; j++)
                        {
                            newData[i * Columns + Columns - j - 1] = Data[i * Columns + j];
                        }
                    }
                    Data = newData;
                    break;
                case eDirection.Horizontal:
                    for (int i = 0; i < Rows; i++)
                    {
                        for (int j = 0; j < Columns; j++)
                        {
                            newData[(Rows - i - 1) * Columns + j] = Data[i * Columns + j];
                        }
                    }
                    Data = newData;
                    break;
                default:
                    throw new Exception("This case of reflection isn't supported.");
            }
        }

        /// <summary>
        /// Retourne une nouvelle matrice correspondant au résultat de la rotation de la matrice passée en paramètre.
        /// </summary>
        /// <param name="pMatrix">Matrice contenant les données qui doivent être pivotées.</param>
        /// <param name="rotation">Rotation à effectuer.</param>
        /// <returns>Matrice résultante de la rotation de la matrice passée en paramètre.</returns>
        public static Matrix Rotate(Matrix pMatrix, eRotation rotation)
        {
            Matrix result = pMatrix.Copy();
            result.Rotate(rotation);
            return result;
        }

        /// <summary>
        /// Effectue une rotation de la matrice.
        /// </summary>
        /// <param name="rotation">Rotation à effectuer.</param>
        public void Rotate(eRotation rotation)
        {
            switch (rotation)
            {
                case eRotation.Rotation90:
                    Transpose();
                    Reflection(eDirection.Horizontal);
                    break;
                case eRotation.Rotation180:
                    float[] newData = new float[Rows * Columns];
                    for (int i = 0; i < Data.Length; i++)
                    {
                        newData[Data.Length - i - 1] = Data[i];
                    }
                    Data = newData;
                    break;
                case eRotation.Rotation270:
                    Transpose();
                    Reflection(eDirection.Vertical);
                    break;
                default:
                    throw new Exception("This case of rotation isn't supported.");
            }
        }

        /// <summary>
        /// Retourne une nouvelle matrice correspond au résultat de l'exécution de la fonction donnée sur la matrice passée en paramètre.
        /// </summary>
        /// <param name="pMatrix">Matrice contenant les données qui doivent passer dans la fonction.</param>
        /// <param name="function">Fonction à appliquer sur la matrice.</param>
        /// <returns>Matrice correspondante au résultat de l'exécution de la fonction sur la matrice passée en paramètre.</returns>
        public static Matrix ExecuteOnMatrix(Matrix pMatrix, FunctionOnMatrix function)
        {
            Matrix result = pMatrix.Copy();
            result.ExecuteOnMatrix(function);
            return result;
        }

        /// <summary>
        /// Exécutes la fonction passée en paramètre sur chaque valeur à l'intérieur de la matrice.
        /// </summary>
        /// <param name="function">Fonction à appliquer sur la matrice.</param>
        public void ExecuteOnMatrix(FunctionOnMatrix function)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    float val = Data[i * Columns + j];
                    Data[i * Columns + j] = function(val, i, j);
                }
            }
        }

        /// <summary>
        /// Transforme une matrice en une matrice d'une dimension (mets toutes les valeurs à la suite).
        /// </summary>
        /// <param name="pMatrix">Matrix source, contenant les données.</param>
        /// <param name="direction">Sens du vecteur (Horizontal = 1 Ligne / Vertical = 1 Colonne).</param>
        /// <returns>Matrix d'une dimension contenant les données mises à plat de la matrice passée en paramètre.</returns>
        public static Matrix ToVector(Matrix pMatrix, eDirection direction)
        {
            Matrix result;
            switch (direction)
            {
                case eDirection.Vertical:
                    result = new Matrix(pMatrix.Data.Length, 1);
                    break;
                case eDirection.Horizontal:
                    result = new Matrix(1, pMatrix.Data.Length);
                    break;
                default:
                    throw new Exception("This case of direction isn't supported.");
            }
            pMatrix.Data.CopyTo(result.Data, 0);
            return result;
        }

        /// <summary>
        /// Renvoies une matrice de la taille donnée en paramètre, donnant contenant les valeurs de la matrice à l'emplacement colonne/ligne donné en paramètre.
        /// </summary>
        /// <param name="pWidth">Largeur de la matrice désirée.</param>
        /// <param name="pHeight">Hauteur de la matrice désirée.</param>
        /// <param name="pColumn">Coordonnée de la première colonne à extraire de la matrice.</param>
        /// <param name="pRow">Coordonnée de la première ligne à extraire de la matrice.</param>
        /// <returns>Retourne une matrice de la taille envoyée en paramèttre avec les données de la matrice source désigné à l'emplacement colonne/ligne</returns>
        public Matrix GetPad(int pWidth, int pHeight, int pColumn, int pRow)
        {
            Matrix result = new Matrix(pHeight, pWidth);
            int index = 0;
            for (int i = pRow; i < pRow + pHeight; i++)
            {
                for (int j = pColumn; j < pColumn + pWidth; j++)
                {
                    float val = 0;
                    if (i >= 0 && i < Rows && j >= 0 && j < Columns)
                    {
                        val = Data[i * Columns + j];
                    }
                    result.Data[index] = val;
                    index++;
                }
            }
            return result;
        }

        /// <summary>
        /// Effectue une copie de la matrice.
        /// </summary>
        /// <returns>Retourne la matrice copiée.</returns>
        public Matrix Copy()
        {
            Matrix result = new Matrix(Rows, Columns);
            Data.CopyTo(result.Data, 0);
            return result;
        }

        /// <summary>
        /// Renvoies une chaine de caractère contenant chacune des valeurs séparés par ' , ' pour les colones et d'un retour à la ligne pour les lignes.
        /// </summary>
        /// <returns>Chaine de caractère contenant les valeurs de la matrice.</returns>
        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result += Data[i * Columns + j].ToString() + " , ";
                }
                result.Substring(0, result.Length - 1);
                result += "\n";
            }
            return result;
        }
    }
}
