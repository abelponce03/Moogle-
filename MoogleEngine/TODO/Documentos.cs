using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
namespace MoogleEngine;
//Falta hacer los operadores como tal el de negacion ya esta hecho xd ya que si escribes una palabra con un signo de
//exclamacion obviamente no va a salir en la lista de palabras xque la exclui entonces su tfidf va a ser de cero
public class Documentos
{
    public string[] archivos;
    public static string[] textos;
    public static List<string> palabrasUnicas;
    public Dictionary<string, int> ayudaParaSnipet;
    public static double[,] matriz;
    public static double[,] prueba;
    public static double[] magnitud;
    public static Dictionary<string, int>[] posicionesPalabras;
    public static int cantidadMaxPalabrasDeTextos;
    public static string[,] matrizDePalabras;
    public static Dictionary<int, string> [] Diccionario_snipet;
    public Documentos()
    {
        string rutaDeEjecucion = Directory.GetCurrentDirectory();
        string ruta = rutaDeEjecucion + "..\\..\\Content";
        archivos = Directory.GetFiles(ruta);
        textos = Documento(archivos);
        palabrasUnicas = Listilla().Item1;
        cantidadMaxPalabrasDeTextos = Listilla().Item2;
        ayudaParaSnipet = Listilla().Item3;
        matriz = MatrizNumerica().Item1;
        magnitud = Magnitud();
        //esta posicion es para encontrar mucho mas rapido la primera aparicion de la palabra 
        //de la query con mayor TFIDF para luego buscar una vecindad de ella y ponerla de snipet
        posicionesPalabras = MatrizNumerica().Item2;
        matrizDePalabras = MatrizPalabras();
        Diccionario_snipet = Diccionario_para_snipet(); 
        prueba = matriz;
    }
    private static string[] Documento(string[] archivos)
    {
        string[] documentos = new string[archivos.Length];
        for (int i = 0; i < archivos.Length; i++)
        {
            documentos[i] = QuitarPuntuacion(File.ReadAllText(archivos[i]));
        }
        return documentos;
    }
    private static string QuitarPuntuacion(string documentos)
    {
        string documentosArreglados = "";
        documentosArreglados = documentos.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "");
        return documentosArreglados;
    }
    //Esto es para crear mi diccionario de palabras, aqui tomo todas las palabras de todos los documentos
    public static Tuple<List<string>, int, Dictionary<string, int>> Listilla()
    {
        HashSet<string> palabras = new HashSet<string>();
        Dictionary<string, int> ayudaParaSnipet = new Dictionary<string, int>();
        int[] cantidadDePalabras = new int[textos.Length];
        int max = 0;
        for (int i = 0; i < textos.Length; i++)
        {
            string[] palabrasEnString = textos[i].Split(' ');
            cantidadDePalabras[i] = palabrasEnString.Length;
            //Aqui es donde se crea mi lista de palabras
            for (int j = 0; j < palabrasEnString.Length; j++)
            {
                if (palabrasEnString[j] != "")
                {
                    palabras.Add(palabrasEnString[j]);
                }
            }
        }
        List<string> Lista = palabras.ToList();
        int posicion = 0;
        //esto es para ayudarme en la creacion del snipet
        for (int i = 0; i < Lista.Count; i++)
        {
            ayudaParaSnipet[Lista[i]] = posicion;
            posicion++;
        }
        for (int i = 0; i < cantidadDePalabras.Length; i++)
        {
            if (max < cantidadDePalabras[i])
            {
                max = cantidadDePalabras[i];
            }
        }
        return new Tuple<List<string>, int, Dictionary<string, int>>(Lista, max, ayudaParaSnipet);
    }
    //matriz que sera utilizada para sacar el snipet segun la posicion que tengan las palabras
    //que eso lo sabre gracias al diccionario ayudaParaSnipet
    public static string[,] MatrizPalabras()
    {
        string[,] matrizPalabras = new string[textos.Length, cantidadMaxPalabrasDeTextos];
        for (int i = 0; i < textos.Length; i++)
        {
            string[] palabrasEnString = textos[i].Split(' ');
            for (int j = 0; j < palabrasEnString.Length; j++)
            {
                matrizPalabras[i, j] = palabrasEnString[j];
            }
        }
        return matrizPalabras;
    }
    private static Tuple<double[,], Dictionary<string, int>[]> MatrizNumerica()
    {
        double[,] matriz = new double[textos.Length, palabrasUnicas.Count];
        double[] idf = new double[palabrasUnicas.Count];
        //este es un diccionario que va a guardar las palabras y su posicion segun su primera aparicion
        //en cada texto
        Dictionary<string, int>[] PosicionDePalabrasEnDoc = new Dictionary<string, int>[textos.Length];
        //ahora voy a crear un diccionario de recuento de palabras para cada documento
        Dictionary<string, int>[] frecuenciaBruta = new Dictionary<string, int>[textos.Length];
        for (int i = 0; i < textos.Length; i++)
        {   //hay que separar las palabras del doc 
            string[] palabrasDeDoc = textos[i].Split(' ');
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
        return new Tuple<double[,], Dictionary<string, int>[]>(matriz, PosicionDePalabrasEnDoc);
    }
    public static double[] Magnitud()
    {
        double[] magnitud = new double[matriz.GetLength(0)];
        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            double sumatoriaDocumento = 0;
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                sumatoriaDocumento += Math.Pow(matriz[i, j], 2);
            }
            //ahora vamos a ver la magnitud de cada documento
            double docMagnitud = Math.Sqrt(sumatoriaDocumento);
            magnitud[i] = docMagnitud;
        }
        return magnitud;
    }
    public double[] SimilitudDelCoseno(string query)
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
            {
                idf[i] = Math.Log((double)textos.Length / aparicion);
            }
        }
        //esta parte aqui voy a poner un poco de trabajo con operadores
        //este primer array es temporal ya que va a tener operadores y todo
        string[] Temporal = query.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("?", "").Replace("(", "").Replace(")", "").Split(' ');
        //este es el tipo ya sin operadores
        string[] vector = new string[Temporal.Length];
        //aqui se guardan las palabras con operadores de la query para mas adelante ser utilizadas segun su funcion
        //en la parte del calculo
        string[] palabrasConOperadorImportancia = new string[Temporal.Length];
        string[] palabrasConOperadorAparicion = new string[Temporal.Length];
        for (int i = 0; i < Temporal.Length; i++)
        {
            string palabra = "";
            char[] union = new char[Temporal[i].Length];
            for (int j = 0; j < Temporal[i].Length; j++)
            {
                union[j] = Temporal[i][j];
                if (j == Temporal[i].Length - 1)
                {
                    if (union[0] == '*')
                    {
                        char[] sinOperador = new char[union.Length - 1];
                        for (int k = 0; k < union.Length - 1; k++)
                        {
                            sinOperador[k] = union[k + 1];
                        }
                        palabra = string.Join("", sinOperador);
                        palabrasConOperadorImportancia[i] = palabra;
                    }
                    else if (union[0] == '^')
                    {
                        char[] sinOperador = new char[union.Length - 1];
                        for (int k = 0; k < union.Length - 1; k++)
                        {
                            sinOperador[k] = union[k + 1];
                        }
                        palabra = string.Join("", sinOperador);
                        palabrasConOperadorAparicion[i] = palabra;
                    }
                    else
                    {
                        palabra = string.Join("", union);
                        palabrasConOperadorImportancia[i] = "";
                        palabrasConOperadorAparicion[i] = "";
                    }
                }
            }
            vector[i] = palabra;
        }
        double[] vectorNum = new double[palabrasUnicas.Count];
        //aqui realice un proceso similar al de la matriz para el tf idf del vector query
        Dictionary<string, int> diccionario = new Dictionary<string, int>();
        foreach (string palabra in vector)
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
        int cantDePalabras = diccionario.Values.Sum();
        for (int i = 0; i < palabrasUnicas.Count; i++)
        {
            string palabra = palabrasUnicas[i];
            double contador = 0;
            if (diccionario.ContainsKey(palabra))
            {
                contador = diccionario[palabra];
            }
            //esta parte es para trabajar con el operador de importancia a partir del array que tiene
            //las palabras que poseen el operador multiplicamos  x 2 su tfidf en el vector query
            for(int j = 0; j < Temporal.Length; j++)
                {
                    if(palabra == palabrasConOperadorImportancia[j])
                    {
                        vectorNum[i] = 2 * (contador / cantDePalabras) * idf[i];
                        int posicion = ayudaParaSnipet[palabra];
                        //en este for estoy aumentando el valor de tfidf en la columna correspondiente
                        //a la palabra con operador en la matriz
                        for(int k = 0; k < matriz.GetLength(0); k++)
                        {
                            //aqui esta pasando algo raro a pesar de crear una matriz copia
                            //que no se guardan los valores q tuvo despues de la ultima busqueda
                            //sino los de la matriz ya establecida 
                            //continua guardando los valores
                            //revisar para terminar con el operador
                            prueba[k,posicion] = 2 * prueba[k,posicion];
                        }
                    }
                    else
                    {
                        vectorNum[i] = (contador / cantDePalabras) * idf[i];
                    }
                }
        }
        //Ahora me parece que voy a tener que trabajar con la matriz tammbien para aumentarle el tfidf
        //a las palabras con el operador importancia

        //vamos a proceder a calcular el producto punto que no es mas que la suma de los
        //productos de los elementos correspondientes del vectorquery con los vectores documentos 
        //y los ire guardando en el array producto punto
        double[] productoPunto = new double[matriz.GetLength(0)];
        double sumatoriaVector = 0;
        int contador1 = 0;
        double vectorMagnitud = 0;
        double[] similitud = new double[matriz.GetLength(0)];
        for (int i = 0; i < matriz.GetLength(0); i++)
        {
            for (int j = 0; j < matriz.GetLength(1); j++)
            {
                productoPunto[i] += prueba[i, j] * vectorNum[j];
                if (contador1 == 0)
                {
                    sumatoriaVector += Math.Pow(vectorNum[j], 2);
                    //magnitud del vector que sera multiplicada x las magnitudes de cada documento
                    vectorMagnitud = Math.Sqrt(sumatoriaVector);
                }
            }
            //y aqui multiplico la magnitud del vector por la de cada documento
            magnitud[i] = magnitud[i] * vectorMagnitud;
            //ahora voy a aplicar la formula de la similitud del coseno que no es mas que la
            //division del producto punto del vector con el documento entre la magnitud del 
            //vector por la magnitud del documento
            //aqui igual hay que utilizar una constante en la division x si acaso la query o algun documento esta vacio
            //no de error ya que la division por cero no esta definida
            similitud[i] = productoPunto[i] / (magnitud[i] + 1);
            contador1 = 1;
        }
        prueba = matriz;
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
        string[] palabrasDeLaQuery = query.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "").Replace("*", "").Replace("^", "").Split(' ');
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
                if (palabra == palabrasUnicas[j] || palabra == "")
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
        for (int i = 0; i < NoMostrar.Length; i++)
        {
            if (NoMostrar[i] == false)
            {
                //esto lo retorno cuando hay al menos una palabra que no aparece en mi lista
                return final;
            }
        }
        // y esto es que aprobaste espanol
        return "";
    }
    public string[] Snipet(string query)
    {
        string[] snipet = new string[textos.Length];
        string[] palabraMaxTFIDF = new string[snipet.Length];
        string[] palabrasquery = query.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "").Replace("*", "").Replace("^", "").Split(' ');
        for (int i = 0; i < palabraMaxTFIDF.Length; i++)
        {
            double mayor = 0;
            palabraMaxTFIDF[i] = "";
            for (int j = 0; j < palabrasquery.Length; j++)
            {
                if (ayudaParaSnipet.ContainsKey(palabrasquery[j]))
                {
                    int posicion = ayudaParaSnipet[palabrasquery[j]];
                    double temporal = matriz[i, posicion];
                    if (mayor < temporal)
                    {
                        mayor = temporal;
                        palabraMaxTFIDF[i] = palabrasquery[j];
                    }
                }

            }
        }
        for (int i = 0; i < snipet.Length; i++)
        {
            snipet[i] = "";
            Dictionary<string, int> candela = posicionesPalabras[i];
            string[] parrafito = new string[82];
            if (candela.ContainsKey(palabraMaxTFIDF[i]) && palabraMaxTFIDF[i] != "")
            {
                int posicion = candela[palabraMaxTFIDF[i]];
                if (posicion >= 40 && (posicion + 40) <= candela.Count)
                {
                    int z = 0;
                    for (int j = posicion - 40; j < posicion + 40; j++)
                    {
                        parrafito[z] = matrizDePalabras[i, j];
                        z++;
                    }
                }
                else if (posicion < 80 && (posicion + 80) <= candela.Count)
                {
                    int z = 0;
                    for (int j = posicion; j < posicion + 80; j++)
                    {
                        parrafito[z] = matrizDePalabras[i, j];
                        z++;
                    }
                }
                else if (posicion >= 80 && (posicion + 80) > candela.Count)
                {
                    int z = 0;
                    for (int j = posicion - 78; j <= posicion; j++)
                    {
                        parrafito[z] = matrizDePalabras[i, j];
                        z++;
                    }
                }
                else if (posicion < 80 && (posicion + 80) > candela.Count)
                {
                    int z = 0;
                    int resta = candela.Count - posicion;
                    for (int j = posicion; j < resta; j++)
                    {
                        parrafito[z] = matrizDePalabras[i, j];
                        z++;
                    }
                }
                string unido = string.Join(" ", parrafito);
                snipet[i] = unido;
            }
        }
        return snipet;
    }
    public static Dictionary<int, string>[] Diccionario_para_snipet()
    {
        Dictionary<int, string> [] documentos = new Dictionary<int, string>[textos.Length];
        for(int i = 0; i < textos.Length; i++)
        {
            Dictionary<int, string> palabras = new Dictionary<int, string>();
            string[] palabras_separadas = textos[i].Split(' ');
            for(int j = 0; j < palabras_separadas.Length; j++)
            {
                palabras[j] = palabras_separadas[j]; 
            }
            documentos[i] = palabras;
        }
        return documentos;
    }
    public string[] Snipet_Mejorado (string query)
    {
        string[] snipet = new string[textos.Length];
        string[] palabrasquery = query.ToLower().Replace(",", "").Replace(".", "").Replace(";", "").Replace(":", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "").Replace("*", "").Replace("^", "").Split(' ');
        string[] aparecen = new string[palabrasquery.Length];
        for(int i = 0; i < palabrasquery.Length; i++)
        {
            
        }
        return null;  
    }
}