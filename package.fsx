#r @"packages\FAKE.3.17.0\tools\FakeLib.dll" // include Fake lib
open Fake 
open Fake.AssemblyInfoFile
open Fake.NuGetHelper
open System
open Fake.Git
open Fake.Git.Staging

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

[<Literal>]
let version = "0.5.1.13"
// 1. Increment the minor number assemblyinfo version
CreateCSharpAssemblyInfo (sprintf @"%s\Elastacloud.AzureManagement.Fluent\Properties\AssemblyInfo.cs" Environment.CurrentDirectory)    [Attribute.Title("Elastacloud.AzureManagement.Fluent")
                                                                                                                                        Attribute.Description("Library used for management of Windows Azure services, SQL, storage, networking, WASD, WAMS and VM's")
                                                                                                                                        Attribute.Company("Elastacloud Limited")
                                                                                                                                        Attribute.Product("Azure Fluent Management")
                                                                                                                                        Attribute.Copyright("Copyright Elastacloud Limited © 2015")
                                                                                                                                        Attribute.ComVisible(false)
                                                                                                                                        Attribute.Guid("909b2b8c-10bf-4f0d-a94e-208ef3bdca52")
                                                                                                                                        Attribute.InternalsVisibleTo("DynamicProxyGenAssembly2")
                                                                                                                                        Attribute.InternalsVisibleTo("Elastacloud.AzureManagement.Fluent.Tests")
                                                                                                                                        Attribute.Version(version)
                                                                                                                                        Attribute.FileVersion(version)]
// 1.5. Clean the bin folder
CleanDirs [ @"Elastacloud.AzureManagement.Fluent\bin" ] |> ignore
// 2. Build of fluent in release mode
MSBuildRelease "" "Rebuild" [@"AzureFluentManagement.sln"]
// 3. Create the nuget package 
NuGet (fun defaults ->
    printfn "%A" defaults
    { defaults with WorkingDir = "."; OutputPath = "."; Version = version }) @"Elastacloud.AzureManagement.Fluent.nuspec"
// 4. Push a label to github
StageAll ""
Commit "" (sprintf "Release %s" version)
Branches.push ""
Branches.tag "" version
Branches.pushTag "" "origin" version