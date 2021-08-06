using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.Model;
using Gnoss.ApiWrapper.Helpers;
using GnossBase;
using Es.Riam.Gnoss.Web.MVC.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections;
using Gnoss.ApiWrapper.Exceptions;

namespace CurriculumvitaeOntology
{
	public class ScientificActivity : GnossOCBase
	{

		public ScientificActivity() : base() { } 

		public ScientificActivity(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Roh_worksSubmittedConferences = new List<RelatedDocuments>();
			SemanticPropertyModel propRoh_worksSubmittedConferences = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/worksSubmittedConferences");
			if(propRoh_worksSubmittedConferences != null && propRoh_worksSubmittedConferences.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_worksSubmittedConferences.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						RelatedDocuments roh_worksSubmittedConferences = new RelatedDocuments(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_worksSubmittedConferences.Add(roh_worksSubmittedConferences);
					}
				}
			}
			this.Roh_otherDisseminationActivities = new List<RelatedDocuments>();
			SemanticPropertyModel propRoh_otherDisseminationActivities = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/otherDisseminationActivities");
			if(propRoh_otherDisseminationActivities != null && propRoh_otherDisseminationActivities.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_otherDisseminationActivities.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						RelatedDocuments roh_otherDisseminationActivities = new RelatedDocuments(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_otherDisseminationActivities.Add(roh_otherDisseminationActivities);
					}
				}
			}
			this.Roh_scientificPublications = new List<RelatedDocuments>();
			SemanticPropertyModel propRoh_scientificPublications = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/scientificPublications");
			if(propRoh_scientificPublications != null && propRoh_scientificPublications.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_scientificPublications.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						RelatedDocuments roh_scientificPublications = new RelatedDocuments(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_scientificPublications.Add(roh_scientificPublications);
					}
				}
			}
			this.Roh_worksSubmittedSeminars = new List<RelatedDocuments>();
			SemanticPropertyModel propRoh_worksSubmittedSeminars = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/worksSubmittedSeminars");
			if(propRoh_worksSubmittedSeminars != null && propRoh_worksSubmittedSeminars.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_worksSubmittedSeminars.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						RelatedDocuments roh_worksSubmittedSeminars = new RelatedDocuments(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_worksSubmittedSeminars.Add(roh_worksSubmittedSeminars);
					}
				}
			}
		}

		public virtual string RdfType { get { return "http://w3id.org/roh/ScientificActivity"; } }
		public virtual string RdfsLabel { get { return "http://w3id.org/roh/ScientificActivity"; } }
		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Trabajos presentados en congresos")]
		[RDFProperty("http://w3id.org/roh/worksSubmittedConferences")]
		public  List<RelatedDocuments> Roh_worksSubmittedConferences { get; set;}

		[LABEL(LanguageEnum.es,"Otras actividades de divulgación")]
		[RDFProperty("http://w3id.org/roh/otherDisseminationActivities")]
		public  List<RelatedDocuments> Roh_otherDisseminationActivities { get; set;}

		[LABEL(LanguageEnum.es,"Publicaciones, documentos científicos y técnicos")]
		[RDFProperty("http://w3id.org/roh/scientificPublications")]
		public  List<RelatedDocuments> Roh_scientificPublications { get; set;}

		[LABEL(LanguageEnum.es,"Trabajos presentados en jornadas, seminarios, talleres de trabajo y/o cursos")]
		[RDFProperty("http://w3id.org/roh/worksSubmittedSeminars")]
		public  List<RelatedDocuments> Roh_worksSubmittedSeminars { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Roh_worksSubmittedConferences!=null){
				foreach(RelatedDocuments prop in Roh_worksSubmittedConferences){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRelatedDocuments = new OntologyEntity("http://w3id.org/roh/RelatedDocuments", "http://w3id.org/roh/RelatedDocuments", "roh:worksSubmittedConferences", prop.propList, prop.entList);
				entList.Add(entityRelatedDocuments);
				prop.Entity= entityRelatedDocuments;
				}
			}
			if(Roh_otherDisseminationActivities!=null){
				foreach(RelatedDocuments prop in Roh_otherDisseminationActivities){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRelatedDocuments = new OntologyEntity("http://w3id.org/roh/RelatedDocuments", "http://w3id.org/roh/RelatedDocuments", "roh:otherDisseminationActivities", prop.propList, prop.entList);
				entList.Add(entityRelatedDocuments);
				prop.Entity= entityRelatedDocuments;
				}
			}
			if(Roh_scientificPublications!=null){
				foreach(RelatedDocuments prop in Roh_scientificPublications){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRelatedDocuments = new OntologyEntity("http://w3id.org/roh/RelatedDocuments", "http://w3id.org/roh/RelatedDocuments", "roh:scientificPublications", prop.propList, prop.entList);
				entList.Add(entityRelatedDocuments);
				prop.Entity= entityRelatedDocuments;
				}
			}
			if(Roh_worksSubmittedSeminars!=null){
				foreach(RelatedDocuments prop in Roh_worksSubmittedSeminars){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRelatedDocuments = new OntologyEntity("http://w3id.org/roh/RelatedDocuments", "http://w3id.org/roh/RelatedDocuments", "roh:worksSubmittedSeminars", prop.propList, prop.entList);
				entList.Add(entityRelatedDocuments);
				prop.Entity= entityRelatedDocuments;
				}
			}
		} 




		protected List<object> ObtenerObjetosDePropiedad(object propiedad)
		{
			List<object> lista = new List<object>();
			if(propiedad is IList)
			{
				foreach (object item in (IList)propiedad)
				{
					lista.Add(item);
				}
			}
			else
			{
				lista.Add(propiedad);
			}
			return lista;
		}
		protected List<string> ObtenerStringDePropiedad(object propiedad)
		{
			List<string> lista = new List<string>();
			if (propiedad is IList)
			{
				foreach (string item in (IList)propiedad)
				{
					lista.Add(item);
				}
			}
			else if (propiedad is IDictionary)
			{
				foreach (object key in ((IDictionary)propiedad).Keys)
				{
					if (((IDictionary)propiedad)[key] is IList)
					{
						List<string> listaValores = (List<string>)((IDictionary)propiedad)[key];
						foreach(string valor in listaValores)
						{
							lista.Add(valor);
						}
					}
					else
					{
					lista.Add((string)((IDictionary)propiedad)[key]);
					}
				}
			}
			else if (propiedad is string)
			{
				lista.Add((string)propiedad);
			}
			return lista;
		}

		private string GenerarTextoSinSaltoDeLinea(string pTexto)
		{
			return pTexto.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"");
		}



		private void AgregarTripleALista(string pSujeto, string pPredicado, string pObjeto, List<string> pLista, string pDatosExtra)
		{
			if(!string.IsNullOrEmpty(pObjeto) && !pObjeto.Equals("\"\"") && !pObjeto.Equals("<>"))
			{
				pLista.Add($"<{pSujeto}> <{pPredicado}> {pObjeto}{pDatosExtra}");
			} 
		} 

		private void AgregarTags(List<string> pListaTriples)
		{
			foreach(string tag in tagList)
			{
				AgregarTripleALista($"http://gnoss/{ResourceID.ToString().ToUpper()}", "http://rdfs.org/sioc/types#Tag", tag.ToLower(), pListaTriples, " . ");
			}
		}


	}
}
