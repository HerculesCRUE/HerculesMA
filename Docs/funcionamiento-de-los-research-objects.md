![](./media/CabeceraDocumentosMD.png)

| Fecha         | 29/8/2022                                                   |
| ------------- | ------------------------------------------------------------ |
|Título|Funcionamiento de los research objects|
|Descripción|Guía de funcionamiento de los research objects|
|Versión|1.0|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Versión inicial|

# Funcionamiento de los Research Objects

## Descripción
Los research objects son un recurso en la plataforma de Hércules que cumple la funcionalidad de recursos externos de investigación, y que son importados desde las fuentes externas de los investigadores una vez que han conectado sus diferentes cuentas. Para la muestra pública de los mismos deben de ser elementos validados, ya sea porque la fuente se le considere como 'válida', o porque ha sido posteriormente validada por la universidad.

### Información ontológica
Los research object hacen referencia al objeto de conocimiento de los Research Object (researchobject.owl).

### Resumen funcional
Los publicación poseen un buscador público junto a una ficha de la misma


## Buscador general de los research objects
Existe un buscador general de los research objects

### Resumen funcional
El buscador de research objects contiene un listado facetado con un resumen de la información de los research objects, junto a un buscador por texto de los mismos. El buscador carga 10 resúmenes del research object por defecto, con un enlace en el título a cada ficha del mismo, y mientras haces scroll, se irán haciendo peticiones al servicio de resultados para ir cargando el resto. 
Las facetas que se muestran están predefinidas y se irán mostrando en base al resultado de las búsqueda en sí disponible. Las facetas mostradas, como las opciones de las mismas y el número de resultados de las mismas, se muestran según haya resultados para la búsqueda actual.
Mientras se vayan añadiendo filtros de las propias facetas, se irán mostrando las facetas añadidas, filtrando el resultado de la búsqueda y ajustando las facetas a las opciones con resultados de las mismas.

### Datos del gestor
El buscador de los research objects es una página de 'búsqueda semántica' llamada 'Research Objects'.

La vista personalizadas de la búsqueda de los research objects es la general para las búsquedas, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Buscador (general): https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/Busqueda/Index.cshtml
- Carga de resultados: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml
- Minificha: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml

Las vistas de personalización de las facetas son las siguentes:
- Faceta: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml
- Elemento de la facetas: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml


### Resumen del desarrollo

- **Búsqueda personalizada (Búsuqeda):** searcherPublications
- **RdfTypes:** researchobject
- **Campo Filtro:** rdf:type=researchobject|roh:isValidated=true



## Ficha del research object
Se accede desde el listado de los research objects o desde el listado de los mismos.

### Resumen funcional
En esta pantalla nos encontramos con las funciones típicas de las fichas de los diferentes contenidos; nos encontramos con el título, la fecha de creación de la página, la descripción del mismo y las areas temáticas.
Respecto al menú de acciones, nos encontramos con los siguientes enlaces:


**Datos enriquecidos:**
- **Información**: 
- **ROs relacionados**: 
- **Referencias**: 
- **Anotaciones**: 
- **ROs vinculados**: 
 

### Datos del gestor
La ficha de los research objects es un recurso, y la vista personalizada se encuentra en "https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Recursos/researchobject.cshtml".
