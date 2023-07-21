namespace MoogleEngine;
public static class Moogle
{
    public static Documentos doc;
    public static SearchResult Query(string query)
    {
        //este for aqui es por si se les ocurre empezar a poner espacios x gusto no ponga la sugerencia
        //tuve que anadir una disyuncion en una condicional tambien del metodo levenshtein por eso
        string[] queryton = query.Split(' ');
        List<string> palabrasquery = new List<string>();
        for (int i = 0; i < queryton.Length; i++)
        {
            if (queryton[i] != "")
            {
                palabrasquery.Add(queryton[i]);
            }
        }
        double[] similitud = doc.SimilitudDelCoseno(query);
        string[] parrafitos = doc.Snipet(query);
        string[] Todostitulos = Resultado(similitud);
        double[] Todoscores = Ordenar(similitud);
        string levenshtein = Documentos.Levenshtein(query);
        string[] parrafitosOrdenados = Parrafillos(Todostitulos, parrafitos);
        string[] titulos = Mostrar(Todostitulos, Todoscores, parrafitosOrdenados).Item1;
        double[] scores = Mostrar(Todostitulos, Todoscores, parrafitosOrdenados).Item2;
        string[] snipet = Mostrar(Todostitulos, Todoscores, parrafitosOrdenados).Item3;
        if (palabrasquery.Count == 0)
        {
            return new SearchResult();
        }
        if (titulos.Length < 5)
        {
            SearchItem[] items = new SearchItem[titulos.Length];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new SearchItem(titulos[i], snipet[i], ((float)scores[i]));
            }
            return new SearchResult(items, levenshtein);
        }
        else
        {
            SearchItem[] items = new SearchItem[5];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new SearchItem(titulos[i], snipet[i], ((float)scores[i]));
            }
            return new SearchResult(items, levenshtein);
        }
    }
    public static Tuple<string[], double[], string[]> Mostrar(string[] Todostitulos, double[] Todoscores, string[] parrafitosOrdenados)
    {
        int contador = 0;
        for (int i = 0; i < Todoscores.Length; i++)
        {
            if (Todoscores[i] != 0)
            {
                contador++;
            }
        }
        string[] titulos = new string[contador];
        double[] scores = new double[contador];
        string[] snipet = new string[contador];
        for (int i = 0; i < titulos.Length; i++)
        {
            titulos[i] = Todostitulos[i];
            scores[i] = Todoscores[i];
            snipet[i] = parrafitosOrdenados[i];
        }
        return new Tuple<string[], double[], string[]>(titulos, scores, snipet);
    }
    //Este metodo me va a dar los titulos de los txt ordenados segun su score
    public static string[] Resultado(double[] similitud)
    {
        string[] archivos = doc.archivos;
        string[] candela = new string[similitud.Length];
        double[] temporal = new double[similitud.Length];
        for (int i = 0; i < similitud.Length; i++)
        {
            temporal[i] = similitud[i];
        }
        double[] ordenado = Ordenar(temporal);
        for (int i = 0; i < archivos.Length; i++)
        {
            for (int j = 0; j < archivos.Length; j++)
            {
                if (ordenado[i] == similitud[j])
                {
                    candela[i] = Path.GetFileNameWithoutExtension(archivos[j]);
                    break;
                }
            }
        }
        return candela;
    }
    //Aqui es donde voy a crear el snipet, voy a tomar solamente las 20 palabras iniciales de cada texto
    //si no llega a 20 palabras tomo la cantidad de palabras que tenga
    public static string[] Parrafillos(string[] Todostitulos, string[] parrafito)
    {
        string[] parrafitosOrdenados = new string[doc.archivos.Length];
        for (int i = 0; i < parrafitosOrdenados.Length; i++)
        {
            for (int j = 0; j < parrafitosOrdenados.Length; j++)
            {
                if (Path.GetFileNameWithoutExtension(doc.archivos[j]) == Todostitulos[i])
                {
                    parrafitosOrdenados[i] = parrafito[j];
                    break;
                }
            }
        }
        return parrafitosOrdenados;
    }
    //esto es para crear mi matriz y lista de palabras antes de ejecutar el programa
    public static void Iniciar()
    {
        doc = new Documentos();
    }
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
}
