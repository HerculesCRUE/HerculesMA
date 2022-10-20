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
El buscador de publicaciones contiene un listado facetado con un resumen de la información de las publicaciones, junto a un buscador por texto de los mismos. El buscador carga 10 resúmenes de las publicaciones por defecto, con un enlace en el título a cada ficha del mismo, y mientras haces scroll, se irán haciendo peticiones al servicio de resultados para ir cargando el resto. 
Las facetas que se muestran están predefinidas y se irán mostrando en base al resultado de las búsqueda en sí disponible. Las facetas mostradas, como las opciones de las mismas y el número de resultados de las mismas, se muestran según haya resultados para la búsqueda actual.
Mientras se vayan añadiendo filtros de las propias facetas, se irán mostrando las facetas añadidas, filtrando el resultado de la búsqueda y ajustando las facetas a las opciones con resultados de las mismas.

### Datos del gestor
El buscador de las publicaciones es una página de 'búsqueda semántica' llamada 'Publicaciones'.

La vista personalizadas de la búsqueda de las publicaciones es la general para las búsquedas, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Buscador (general): [Index.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/Busqueda/Index.cshtml)
- Carga de resultados: [CargarResultados.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml)
- Minificha: [\_ResultadoRecurso.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml)

Las vistas de personalización de las facetas son las siguentes:
- Faceta: [\_Faceta.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml)
- Elemento de la facetas: [\_ItemFaceta.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml)


### Resumen del desarrollo

- **Búsqueda personalizada (Búsuqeda):** searcherPublications
- **RdfTypes:** document
- **Campo Filtro:** rdf:type=document|roh:isValidated=true



## Ficha de la publicación
Se accede desde el listado de las publicaciones o desde el listado de los mismos.
### Resumen funcional
En esta pantalla nos encontramos con las funciones típicas de las fichas de los diferentes contenidos; nos encontramos con el título, la fecha de creación de la página, la descripción del mismo y las areas temáticas.
Respecto al menú de acciones, nos encontramos con los siguientes enlaces:


**Datos enriquecidos:**
- **Relacionados**: Pestaña con un listado de publicaciones relacionadas.
- **Referencias**: Enlace a publicaciones externas que han hecho referencias a la publicación.
- **Mis notas**: Pestaña que muestra las anotaciones que el usuario que se encuentra 'logueado' actualmente en el portal ha realizado sobre esa publicación.
- **ROs vinculados**: Muestra las relaciones que haya podido hacer desde esta publicación sobre otro RO (Research Objects y publicaciones).
 

### Datos del gestor
La ficha de las publicaciones es un recurso, y la vista personalizada se encuentra en "[document.cshtml](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/Web/Views/Recursos/document.cshtml)".
