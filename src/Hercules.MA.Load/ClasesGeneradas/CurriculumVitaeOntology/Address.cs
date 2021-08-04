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
using Feature = FeatureOntology.Feature;

namespace CurriculumVitaeOntology
{
	public class Address : GnossOCBase
	{

		public Address() : base() { } 

		public Address(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()
		{
			this.mGNOSSID = pSemCmsModel.Entity.Uri;
			this.mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;
			SemanticPropertyModel propVcard_hasCountryName = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#hasCountryName");
			if(propVcard_hasCountryName != null && propVcard_hasCountryName.PropertyValues.Count > 0)
			{
				this.Vcard_hasCountryName = new Feature(propVcard_hasCountryName.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propVcard_hasRegion = pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#hasRegion");
			if(propVcard_hasRegion != null && propVcard_hasRegion.PropertyValues.Count > 0)
			{
				this.Vcard_hasRegion = new Feature(propVcard_hasRegion.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			SemanticPropertyModel propRoh_hasProvince = pSemCmsModel.GetPropertyByPath("http://w3id.org/roh/hasProvince");
			if(propRoh_hasProvince != null && propRoh_hasProvince.PropertyValues.Count > 0)
			{
				this.Roh_hasProvince = new Feature(propRoh_hasProvince.PropertyValues[0].RelatedEntity,idiomaUsuario);
			}
			this.Vcard_postal_code = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#postal-code"));
			this.Vcard_extended_address = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#extended-address"));
			this.Vcard_street_address = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#street-address"));
			this.Vcard_locality = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath("https://www.w3.org/2006/vcard/ns#locality"));
		}

		public OntologyEntity Entity { get; set; }

		[LABEL(LanguageEnum.es,"País")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#hasCountryName")]
		public  Feature Vcard_hasCountryName  { get; set;} 
		public string IdVcard_hasCountryName  { get; set;} 

		[LABEL(LanguageEnum.es,"C. Autón./Reg.")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#hasRegion")]
		public  Feature Vcard_hasRegion  { get; set;} 
		public string IdVcard_hasRegion  { get; set;} 

		[LABEL(LanguageEnum.es,"Provincia")]
		[RDFProperty("http://w3id.org/roh/hasProvince")]
		public  Feature Roh_hasProvince  { get; set;} 
		public string IdRoh_hasProvince  { get; set;} 

		[LABEL(LanguageEnum.es,"Código postal")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#postal-code")]
		public  string Vcard_postal_code { get; set;}

		[LABEL(LanguageEnum.es,"Resto de dirección contacto")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#extended-address")]
		public  string Vcard_extended_address { get; set;}

		[LABEL(LanguageEnum.es,"Dirección de contacto")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#street-address")]
		public  string Vcard_street_address { get; set;}

		[LABEL(LanguageEnum.es,"Ciudad")]
		[RDFProperty("https://www.w3.org/2006/vcard/ns#locality")]
		public  string Vcard_locality { get; set;}


		internal override void GetProperties()
		{
			base.GetProperties();
			propList.Add(new StringOntologyProperty("vcard:hasCountryName", this.IdVcard_hasCountryName));
			propList.Add(new StringOntologyProperty("vcard:hasRegion", this.IdVcard_hasRegion));
			propList.Add(new StringOntologyProperty("roh:hasProvince", this.IdRoh_hasProvince));
			propList.Add(new StringOntologyProperty("vcard:postal_code", this.Vcard_postal_code));
			propList.Add(new StringOntologyProperty("vcard:extended_address", this.Vcard_extended_address));
			propList.Add(new StringOntologyProperty("vcard:street_address", this.Vcard_street_address));
			propList.Add(new StringOntologyProperty("vcard:locality", this.Vcard_locality));
		}

		internal override void GetEntities()
		{
			base.GetEntities();
			entList = new List<OntologyEntity>();
		} 








	}
}
