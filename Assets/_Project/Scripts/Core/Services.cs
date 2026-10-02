using System;
using System.Collections.Generic;
using UnityEngine;

namespace TitansOfTheSea.Core
{
    /// Main-thread service registry. A real provider may replace a sandbox fallback.
    public static class Services
    {
        private sealed class Entry
        {
            public object Instance;
            public bool Fallback;
        }
        private static readonly Dictionary<Type, Entry> _entries = new Dictionary<Type, Entry>();
        public static event Action<Type> Changed;
        public static int Count => _entries.Count;

        public static void Register<T>(T instance) where T : class => Register(instance, false);
        public static bool RegisterFallback<T>(T instance) where T : class => Register(instance, true);
        private static bool Register<T>(T instance, bool fallback) where T : class
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            Type type = typeof(T);
            if (_entries.TryGetValue(type, out Entry current))
            {
                if (ReferenceEquals(current.Instance, instance)) return true;
                if (fallback) return false;
                if (!current.Fallback) throw new InvalidOperationException($"A real {type.Name} provider is already registered.");
            }
            _entries[type] = new Entry { Instance = instance, Fallback = fallback };
            Changed?.Invoke(type);
            return true;
        }
        public static T Get<T>() where T : class
        {
            if (TryGet(out T service)) return service;
            throw new InvalidOperationException($"Missing service {typeof(T).Name}. Register its provider before use.");
        }
        public static bool TryGet<T>(out T service) where T : class
        {
            service = _entries.TryGetValue(typeof(T), out Entry entry) ? entry.Instance as T : null;
            return service != null;
        }
        public static bool Unregister<T>(T owner) where T : class
        {
            if (!_entries.TryGetValue(typeof(T), out Entry entry) || !ReferenceEquals(entry.Instance, owner)) return false;
            _entries.Remove(typeof(T));
            Changed?.Invoke(typeof(T));
            return true;
        }
        public static string Describe()
        {
            var lines = new List<string>(_entries.Count);
            foreach (var pair in _entries)
                lines.Add($"{pair.Key.Name}: {pair.Value.Instance.GetType().Name} (fallback={pair.Value.Fallback})");
            lines.Sort(StringComparer.Ordinal);
            return lines.Count == 0 ? "No services registered." : string.Join("\n", lines);
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetForNewSession()
        {
            _entries.Clear();
            Changed = null;
        }
    }
}
