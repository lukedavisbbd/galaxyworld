using System.Text.Json.Serialization;

namespace GalaxyWorld.Models;

public struct Optional<T>
{
    private T? _value;
    public T Some {
        get => IsSome ? _value! : throw new InvalidOperationException();
        init {
            _value = value;
            IsSome = true;
        }
    }
    [JsonIgnore]
    public bool IsSome { get; private init; }
    [JsonIgnore]
    public T? ValueOrDefault => IsSome ? _value! : default;

    public Optional(T value) {
        Some = value;
        IsSome = true;
    }
    
    public Optional() {
        IsSome = false;
    }

    public Optional<U> Map<U>(Func<T, U> map)
    {
        if (IsSome)
            return new Optional<U>
            {
                Some = map(Some),
                IsSome = IsSome,
            };
        else
            return new Optional<U>();
    }
}
