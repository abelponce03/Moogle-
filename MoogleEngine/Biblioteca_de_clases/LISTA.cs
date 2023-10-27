namespace MoogleEngine;
public class Lista
{
    public static int Cantidad_Max_Palabras_De_Textos {get; set;}
    public static List<string> Palabras {get; set;}
    public static Dictionary<string, int> Ayuda_Para_Snipet {get; set;}
    public Lista()
    {
        Palabras = Listilla().Item1;
        Cantidad_Max_Palabras_De_Textos = Listilla().Item2;
        Ayuda_Para_Snipet = Listilla().Item3;
    }
    
    //Esto es para crear mi diccionario de palabras, aqui tomo todas las palabras de todos los documentos
    public static Tuple<List<string>, int, Dictionary<string, int>> Listilla()
    {   
        HashSet<string> palabras = new HashSet<string>();
        Dictionary<string, int> ayudaParaSnipet = new Dictionary<string, int>();
        int[] cantidadDePalabras = new int[Textos.documentos.Length];
        int max = 0;
        for (int i = 0; i < Textos.documentos.Length; i++)
        {
            string[] palabrasEnString = Textos.documentos[i].Split(' ');
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
        //este for es para saber la maxima cantidad de columnas que tendra mi matriz de palabras 
        for (int i = 0; i < cantidadDePalabras.Length; i++)
        {
            if (max < cantidadDePalabras[i])
            {
                max = cantidadDePalabras[i];
            }
        }
        return new Tuple<List<string>, int, Dictionary<string, int>>(Lista, max, ayudaParaSnipet);
    }
}