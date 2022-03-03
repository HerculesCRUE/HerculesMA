![](../../Docs/media/CabeceraDocumentosMD.png)

| Fecha         | 1/3/2022                                                   |
| ------------- | ------------------------------------------------------------ |
|Título|Hércules MA. Servicio externo| 
|Descripción|Servicio Web con funciones propias de Hércules Métodos de Análisis|
|Versión|1|
|Módulo|Documentación|
|Tipo|Código|
|Cambios de la Versión|Versión inicial|

# Servicio Externo

Este servicio es un servicio web sobre el que se apoya la web de **Hércules Métodos de Análisis**, a la que realiza diferentes peticiones vía AJAX. 

El servicio externo se compone a su vez de 3 servicios o funcionalidades bien marcadas:


## HerculesController
Es el encargado principalmente de obtener los datos de las gráficas en las diferentes fichas, como puedan ser las fichas de los investigadores, los grupos de investigación, las publicaciones, etc...

## RedesUsuarioController
Controlador para obtener y modificar los datos de las fuentes de *ResearchObjects* (Otros objetos de investigación).

## SearchController
Este controlador se encarga de las peticiones para realizar una búsqueda de los datos almacenados en memoria de las entidades, o también llamado **metabuscador**:
 - Investigadores
 - Grupos de investigación
 - Proyectos
 - Publicaciones
 - Otros objetos de investigación

Un ejemplo de una petición sería:

> https://something.com/servicioexterno/Search/DoMetaSearch?stringSearch=skarmeta&lang=es

Usando un terminal:
   ```bash
curl -X 'GET' \
  'https://something.com/servicioexterno/Search/DoMetaSearch?stringSearch=skarmeta&lang=es' \
  -H 'accept: */*'
```

Donde los parámetros serían **stringSearch**, que sería la cadena de texto a buscar, y **lang** el idioma de búsqueda.

## Otras funcionalidades
También posee la funcionalidad de cargar en memoria periódicamente un listado con los objetos de las diferentes entidades buscables por el metabuscador. Este proceso se iniciará al iniciar el servicio y periódicamente se volverá a ejecutar cargando los nombres, descripciones, urls, tags y autores (según corresponda a cada tipo de entidad) necesarios para realizar la búsqueda en a través del SearchController.
