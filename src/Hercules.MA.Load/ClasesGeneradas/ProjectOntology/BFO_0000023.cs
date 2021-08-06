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
using ParticipationType = ParticipationtypeOntology.ParticipationType;
using DedicationRegime = DedicationregimeOntology.DedicationRegime;
using Person = PersonOntology.Person;

namespace ProjectOntology
{
	public class BFO_0000023 : GnossOCBase
	{

		public BFO_0000023() : base() { } 

		public BFO_0000023(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propRoh_participationType = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/participationType");
			if(propRoh_participationType != null && propRoh_participationType.PropertyValues.Count > 0)
			{
				this.Roh_participationType = new ParticipationType(propRoh_participationType.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_dedicationRegime = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/dedicationRegime"));
			this.Roh_participationTypeOther = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/participationTypeOther"));
			this.Roh_applicantContribution = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/applicantContribution"));
			this.Roh_contributionGradeOther = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/contributionGradeOther"));
			SemanticPropertyModel propRoh_roleOf = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/roleOf");
			if(propRoh_roleOf != null && propRoh_roleOf.PropertyValues.Count > 0)
			{
				this.Roh_roleOf = new Person(propRoh_roleOf.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_order = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/order"));
		}

		public virtual string RdfType { get { return "http://purl.obolibrary.org/obo/BFO_0000023"; } }
		public virtual string RdfsLabel { get { return "http://purl.obolibrary.org/obo/BFO_0000023"; } }
		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Tipo de participación")]
		[RDFProperty("http://w3id.org/roh/participationType")]
		public  ParticipationType Roh_participationType  { get; set;} 
		public string IdRoh_participationType  { get; set;} 

		[LABEL(LanguageEnum.es,"Grado de contribución")]
		[RDFProperty("http://w3id.org/roh/contributionGrade")]
		public  object Roh_contributionGrade  { get; set;} 
		public string IdRoh_contributionGrade  { get; set;} 

		[LABEL(LanguageEnum.es,"Régimen de dedicación")]
		[RDFProperty("http://w3id.org/roh/dedicationRegime")]
		public  string Roh_dedicationRegime { get; set;}

		[LABEL(LanguageEnum.es,"Tipo de participación, otros")]
		[RDFProperty("http://w3id.org/roh/participationTypeOther")]
		public  string Roh_participationTypeOther { get; set;}

		[LABEL(LanguageEnum.es,"Aportación del solicitante")]
		[RDFProperty("http://w3id.org/roh/applicantContribution")]
		public  string Roh_applicantContribution { get; set;}

		[LABEL(LanguageEnum.es,"Grado de contribución, otros")]
		[RDFProperty("http://w3id.org/roh/contributionGradeOther")]
		public  string Roh_contributionGradeOther { get; set;}

		[LABEL(LanguageEnum.es,"Rol de")]
		[RDFProperty("http://w3id.org/roh/roleOf")]
		[Required]
		public  Person Roh_roleOf  { get; set;} 
		public string IdRoh_roleOf  { get; set;} 

		[LABEL(LanguageEnum.es,"Posición")]
		[RDFProperty("http://w3id.org/roh/order")]
		public  string Roh_order { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("roh:participationType", this.IdRoh_participationType));
			//propList.Add(new StringOntologyProperty("roh:contributionGrade", this.Roh_contributionGrade));
			propList.Add(new StringOntologyProperty("roh:dedicationRegime", this.Roh_dedicationRegime));
			propList.Add(new StringOntologyProperty("roh:participationTypeOther", this.Roh_participationTypeOther));
			propList.Add(new StringOntologyProperty("roh:applicantContribution", this.Roh_applicantContribution));
			propList.Add(new StringOntologyProperty("roh:contributionGradeOther", this.Roh_contributionGradeOther));
			propList.Add(new StringOntologyProperty("roh:roleOf", this.IdRoh_roleOf));
			propList.Add(new StringOntologyProperty("roh:order", this.Roh_order));
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
