/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Linq;
using System.Net;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
	/// <summary>
	/// Extensions on <see cref="IStorageClient"/>
	/// </summary>
	public static class StorageClientExtensions
	{
		/// <summary>
		/// If the subscription already contains a storage account by the same name create will be skipped.
		/// </summary>
		/// <param name="storageClient">The StorageClient that mapps our subscription.</param>
		/// <param name="storageAccountName">The Storage Account we want to create.</param>
		/// <param name="location">The location where we want the storage account.</param>
		/// <exception cref="InvalidOperationException">If the <paramref name="storageAccountName"/> is not globally unique.</exception>
		public static void CreateNewStorageAccountIfNotExists(this IStorageClient storageClient, string storageAccountName, string location = LocationConstants.NorthEurope)
		{
			var storageAccountList = storageClient.GetStorageAccountList();
			if (storageAccountList.Any(sa => sa.Name == storageAccountName))
			{
				return;
			}

			try
			{
				storageClient.CreateNewStorageAccount(storageAccountName, location);
			}
			catch (WebException we)
			{
				if (we.Status != WebExceptionStatus.ProtocolError ||
						we.Message != "The remote server returned an error: (409) Conflict.")
				{
					throw new InvalidOperationException(string.Format("The storage account '{0}' already exists.", storageAccountName), we);
				}
				throw;
			}
		}

		/// <summary>
		/// If the storage account already exists (have to be globally unique) - none will be created. Exceptions are handled.
		/// </summary>
		/// <param name="storageClient">The StorageClient that mapps our subscription.</param>
		/// <param name="storageAccountName">The Storage Account we want to create.</param>
		/// <param name="location">The location where we want the storage account.</param>
		/// <returns>True if the storage account was created or already exists. False if it could not be created.</returns>
		public static bool TryCreateNewStorageAccount(this IStorageClient storageClient, string storageAccountName, string location = LocationConstants.NorthEurope)
		{
			try
			{
				storageClient.CreateNewStorageAccountIfNotExists(storageAccountName, location);
			}
			catch (WebException)
			{
				return false;
			}
			catch (InvalidOperationException)
			{
				return false;
			}

			return true;
		}
	}
}