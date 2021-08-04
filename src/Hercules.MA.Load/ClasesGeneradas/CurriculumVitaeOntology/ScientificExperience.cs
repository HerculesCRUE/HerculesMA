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
			entList = new List<OntologyEntity>();
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








	}
}
