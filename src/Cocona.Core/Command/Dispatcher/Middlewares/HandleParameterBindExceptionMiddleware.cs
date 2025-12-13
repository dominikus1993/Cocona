using Cocona.Application;
using Cocona.Command.Binder;
using Cocona.Resources;

namespace Cocona.Command.Dispatcher.Middlewares;

public sealed class HandleParameterBindExceptionMiddleware : CommandDispatcherMiddleware
{
    private readonly ICoconaConsoleProvider _console;

    public HandleParameterBindExceptionMiddleware(CommandDispatchDelegate next, ICoconaConsoleProvider console) : base(next)
    {
        _console = console;
    }

    public override async ValueTask<int> DispatchAsync(CommandDispatchContext ctx)
    {
        try
        {
            return await Next(ctx);
        }
        catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientArgument)
        {
            await _console.Error.WriteLineAsync(string.Format(Strings.Command_Error_Insufficient_Argument, paramEx.Argument!.Name));
        }
        catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientOption)
        {
            await _console.Error.WriteLineAsync(string.Format(Strings.Command_Error_Insufficient_Option, paramEx.Option!.Name));
        }
        catch (ParameterBinderException paramEx) when (paramEx.Result == ParameterBinderResult.InsufficientOptionValue)
        {
            await _console.Error.WriteLineAsync(string.Format(Strings.Command_Error_Insufficient_OptionValue, paramEx.Option!.Name));
        }
        catch (ParameterBinderException paramEx) when (paramEx.Result is ParameterBinderResult.TypeNotSupported or ParameterBinderResult.ValidationFailed)
        {
            await _console.Error.WriteLineAsync(string.Format(Strings.Command_Error_ParameterBind, paramEx.Message));
        }

        return 1;
    }
}
