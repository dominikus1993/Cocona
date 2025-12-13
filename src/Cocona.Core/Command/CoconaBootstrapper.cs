using Cocona.Application;
using Cocona.Command.Dispatcher;
using Cocona.CommandLine;
using Cocona.Internal;
using Cocona.Resources;

namespace Cocona.Command;

public sealed class CoconaBootstrapper : ICoconaBootstrapper
{
    private readonly ICoconaCommandLineArgumentProvider _commandLineArgumentProvider;
    private readonly ICoconaCommandProvider _commandProvider;
    private readonly ICoconaCommandResolver _commandResolver;
    private readonly ICoconaCommandDispatcher _dispatcher;
    private readonly ICoconaConsoleProvider _console;

    private CommandCollection? _commandCollection;
    public CoconaBootstrapper(
        ICoconaCommandLineArgumentProvider commandLineArgumentProvider,
        ICoconaCommandProvider commandProvider,
        ICoconaCommandResolver commandResolver,
        ICoconaCommandDispatcher dispatcher,
        ICoconaConsoleProvider console
    )
    {
        _commandLineArgumentProvider = commandLineArgumentProvider;
        _commandProvider = commandProvider;
        _commandResolver = commandResolver;
        _dispatcher = dispatcher;
        _console = console;
    }

    public void Initialize()
    {
        _commandCollection = _commandProvider.GetCommandCollection();
    }

    public async ValueTask<int> RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            var resolved = _commandResolver.ParseAndResolve(_commandCollection ?? throw new CoconaException("Call Initialize before RunAsync"), _commandLineArgumentProvider.GetArguments());
            return await _dispatcher.DispatchAsync(resolved, cancellationToken);
        }
        catch (CommandNotFoundException cmdNotFoundEx)
        {
            if (string.IsNullOrWhiteSpace(cmdNotFoundEx.Command))
            {
                await _console.Error.WriteLineAsync(string.Format(Strings.Dispatcher_Error_CommandNotFound, cmdNotFoundEx.Message));
            }
            else
            {
                await _console.Error.WriteLineAsync(string.Format(Strings.Dispatcher_Error_NotACommand, cmdNotFoundEx.Command));
            }

            var similarCommands = cmdNotFoundEx.ImplementedCommands.All
                .Where(x => !x.IsHidden && Levenshtein.GetDistance(cmdNotFoundEx.Command.ToLowerInvariant(), x.Name.ToLowerInvariant()) < 3).ToArray();

            if (similarCommands is not { Length: > 0 })
            {
                return 1;
            }

            await _console.Error.WriteLineAsync();
            await _console.Error.WriteLineAsync(Strings.Dispatcher_Error_SimilarCommands);
            foreach (var c in similarCommands)
            {
                await _console.Error.WriteLineAsync($"  {c.Name}");
            }

            return 1;
        }
    }
}
