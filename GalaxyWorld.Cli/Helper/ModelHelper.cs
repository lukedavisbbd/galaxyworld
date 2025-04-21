using GalaxyWorld.Core.Models;
using Spectre.Console;

namespace GalaxyWorld.Cli.Helper;

public static class ModelHelper
{
    public static void PrintModel<T>(T model, string[]? excludedProperties = null)
    {
        excludedProperties ??= [];

        foreach (var prop in typeof(T).GetProperties())
        {
            if (excludedProperties.Contains(prop.Name))
                continue;

            var value = prop.GetValue(model)?.ToString() ?? "";
            AnsiConsole.MarkupLineInterpolated($"[grey]{FormatHelper.PascalToTitleCase(prop.Name)}:[/] {value}");
        }
    }

    public static T PromptModel<T>(string[]? excludedProperties = null)
    {
        excludedProperties ??= [];

        var model = Activator.CreateInstance<T>();

        foreach (var prop in typeof(T).GetProperties())
        {
            if (excludedProperties.Contains(prop.Name))
                continue;

            var label = FormatHelper.PascalToTitleCase(prop.Name);

            var type = prop.PropertyType;

            var optional = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>);

            if (optional)
            {
                optional = true;
                type = type.GenericTypeArguments[0];

                if (!AnsiConsole.Confirm($"Edit {label}?", false))
                {
                    var none = Activator.CreateInstance(typeof(Optional<>).MakeGenericType(type));
                    prop.SetValue(model, none);
                    continue;
                }
            }
            else
            {
                var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                var result = typeof(InputHelper).GetMethod(nameof(InputHelper.Prompt))?.MakeGenericMethod(type).Invoke(null, [label, defaultValue]);

                if (optional)
                    result = Activator.CreateInstance(typeof(Optional<>).MakeGenericType(type), result);

                prop.SetValue(model, result);
            }
        }

        return model;
    }
}
