![](./media/CabeceraDocumentosMD.png)

| Fecha         | 29/8/2022                                                   |
| ------------- | ------------------------------------------------------------ |
|Título|Funcionamiento de los Cluster|
|Descripción|Guía de funcionamiento de los cluster|
|Versión|1.0|
|Módulo|Documentación|
|Tipo|Especificación|
|Cambios de la Versión|Versión inicial|

# Funcionamiento de los Cluster

## Descripción
Los clusters son conjuntos de investigadores asociados a perfiles de investigación que los diferentes usuarios de la web crean para poder guardar y disponer información sobre potenciales equipos de desarrollo.
### Información ontológica
Los cluster hacen referencia al objeto de conocimiento de los Cluster (cluster.owl).
### Resumen funcional
Los cluster no poseen un listado público, pero si una página para la creación de cluster, otra para la edición de los mismos y un listado de tus clusters.


## Listado general de los cluster
(No existe un listado general de los cluster, únicamente existe un listado de mis clusters)
- RdfType
- Url (y cómo llegar en la página)
- Facetas
- Búsqueda personalizada

## Listado de mis cluster
Existe un listado simple de "Mis clusters", para acceder a él hay que ir al menú lateral derecho > "Gestión de clusters" > "Mis clusters" 

Para acceder a la página de 'Mis clusters' hay que estar logueado y únicamente se mostrarán los clusters que tú has creado.
### Resumen funcional
El listado de clusters contiene un listado simple de los clusters y un listado de los investigadores por cada perfil de investigador. Así mismo dispone de un enlace a la ficha de los clusters de investigación y un botón de edición al mismo. El enlace de editar enlaza a la página de creación del cluster pasándole como parámetro el id del mismo.

### Datos del gestor
El listado de los clusters es una página del cms llamada 'Cluster'
La vista de creación del cluster se encuentra 'Views/Views/CMS/ConsultaSPARQL/Listado_Cluster.cshtml'. La vista de personalización en el gestor de GNOSS se encuentra en 'Plantillas de los componentes del CMS/Consulta SPARQL/Listado_Cluster'

## Creación del cluster
Para la creación del cluster, debes de estar loguedado como personal investigador y acceder al menú lateral derecho > "Gestión de clusters" > "Nuevo Cluster".
### Resumen funcional
La creación de un cluster consiste en un formaulario de varios pasos (o Stepper), y dispone de las siguientes funcionalidades:
- Step 1
- Step 2
- Step 3: Nos encontramos en la pantalla donde se eligen los investigadores que corresponden con los perfiles de investigación. En esta pantalla se realiza una llamada a 'buscadorPersonalizado' (ficharecurso.js), con las búsquedas personalizadas 'searchClusterMixto', 'searchClusterAjuste' y 'searchClusterVolumen' dependiendo del orden seleccionado

### Datos del gestor
La creación de los clusters es una página del cms llamada 'Nuevo Cluster'
La vista de creación del cluster se encuentra 'Views/Views/CMS/Destacado/CreacionCluster.cshtml'. La vista de personalización en el gestor de GNOSS se encuentra en 'Plantillas de los componentes del CMS/Destacado/CreacionCluster'

### Resumen del desarrollo
La lógica javascript se encuentra en el archivo cluster.js
Se realizan llamadas a los diferentes servicios para las diferentes funcionalidades, la lista de llamadas son las siguientes:
- Carga de taxonomías `Áreas temáticas`:
	- **API:** ServicioExterno
	- **Controlador:** ClusterController
	- **Función:** [GET]GetThesaurus
- Carga de los `Descriptores específicos`:
	 - **API:** ServicioExterno
	- **Controlador:** ClusterController
	 - **Función:** [GET]SearchTags
- Gráfica de las relaciones entre los investigadores:
	- **API:** ServicioExterno
	- **Controlador:** ClusterController
	- **Función:** [POST]DatosGraficaColaboradoresCluster
- Carga de los perfiles de investigación creados:
	- **API:** ServicioExterno
	- **Controlador:** ClusterController
	- **Función:** [GET]LoadSavedProfiles
- Carga de los perfiles de investigación y el porcentaje de acierto en cada usuario sugerido:
	- **API:** ServicioExterno
	- **Controlador:** ClusterController
	- **Función:** [POST]LoadProfiles
- Creación del cluster:
	- **API:** ServicioExterno
	- **Controlador:** ClusterController
	- **Función:** [POST]SaveCluster

## Edición del cluster
La edición del cluster se realiza en la misma págína que la creación del mismo, únicamente que se le pasa un parámetro indicando el id del mismo.

Para acceder a la edición del mimo, se puede acceder desde la página del listado de los clusters o desde la ficha del mismo.

### Resumen del desarrollo
Toda la lógica de edición del cluster y la de la creación del mismo es igual, únicamente se carga el contenido del cluster si se le pasa el id como parámetro, y se llama a la API para obtener los datos necesarios para precargar los datos. Las llamadas a la API que no se encuentran en la creación del mismo son las siguientes:

- Carga del cluster:
	- **API:** ServicioExterno
	- **Controlador:** ClusterController
	- **Función:** [GET]LoadCluster
- Guardado del cluster (Igual que en la creación del cluster, pero con un parámetro más):
	- **API:** ServicioExterno
	- **Controlador:** ClusterController
	- **Función:** [POST]SaveCluster

## Ficha del cluster
Se accede desde el listado de los clusters o desde la creación/edición de los mismos una vez que se han guardado los cambios.
### Resumen funcional
Resumen del contenido
Detalles relevantes
Acciones del menú a destacar

Datos enriquecidos:
- Participantes: Pestaña con...
- Temas de investigación:

### Resumen del desarrollo
Archivo:

### Datos del gestor
La ficha del cluster equivale  'Nuevo Cluster'
La vista de creación del cluster se encuentra 'Views/Views/CMS/Destacado/CreacionCluster.cshtml'. La vista de personalización en el gestor de GNOSS se encuentra en 'Plantillas de los componentes del CMS/Destacado/CreacionCluster'

## Permisos
Los permisos para la edición, creación y borrado de los clusters requieren ser un usuario registrado.
- Creación: Únicamente un usuario logueado
- Edición: El usuario creador del cluster
- Listado: El usuario creador de los clusters
- Ficha: El usuario creador del cluster


