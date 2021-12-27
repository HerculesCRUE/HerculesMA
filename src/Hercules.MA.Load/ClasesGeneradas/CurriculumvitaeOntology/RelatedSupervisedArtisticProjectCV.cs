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
using System.Diagnostics.CodeAnalysis;
using ContributionGradeProject = ContributiongradeprojectOntology.ContributionGradeProject;

namespace CurriculumvitaeOntology
{
	[ExcludeFromCodeCoverage]
	public class RelatedSupervisedArtisticProjectCV : GnossOCBase
	{

		public RelatedSupervisedArtisticProjectCV() : base() { } 

		public RelatedSupervisedArtisticProjectCV(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propRoh_contributionGradeProject = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/contributionGradeProject");
			if(propRoh_contributionGradeProject != null && propRoh_contributionGradeProject.PropertyValues.Count > 0)
			{
				this.Roh_contributionGradeProject = new ContributionGradeProject(propRoh_contributionGradeProject.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_curator= GetBooleanPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/curator"));
			this.Roh_contributionGradeProjectOther = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/contributionGradeProjectOther"));
		}

		public virtual string RdfType { get { return "http://w3id.org/roh/RelatedSupervisedArtisticProjectCV"; } }
		public virtual string RdfsLabel { get { return "http://w3id.org/roh/RelatedSupervisedArtisticProjectCV"; } }
		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"http://w3id.org/roh/contributionGradeProject")]
		[RDFProperty("http://w3id.org/roh/contributionGradeProject")]
		public  ContributionGradeProject Roh_contributionGradeProject  { get; set;} 
		public string IdRoh_contributionGradeProject  { get; set;} 

		[LABEL(LanguageEnum.es,"http://w3id.org/roh/curator")]
		[RDFProperty("http://w3id.org/roh/curator")]
		public  bool Roh_curator { get; set;}

		[LABEL(LanguageEnum.es,"http://w3id.org/roh/contributionGradeProjectOther")]
		[RDFProperty("http://w3id.org/roh/contributionGradeProjectOther")]
		public  string Roh_contributionGradeProjectOther { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("roh:contributionGradeProject", this.IdRoh_contributionGradeProject));
			propList.Add(new BoolOntologyProperty("roh:curator", this.Roh_curator));
			propList.Add(new StringOntologyProperty("roh:contributionGradeProjectOther", this.Roh_contributionGradeProjectOther));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
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