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

namespace CurriculumVitaeOntology
{
	public class Document : GnossOCBase
	{

		public Document() : base() { } 

		public Document(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Foaf_topic = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/topic"));
			this.Dc_title = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://purl.org/dc/elements/1.1/title"));
			this.Foaf_primaryTopic = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://xmlns.com/foaf/0.1/primaryTopic"));
		}

		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Tipo identificador, otros")]
		[RDFProperty("http://xmlns.com/foaf/0.1/topic")]
		public  string Foaf_topic { get; set;}

		[LABEL(LanguageEnum.es,"Identificador")]
		[RDFProperty("http://purl.org/dc/elements/1.1/title")]
		public  string Dc_title { get; set;}

		[LABEL(LanguageEnum.es,"Tipo identificador")]
		[RDFProperty("http://xmlns.com/foaf/0.1/primaryTopic")]
		public  string Foaf_primaryTopic { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("foaf:topic", this.Foaf_topic));
			propList.Add(new StringOntologyProperty("dc:title", this.Dc_title));
			propList.Add(new StringOntologyProperty("foaf:primaryTopic", this.Foaf_primaryTopic));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
		} 








	}
}
