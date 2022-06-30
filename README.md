![](./Docs/media/CabeceraDocumentosMD.png)

# Portal Nacional Avanzado de Investigación. Hércules MA - Métodos de Análisis

Hércules MA - Métodos de Análisis constituye el Portal Nacional Avanzado de Investigación, con una serie de aplicaciones que permitirán la explotación y el análisis de los datos existentes en HERCULES.

El análisis funcional se puede consultar en [Análisis funcional. Portal Nacional Avanzado de Investigación. Hércules MA - Métodos de Análisis](https://confluence.um.es/confluence/pages/viewpage.action?pageId=397534292).

Los módulos del portal son:

- **Búsqueda de investigadores - Módulo Research Synergy Finder**. Permite que un usuario realice búsquedas de investigadores y su producción científica con la finalidad de facilitar la detección de posibles nichos de colaboración y alianzas estratégicas. Además de utilidades de búsqueda, el módulo proporciona un Asistente de Cluster (grupos de proyectos) y un gestor de ofertas tecnológicas.
- **Análisis de proyectos de investigación - Módulo de Gestión y Análisis de Proyectos**. Permite que el usuario sea capaz de analizar los resultados de los proyectos de investigación que se están llevando a cabo en un centro/estructura de investigación. 
- **Análisis de indicadores de investigación - Módulo de Catálogo de indicadores**. Se proveerá al usuario de indicadores de investigación e innovación que le permitan medir la investigación nacional a partir de la información disponible en la red HERCULES y en fuentes externas.

## Relación con otros repositorios

Existen 2 repositorios relacionados con Hércules ED:

- [Herramienta de CV. Hércules ED - Enriquecimiento de Datos](https://github.com/HerculesCRUE/HerculesED). Proporciona su funcionalidad en torno a un conjunto de aplicaciones entre las que destaca la herramienta de edición y gestión de CV, en la que se cargan datos obtenidos desde fuentes externas confiables de información de producción científica y datos provenientes de otros sistemas de la Universidad, particularmente [Hércules SGI](https://github.com/HerculesCRUE/SGI).
- [Commons-ED-MA](https://github.com/HerculesCRUE/Commons-ED-MA). Contiene componentes o diseños compartidos en los proyectos [Hércules ED]((https://github.com/HerculesCRUE/HerculesED)) y Hércules MA.

## Estructura del repositorio

Las carpetas del repositorio son:

- [Docs](./Docs). Contiene documentos del proyecto y recursos estáticos, como imágenes, que se usan en los documentos del repositorio.
- [Web](./Web). Contendrá la configuración de vistas y estilos que configuran la presentación y funcionamiento propios del proyecto.
- [src](/.src). Contiene los servicios web y back que proporcionan la funcionalidad propia del proyecto.

## Despliegue

La información de despliegue se puede consultar en [Kubernetes Helm Deploy](./Docs/kubernetes-helm-deploy.md).

La descripción de la arquitectura de Hércules MA se puede consultar en [Arquitectura de Hércules MA](https://confluence.um.es/confluence/pages/viewpage.action?pageId=421167229).

## Información de versión y compatibilidad

![](https://content.gnoss.ws/imagenes/proyectos/personalizacion/7e72bf14-28b9-4beb-82f8-e32a3b49d9d3/cms/logognossazulprincipal.png)

Los componentes de Hércules MA funcionan y son compatibles con la versión 5 de [GNOSS Semantic AI Platform Open Core](https://github.com/equipognoss/Gnoss.SemanticAIPlatform.OpenCORE) y con la [versión Enterprise de GNOSS Semantic AI Platform](https://www.gnoss.com/contacto).
