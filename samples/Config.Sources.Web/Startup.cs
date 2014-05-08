// Copyright (c) Microsoft Open Technologies, Inc.
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING
// WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF
// TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR
// NON-INFRINGEMENT.
// See the Apache 2 License for the specific language governing
// permissions and limitations under the License.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Net.Runtime;

public class Startup
{
    public void Configuration(IBuilder app)
    {
        var applicationEnvironment = (IApplicationEnvironment)app.ApplicationServices.GetService(typeof(IApplicationEnvironment));

        var config = new Configuration();
        config.AddIniFile(Path.Combine(applicationEnvironment.ApplicationBasePath, "Config.Sources.ini"));
        config.AddEnvironmentVariables();

        app.Run(async ctx =>
        {
            ctx.Response.ContentType = "text/plain";

            Func<String, String> formatKeyValue = key => "[" + key + "] " + config.Get(key) + "\r\n\r\n";
            await ctx.Response.WriteAsync(formatKeyValue("Services:One.Two"));
            await ctx.Response.WriteAsync(formatKeyValue("Services:One.Two:Six"));
            await ctx.Response.WriteAsync(formatKeyValue("Data:DefaultConnecection:ConnectionString"));
            await ctx.Response.WriteAsync(formatKeyValue("Data:DefaultConnecection:Provider"));
            await ctx.Response.WriteAsync(formatKeyValue("Data:Inventory:ConnectionString"));
            await ctx.Response.WriteAsync(formatKeyValue("Data:Inventory:Provider"));
            await ctx.Response.WriteAsync(formatKeyValue("PATH"));
            await ctx.Response.WriteAsync(formatKeyValue("COMPUTERNAME"));
        });
    }

    private static async Task DumpConfig(HttpResponse response, IConfiguration config, string indentation = "")
    {
        foreach (var child in config.GetSubKeys())
        {
            await response.WriteAsync(indentation + "[" + child.Key + "] " + config.Get(child.Key) + "\r\n");
            await DumpConfig(response, child.Value, indentation + "  ");
        }
    }
}



namespace Microsoft.Net.Runtime
{
    using System;

    [AssemblyNeutral]
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class AssemblyNeutralAttribute : Attribute
    {
    }
}

namespace Microsoft.Net.Runtime
{
    using System.Runtime.Versioning;
    [AssemblyNeutral]
    public interface IApplicationEnvironment
    {
        string ApplicationName { get; }
        string Version { get; }
        string ApplicationBasePath { get; }
        FrameworkName TargetFramework { get; }
    }
}