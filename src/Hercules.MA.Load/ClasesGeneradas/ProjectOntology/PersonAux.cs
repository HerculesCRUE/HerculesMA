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

namespace ProjectOntology
{
	[ExcludeFromCodeCoverage]
	public class PersonAux : GnossOCBase
	{

		public PersonAux() : base() { } 

		public PersonAux(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propRdf_member = pSemCmsModel.GetPropertyByPath("http://www.w3.org/1999/02/22-rdf-syntax-ns#member");
			if(propRdf_member != null && propRdf_member.PropertyValues.Count > 0)
			{
				this.Rdf_member = new Person(propRdf_member.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Foaf_familyName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/familyName"));
			this.Roh_secondFamilyName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/secondFamilyName"));
			this.Foaf_firstName = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/firstName"));
			this.Rdf_comment = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://www.w3.org/1999/02/22-rdf-syntax-ns#comment"));
			this.Foaf_nick = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/nick"));
		}

		public virtual string RdfType { get { return "http://w3id.org/roh/PersonAux"; } }
		public virtual string RdfsLabel { get { return "http://w3id.org/roh/PersonAux"; } }
		public OntologyEntity Entity { get; set; }

		[RDFProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#member")]
		public  Person Rdf_member  { get; set;} 
		public string IdRdf_member  { get; set;} 

		[RDFProperty("http://xmlns.com/foaf/0.1/familyName")]
		public  string Foaf_familyName { get; set;}

		[RDFProperty("http://w3id.org/roh/secondFamilyName")]
		public  string Roh_secondFamilyName { get; set;}

		[RDFProperty("http://xmlns.com/foaf/0.1/firstName")]
		public  string Foaf_firstName { get; set;}

		[RDFProperty("http://www.w3.org/1999/02/22-rdf-syntax-ns#comment")]
		public  int? Rdf_comment { get; set;}

		[RDFProperty("http://xmlns.com/foaf/0.1/nick")]
		public  string Foaf_nick { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("rdf:member", this.IdRdf_member));
			propList.Add(new StringOntologyProperty("foaf:familyName", this.Foaf_familyName));
			propList.Add(new StringOntologyProperty("roh:secondFamilyName", this.Roh_secondFamilyName));
			propList.Add(new StringOntologyProperty("foaf:firstName", this.Foaf_firstName));
			propList.Add(new StringOntologyProperty("rdf:comment", this.Rdf_comment.ToString()));
			propList.Add(new StringOntologyProperty("foaf:nick", this.Foaf_nick));
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