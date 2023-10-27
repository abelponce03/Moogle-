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
public class Textos
{
    public static string[] archivos;
    public static string[] documentos;
    public Textos()
    {
        string rutaDeEjecucion = Directory.GetCurrentDirectory();
        string ruta = rutaDeEjecucion + "..\\..\\Content";
        archivos = Directory.GetFiles(ruta);
        documentos = Documentos(archivos);
    }
    private static string[] Documentos(string[] archivos)
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
}