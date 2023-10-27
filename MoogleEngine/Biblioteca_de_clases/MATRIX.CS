namespace MoogleEngine;
public class Matrix
{
    public static double[,] TF_IDF {get; set;}
    public static string[,] Palabras {get; set;}

    public static Dictionary<string, int>[] Posiciones_De_Palabras {get; set;}
    public Matrix()
    {
        TF_IDF = MatrizNumerica().Item1;
        Posiciones_De_Palabras = MatrizNumerica().Item2;
        Palabras = MatrizPalabras();
    }
     //matriz que sera utilizada para sacar el snipet segun la posicion que tengan las palabras
    //que eso lo sabre gracias al diccionario ayudaParaSnipet
     public static string[,] MatrizPalabras()
    {
        string[,] matrizPalabras = new string[Textos.documentos.Length, Lista.Cantidad_Max_Palabras_De_Textos];
        for (int i = 0; i < Textos.documentos.Length; i++)
        {
            string[] palabrasEnString = Textos.documentos[i].Split(' ');
            for (int j = 0; j < palabrasEnString.Length; j++)
            {
                matrizPalabras[i, j] = palabrasEnString[j];
            }
        }
        return matrizPalabras;
    }
     public static Tuple<double[,], Dictionary<string, int>[]> MatrizNumerica()
    {
        double[,] matriz = new double[Textos.documentos.Length, Lista.Palabras.Count];
        double[] idf = new double[Lista.Palabras.Count];
        //este es un diccionario que va a guardar las palabras y su posicion segun su primera aparicion
        //en cada texto
        Dictionary<string, int>[] PosicionDePalabrasEnDoc = new Dictionary<string, int>[Textos.documentos.Length];
        //ahora voy a crear un diccionario de recuento de palabras para cada documento
        Dictionary<string, int>[] frecuenciaBruta = new Dictionary<string, int>[Textos.documentos.Length];
        for (int i = 0; i < Textos.documentos.Length; i++)
        {   //hay que separar las palabras del doc 
            string[] palabrasDeDoc = Textos.documentos[i].Split(' ');
            //diccionario con las posiciones de cada palabra
            int posicion = 0;
            Dictionary<string, int> posiciones = new Dictionary<string, int>();
            //voy a crear un diccionario temporal que va a ir guardando la frecuencia bruta de las
            //palabras de los documentos
            Dictionary<string, int> diccionario = new Dictionary<string, int>();
            //aqui lo que voy a hacer es recorrer mi array de palabras y voy a ir anadiendo esas
            //palabras a mi diccionario como llaves y asignandole valor segun su aparicion
            foreach (string palabra in palabrasDeDoc)
            {
                if (diccionario.ContainsKey(palabra))
                {
                    diccionario[palabra]++;
                    posicion++;
                }
                else
                {
                    diccionario[palabra] = 1;
                    posiciones[palabra] = posicion;
                    posicion++;
                }
            }
            frecuenciaBruta[i] = diccionario;
            PosicionDePalabrasEnDoc[i] = posiciones;
        }
        //y ahora procedemos a calcular la matriz
        for (int i = 0; i < Textos.documentos.Length; i++)
        {
            //voy a crear un diccionario temporal que va a tener los mismos valores de frecuencia bruta
            //de las palabras
            Dictionary<string, int> diccionario = frecuenciaBruta[i];
            //aqui sumo todas las frecuencias brutas para obtener la cantidad de palabras del documento
            double cantPalabrasDoc = diccionario.Values.Sum();
            for (int j = 0; j < Lista.Palabras.Count; j++)
            {
                //ahora voy a ir viendo cada palabra de mi lista y asignadole a mi contador el valor que tiene
                //la frecuencia bruta de la palabra en el diccionario que le corresponde al documento y si no esta
                //la palabra en el documento se queda con valor cero 
                string palabra = Lista.Palabras[j];
                double contador = 0;
                if (diccionario.ContainsKey(palabra))
                {
                    contador = diccionario[palabra];
                }
                //y bueno la frecuencia bruta de la palabra de la lista en el documento entre la cantidad 
                //de palabras del documento es tf
                matriz[i, j] = contador / cantPalabrasDoc;
            }
        }
        //calcular idf que no es mas que la cantidad de documentos entre la aparicion de la palabra en
        //el conjunto de documentos
        for (int i = 0; i < matriz.GetLength(1); i++)
        {
            double aparicion = 0;
            for (int j = 0; j < matriz.GetLength(0); j++)
            {
                if (matriz[j, i] != 0)
                {
                    aparicion++;
                }
            }
            if (aparicion != 0)
                idf[i] = Math.Log(Textos.documentos.Length / aparicion);
        }
        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                matriz[i, j] = matriz[i, j] * idf[j];
            }
        }
        return new Tuple<double[,], Dictionary<string, int>[]>(matriz, PosicionDePalabrasEnDoc);
    }
}