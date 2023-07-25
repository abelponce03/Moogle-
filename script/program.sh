#!/bin/bash

# Opción: run - Ejecutar la aplicación web
function run_web_app() {
    echo "Retrocediendo un directorio..."
    cd ..
    
    echo "Ejecutando la aplicación web..."
    # Comandos para iniciar tu aplicación web, por ejemplo:
    dotnet watch run --project MoogleServer
    # La ejecución de la aplicación web comenzará en el directorio anterior al actual.
}
# Variables
TEX_REPORT="informe.tex" # Nombre del archivo LaTeX del informe
TEX_SLIDES="document.tex" # Nombre del archivo LaTeX de las presentaciones
PDF_REPORT="${TEX_REPORT%.tex}.pdf" # Nombre del archivo PDF del informe generado
PDF_SLIDES="${TEX_SLIDES%.tex}.pdf" # Nombre del archivo PDF de las presentaciones generado
APP_PROJECT="Moogle-" # Nombre del proyecto de la aplicación web ASP.NET Core
VIEWER_CMD_DEFAULT="xdg-open" # Comando por defecto para visualizar archivos PDF

# Función para compilar un archivo LaTeX y generar el PDF
function compile_pdf() 
{
    latexmk -pdf "$1"
}

# Función para generar el PDF del informe
function generate_report_pdf()
{
    cd ..
    cd Informe
    echo "Compilando el informe LaTeX..."
    compile_pdf "$TEX_REPORT"
}

# Función para generar el PDF de las presentaciones
function generate_slides_pdf() 
{   cd ..
    cd Presentacion
    echo "Compilando las presentaciones LaTeX..."
    compile_pdf "$TEX_SLIDES"
}

# Opción: show_report - Visualizar el informe PDF
function show_report_pdf() 
{
    cd ..
    cd Informe
    if [ ! -f "$PDF_REPORT" ]; then
        echo "Generando el informe PDF..."
        generate_report_pdf
    fi

    echo "Mostrando el informe..."
    start "$PDF_REPORT"
}

# Opción: show_slides - Visualizar las presentaciones PDF
function show_slides_pdf() 
{
    cd..
    cd Presentacion
    if [ ! -f "$PDF_SLIDES" ]; then
        echo "Generando las presentaciones PDF..."
        generate_slides_pdf
    fi

    echo "Mostrando las presentaciones..."
    start "$PDF_SLIDES"
}

# Opción: clean - Eliminar archivos, excepto fotos y .tex
function clean_files_informe()
{
    cd ..
    cd Informe
    echo "Limpiando archivos..."
     
    # Extensiones válidas para imágenes
    image_extensions=("jpg" "jpeg" "png" "gif")

    # Obtener lista de archivos a borrar
    files_to_delete=()
    for file in *; do
        if [ -f "$file" ] && ! is_valid_extension "$file"; then
            files_to_delete+=("$file")
        fi
    done
    # Eliminar archivos
    if [ ${#files_to_delete[@]} -gt 0 ]; then
        echo "Archivos a borrar:"
        for file in "${files_to_delete[@]}"; do
            echo "$file"
            rm "$file"
        done
    else
        echo "No hay archivos para borrar."
    fi

    echo "Limpieza completada."
}

function clean_files_presentacion()
{
    cd ..
    cd Presentacion
    echo "Limpiando archivos..."
     
    # Extensiones válidas para imágenes
    image_extensions=("jpg" "jpeg" "png" "gif")

    # Obtener lista de archivos a borrar
    files_to_delete=()
    for file in *; do
        if [ -f "$file" ] && ! is_valid_extension "$file"; then
            files_to_delete+=("$file")
        fi
    done
    # Eliminar archivos
    if [ ${#files_to_delete[@]} -gt 0 ]; then
        echo "Archivos a borrar:"
        for file in "${files_to_delete[@]}"; do
            echo "$file"
            rm "$file"
        done
    else
        echo "No hay archivos para borrar."
    fi

    echo "Limpieza completada."
}

# Función para verificar si la extensión del archivo es válida
function is_valid_extension() {
    local file_extension=${1##*.}
    if [[ ${image_extensions[*]} =~ $file_extension ]] || [ "$file_extension" == "tex" ]; then
        return 0
    else
        return 1
    fi
}

# Opciones del script
case "$1" in
    "run")
        run_web_app
        ;;
    "report")
        generate_report_pdf
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
        clean_files_informe
        clean_files_presentacion
        ;;
    *)
        echo "Uso: $0 {run|report|slides|show_report|show_slides|clean}"
        exit 1
        ;;
esac