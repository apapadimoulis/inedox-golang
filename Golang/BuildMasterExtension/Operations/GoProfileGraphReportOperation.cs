﻿using Inedo.Agents;
using Inedo.BuildMaster.Extensibility;
using Inedo.BuildMaster.Extensibility.Operations;
using Inedo.Documentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Inedo.Extensions.Golang.Operations
{
    [DisplayName("Generate Go Flow Chart")]
    [Description("Generate a flow chart based on profiling data generated by Golang::Test or by a Go program that uses pprof.")]
    [ScriptAlias("Flow-Chart-Report")]
    public sealed class GoProfileGraphReportOperation : GoProfileReportOperationBase
    {
        protected override IEnumerable<string> ProfileArgs => new[] { "-svg" };

        protected override ExtendedRichDescription GetDescription(IOperationConfiguration config)
        {
            return new ExtendedRichDescription(new RichDescription("Generate profile flow chart report ", new Hilite(config[nameof(OutputName)]), " from ", new Hilite(config[nameof(ProfileFile)])));
        }

        protected override async Task GenerateReportAsync(IOperationExecutionContext context, string outputPath)
        {
            await base.GenerateReportAsync(context, outputPath).ConfigureAwait(false);

            var fileOps = await context.Agent.GetServiceAsync<IFileOperationsExecuter>().ConfigureAwait(false);
            var contents = await fileOps.ReadFileBytesAsync(outputPath).ConfigureAwait(false);
            await fileOps.WriteAllTextAsync(outputPath, $"<iframe style=\"position: absolute; top: 0; left: 0; right: 0; bottom: 0; border: 0;\" src=\"data:application/svg+xml;charset=utf-8;base64,{Convert.ToBase64String(contents)}\"></iframe>", InedoLib.UTF8Encoding).ConfigureAwait(false);
        }
    }
}
