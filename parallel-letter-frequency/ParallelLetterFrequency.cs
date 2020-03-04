using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class ParallelLetterFrequency
{
    private static object loc = new object();

    public static Dictionary<char, int> Calculate(IEnumerable<string> texts)
    {
        var result = new ConcurrentDictionary<char, int>();

        Parallel.ForEach(texts,
            () => new Dictionary<char, int>(),
            (text, loop, charOccurrence) =>
            {
                foreach (char c in text)
                {
                    if (!char.IsLetter(c))
                    {
                        continue;
                    }

                    char lc = char.ToLower(c);

                    if (charOccurrence.ContainsKey(lc))
                    {
                        charOccurrence[lc]++;
                    }
                    else
                    {
                        charOccurrence.Add(lc, 1);
                    }
                }

                return charOccurrence;
            },
            (charOccurrence) =>
            {
                foreach (var kv in charOccurrence)
                {
                    if (kv.Value > 0)
                    {
                        result.AddOrUpdate(kv.Key, kv.Value, (key, value) => value + kv.Value);                       
                    }
                }
            });


        return new Dictionary<char, int>(result);
    }
}