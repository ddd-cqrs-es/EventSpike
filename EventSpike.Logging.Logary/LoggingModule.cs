using System.Reflection;
using System.Text.RegularExpressions;
using Autofac;
using Logary;
using Logary.Configuration;
using Logary.Targets;
using Module = Autofac.Module;
using Console = System.Console;

namespace EventSpike.Logging.Logary
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceName = Assembly.GetEntryAssembly().GetName().Name;

            builder.Register(context => LogaryFactory.New(serviceName, with =>
                with.Target<TextWriter.Builder>("console", conf => conf.Target.WriteTo(Console.Out, Console.Error)
                    .MinLevel(LogLevel.Verbose)
                    .AcceptIf(line => true)
                    .SourceMatching(new Regex(".*")))))
                .AsSelf();
        }
    }
}
