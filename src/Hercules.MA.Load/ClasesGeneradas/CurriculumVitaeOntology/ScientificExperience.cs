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
	public class ScientificExperience : GnossOCBase
	{

		public ScientificExperience() : base() { } 

		public ScientificExperience(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Roh_nonCompetitiveProjects = new List<RelatedProjects>();
			SemanticPropertyModel propRoh_nonCompetitiveProjects = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/nonCompetitiveProjects");
			if(propRoh_nonCompetitiveProjects != null && propRoh_nonCompetitiveProjects.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_nonCompetitiveProjects.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						RelatedProjects roh_nonCompetitiveProjects = new RelatedProjects(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_nonCompetitiveProjects.Add(roh_nonCompetitiveProjects);
					}
				}
			}
			this.Roh_competitiveProjects = new List<RelatedProjects>();
			SemanticPropertyModel propRoh_competitiveProjects = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/competitiveProjects");
			if(propRoh_competitiveProjects != null && propRoh_competitiveProjects.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_competitiveProjects.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						RelatedProjects roh_competitiveProjects = new RelatedProjects(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_competitiveProjects.Add(roh_competitiveProjects);
					}
				}
			}
			this.Foaf_name = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/name"));
		}

		public virtual string RdfType { get { return "http://w3id.org/roh/ScientificExperience"; } }
		public virtual string RdfsLabel { get { return "http://w3id.org/roh/ScientificExperience"; } }
		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Proyectos no Competitivos")]
		[RDFProperty("http://w3id.org/roh/nonCompetitiveProjects")]
		public  List<RelatedProjects> Roh_nonCompetitiveProjects { get; set;}

		[LABEL(LanguageEnum.es,"Proyectos competitivos")]
		[RDFProperty("http://w3id.org/roh/competitiveProjects")]
		public  List<RelatedProjects> Roh_competitiveProjects { get; set;}

		[LABEL(LanguageEnum.es,"Nombre")]
		[RDFProperty("http://xmlns.com/foaf/0.1/name")]
		public  string Foaf_name { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("foaf:name", this.Foaf_name));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Roh_nonCompetitiveProjects!=null){
				foreach(RelatedProjects prop in Roh_nonCompetitiveProjects){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRelatedProjects = new OntologyEntity("http://w3id.org/roh/RelatedProjects", "http://w3id.org/roh/RelatedProjects", "roh:nonCompetitiveProjects", prop.propList, prop.entList);
				entList.Add(entityRelatedProjects);
				prop.Entity= entityRelatedProjects;
				}
			}
			if(Roh_competitiveProjects!=null){
				foreach(RelatedProjects prop in Roh_competitiveProjects){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRelatedProjects = new OntologyEntity("http://w3id.org/roh/RelatedProjects", "http://w3id.org/roh/RelatedProjects", "roh:competitiveProjects", prop.propList, prop.entList);
				entList.Add(entityRelatedProjects);
				prop.Entity= entityRelatedProjects;
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
