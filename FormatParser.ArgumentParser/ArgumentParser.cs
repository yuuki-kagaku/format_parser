namespace FormatParser.ArgumentParser;

public class ArgumentParser<TSettings> where TSettings : class, new()
{
    private readonly Dictionary<string, Action<TSettings>> parameterlessArgumentsActions = new ();
    private readonly HashSet<string> allArguments = new ();
    private readonly Dictionary<string, Action<TSettings, int>> intArgumentsActions = new ();
    private readonly Dictionary<string, Action<TSettings, string>> stringArgumentsActions = new ();
    private readonly List<Action<TSettings, string>> positionalArgumentsActions = new ();
    private readonly HashSet<string> requiredArguments = new();

    public ArgumentParser<TSettings> OnNamedParameter(string parameterName, Action<TSettings> action, bool required = true)
    {
        if (!allArguments.Add($"--{parameterName}"))
            throw new ArgumentParserException($"Parameter with name {parameterName} already added.");

        if (required)
            requiredArguments.Add($"--{parameterName}");
        
        parameterlessArgumentsActions.Add($"--{parameterName}", action);
        return this;
    }
    
    public ArgumentParser<TSettings> OnNamedParameter(string parameterName, Action<TSettings, int> action, bool required = true)
    {
        if (!allArguments.Add($"--{parameterName}"))
            throw new ArgumentParserException($"Parameter with name {parameterName} already added.");
        
        if (required)
            requiredArguments.Add($"--{parameterName}");
        
        intArgumentsActions.Add($"--{parameterName}", action);
        return this;
    }
    
    public ArgumentParser<TSettings> OnNamedParameter(string parameterName, Action<TSettings, string> action, bool required = true)
    {
        if (!allArguments.Add($"--{parameterName}"))
            throw new ArgumentParserException($"Parameter with name {parameterName} already added.");
        
        if (required)
            requiredArguments.Add($"--{parameterName}");
        
        stringArgumentsActions.Add($"--{parameterName}", action);
        return this;
    }

    public ArgumentParser<TSettings> WithPositionalArgument(Action<TSettings, string> action)
    {
        positionalArgumentsActions.Add(action);
        return this;
    }

    public TSettings Parse(string[] args)
    {
        var setting = new TSettings();

        var positionalArguments = new List<string>();
        for (var i = 0; i < args.Length; i++)
        {
            var currentArg = args[i];
            
            if (!currentArg.StartsWith("--"))
            {
                positionalArguments.Add(currentArg);
                continue;
            }

            if (parameterlessArgumentsActions.TryGetValue(currentArg, out var parameterlessAction))
            {
                parameterlessAction(setting);
                requiredArguments.Remove(currentArg);
                continue;
            }
            
            if (stringArgumentsActions.TryGetValue(currentArg, out var stringAction))
            {
                if (args.Length <= i++)
                    throw new ArgumentParserException($"Argument --{currentArg} should be proceeded with string.");
                
                stringAction(setting, args[i]);
                requiredArguments.Remove(currentArg);
                continue;
            }
            
            if (intArgumentsActions.TryGetValue(currentArg, out var intAction))
            {
                if (args.Length <= i++)
                    throw new ArgumentParserException($"Argument --{currentArg} should be proceeded with string.");
                
                intAction(setting, int.Parse(args[i]));
                requiredArguments.Remove(currentArg);
                continue;
            }

            if (currentArg.StartsWith("--"))
            {
                Console.Error.WriteLine($"Unknown argument {currentArg}");
            }
        }

        if (requiredArguments.Any())
        {
            foreach (var requiredArgument in requiredArguments)
            {
                Console.Error.WriteLine($"Missing required argument: {requiredArgument}");
            }

            throw new ArgumentParserException("Missing required argument.");
        }

        if (positionalArguments.Count < positionalArgumentsActions.Count)
            throw new ArgumentParserException($"Required {positionalArgumentsActions.Count} positional arguments, but found {positionalArguments.Count}.");

        for (var i = 0; i < positionalArgumentsActions.Count; i++)
            positionalArgumentsActions[i].Invoke(setting, positionalArguments[i]);

        return setting;
    }
}