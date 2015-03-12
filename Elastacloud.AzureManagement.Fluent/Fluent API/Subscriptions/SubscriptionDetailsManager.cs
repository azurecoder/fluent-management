/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Subscriptions;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Subscriptions
{
	/// <summary>
	/// Used to bring back subscription information for a user and also query the history of a subscription
	/// </summary>
	public class SubscriptionDetailsManager : IAzureManager, ICertificateActivity, ISubscriptionQuery
	{
		/// <summary>
		/// Sets the subscription id with the manager class
		/// </summary>
		internal SubscriptionDetailsManager(string subscriptionId, string defaultLocation = LocationConstants.NorthEurope)
		{
			SubscriptionId = subscriptionId;
		    Location = defaultLocation;
		}

		#region Implementation of IAzureManager

	    public string Location { get; set; }
		/// <summary>
		/// Event used to capture any trace information for long running async processes
		/// </summary>
		public event EventReached AzureTaskComplete;

		/// <summary>
		/// The relevant subscription id
		/// </summary>
		public string SubscriptionId { get; set; }

		/// <summary>
		/// One of the management certificates loaded to the portal
		/// </summary>
		public X509Certificate2 ManagementCertificate { get; set; }

		#endregion

		#region Implementation of ICertificateActivity

		/// <summary>
		/// Adds an X509Certificate2 certificate to the request
		/// </summary>
		ISubscriptionQuery ICertificateActivity.AddCertificate(X509Certificate2 certificate)
		{
			ManagementCertificate = certificate;
			return this;
		}

		/// <summary>
		/// Adds a .publishsettings file and extracts the certificate
		/// </summary>
		public ISubscriptionQuery AddPublishSettingsFromFile(string path)
		{
			var settings = new PublishSettingsExtractor(path);
			ManagementCertificate = settings.GetCertificateFromFile();
			return this;
		}

		/// <summary>
		/// Adds a .publishsettings profile from a given body of Xml
		/// </summary>
		public ISubscriptionQuery AddPublishSettingsFromXml(string xml)
		{
			ManagementCertificate = PublishSettingsExtractor.GetCertificateFromXml(xml);
			return this;
		}

		/// <summary>
		/// Searches various stores to find the certificate with the matching thumbprint id
		/// </summary>
		ISubscriptionQuery ICertificateActivity.AddCertificateFromStore(string thumbprint)
		{
			ManagementCertificate = PublishSettingsExtractor.FromStore(thumbprint);
			return this;
		}

		#endregion

		#region Implementation of ISubscriptionQuery

		/// <summary>
		/// Returns subscription information to the user 
		/// </summary>
		SubscriptionInformation ISubscriptionQuery.GetSubscriptionInformation()
		{
			var subscriptionCommand = new GetSubscriptionCommand
			{
				SubscriptionId = SubscriptionId,
				Certificate = ManagementCertificate,
                Location = Location
			};
			subscriptionCommand.Execute();
			return subscriptionCommand.SubscriptionInformation;
		}

		/// <summary>
		/// Invoked when the user needs a list of available locations they can deploy to 
		/// Particularly relevant with free accounts given that they contain details of where deployment can occur
		/// </summary>
		/// <returns>Returns a subscriber locations list</returns>
		List<LocationInformation> ISubscriptionQuery.GetSubscriberLocations()
		{
			var subscriptionCommand = new GetSubscriberLocationsCommand
			{
				SubscriptionId = SubscriptionId,
				Certificate = ManagementCertificate,
                Location = Location
			};
			subscriptionCommand.Execute();
			return subscriptionCommand.Locations;
		}

		#endregion

		// ToDo: Find where this is supposed to be used and use it.
		protected virtual void OnAzureTaskComplete(EventPoint point, string message)
		{
			var handler = AzureTaskComplete;
			if (handler != null)
			{
				handler(point, message);
			}
		}
	}
}