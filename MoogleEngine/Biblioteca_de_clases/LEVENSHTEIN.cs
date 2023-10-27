namespace MoogleEngine;
public class Levenshtein
{
    public static string sugerencia(string query)
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
            for (int j = 0; j < Lista.Palabras.Count; j++)
            {
                if (palabra == Lista.Palabras[j] || palabra == "")
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
                for (int j = 0; j < Lista.Palabras.Count; j++)
                {
                    //aqui voy a utilizar el metodo comparando las palabras de mi query con cada palabra de mi lista
                    //para encontrar la de mayor similitud
                    //por lo tanto tengo que dividir en char tambien cada palabra de mi lista
                    string sugerenciaDePalabra = Lista.Palabras[j];
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
                        sugerencia[i] = Lista.Palabras[j];
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
}