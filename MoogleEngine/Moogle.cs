namespace MoogleEngine;
public static class Moogle
{
    public static Documentos doc; 
    public static SearchResult Query(string query)
    {
        double[] similitud = Documentos.SimilitudDelCoseno(Documentos.VectorQuery(query)) ;
        string[] Todostitulos = Resultado(similitud);
        double[] Todoscores = Ordenar(similitud);
        string levenshtein = Documentos.Levenshtein(query);
        string[]parrafitosOrdenados = Parrafillos(doc,Todostitulos);
        string[] titulos = Mostrar(Todostitulos,Todoscores,parrafitosOrdenados).Item1;
        double[] scores =  Mostrar(Todostitulos,Todoscores,parrafitosOrdenados).Item2;
        string[] snipet =  Mostrar(Todostitulos,Todoscores,parrafitosOrdenados).Item3;
       
        SearchItem[] items = new SearchItem[titulos.Length];
        for(int i = 0; i < items.Length; i++)
        {
            items[i] = new SearchItem(titulos[i],snipet[i],((float)scores[i]));
        }
        return new SearchResult(items, levenshtein);
    }
    public static Tuple<string[],double[],string[]> Mostrar(string[] Todostitulos, double[] Todoscores, string[] parrafitosOrdenados)
    {
         int contador = 0;
        for(int i = 0; i < Todoscores.Length; i++)
        {
            if(Todoscores[i] != 0)
            {
                contador++;
            }
        }
        string[] titulos = new string[contador];
        double[] scores = new double[contador];
        string[] snipet = new string[contador];
        for(int i = 0; i < titulos.Length; i++)
        {
            titulos[i] = Todostitulos[i];
            scores[i] = Todoscores[i];
            snipet[i] = parrafitosOrdenados[i];
        }
        return new Tuple<string[], double[], string[]> (titulos,scores,snipet);
    }
    //Este metodo me va a dar los titulos de los txt ordenados segun su score
     public static string[] Resultado(double[] similitud)
    {   
        string[]archivos = doc.archivos;
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
    public static string[] Parrafillos(Documentos doc, string[] Todostitulos)
    {
      string[] parrafitos = new string[doc.archivos.Length];
      
      for(int i = 0; i < parrafitos.Length; i++)
      {
        string[] temporal = File.ReadAllText(doc.archivos[i]).Split(' ');
        string[] material = new string[20];
        if(temporal.Length < 20)
        {
            material = new string[temporal.Length];
            for(int j = 0; j < material.Length; j++)
            {
                material[j] = temporal[j]; 
            }
        }
        else
        {
            for(int j = 0; j < 18; j++)
            {
               material[j] = temporal[j];
            }
        }
       parrafitos[i] = string.Join(" ", material);
      }
        string[]parrafitosOrdenados = new string[doc.archivos.Length];
         for(int i = 0; i < parrafitosOrdenados.Length; i++)
        {
            for(int j = 0; j < parrafitosOrdenados.Length; j++)
            {
                if(Path.GetFileNameWithoutExtension(doc.archivos[j]) == Todostitulos[i])
                {
                    parrafitosOrdenados[i] = parrafitos[j];
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
