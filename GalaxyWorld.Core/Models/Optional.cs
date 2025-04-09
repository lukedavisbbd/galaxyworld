using System.Text.Json.Serialization;

namespace GalaxyWorld.Core.Models;

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

    public T? Or()
    {
        return IsSome ? Some : default;
    }

    public T Or(T value)
    {
        return IsSome ? Some : value;
    }

    public Optional<T> Or(Optional<T> other)
    {
        return IsSome ? this : other;
    }
}
