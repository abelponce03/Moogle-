#! /bin/bash
# variables
Tex_Report="informe.tex" #Nombre del archivo latex del informe
Tex_Slides="document.tex" #Nombre del archivo latex de las presentaciones
PDF_Report="${Tex_Report%TEX}.pdf" #Nombre del archivo pdf del informe generado
PDF_Slides="${Tex_Slides%TEX}.pdf" #Nombre del archivo pdf de la presentacion generado
APP_Project="Moogle-" #Nombre del proyecto de la aplicacion web
VIEWER_CMD_DEFAULT="xdg-open" #Comando por defecto para visualizar archivos pdf

echo "se ha ejecutado el programa"
#Funcion para compilar un archivo latex y generar el pdf
function compile_pdf()
{
    latexmk -pdf "$1"
}
#Opcion: RUN - Ejecutar el proyecto web en c#
function run_proyect()
{
  #Comandos para compilar y ejecutar el proyecto web
  echo "Compilando y ejecutando el proyecto web"
  cd Moogle-
  dotnet watch run --project MoogleServer 
}
#Opcion: REPORT - Compilar y generar el pdf del informe
function generate__report_pdf()
{
    echo "Compilando el informe LaTex"
    compile_pdf"$Tex_Report"
}
#Opcion: slides - Compilar y generar el pdf de la presentacion
function generate_slides_pdf()
{
    echo "Compilando la presentacion LaTex"
    compile_pdf"$Tex_Slides"
}
#Opcion: show_report - Visualizar el informe PDF function show_report_pdf()
{
    if [ ! -f "$PDF_Report" ]; then echo "Generando el informe PDF"
    generate__report_pdf
    fi 
    echo "Mostrando el informe"
    "$VIEWER_CMD"
    "$PDF_Report"
}
#Opcion: show_slides - Visualizar la presentacion PDF
function show_slides_pdf()
{
    if [ ! -f "$PDF_Slides" ]; then echo "Generando la presentacion pdf"
    generate_slides_pdf
    fi
    echo "Mostrando la presentacion"
    "$VIEWER_CMD"
    "$PDF_Slides"   
}
#Opcion: clean - Eliminar ficheros auxiliares generados en la compilacion
function clean_proyect () 
{
    echo "Limpiando ficheros auxiliares"
    latexmk -c
    rm -f*.snm*.nav
    cd "Moogle-"
    dotnet clean
}
#Opciones del script
case "$1" in 
"run")
run_proyect
;;
"report")
generate__report_pdf
;;
"slides")
generate_slides_pdf
;;
"show_report")
VIEWER_CMD=${2:-$VIEWER_CMD_DEFAULT}
show_report_pdf
;;
"show_slides")
VIEWER_CMD=${2:-$VIEWER_CMD_DEFAULT}
show_slides_pdf
;;
"clean")
clean_proyect
;;
*)
echo "uso: $0 {run|report|slides|show_report|show_slides|clean}"
exit 1
;;
esac