![](./media/CabeceraDocumentosMD.png)

| Fecha         | 29/8/2022                                                   |
| ------------- | ------------------------------------------------------------ |
|Título|Funcionamiento de los investigadores|
|Descripción|Guía de funcionamiento del buscador y ficha de los investigadores|
|Versión|1.0|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Versión inicial|

# Funcionamiento de los Investigadores

## Descripción
Los investigadores son un recurso en la plataforma de Hércules que cumple la funcionalidad de ser el personal investigador de las universidades y disponible en la plataforma de Hércules. El personal investigador también tiene la posibilidad de realizar acciones en la platagorma tanto de MA como de ED en calidad de usuario registrado. 

### Información ontológica
Los investigadores hacen referencia al objeto de conocimiento de los Investigadores (person.owl).

### Resumen funcional
Los investigadores poseen un buscador público junto a una ficha de la misma


## Buscador general de los investigadores
Existe un buscador general de los investigadores

### Resumen funcional
El buscador del personal investigador contiene un listado facetado con un resumen de la información de los investigadores, junto a un buscador por texto de los mismos. El buscador carga 10 resúmenes del investigador por defecto, con un enlace en el título a cada ficha del mismo, y mientras haces scroll, se irán haciendo peticiones al servicio de resultados para ir cargando el resto. 
Las facetas que se muestran están predefinidas y se irán mostrando en base al resultado de las búsqueda en sí disponible. Las facetas mostradas, como las opciones de las mismas y el número de resultados de las mismas, se muestran según haya resultados para la búsqueda actual.
Mientras se vayan añadiendo filtros de las propias facetas, se irán mostrando las facetas añadidas, filtrando el resultado de la búsqueda y ajustando las facetas a las opciones con resultados de las mismas.

### Datos del gestor
El buscador del personal investigador es una página de 'búsqueda semántica' llamada 'Personal investigador'.

La vista personalizadas de la búsqueda de los investigadores es la general para las búsquedas, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Buscador (general): [Index.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/Busqueda/Index.cshtml)
- Carga de resultados: [CargarResultados.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml)
- Minificha: [\_ResultadoRecurso.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml)

Las vistas de personalización de las facetas son las siguentes:
- Faceta: [\_Faceta.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml)
- Elemento de la facetas: [\_ItemFaceta.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml)


### Resumen del desarrollo

- **Búsqueda personalizada (Búsuqeda):** searcherPersons
- **RdfTypes:** person
- **Campo Filtro:** rdf:type=person|roh:isActive=true



## Ficha del investigador
Se accede desde el listado de los investigadores o desde el listado de los mismos, también se puede llegar desde los datos enriquecidos y otro recurso dentro de la plataforma de Hércules desde el que se le haga referencia.
### Resumen funcional
En esta pantalla nos encontramos con las funciones típicas de las fichas de los diferentes contenidos; nos encontramos con el título, la fecha de creación de la página, la descripción del mismo y las areas temáticas.
Respecto al menú de acciones, nos encontramos con los siguientes enlaces:


**Datos enriquecidos:**
- **Publicaciones**: Gráfica que pone de relieve las publicaciones y las citas por año, segmentando los resultados por cuartiles, junto a una búsqueda facetada de las publicaciones. Las publicaciones mostradas son las publicaciones que se encuentran en el marco del investigador
- **Proyectos**: Se muestran 3 gráficas. La primera indica el número de proyetos por año del investigador, indicando 2 columnas, una de los proyectos iniciados en un determinado año, y otra indicando los proyectos finalizados en los diferentes años. la segunda es una gráfica de los investigadores por proyecto, y la última indica el número de proyectos por el ámbito de los mismos. Todo junto a una búsqueda facetada de los proyectos. Los proyectos mostradas son aquellos que se encuentran en el marco del investigador.
- **Otros resultados**: Pestaña con una un listado de los research objects obtenidos y validados del investigador.
- **Áreas temáticas**: Gráfica que pone de relieve los Áreas temáticas que se encuentran en la publicaciones científicas correspondientes al investigador actual, junto a una gráfica de las relaciones entre las propias áreas temáticas.
- **Colaboradores**: Gráfica con el personal investigador, junto sus relaciones, y junto a un buscador los mismos, que han colaborado con el investigador.
- **Otros méritos**: Información que el investigador quiere destacar.
 

### Datos del gestor
La ficha de los investigadores es un recurso, y la vista personalizada se encuentra en "[person.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Recursos/person.cshtml)".
