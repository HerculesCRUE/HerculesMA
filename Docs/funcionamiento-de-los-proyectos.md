![](./media/CabeceraDocumentosMD.png)

| Fecha         | 29/8/2022                                                   |
| ------------- | ------------------------------------------------------------ |
|Título|Funcionamiento de los proyectos|
|Descripción|Guía de funcionamiento de los proyectos|
|Versión|1.0|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Versión inicial|

# Funcionamiento de los Proyectos

## Descripción
Los proyectos son un recurso en la plataforma de Hércules que cumple la funcionalidad de los diferentes proyectos disponibles en las universidades, y que son ejecutados por el personal investigador de las mismas. El objetivo de los mismos es la ejecución de tareas de investigación y desarrollo llevadas a cabo por las mismas universidades con o sin colaboración con otras entidades.

### Información ontológica
Los proyectos hacen referencia al objeto de conocimiento de los Proyectos (project.owl).

### Resumen funcional
Los publicación poseen un buscador público junto a una ficha de la misma


## Buscador general de los proyectos
Existe un buscador general de los proyectos

### Resumen funcional
El buscador de proyectos contiene un listado facetado con un resumen de la información de los proyectos, junto a un buscador por texto de los mismos. El buscador carga 10 resúmenes del proyecto por defecto, con un enlace en el título a cada ficha del mismo, y mientras haces scroll, se irán haciendo peticiones al servicio de resultados para ir cargando el resto. 
Las facetas que se muestran están predefinidas y se irán mostrando en base al resultado de las búsqueda en sí disponible. Las facetas mostradas, como las opciones de las mismas y el número de resultados de las mismas, se muestran según haya resultados para la búsqueda actual.
Mientras se vayan añadiendo filtros de las propias facetas, se irán mostrando las facetas añadidas, filtrando el resultado de la búsqueda y ajustando las facetas a las opciones con resultados de las mismas.

### Datos del gestor
El buscador de los proyectos es una página de 'búsqueda semántica' llamada 'Proyectos'.

La vista personalizadas de la búsqueda de los proyectos es la general para las búsquedas, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Buscador (general): [Index.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/Busqueda/Index.cshtml)
- Carga de resultados: [CargarResultados.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml)
- Minificha: [\_ResultadoRecurso.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml)

Las vistas de personalización de las facetas son las siguentes:
- Faceta: [\_Faceta.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml)
- Elemento de la facetas: [\_ItemFaceta.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml)

### Resumen del desarrollo

- **Búsqueda personalizada (Búsqueda):** searcherProjects
- **RdfTypes:** project
- **Campo Filtro:** rdf:type=project|roh:isValidated=true



## Ficha del proyecto
Se accede desde el listado de los proyectos o desde el listado de los mismos.
### Resumen funcional
En esta pantalla nos encontramos con las funciones típicas de las fichas de los diferentes contenidos; nos encontramos con el título, la fecha de creación de la página, la descripción del mismo y las areas temáticas.
Respecto al menú de acciones, nos encontramos con los siguientes enlaces:


**Datos enriquecidos:**
- **Participantes**: Pestaña con una gráfica de las relaciones de los participantes elegidos para el proyecto y un listado del personal investigador seleccionado indicando a qué perfil se ha añadido.
- **Publicaciones**: Gráfica que pone de relieve las publicaciones y las citas por año, segmentando los resultados por cuartiles, junto a una búsqueda facetada de las publicaciones. Las publicaciones mostradas son las publicaciones que se encuentran en el marco del proyecto
- **Áreas temáticas**: Gráfica que pone de relieve los Áreas temáticas que se encuentran en la publicaciones científicas correspondientes al proyecto actual.
- **Colaboradores externos**: Gráfica con el personal investigador, junto sus relaciones, y junto a un buscador los mismos, que han colaborado de forma externa en el proyecto.
 

### Datos del gestor
La ficha de los proyectos es un recurso, y la vista personalizada se encuentra en "[project.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Recursos/project.cshtml)".
