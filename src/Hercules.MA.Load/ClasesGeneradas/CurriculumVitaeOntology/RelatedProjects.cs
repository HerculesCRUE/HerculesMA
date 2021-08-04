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
using Project = ProjectOntology.Project;

namespace CurriculumvitaeOntology
{
	public class RelatedProjects : GnossOCBase
	{

		public RelatedProjects() : base() { } 

		public RelatedProjects(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propVivo_relatedBy = pSemCmsModel.GetPropertyByPath("http://vivoweb.org/ontology/core#relatedBy");
			if(propVivo_relatedBy != null && propVivo_relatedBy.PropertyValues.Count > 0)
			{
				this.Vivo_relatedBy = new Project(propVivo_relatedBy.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Roh_order = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/order")).Value;
			this.Roh_isPublic= GetBooleanPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/isPublic"));
		}

		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Relacionado con")]
		[RDFProperty("http://vivoweb.org/ontology/core#relatedBy")]
		[Required]
		public  Project Vivo_relatedBy  { get; set;} 
		public string IdVivo_relatedBy  { get; set;} 

		[LABEL(LanguageEnum.es,"Posición")]
		[RDFProperty("http://w3id.org/roh/order")]
		public  int Roh_order { get; set;}

		[LABEL(LanguageEnum.es,"Público")]
		[RDFProperty("http://w3id.org/roh/isPublic")]
		public  bool Roh_isPublic { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("vivo:relatedBy", this.IdVivo_relatedBy));
			propList.Add(new StringOntologyProperty("roh:order", this.Roh_order.ToString()));
			propList.Add(new BoolOntologyProperty("roh:isPublic", this.Roh_isPublic));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
		} 








	}
}
