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

## OfertasController
La documentación funcional del creador de ofertas tecnológicas está en [Ofertas tecnológicas](https://confluence.um.es/confluence/pages/viewpage.action?pageId=468647949).

Los métodos de éste controlador tendrán la siguiente url:
> https://something.com/servicioexterno/Ofertas/[METODO]

Los métodos son los siguientes:

### [GET] BorrarOferta
Borra una oferta

*Parámetros:*
 - **pIdOfferID** *(string)*: Id de la oferta a borrar

*Devuelve:*
*Boolean* True o false si ha sido borrado.


### [POST] CambiarEstado
Cambia el estado de una oferta

*Parámetros:*
 - **pIdOfferId** *(string)*: Id de la oferta a modificar
 - **estado** *(string)*: Id del estado al que se quiere establecer
 - **estadoActual** *(string)*: Id del estado que tiene actualmente (Necesario para la modificación del mismo)
 - **pIdGnossUser** *(Guid)*: Id del usuario que modifica el estado, necesario para actualizar el historial

*Devuelve:*
*String* Id del nuevo estado.


### [GET] LoadOffer
Controlador para guardar los datos de la oferta

*Parámetros:*
 - **pIdOfferId** *(string)*: Id de la oferta a borrar
 
*Devuelve:*
*Object* Objeto "leible" de la oferta.


### [GET] LoadUsers
Controlador para Obtener los usuarios del/los grupos de un investigador

*Parámetros:*
 - **pIdUserId** *(string)*: Usuario investigador
 
*Devuelve:*
*Object* Diccionario con los datos necesarios para cada persona.


### [POST] LoadLineResearchs
Controlador para Obtener las líneas de invetigación de los grupos de los usuarios investigadores dados

*Parámetros:*
 - **pIdUsersId** *(string[])*: Usuarios investigadores
 
*Devuelve:*
*string[]* Listado de las líneas de investigación.



### [GET] LoadFramingSectors
Controlador para Obtener los sectores de encuadre

*Parámetros:*
 - **lang** *(string)*: Idioma a cargar
 
*Devuelve:*
*string[]* Listado de las líneas de investigación.




### [GET] LoadMatureStates
Controlador para Obtener los estados de madurez de las ofertas tecnológicas

*Parámetros:*
 - **lang** *(string)*: Idioma a cargar
 
*Devuelve:*
*string[]* Listado de las líneas de investigación.




### [POST] SaveOffer
Controlador para crear/actualizar los datos de la oferta

*Parámetros:*
 - **pIdGnossUser** *(string)*: Usuario de gnoss que realiza la acción
 - **oferta** *(Object)*: Objeto con la oferta tecnológica a crear/actualizar
 
*Devuelve:*
*string* Id de la oferta creada o modificada.




## HerculesController
Es el encargado principalmente de obtener los datos de las gráficas en las diferentes fichas, como puedan ser las fichas de los investigadores, los grupos de investigación, las publicaciones, etc...

## RedesUsuarioController
Controlador para obtener y modificar los datos de las fuentes de *ResearchObjects* (Otros objetos de investigación).

## SearchController
La documentación funcional del metabuscador está en [Hércules MA - Metabuscador](https://confluence.um.es/confluence/display/HERCULES/MA.+Metabuscador).

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
