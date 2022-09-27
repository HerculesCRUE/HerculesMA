![](./media/CabeceraDocumentosMD.png)

| Fecha         | 29/8/2022                                                   |
| ------------- | ------------------------------------------------------------ |
|Título|Funcionamiento de los grupos|
|Descripción|Guía de funcionamiento de los grupos de investigación|
|Versión|1.0|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Versión inicial|

# Funcionamiento de los Grupos

## Descripción
Los grupos son un recurso en la plataforma de Hércules que cumple la funcionalidad de los diferentes grupos de investigación disponibles en las universidades. 

### Información ontológica
Los grupo hacen referencia al objeto de conocimiento de los Grupos (group.owl).

### Resumen funcional
Los publicación poseen un buscador público junto a una ficha de la misma


## Buscador general de los grupos
Existe un buscador general de los grupos

### Resumen funcional
El buscador de grupos contiene un listado facetado con un resumen de la información de los grupos, junto a un buscador por texto de los mismos. El buscador carga 10 resúmenes del grupo por defecto, con un enlace en el título a cada ficha del mismo, y mientras haces scroll, se irán haciendo peticiones al servicio de resultados para ir cargando el resto. 
Las facetas que se muestran están predefinidas y se irán mostrando en base al resultado de las búsqueda en sí disponible. Las facetas mostradas, como las opciones de las mismas y el número de resultados de las mismas, se muestran según haya resultados para la búsqueda actual.
Mientras se vayan añadiendo filtros de las propias facetas, se irán mostrando las facetas añadidas, filtrando el resultado de la búsqueda y ajustando las facetas a las opciones con resultados de las mismas.

### Datos del gestor
El buscador de los grupos es una página de 'búsqueda semántica' llamada 'Grupos de investigación'.

La vista personalizadas de la búsqueda de los grupos es la general para las búsquedas, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Buscador (general): https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/Busqueda/Index.cshtml
- Carga de resultados: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml
- Minificha: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml

Las vistas de personalización de las facetas son las siguentes:
- Faceta: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml
- Elemento de la facetas: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml


### Resumen del desarrollo

- **Búsqueda personalizada (Búsuqeda):** searcherPersons
- **RdfTypes:** group
- **Campo Filtro:** rdf:type=group|roh:isValidated=true



## Ficha del grupo
Se accede desde el listado de los grupos o desde el listado de los mismos.
### Resumen funcional
En esta pantalla nos encontramos con las funciones típicas de las fichas de los diferentes contenidos; nos encontramos con el título, la fecha de creación de la página, la descripción del mismo y las areas temáticas.
Respecto al menú de acciones, nos encontramos con los siguientes enlaces:


**Datos enriquecidos:**
- **Proyectos**: Se muestran 3 gráficas. La primera indica el número de proyetos por año del grupo, indicando 2 columnas, una de los proyectos iniciados en un determinado año, y otra indicando los proyectos finalizados en los diferentes años. la segunda es una gráfica de los grupos por proyecto, y la última indica el número de proyectos por el ámbito de los mismos. Todo junto a una búsqueda facetada de los proyectos. Los proyectos mostradas son aquellos que se encuentran en el marco del grupo.
- **Publicaciones**: Gráfica que pone de relieve las publicaciones y las citas por año, segmentando los resultados por cuartiles, junto a una búsqueda facetada de las publicaciones. Las publicaciones mostradas son las publicaciones que se encuentran en el marco del grupo
- **Miembros**: Pestaña con una un listado del personal investigador perteneciente al grupo, así como una gráfica del mismo.
- **Áreas temáticas**: Gráfica que pone de relieve los Áreas temáticas que se encuentran en la publicaciones científicas correspondientes al grupo actual, junto a una gráfica de las relaciones entre las propias áreas temáticas.
- **Colaboradores externos**: Gráfica con personal investigador, junto sus relaciones, y junto a un buscador los mismos, que han colaborado con el grupo de forma externa en los diferentes proyectos del grupo de investigación.
 

### Datos del gestor
La ficha de los grupos es un recurso, y la vista personalizada se encuentra en "https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Recursos/group.cshtml".
