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
			SemanticPropertyModel propRoh_nonCompetitiveProjects = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/nonCompetitiveProjects");
			if(propRoh_nonCompetitiveProjects != null && propRoh_nonCompetitiveProjects.PropertyValues.Count > 0)
			{
				this.Roh_nonCompetitiveProjects = new RelatedProjects(propRoh_nonCompetitiveProjects.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
		}

		public virtual string RdfType { get { return "http://w3id.org/roh/ScientificExperience"; } }
		public virtual string RdfsLabel { get { return "http://w3id.org/roh/ScientificExperience"; } }
		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Proyectos competitivos")]
		[RDFProperty("http://w3id.org/roh/competitiveProjects")]
		public  List<RelatedProjects> Roh_competitiveProjects { get; set;}

		[LABEL(LanguageEnum.es,"Proyectos no Competitivos")]
		[RDFProperty("http://w3id.org/roh/nonCompetitiveProjects")]
		public  RelatedProjects Roh_nonCompetitiveProjects { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			if(Roh_competitiveProjects!=null){
				foreach(RelatedProjects prop in Roh_competitiveProjects){
					prop.GetProperties();
					prop.GetEntities();
					OntologyEntity entityRelatedProjects = new OntologyEntity("http://w3id.org/roh/RelatedProjects", "http://w3id.org/roh/RelatedProjects", "roh:competitiveProjects", prop.propList, prop.entList);
				entList.Add(entityRelatedProjects);
				prop.Entity= entityRelatedProjects;
				}
			}
			Roh_nonCompetitiveProjects.GetProperties();
			Roh_nonCompetitiveProjects.GetEntities();
			OntologyEntity entityRoh_nonCompetitiveProjects = new OntologyEntity("http://w3id.org/roh/RelatedProjects", "http://w3id.org/roh/RelatedProjects", "roh:nonCompetitiveProjects", Roh_nonCompetitiveProjects.propList, Roh_nonCompetitiveProjects.entList);
			entList.Add(entityRoh_nonCompetitiveProjects);
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
