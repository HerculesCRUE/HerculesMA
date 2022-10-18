![](./media/CabeceraDocumentosMD.png)

| Fecha         | 30/8/2022                                                   |
| ------------- | ------------------------------------------------------------ |
|Título|Funcionamiento de las ofertas tecnológicas|
|Descripción|Guía de funcionamiento de las ofertas tecnológicas|
|Versión|1.0|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Versión inicial|

# Funcionamiento de las ofertas tecnológicas

La mayoría del apartado de javascript se encuentra en https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Estilos/theme/offer.js


## Buscador general de las ofertas tecnológicas
Existe un buscador general de las ofertas tecnológicas

### Resumen funcional

Ver en https://confluence.um.es/confluence/pages/viewpage.action?pageId=563806294

### Datos del gestor
El buscador de las ofertas tecnológicas es una página del CMS 'búsqueda semántica' llamada 'Ofertas tecnológicas'.

La vista personalizadas de la búsqueda de las ofertas es la general para las búsquedas, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Buscador (general): https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/Busqueda/Index.cshtml
- Carga de resultados: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml
- Minificha: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml

Las vistas de personalización de las facetas son las siguentes:
- Faceta: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml
- Elemento de la facetas: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml

### Resumen del desarrollo

- **Búsqueda personalizada (Búsqueda):** searcherOffers
- **RdfTypes:** offer
- **Campo Filtro:** rdf:type=offer



## Listado de "mis ofertas tecnológicas"
Existe un buscador para listar las ofertas tecnológicas que el usuario ha creado, o un miembro de su grupo si el usuario es investigador principal del autor de la oferta

### Resumen funcional

Ver en https://confluence.um.es/confluence/pages/viewpage.action?pageId=563806294

### Datos del gestor
El buscador de mis ofertas tecnológicas es una página personalizada del CMS llamada 'Mis ofertas tecnológicas'.

La vista personalizadas para la búsqueda de mis ofertas es una página personalizada que muestra un buscador estándar, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Personalización del buscador (Página CMS): https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/CMS/Destacado/_Destacado_listado-mis-ofertas%24%24%24cc429e2d-69b5-4e2f-86f1-8e4589f07e5c.cshtml
- Carga de resultados: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml
- Minificha: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml

Las vistas de personalización de las facetas son las siguentes:
- Faceta: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml
- Elemento de la facetas: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml


### Resumen del desarrollo

- **Búsqueda personalizada (Búsqueda):** searchOwnOffers
- **RdfTypes:** offer
- **Campo Filtro:** rdf:type=offer



## Gestión de las ofertas tecnológicas (Usuarios OTRI)
Existe un buscador para listar las ofertas tecnológicas que el usuario ha creado, o un miembro de su grupo si el usuario es investigador principal del autor de la oferta

### Resumen funcional

Ver en https://confluence.um.es/confluence/pages/viewpage.action?pageId=563806294

### Datos del gestor
El buscador de "gestión de ofertas tecnológicas" es una página personalizada del CMS llamada 'Gestión de ofertas tecnológicas'.

La vista personalizadas para la búsqueda de gestión de las ofertas es una página personalizada que muestra un buscador estándar, así como la 'minificha' del propio buscador, siendo éstas las rutas:
- Personalización del buscador (Página CMS): https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/CMS/Destacado/_Destacado_gestor-ofertas%24%24%24fb9e53e9-879c-4680-9796-fe5543bd6be4.cshtml
- Carga de resultados: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/CargarResultados.cshtml
- Minificha: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorResultados/_ResultadoRecurso.cshtml

Las vistas de personalización de las facetas son las siguentes:
- Faceta: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_Faceta.cshtml
- Elemento de la facetas: https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Views/CargadorFacetas/_ItemFaceta.cshtml


### Resumen del desarrollo

- **Búsqueda personalizada (Búsqueda):** searchOffersOtri
- **RdfTypes:** offer
- **Campo Filtro:** rdf:type=offer




## Creación de la oferta
Consta de un 'STEP' de 5 pasos, en los cuales se irá avanzando para la configuración de dicha oferta. Por defecto, ya sea al crearla o editarla, el estado de la oferta será de 'Borrador'.

### Resumen funcional

Ver en https://confluence.um.es/confluence/pages/viewpage.action?pageId=563806277

### Resumen del desarrollo
La lógica javascript se encuentra en el archivo offer.js (https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Estilos/theme/offer.js)
Se realizan llamadas a los diferentes servicios para las diferentes funcionalidades, la lista de llamadas son las siguientes:
- Controlador para obtener los thesaurus usados por las ofertas:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]GetThesaurus
- Borra una oferta:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]BorrarOferta
- Cambiar el estado de una oferta:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]CambiarEstado
- Cambiar el estado de un listado de ofertas:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]CambiarEstadoAll
- Controlador para guardar los datos de la oferta:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadOffer
- Controlador para Obtener los usuarios del/los grupos de un investigador:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadUsers
- Controlador para Obtener las líneas de invetigación de los grupos de los usuarios investigadores dados:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]LoadLineResearchs
- Controlador para Obtener los sectores de encuadre:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadFramingSectors
- Controlador para Obtener los estados de madurez de las ofertas tecnológicas:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadMatureStates
- Controlador para crear/actualizar los datos de la oferta:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]SaveOffer
- Controlador para crear/actualizar los datos de la oferta:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]ModificarTripleteUsuario
- Controlador que lista el perfil de usuarios al que pertenece el usuario actual respecto a una oferta tecnológica dada:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]GetUserProfileInOffer

## Edición de la oferta
La edición de la oferta se realiza en la misma págína que la creación del misma, únicamente que se le pasa un parámetro indicando el id del misma.

Para acceder a la edición de la misma, se puede acceder desde la página del listado de las ofertas o desde la ficha de la misma.

### Resumen del desarrollo 
Toda la lógica de edición de las ofertas y la de la creación de las mismas es igual, únicamente se carga el contenido de las ofertas si se le pasa el id como parámetro, y se llama a la API para obtener los datos necesarios para precargar los datos. Las llamadas a la API que no se encuentran en la creación del mismo son las siguientes:
- Carga de la oferta:
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadOffer
- Guardado de la oferta (Igual que en la creación del cluster, pero con un parámetro más):
	- **API:** [ServicioExterno](https://github.com/HerculesCRUE/Commons-ED-MA/blob/main/src/Hercules.CommonsEDMA.ServicioExterno)
	- **Controlador:** OfertasController
	- **Función:** [POST]SaveOffer

### Datos del gestor
La ficha de la oferta equivale a 'Nueva Oferta Tecnológica'

## Ficha de la oferta tecnológica
Se accede desde el listado de las ofertas tecnológicas, tanto las públicas o las de gestión.


### Resumen funcional

Ver en https://confluence.um.es/confluence/pages/viewpage.action?pageId=563806339

### Datos del gestor
La ficha de las ofertas tecnológicas es un recurso, y la vista personalizada se encuentra en "https://github.com/HerculesCRUE/HerculesED/blob/main/Web/Views/Recursos/offer.cshtml".


## Permisos
Los permisos para la edición, creación y borrado de las ofertas requieren diferentes permisos.
- Creación: Únicamente un usuario logueado
- Edición: El usuario creador de la oferta o gestor OTRI
- Validado: Los gestores OTRI
- Borrado: El usuario creador de la oferta o gestor OTRI
- Listado: Todos los usuarios
- Ficha: Todos los usuarios

