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
using Person = PersonOntology.Person;

namespace ResearchobjectOntology
{
	[ExcludeFromCodeCoverage]
	public class Annotation : GnossOCBase
	{

		public Annotation() : base() { } 

		public Annotation(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Dct_issued= GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/terms/issued"));
			this.Roh_annotation = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/annotation"));
			SemanticPropertyModel propRdf_member = pSemCmsModel.GetPropertyByPath("http://www.w3.org/1999/02/22-rdf-syntax-ns#member");
			if(propRdf_member != null && propRdf_member.PropertyValues.Count > 0)
			{
				this.Rdf_member = new Person(propRdf_member.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
		}

		public virtual string RdfType { get { return "http://w3id.org/roh/Annotation"; } }
		public virtual string RdfsLabel { get { return "http://w3id.org/roh/Annotation"; } }
		public OntologyEntity Entity { get; set; }

		[RDFProperty("http://purl.org/dc/terms/issued")]
		public  DateTime? Dct_issued { get; set;}

		[RDFProperty("http://w3id.org/roh/annotation")]
		public  string Roh_annotation { get; set;}

		[LABEL(LanguageEnum.es,"http://www.w3.org/1999/02/22-rdf-syntax-ns#member")]
		[RDFProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#member")]
		[Required]
		public  Person Rdf_member  { get; set;} 
		public string IdRdf_member  { get; set;} 


		internal override void GetProperties()
		{
			base.GetProperties();
			if (this.Dct_issued.HasValue){
				propList.Add(new DateOntologyProperty("dct:issued", this.Dct_issued.Value));
				}
			propList.Add(new StringOntologyProperty("roh:annotation", this.Roh_annotation));
			propList.Add(new StringOntologyProperty("rdf:member", this.IdRdf_member));
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
