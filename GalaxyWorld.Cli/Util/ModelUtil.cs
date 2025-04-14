using System.Text.Json;
using GalaxyWorld.Cli.Helper;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Core.Models.Star;
using Spectre.Console;

namespace GalaxyWorld.Cli.Util;

public static class ModelUtil
{
    public static Table ModelToTable<T>(T model, string suffix)
    {
        var table = new Table().Title($"[bold]{FormatUtil.PascalToTitleCase(typeof(T).Name)} {suffix}[/]").AddColumns("Field", "Value");
        
        foreach (var prop in typeof(T).GetProperties())
        {
            var value = prop.GetValue(model)?.ToString() ?? "";
            table.AddRow(FormatUtil.PascalToTitleCase(prop.Name), value);
        }
        
        return table;
    }

    public static T PromptModel<T>(string[]? excludedProperties = null)
    {
        excludedProperties ??= [];

        var model = Activator.CreateInstance<T>();

        foreach (var prop in typeof(T).GetProperties())
        {
            if (excludedProperties.Contains(prop.Name))
                continue;

            var label = FormatUtil.PascalToTitleCase(prop.Name);

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
            
            var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
            var result = typeof(InputHelper).GetMethod(nameof(InputHelper.Prompt))?.MakeGenericMethod(type).Invoke(null, [label, defaultValue]);

            if (optional)
                result = Activator.CreateInstance(typeof(Optional<>).MakeGenericType(type), result);

            prop.SetValue(model, result);
        }

        return model;
    }
}
