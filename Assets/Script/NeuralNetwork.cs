using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NeuralNetwork
{
    int inputSize;
    int outputSize;
    int hiddenSize;

    float[,] weights_1;
    float[,] weights_2;

    public NeuralNetwork(int i, int h, int o)
    {
        outputSize  = o;
        inputSize = i;
        hiddenSize = h;

        weights_1 = new float[i,h];
        for(int j = 0; j<i; j++)
        {
            for (int k = 0; k < h; k++)
            {
                weights_1[j,k] = Random.Range(-1f,1f);
            }
        }

        weights_2 = new float[h,o];
        for(int j = 0; j<h; j++)
        {
            for (int k = 0; k < o; k++)
            {
                weights_2[j,k] = Random.Range(-1f,1f);
            }
        }
    }

    public NeuralNetwork(float[] flattened, int i, int h, int o)
    {
        outputSize  = o;
        inputSize = i;
        hiddenSize = h;

        weights_1 = new float[i,h];
        for(int j = 0; j<i; j++)
        {
            for (int k = 0; k < h; k++)
            {
                weights_1[j,k] = flattened[j*h + k];
            }
        }

        weights_2 = new float[h,o];
        for(int j = 0; j<h; j++)
        {
            for (int k = 0; k < o; k++)
            {
                weights_2[j,k] = flattened[j*o + k];
            }
        }
    }

    public float[] Flatten()
    {
        List<float> flattened = new List<float>();

        for (int i = 0; i < inputSize; i++)
        {
            for (int j = 0; j < hiddenSize; j++)
            {
                flattened.Add(weights_1[i,j]);
            }
        }

        for (int i = 0; i < hiddenSize; i++)
        {
            for (int j = 0; j < outputSize; j++)
            {
                flattened.Add(weights_2[i,j]);
            }
        }

        return flattened.ToArray();
    }

    static float[,] MatrixProduct(float[,] A, float[,] B)
    {
       Debug.Assert(A.GetLength(1)==B.GetLength(0));

       float[,] result = new float[A.GetLength(0), B.GetLength(1)];
       for (int i = 0; i < A.GetLength(0); i++)
       {
            for (int j = 0; j < B.GetLength(1); j++)
            {
                result[i,j] = 0;

                for (int k = 0; k<A.GetLength(1); k++)
                {
                    result[i,j]+= A[i,k] * B[k, j];
                }
            }
       }
       return result;
    }

    public float[] Propagation(float[] input)
    {
        Debug.Assert(input.Length == inputSize);
        float[,] inputMatrix = new float[1,input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            inputMatrix[0,i] = input[i];
        }

        float[,] hiddenMatrix = MatrixProduct(inputMatrix, weights_1);
        hiddenMatrix = Activation(hiddenMatrix);
        float[,] outputMatrix = MatrixProduct(hiddenMatrix, weights_2);
        outputMatrix = Activation(outputMatrix);

        //format output
        float[] output = new float[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            output[i] = outputMatrix[0,i];
        }
        return output;
    }

    public float[,] Activation(float[,] matrix)
    {
        Func<float, float> sigmoid = x => 1f/(1+(float)Math.Exp(-x));

        float [,] activated = new float[1,matrix.GetLength(1)];
        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            activated[0, i] = sigmoid(matrix[0,i]);
        }
        return activated;
    }
}


