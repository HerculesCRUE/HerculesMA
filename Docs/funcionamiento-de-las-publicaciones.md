![](./media/CabeceraDocumentosMD.png)

| Fecha         | 29/8/2022                                                   |
| ------------- | ------------------------------------------------------------ |
|Título|Funcionamiento de las publicaciones|
|Descripción|Guía de funcionamiento de las publicaciones|
|Versión|1.0|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Versión inicial|

# Funcionamiento de las Publicaciones

## Descripción
Las publicaciones son un recurso en la plataforma de Hércules que cumple la funcionalidad de los diferentes publicaciones del personal investigador disponibles en las universidades.

### Información ontológica
Los publicación hacen referencia al objeto de conocimiento de los Publicación (document.owl).

### Resumen funcional
Los publicación poseen un buscador público junto a una ficha de la misma


## Buscador general de las publicaciones
Existe un buscador general de las publicaciones

### Resumen funcional
El buscador de publicaciones contiene un listado facetado con un resumen de la información de las publicaciones, junto a un buscador por texto de los mismos. El buscador carga 10 resúmenes del publicación por defecto, con un enlace en el título a cada ficha del mismo, y mientras haces scroll, se irán haciendo peticiones al servicio de resultados para ir cargando el resto. 
Las facetas que se muestran están predefinidas y se irán mostrando en base al resultado de las búsqueda en sí disponible. Las facetas mostradas, como las opciones de las mismas y el número de resultados de las mismas, se muestran según haya resultados para la búsqueda actual.
Mientras se vayan añadiendo filtros de las propias facetas, se irán mostrando las facetas añadidas, filtrando el resultado de la búsqueda y ajustando las facetas a las opciones con resultados de las mismas.

### Datos del gestor
El buscador de las publicaciones es una página de 'búsqueda semántica' llamada 'Publicaciones'.

La vista personalizadas de la búsqueda de las publicaciones es la general para las búsquedas, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Buscador (general): https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/Busqueda/Index.cshtml
- Carga de resultados: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml
- Minificha: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml

Las vistas de personalización de las facetas son las siguentes:
- Faceta: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml
- Elemento de la facetas: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml


### Resumen del desarrollo

- **Búsqueda personalizada (Búsuqeda):** searcherPublications
- **RdfTypes:** document
- **Campo Filtro:** rdf:type=document|roh:isValidated=true



## Ficha del publicación
Se accede desde el listado de las publicaciones o desde el listado de los mismos.
### Resumen funcional
En esta pantalla nos encontramos con las funciones típicas de las fichas de los diferentes contenidos; nos encontramos con el título, la fecha de creación de la página, la descripción del mismo y las areas temáticas.
Respecto al menú de acciones, nos encontramos con los siguientes enlaces:


**Datos enriquecidos:**
- **Participantes**: Pestaña con una gráfica de las relaciones de los participantes elegidos para el publicación y un listado del personal investigador seleccionado indicando a qué perfil se ha añadido.
- **Publicaciones**: Gráfica que pone de relieve las publicaciones y las citas por año, segmentando los resultados por cuartiles, junto a una búsqueda facetada de las publicaciones. Las publicaciones mostradas son las publicaciones que se encuentran en el marco del publicación
- **Áreas temáticas**: Gráfica que pone de relieve los Áreas temáticas que se encuentran en la publicaciones científicas correspondientes al publicación actual.
- **Colaboradores externos**: Gráfica con el personal investigador, junto sus relaciones, y junto a un buscador los mismos, que han colaborado de forma externa en el publicación.
 

### Datos del gestor
La ficha de las publicaciones es un recurso, y la vista personalizada se encuentra en "https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Recursos/document.cshtml".
