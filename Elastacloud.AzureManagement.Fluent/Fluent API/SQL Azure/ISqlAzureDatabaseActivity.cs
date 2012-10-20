/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.SqlAzure
{
    /// <summary>
    /// An interface which provides methods for the activities which take place on the database
    /// </summary>
    public interface ISqlAzureDatabaseActivity : ISqlCompleteActivity
    {
        /// <summary>
        /// Executes scripts on the database using SMO given a valid script directory
        /// </summary>
        /// <param name="scriptDirectory"></param>
        /// <returns>An ISqlAzureDatabaseActivity interface</returns>
        ISqlAzureDatabaseActivity ExecuteScripts(string scriptDirectory);

        /// <summary>
        /// Adds a new database admin user (as opposed to a server admin)
        /// </summary>
        /// <param name="username">The username of the admin</param>
        /// <param name="password">The password in line with the password rules</param>
        /// <returns>An ISqlAzureDatabaseActivity interface</returns>
        ISqlAzureDatabaseActivity AddNewDatabaseAdminUser(string username, string password);

        /// <summary>
        /// Used to execute an arbitrary SQL Commd
        /// </summary>
        /// <param name="sql">A valid SQL statement</param>
        /// <returns>An ISqlAzureDatabaseActivity interface</returns>
        ISqlAzureDatabaseActivity ExecuteSql(string sql);
    }
}