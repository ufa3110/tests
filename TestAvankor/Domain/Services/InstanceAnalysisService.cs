using System.Collections.ObjectModel;
using TestAvankor.Domain.Models;

namespace TestAvankor.Domain.Services;

/// <summary>
/// Доменные операции анализа XBRL-инстансов: поиск дублей и сравнение фактов.
/// </summary>
public class InstanceAnalysisService
{
    /// <summary>
    /// Информация о повторяющемся контексте.
    /// </summary>
    public class DuplicateContextInfo
    {
        public string Id { get; init; } = string.Empty;
        public int Count { get; init; }
    }

    /// <summary>
    /// Различие по одному факту.
    /// </summary>
    public class FactDifference
    {
        public string Id { get; init; } = string.Empty;
        public Fact First { get; init; } = null!;
        public Fact Second { get; init; } = null!;
    }

    /// <summary>
    /// Результат сравнения фактов двух инстансов.
    /// </summary>
    public class InstanceComparisonResult
    {
        public IReadOnlyCollection<string> MissingInSecond { get; init; } = Array.Empty<string>();
        public IReadOnlyCollection<string> NewInSecond { get; init; } = Array.Empty<string>();
        public IReadOnlyCollection<FactDifference> ValueDifferences { get; init; } =
            Array.Empty<FactDifference>();
    }

    /// <summary>
    /// Находит повторяющиеся контексты (по Id) внутри одного отчёта.
    /// </summary>
    public IReadOnlyCollection<DuplicateContextInfo> FindDuplicateContexts(Instance instance)
    {
        var result = instance.Contexts
            .Where(c => !string.IsNullOrEmpty(c.Id))
            .GroupBy(c => c.Id)
            .Where(g => g.Count() > 1)
            .Select(g => new DuplicateContextInfo
            {
                Id = g.Key,
                Count = g.Count()
            })
            .ToList();

        return new ReadOnlyCollection<DuplicateContextInfo>(result);
    }

    /// <summary>
    /// Сравнивает факты двух отчётов.
    /// Сопоставление идёт по идентификатору факта (Fact.Id).
    /// </summary>
    public InstanceComparisonResult CompareFacts(Instance first, Instance second)
    {
        var firstById = first.Facts
            .Where(f => !string.IsNullOrEmpty(f.Id))
            .ToDictionary(f => f.Id, f => f, StringComparer.Ordinal);

        var secondById = second.Facts
            .Where(f => !string.IsNullOrEmpty(f.Id))
            .ToDictionary(f => f.Id, f => f, StringComparer.Ordinal);

        var missingInSecond = firstById.Keys.Except(secondById.Keys).ToList();
        var newInSecond = secondById.Keys.Except(firstById.Keys).ToList();
        var common = firstById.Keys.Intersect(secondById.Keys).ToList();

        var valueDiffs = new List<FactDifference>();
        foreach (var id in common)
        {
            var f1 = firstById[id];
            var f2 = secondById[id];

            if (!string.Equals(f1.Value, f2.Value, StringComparison.Ordinal) ||
                f1.Decimals != f2.Decimals ||
                f1.Precision != f2.Precision ||
                !string.Equals(f1.UnitRef, f2.UnitRef, StringComparison.Ordinal) ||
                !string.Equals(f1.ContextRef, f2.ContextRef, StringComparison.Ordinal))
            {
                valueDiffs.Add(new FactDifference
                {
                    Id = id,
                    First = f1,
                    Second = f2
                });
            }
        }

        return new InstanceComparisonResult
        {
            MissingInSecond = new ReadOnlyCollection<string>(missingInSecond),
            NewInSecond = new ReadOnlyCollection<string>(newInSecond),
            ValueDifferences = new ReadOnlyCollection<FactDifference>(valueDiffs)
        };
    }
}

