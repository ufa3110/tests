using System.Xml.Linq;

namespace TestAvankor.Infrastructure.XbrlParser;

/// <summary>
/// Инфраструктурный сервис для объединения XBRL-инстансов на уровне XML.
/// </summary>
public static class XbrlMerger
{
    /// <summary>
    /// Объединяет два XBRL-файла и сохраняет результат в <paramref name="outputPath"/>.
    /// </summary>
    public static void MergeFiles(string path1, string path2, string outputPath, TextWriter? log = null)
    {
        var doc1 = XDocument.Load(path1);
        var doc2 = XDocument.Load(path2);

        var root1 = doc1.Root;
        var root2 = doc2.Root;

        if (root1 == null || root2 == null)
            throw new InvalidOperationException("Один из отчетов не содержит корневого элемента xbrli:xbrl.");

        XNamespace xbrli = "http://www.xbrl.org/2003/instance";
        XNamespace link = "http://www.xbrl.org/2003/linkbase";
        XNamespace xlink = "http://www.w3.org/1999/xlink";

        // Контексты
        var contextById = root1.Elements(xbrli + "context")
            .Where(e => e.Attribute("id") != null)
            .ToDictionary(e => (string)e.Attribute("id")!, e => e, StringComparer.Ordinal);

        foreach (var ctx in root2.Elements(xbrli + "context").Where(e => e.Attribute("id") != null))
        {
            var id = (string)ctx.Attribute("id")!;
            if (contextById.TryGetValue(id, out var existing))
            {
                if (!XNode.DeepEquals(NormalizeElement(existing), NormalizeElement(ctx)))
                {
                    log?.WriteLine($"  ВНИМАНИЕ: конфликт контекста с id={id} при объединении. Оставлен вариант из первого отчета.");
                }
            }
            else
            {
                contextById[id] = new XElement(ctx);
            }
        }

        // Units
        var unitById = root1.Elements(xbrli + "unit")
            .Where(e => e.Attribute("id") != null)
            .ToDictionary(e => (string)e.Attribute("id")!, e => e, StringComparer.Ordinal);

        foreach (var unit in root2.Elements(xbrli + "unit").Where(e => e.Attribute("id") != null))
        {
            var id = (string)unit.Attribute("id")!;
            if (unitById.TryGetValue(id, out var existing))
            {
                if (!XNode.DeepEquals(NormalizeElement(existing), NormalizeElement(unit)))
                {
                    log?.WriteLine($"  ВНИМАНИЕ: конфликт unit c id={id} при объединении. Оставлен вариант из первого отчета.");
                }
            }
            else
            {
                unitById[id] = new XElement(unit);
            }
        }

        // schemaRef (link:schemaRef) - собираем уникальные по href
        var schemaRefs = root1.Elements(link + "schemaRef").ToList();
        var existingHrefs = new HashSet<string>(
            schemaRefs.Select(e => (string?)e.Attribute(xlink + "href") ?? string.Empty),
            StringComparer.Ordinal);

        foreach (var sr in root2.Elements(link + "schemaRef"))
        {
            var href = (string?)sr.Attribute(xlink + "href") ?? string.Empty;
            if (!existingHrefs.Contains(href))
            {
                schemaRefs.Add(new XElement(sr));
                existingHrefs.Add(href);
            }
        }

        // Факты: все элементы, кроме context, unit и schemaRef
        var knownNames = new HashSet<XName>
        {
            xbrli + "context",
            xbrli + "unit",
            link + "schemaRef"
        };

        string? FactKey(XElement e)
        {
            var idAttr = (string?)e.Attribute("id");
            if (!string.IsNullOrEmpty(idAttr))
                return idAttr;

            var ctx = (string?)e.Attribute("contextRef") ?? string.Empty;
            var unit = (string?)e.Attribute("unitRef") ?? string.Empty;
            return $"{e.Name}|{ctx}|{unit}|{e.Value}";
        }

        var factByKey = root1.Elements()
            .Where(e => !knownNames.Contains(e.Name))
            .ToDictionary(e => FactKey(e)!, e => e);

        foreach (var fact in root2.Elements().Where(e => !knownNames.Contains(e.Name)))
        {
            var key = FactKey(fact);
            if (key != null && !factByKey.ContainsKey(key))
            {
                factByKey[key] = new XElement(fact);
            }
        }

        // Перестраиваем содержимое root1
        root1.Elements().Remove();

        foreach (var sr in schemaRefs)
            root1.Add(sr);

        foreach (var ctx in contextById.Values.OrderBy(e => (string?)e.Attribute("id")))
            root1.Add(ctx);

        foreach (var unit in unitById.Values.OrderBy(e => (string?)e.Attribute("id")))
            root1.Add(unit);

        foreach (var fact in factByKey.Values)
            root1.Add(fact);

        doc1.Save(outputPath);

        static XElement NormalizeElement(XElement element)
        {
            return XElement.Parse(element.ToString(SaveOptions.DisableFormatting));
        }
    }
}

