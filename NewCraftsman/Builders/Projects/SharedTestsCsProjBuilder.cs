﻿namespace NewCraftsman.Builders.Projects
{
    using System.IO;
    using System.Text;
    using Exceptions;
    using Helpers;
    using Services;

    public class SharedTestsCsProjBuilder
    {
        private readonly ICraftsmanUtilities _utilities;

        public SharedTestsCsProjBuilder(ICraftsmanUtilities utilities)
        {
            _utilities = utilities;
        }

        public void CreateTestsCsProj(string solutionDirectory, string projectBaseName)
        {
            var classPath = ClassPathHelper.SharedTestProjectClassPath(solutionDirectory, projectBaseName);
            _utilities.CreateFile(classPath, GetTestsCsProjFileText(solutionDirectory, projectBaseName));
        }

        public static string GetTestsCsProjFileText(string solutionDirectory, string projectBaseName)
        {
            var apiClassPath = ClassPathHelper.WebApiProjectClassPath(solutionDirectory, projectBaseName);

            return @$"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""AutoBogus"" Version=""2.13.1"" />
    <PackageReference Include=""Bogus"" Version=""34.0.1"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\..\src\{apiClassPath.ClassNamespace}\{apiClassPath.ClassName}"" />
  </ItemGroup>

</Project>";
        }
    }
}