﻿using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Inedo.Documentation;
using Inedo.Extensibility;
using Inedo.Extensibility.Operations;

namespace Inedo.Extensions.Golang.Operations
{
    [DisplayName("Download Go Source Code")]
    [Description("Uses \"go get\" to download Go source code and dependencies based on an import path.")]
    [ScriptAlias("Get")]
    public sealed class GoGetOperation : GoBuildOperationBase
    {
        [Required]
        [DisplayName("Package path")]
        [Description("The import path of the package to download. " + GoUtils.ImportPathDescription)]
        [ScriptAlias("Package")]
        public string Package { get; set; }

        [DisplayName("Include test dependencies")]
        [Description("Download test dependencies of the package in addition to normal dependencies.")]
        [ScriptAlias("Test")]
        public bool IncludeTestDependencies { get; set; } = false;

        [DisplayName("Install command")]
        [Description("Install the command provided by this package in <code>GOPATH/bin</code>.")]
        [ScriptAlias("Install")]
        public bool Install { get; set; } = false;

        [DisplayName("Fix legacy code")]
        [Description("Run <code>go fix</code> on the downloaded code before resolving dependencies.")]
        [ScriptAlias("Fix")]
        public bool FixLegacyCode { get; set; } = false;

        [DisplayName("Allow insecure package sources")]
        [Description("Permit fetching from repositories using insecure schemes such as unencrypted HTTP. Use with caution.")]
        [ScriptAlias("Insecure")]
        public bool Insecure { get; set; } = false;

        public override async Task ExecuteAsync(IOperationExecutionContext context)
        {
            await this.ExecuteCommandLineAsync(context, "get", this.BuildArgs.Concat(new[]
            {
                this.Install ? null : "-d",
                this.IncludeTestDependencies ? "-t" : null,
                this.FixLegacyCode ? "-fix" : null,
                this.Insecure ? "-insecure" : null,
                "-v", "--", this.Package
            })).ConfigureAwait(false);
        }

        protected override ExtendedRichDescription GetDescription(IOperationConfiguration config)
        {
            var desc = new RichDescription(config[nameof(Install)] == "true" ? "Download and install Go package " : "Download Go package ", new Hilite(config[nameof(Package)]));

            var extra = string.Join(", ", new[]
            {
                config[nameof(IncludeTestDependencies)] == "true" ? "with test dependencies" : null,
                config[nameof(FixLegacyCode)] == "true" ? "fixing legacy code" : null,
                config[nameof(Insecure)] == "true" ? "allowing insecure package sources" : null
            }.Where(x => x != null));
            if (extra.Length > 0)
            {
                return new ExtendedRichDescription(desc, new RichDescription(extra));
            }
            return new ExtendedRichDescription(desc);
        }
    }
}
