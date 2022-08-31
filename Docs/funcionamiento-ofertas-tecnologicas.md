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

## Descripción
Las ofertas tecnológicas son una serie de clusters de investigadores creados por las universidades para ofrecérselos a las diferentes empresas.

### Información ontológica
Las ofertas tecnológicas hacen referencia al objeto de conocimiento (offer.owl).
### Resumen funcional
Una oferta tecnológica puede ser creada por parte de investigadores y gestores OTRI (usuarios de ED).
Las ofertas tecnológicas dispondrán de un estado, el cual será importante destacar que el investigador no podrá validarlo, denegarlo, ni eliminar las ofertas (archivarlas), sino los gestores OTRI. Quienes podrán editar las ofertas tecnológicas serán los propios investigadores, sus IP y los gestores OTRI.
Los estados son los siguientes:
- Borrador
- Revisión
- Validada
- Denegada
- Archivada

## Listado general de las ofertas tecnológicas
Existe un listado general público de las ofertas tecnológicas, en el cual sólo aparecerán las ofertas tecnológicas validadas.
Este listado contará con un buscador y unas facetas para filtrar entre las ofertas.
También se podrá ordenar por fecha y por popularidad.

### Datos del Buscador
El listado de las ofertas tecnológicas es una página de busqueda llamada 'Ofertas Tecnológicas', no tiene vista propia pero tiene una sección en la vista de los resultados del buscador, donde se indica que se debe pintar el titulo de la publicacion, la fecha y la anotacion. La vista se encuentra en /Views/Views/CargadorResultados/_ResultadoRecurso.cshtml

- RdfType - offer
- Facetas:
	- Sectores de aplicación
	- Líneas de investigación
	- Área de procedencia
	- Estado de madurez
	- Grupos de investigación
	- Personal investigador de la oferta
	- Fecha de publicación
	- Descriptores específicos
	- Ámbito de proyecto
	- Tipo de proyecto
	- Áreas temáticas de las publicaciones
- Búsqueda personalizada - searcherOffers

## Listado de mis ofertas tecnológicas
Existe un listado simple de "Mis ofertas tecnológicas", para acceder a él hay que ir al menú lateral derecho > "Ofertas tecnológicas" > "Mis ofertas tecnológicas" 

Para acceder a la página de "Mis ofertas tecnológicas" hay que estar logueado y únicamente se mostrarán las ofertas tecnológicas que tú has creado.

### Resumen funcional
El listado de "Mis ofertas tecnológicas" mostrará el estado en el que se encuentra la oferta tecnológica.
En esta sección también se podrá filtrar por facetas o por búsqueda y se podrán realizar unas acciones sobre tus propias ofertas, ya sea enviar a borrador, editar o eliminar la oferta.

## Creación de la oferta
Consta de un 'STEP' de 5 pasos, en los cuales se irá avanzando para la configuración de dicha oferta. Por defecto, ya sea al crearla o editarla, el estado de la oferta será de 'Borrador'.

### Resumen funcional TODO cambiar
Los pasos para la creación (y edición) de ofertas tecnológicas serán los siguientes:
- **Step 1** (Definir oferta): Contiene los campos de 
	- Título de la oferta tecnológica
	- Descriptores específicos: Conjunto de términos libres útiles para definir la oferta, servirán como filtro para los investigadores del paso 2
	- Posibilidad de añadir el grupo / grupos entero a la oferta actual, dependerá de si el usuario es un investigador y no un gestor
- **Step 2** (Selección de investigadores): El paso actual consistirá en un buscador con los investigadores, para ser añadidos a oferta. El listado de usuarios se verá afectado por el conjunto de descriptores específicos añadido.
También se mostrarán los añadidos en una pestaña específica para mostrarlos separadamente y poder eliminarlos si fuera preciso.
- **Step 3** (Datos generales): En este paso se procederá a añadir más campos relacionados con contenido relevante para definir la oferta, los campos serán:
	- Líneas de investigación: Selección de las líneas de investigación obtenidas de los grupos de investigación de los investigadores disponibles en la oferta
	- Estado de Madurez (TRL): Listado con los siguientes elementos; 
		- En investigación (TRL 1-2)
		- Tecnología validada en laboratorio (TRL 3-5)
		- Tecnología demostrada con prototipo funcional (TRL 6-7)
		- Sistema completo disponible para cliente-mercado (TRL 8-9) 
	- Sector de Encuadre: Desplegable que contendrá los valores de los sectores a los que la entidad enfoca sus ofertas
- **Step 4** (Datos descriptivos): En este paso se incluirán campos te texto para dar más información respecto a las ofertas. Los campos serán los siguientes:
	- Descripción (Obligatorio): Texto descriptivo de la oferta, que será el más representativo de todos los datos descriptivos de la oferta. 
	- Aplicaciones (Obligatorio): Texto explicativo de qué aplicaciones tendría la solución aportada, qué problemas se resolverían 
	- Destinatarios (Obligatorio): Texto descriptivo de a quién le puede interesar la oferta: tipos de empresas... 
	- Origen: Texto descriptivo del origen de la Oferta Tecnológica 
	- Innovación: Texto explicativo de las innovaciones que aportaría la solución ofertada 
	- Tipo de socio buscado: Texto descriptivo del Tipo de Socio al que se orientaría la oferta 
	- Tipo de colaboración buscada: Texto descriptivo del Tipo de Colaboración que se espera surja de la oferta 
	- Observaciones: Texto para anotar otros datos relativos a la Oferta
- **Step 5** (Experiencia destacable): Se dispondrá en este apartado de 3 buscadores (y sus correspondientes apartados de "seleccionados") distintos de diferentes recursos pertenecientes al conjunto de investigadores disponibles en la oferta, sobre los que se podrá seleccionar qué elementos desean destacar para mostrarlos en dicha oferta. Los tipos de recursos serán los siguientes:
	- Proyectos
	- Publicaciones
	- Propiedad intelectual industrial (PII)
Una vez que se vayan seleccionando, aparecerán en sus correspondientes secciones de "seleccionados" para cada tipo de recurso.

### Resumen del desarrollo
La lógica javascript se encuentra en el archivo offer.js
Se realizan llamadas a los diferentes servicios para las diferentes funcionalidades, la lista de llamadas son las siguientes:
- Controlador para obtener los thesaurus usados por las ofertas:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]GetThesaurus
- Borra una oferta:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]BorrarOferta
- Cambiar el estado de una oferta:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]CambiarEstado
- Cambiar el estado de un listado de ofertas:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]CambiarEstadoAll
- Controlador para guardar los datos de la oferta:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadOffer
- Controlador para Obtener los usuarios del/los grupos de un investigador:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadUsers
- Controlador para Obtener las líneas de invetigación de los grupos de los usuarios investigadores dados:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]LoadLineResearchs
- Controlador para Obtener los sectores de encuadre:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadFramingSectors
- Controlador para Obtener los estados de madurez de las ofertas tecnológicas:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadMatureStates
- Controlador para crear/actualizar los datos de la oferta:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]SaveOffer
- Controlador para crear/actualizar los datos de la oferta:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]ModificarTripleteUsuario
- Controlador que lista el perfil de usuarios al que pertenece el usuario actual respecto a una oferta tecnológica dada:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]GetUserProfileInOffer

## Edición de la oferta
La edición de la oferta se realiza en la misma págína que la creación del misma, únicamente que se le pasa un parámetro indicando el id del misma.

Para acceder a la edición de la misma, se puede acceder desde la página del listado de las ofertas o desde la ficha de la misma.

### Resumen del desarrollo 
Toda la lógica de edición de las ofertas y la de la creación de las mismas es igual, únicamente se carga el contenido de las ofertas si se le pasa el id como parámetro, y se llama a la API para obtener los datos necesarios para precargar los datos. Las llamadas a la API que no se encuentran en la creación del mismo son las siguientes:
- Carga de la oferta:
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [GET]LoadOffer
- Guardado de la oferta (Igual que en la creación del cluster, pero con un parámetro más):
	- **API:** ServicioExterno
	- **Controlador:** OfertasController
	- **Función:** [POST]SaveOffer

### Datos del gestor
La ficha de la oferta equivale a 'Nueva Oferta Tecnológica'
La vista de creación de la oferta se encuentra 'Views/Views/CMS/Destacado/CreacionOferta.cshtml'. La vista de personalización en el gestor de GNOSS se encuentra en 'Plantillas de los componentes del CMS/Destacado/CreacionOferta'

## Permisos
Los permisos para la edición, creación y borrado de las ofertas requieren diferentes permisos.
- Creación: Únicamente un usuario logueado
- Edición: El usuario creador de la oferta o gestor OTRI
- Validado: Los gestores OTRI
- Borrado: El usuario creador de la oferta o gestor OTRI
- Listado: Todos los usuarios
- Ficha: Todos los usuarios