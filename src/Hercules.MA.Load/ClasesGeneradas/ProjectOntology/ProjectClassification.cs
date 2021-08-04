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
using Concept = TaxonomyOntology.Concept;

namespace ProjectOntology
{
	public class ProjectClassification : GnossOCBase
	{

		public ProjectClassification() : base() { } 

		public ProjectClassification(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			this.Roh_projectClassificationNode = new List<Concept>();
			SemanticPropertyModel propRoh_projectClassificationNode = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/projectClassificationNode");
			if(propRoh_projectClassificationNode != null && propRoh_projectClassificationNode.PropertyValues.Count > 0)
			{
				foreach (SemanticPropertyModel.PropertyValue propValue in propRoh_projectClassificationNode.PropertyValues)
				{
					if(propValue.RelatedEntity!=null){
						Concept roh_projectClassificationNode = new Concept(propValue.RelatedEntity,idiomaUsuario);
						this.Roh_projectClassificationNode.Add(roh_projectClassificationNode);
					}
				}
			}
		}

		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"Nodo de clasificación del proyecto")]
		[RDFProperty("http://w3id.org/roh/projectClassificationNode")]
		public  List<Concept> Roh_projectClassificationNode { get; set;}
		public List<string> IdsRoh_projectClassificationNode { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new ListStringOntologyProperty("roh:projectClassificationNode", this.IdsRoh_projectClassificationNode));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
		} 








	}
}
