
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
namespace MoogleEngine;

public class Documentos
{
    public string[] archivos;
    public static string[] textos;
    public static List<string> palabrasUnicas;
    public static double[,] matriz;
    public Documentos()
    {   
        string rutaDeEjecucion = Directory.GetCurrentDirectory();
        string ruta = rutaDeEjecucion + "..\\..\\Content";
        archivos = Directory.GetFiles(ruta);
        textos = Documento(archivos);
        palabrasUnicas = Listilla();
        matriz = MatrizNumerica();
    }


    private static string[] Documento(string[] archivos)
    {
        string[] documentos = new string[archivos.Length];
        for (int i = 0; i < archivos.Length; i++)
        {
            documentos[i] = File.ReadAllText(archivos[i]);
        }
        string[] documentosArreglados = QuitarPuntuacionYTildes(documentos);
        return documentosArreglados;
    }
    private static string[] QuitarPuntuacionYTildes(string[] documentos)
    {
        string[] documentosArreglados = new string[documentos.Length];
        for (int i = 0; i < documentosArreglados.Length; i++)
        {
            documentosArreglados[i] = documentos[i].ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "");
        }
        return documentosArreglados;
    }
    //Esto es para crear mi diccionario de palabras, aqui tomo todas las palabras de todos los documentos
    private static List<string> Listilla()
    {
        HashSet<string> palabras = new HashSet<string>();
        for (int i = 0; i < textos.Length; i++)
        {
            string[] palabrasEnString = textos[i].Split(' ');
            for (int j = 0; j < palabrasEnString.Length; j++)
            {
                palabras.Add(palabrasEnString[j]);
            }
        }
        return palabras.ToList();
    }
    private static double[,] MatrizNumerica()
    {
        double[,] matriz = new double[textos.Length, palabrasUnicas.Count];
        double[] idf = new double[palabrasUnicas.Count];

        //ahora voy a crear un diccionario de recuento de palabras para cada documento
        Dictionary<string, int>[] frecuenciaBruta = new Dictionary<string, int>[textos.Length];
        for (int i = 0; i < textos.Length; i++)
        {   //hay que separar las palabras del doc 
            string[] palabrasDeDoc = textos[i].Split(' ');
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
                }
                else
                {
                    diccionario[palabra] = 1;
                }
            }
            frecuenciaBruta[i] = diccionario;
        }
        //y ahora procedemos a calcular la matriz
        for (int i = 0; i < textos.Length; i++)
        {
            //voy a crear un diccionario temporal que va a tener los mismos valores de frecuencia bruta
            //de las palabras
            Dictionary<string, int> diccionario = frecuenciaBruta[i];
            //aqui sumo todas las frecuencias brutas para obtener la cantidad de palabras del documento
            double cantPalabrasDoc = diccionario.Values.Sum();
            for (int j = 0; j < palabrasUnicas.Count; j++)
            {
                //ahora voy a ir viendo cada palabra de mi lista y asignadole a mi contador el valor que tiene
                //la frecuencia bruta de la palabra en el diccionario que le corresponde al documento y si no esta
                //la palabra en el documento se queda con valor cero 
                string palabra = palabrasUnicas[j];
                double contador = 0;
                if (diccionario.ContainsKey(palabra))
                {
                    contador = diccionario[palabra];
                }
                //y bueno la frecuencia bruta de la palabra de la lista en el documento entre la cantidad 
                //de palabras del documento es tf
                matriz[i, j] = (contador / cantPalabrasDoc);
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
                idf[i] = Math.Log((double)textos.Length / aparicion);
        }
        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                matriz[i, j] = matriz[i, j] * idf[j];
            }
        }
        return matriz;
    }
    public static double[] VectorQuery(string query)
    {
        double[] idf = new double[palabrasUnicas.Count];
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
            //la condicional mas importante de todo el trabajo para mi que cantidad de errores me dio xd
            //ya que este vector este hecho en funcion de la matriz analizando el tf-idf de columna en 
            //columna para ver la aparicion de la palabra en los documentos
            //pero el tf - idf de las conjunciones, prepociones y demas demas es cero
            //asi que se indefine el idf 
            //por eso ignoro completamente esas palabras para que tome el valor x defecto de un 
            //array numerico
            if (aparicion != 0)
                idf[i] = Math.Log((double)textos.Length / aparicion);
        }
        string[] vector = query.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "").Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Split(' ');
        double[] vectorNum = new double[palabrasUnicas.Count];
        double cantDePalabras = vector.Length;
        double contador = 0;
        for (int i = 0; i < palabrasUnicas.Count; i++)
        {
            for (int j = 0; j < vector.Length; j++)
            {
                if (palabrasUnicas[i] == vector[j])
                {
                    contador++;
                }
            }
            //aqui esta el tf del vectornumerico que es la cantidad de veces que se repiten las palabras entre la cantidad de palabras de la query
            vectorNum[i] = (contador / cantDePalabras) * idf[i];
            contador = 0;
        }
        return vectorNum;
    }
    public static double[] SimilitudDelCoseno(double[] vectornum)
    {
        //vamos a proceder a calcular el producto punto que no es mas que la suma de los
        //productos de los elementos correspondientes del vectorquery con los vectores documentos 
        //y los ire guardando en el array producto punto
        double[] productoPunto = new double[matriz.GetLength(0)];
        double sumatoriaVector = 0;
        double sumatoriaDocumento = 0;
        double[] magnitud = new double[matriz.GetLength(0)];
        int contador = 0;
        double vectorMagnitud = 0;
        double[] similitud = new double[matriz.GetLength(0)];
        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                productoPunto[i] += matriz[i, j] * vectornum[j];
                sumatoriaDocumento += Math.Pow(matriz[i, j], 2);
                if(contador == 0)
                {
                    sumatoriaVector += Math.Pow(vectornum[j], 2);
                    //magnitud del vector que sera multiplicada x las magnitudes de cada documento
                    vectorMagnitud = Math.Sqrt(sumatoriaVector);
                }
            }
            //ahora vamos a ver la magnitud de cada documento
            double docMagnitud = Math.Sqrt(sumatoriaDocumento);
            magnitud[i] = docMagnitud;
            //y aqui multiplico la magnitud del vector por la de cada documento
            magnitud[i] = magnitud[i] * vectorMagnitud;
            //ahora voy a aplicar la formula de la similitud del coseno que no es mas que la
            //division del producto punto del vector con el documento entre la magnitud del 
            //vector por la magnitud del documento
            //aqui igual hay que utilizar una constante en la division x si acaso la query o algun documento esta vacio
            //no de error ya que la division por cero no esta definida
            similitud[i] = productoPunto[i] / (magnitud[i] + 1);
            sumatoriaDocumento = 0;
            contador++;
        }
        return similitud;
    }
    //estos metoditos no eran necesarios podia user el sort y reverse pero bueno
    public static double[] Ordenar(double[] x)
    {
        int min = 0;
        for (int i = 0; i < x.Length; i++)
        {
            min = i;
            for (int j = i + 1; j < x.Length; j++)
            {
                if (x[j] > x[min])
                {
                    min = j;
                }
            }
            Intercambiar(x, i, min);
        }
        return x;
    }
    public static void Intercambiar(double[] x, int i1, int i2)
    {
        double temporal = x[i1];
        x[i1] = x[i2];
        x[i2] = temporal;
    }
    public static string Levenshtein(string query)
    {
        //separo las palabras de la query para hacerle el analisis a cada una 
        string[] palabrasDeLaQuery = query.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "").Split(' ');
        //este va a ser el array de string final que voy a imprimar
        string[] sugerencia = new string[palabrasDeLaQuery.Length];
        //esta va a ser mi sugerencia final que puede tomar varias palabras
        string final = "";
        //esto es para ver que si todas las palabras de la query aparecen en la lista
        //no se muestra sugerencia jeje tuviste buena ortografia :)
        bool[] NoMostrar = new bool[palabrasDeLaQuery.Length];
        for (int i = 0; i < palabrasDeLaQuery.Length; i++)
        {
            string palabra = palabrasDeLaQuery[i];
            int distanciaLevenshtein = palabra.Length;
            char[] comparacionQuery = new char[palabra.Length];
            for (int j = 0; j < palabra.Length; j++)
            {
                //luego cojo cada char de la palabra a analizar de la query y lo pongo en array
                comparacionQuery[j] = palabra[j];
            }
            //esto es para ver si la palabra aparece en la lista y no tener que crear una matriz
            //con cada palabra de la lista
            bool aparece = false;
            for (int j = 0; j < palabrasUnicas.Count; j++)
            {
                if (palabra == palabrasUnicas[j])
                {
                    aparece = true;
                }
            }
            //este if ahorra mucho procesamiento ya que si la palabra esta bien escrita no neceista realizar 
            //la construccion de matrices de todas las palabras de la lista en funcion de la palabra de la query 
            //que se esta analizando
            sugerencia[i] = palabra;
            if (aparece == false)
            {
                for (int j = 0; j < palabrasUnicas.Count; j++)
                {
                    //aqui voy a utilizar el metodo comparando las palabras de mi query con cada palabra de mi lista
                    //para encontrar la de mayor similitud
                    //por lo tanto tengo que dividir en char tambien cada palabra de mi lista
                    string sugerenciaDePalabra = palabrasUnicas[j];
                    char[] comparacionLista = new char[sugerenciaDePalabra.Length];
                    for (int k = 0; k < sugerenciaDePalabra.Length; k++)
                    {
                        comparacionLista[k] = sugerenciaDePalabra[k];
                    }
                    //luego creo una matriz que va a tener propiedades en dependencia de los valores en cuestion
                    //donde la primera fila y columna va a ser los valores sucesivos de cero hasta la longitud de cada palabra
                    int[,] matriz = new int[palabra.Length + 1, sugerenciaDePalabra.Length + 1];
                    int filas = matriz.GetLength(0);
                    int columnas = matriz.GetLength(1);
                    //las letras de las columnas son las letras de la palabra de la lista
                    //las letras de las filas son las letras de la palabra de la query
                    for (int k = 1; k <= 1; k++)
                    {
                        for (int l = 0; l < filas; l++)
                        {
                            matriz[l, 0] = l;
                        }
                    }
                    for (int k = 1; k < columnas; k++)
                    {
                        matriz[0, k] = k;
                    }
                    //aqui es donde se hace el calculo de la distancia de leveshtein
                    int diagonal = 1;
                    for (int k = 1; k < filas; k++)
                    {
                        for (int l = 1; l < columnas; l++)
                        {
                            if (comparacionQuery[k - 1] == comparacionLista[l - 1])
                            {
                                diagonal = 0;
                            }
                            matriz[k, l] = Math.Min(Math.Min(matriz[k - 1, l] + 1, matriz[k, l - 1] + 1), matriz[k - 1, l - 1] + diagonal);
                            diagonal = 1;
                        }
                    }
                    //aqui voy guardando en el array sugerencia las palabras segun el metodo que son mas similares entre la query y las palabras de la lista
                    if (matriz[filas - 1, columnas - 1] < distanciaLevenshtein)
                    {
                        distanciaLevenshtein = matriz[filas - 1, columnas - 1];
                        sugerencia[i] = palabrasUnicas[j];
                    }
                }
            }
            //aqui uno mi array sugerencia en un string para poder pasarle este metodo como parametro
            //al serchresult
            final = string.Join(" ", sugerencia);
            //aqui lleno mi array de bool en funcion de si aparece o no en la lista
            NoMostrar[i] = aparece;
        }
        for(int i = 0; i < NoMostrar.Length; i++)
        {
            if(NoMostrar[i] == false)
            {
                //esto lo retorno cuando hay al menos una palabra que no aparece en mi lista
                return final;
            }
        }
        // y esto es que aprobaste espanol
        return "";
    }
}