﻿namespace Craftsman.Builders.Tests.Utilities
{
    using Craftsman.Enums;
    using Craftsman.Exceptions;
    using Craftsman.Helpers;
    using System;
    using System.IO.Abstractions;
    using System.Text;
    using static Helpers.ConsoleWriter;

    public class IntegrationTestBaseBuilder
    {
        public static void CreateBase(string solutionDirectory, string projectBaseName, string provider, IFileSystem fileSystem)
        {
            try
            {
                var classPath = ClassPathHelper.IntegrationTestProjectRootClassPath(solutionDirectory, "TestBase.cs", projectBaseName);

                if (!fileSystem.Directory.Exists(classPath.ClassDirectory))
                    fileSystem.Directory.CreateDirectory(classPath.ClassDirectory);

                if (fileSystem.File.Exists(classPath.FullClassPath))
                    throw new FileAlreadyExistsException(classPath.FullClassPath);

                using (var fs = fileSystem.File.Create(classPath.FullClassPath))
                {
                    var data = "";
                    data = GetBaseText(classPath.ClassNamespace, provider);
                    fs.Write(Encoding.UTF8.GetBytes(data));
                }
            }
            catch (FileAlreadyExistsException e)
            {
                WriteError(e.Message);
                throw;
            }
            catch (Exception e)
            {
                WriteError($"An unhandled exception occurred when running the API command.\nThe error details are: \n{e.Message}");
                throw;
            }
        }

        public static string GetBaseText(string classNamespace, string provider)
        {
            var testFixtureName = Utilities.GetIntegrationTestFixtureName();
            var equivalency = Enum.GetName(typeof(DbProvider), DbProvider.Postgres) == provider
                ? $@"
            
            // close to equivalency required to reconcile precision differences between EF and Postgres
            AssertionOptions.AssertEquivalencyUsing(options =>
              options.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation)).WhenTypeIs<DateTime>()
            );
            AssertionOptions.AssertEquivalencyUsing(options =>
              options.Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation)).WhenTypeIs<DateTimeOffset>()"
                : null;

            return @$"namespace {classNamespace}
{{
    using FluentAssertions;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;
    using static {testFixtureName};

    public class TestBase
    {{
        [SetUp]
        public async Task TestSetUp()
        {{
            await ResetState();{equivalency}
            );
        }}
    }}
}}";
        }
    }
}