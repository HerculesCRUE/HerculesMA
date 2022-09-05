![](./media/CabeceraDocumentosMD.png)

| Fecha                 | 05/09/2022                                |
| --------------------- | ---------------------------------------- |
| Título                | Configuración de páginas                 |
| Descripción           | Guía de funcionamiento de las páginas de búsqueda y CMS |
| Versión               | 1.0                                      |
| Módulo                | Documentación                            |
| Tipo                  | Especificación                           |
| Cambios de la Versión | Versión inicial                          |

# Funcionamiento de los Buscadores

 1.- [Introducción](#introducción)  
 2.- [CMS](#aa)  
   a.- [Configuración](#aa)  
 3.- [Páginas de búsqueda](#aa)  
   a.- [Componentes](#aa)  
   b.- [Configuración de la página](#aa)  
   c.- [Configuración de las facetas](#aa)  
   d.- [Configuración del parámetro de búsqueda](#aa)  

## Introducción
Los buscadores son un tipo de página que muestra un determinado sistema de interrogación.  
Estos buscadores están compuestos por el servicio de facetas y el servicio de resultados.  
//TODO imagen

## Gestionar Buscadores

### Pagina
- Nombre: Nombre de de la pagina, aparece como cabezera debajo de la caja de busqueda.
- Ruta: Direccion en la que se encontrara la pagina en la url (sin el domino).
- Filtro de orden: Lista con los filtros que aplicaran orden a los elementos de la pagina.
- 
### Aspecto de la minificha.
El comportamiento del buscador esta definido en las vistas /Views/Views/CargadorResultados/_ResultadoRecurso.cshtml y /Views/Views/CargadorResultados/CargarResultados.cshtml
En la vista _ResultadoRecurso.cshtml se define el aspecto que tendra la minificha, basandose en las propiedades del objeto de conocimiento.
### Objeto de conocimiento.
Los objetos de conocimiento definen la informacion que aparecera en la minificha.

Ajustes mas relevantes:
- Nombre: Nombre del objeto de conocimiento
- Namespace: Nombre de la ontologia 
- Lista de Propiedades: Las propiedades que se quiere que aparezcan en la ficha o usadas en las facetas.


## Facetas 
Las facetas son filtros que se aplican a los resultados. Aparecen a la izquierda de los resultados.
Las facetas estan definidas en Administración semántica > Facetas

Ajustes mas relevantes:
- Nombre de la faceta
- Tipo de faceta: Indica el tipo de informacion que representa la faceta (Texto, Fecha, Numero, Tesauro, Siglo, Texto Invariable)
- Objectos de conocimiento en los que va a aparecer: Aqui se indica el objeto de conocimiento del buscador

## Metabuscador
