/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.ScriptMapper.Linux
{
    public interface ICommandShell
    {
        /// <summary>
        /// Executes a command given a string input in the current context of the user
        /// </summary>
        string ExecuteCommand(string command);

        /// <summary>
        /// Executes a shell command in the context of a sudo su
        /// </summary>
        Task<string> ExecuteShell(List<string> commands);
    }
}
