using System.Xml.Linq;
using TestAvankor.Domain.Models;

namespace TestAvankor.Infrastructure.XbrlParser
{
    /// <summary>
    /// Парсер XBRL файлов в модели домена
    /// </summary>
    public class XbrlParser
    {
        private static readonly XNamespace XbrliNamespace = XNamespace.Get("http://www.xbrl.org/2003/instance");
        private static readonly XNamespace XbrldiNamespace = XNamespace.Get("http://xbrl.org/2006/xbrldi");

        #region queries
        /// <summary>
        /// XPath: контексты с периодом xbrli:period/xbrli:instant = "2019-04-30"
        /// </summary>
        public const string XPathContextsWithInstant2019_04_30 =
            "//xbrli:context[xbrli:period/xbrli:instant = \"2019-04-30\"]";

        /// <summary>
        /// XPath: контексты со сценарием, использующим dimension=\"dim-int:ID_sobstv_CZBTaxis\"
        /// (explicitMember или typedMember)
        /// </summary>
        public const string XPathContextsWithDimIntIdSobstvCzbTaxis =
            "//xbrli:context[xbrli:scenario//*[@dimension = \"dim-int:ID_sobstv_CZBTaxis\"]]";

        /// <summary>
        /// XPath: контексты без сценария (без xbrli:scenario)
        /// </summary>
        public const string XPathContextsWithoutScenario =
            "//xbrli:context[not(xbrli:scenario)]";

        #endregion

        /// <summary>
        /// Парсит XBRL файл и возвращает модель Instance
        /// </summary>
        public Task<Instance> ParseAsync(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var instance = new Instance();

            // Парсим контексты
            ParseContexts(doc, instance);

            // Парсим единицы измерения
            ParseUnits(doc, instance);

            // Парсим факты
            ParseFacts(doc, instance);

            return Task.FromResult(instance);
        }

        /// <summary>
        /// Парсит контексты из XBRL документа
        /// </summary>
        private void ParseContexts(XDocument doc, Instance instance)
        {
            var contexts = doc.Descendants(XbrliNamespace + "context");

            foreach (var contextElement in contexts)
            {
                var context = new Context
                {
                    Id = contextElement.Attribute("id")?.Value ?? string.Empty
                };

                // Парсим entity
                var entityElement = contextElement.Element(XbrliNamespace + "entity");
                if (entityElement != null)
                {
                    var identifierElement = entityElement.Element(XbrliNamespace + "identifier");
                    if (identifierElement != null)
                    {
                        context.EntityValue = identifierElement.Value;
                        context.EntityScheme = identifierElement.Attribute("scheme")?.Value ?? string.Empty;
                    }

                    var segmentElement = entityElement.Element(XbrliNamespace + "segment");
                    if (segmentElement != null)
                    {
                        context.EntitySegment = segmentElement.ToString();
                    }
                }

                // Парсим period
                var periodElement = contextElement.Element(XbrliNamespace + "period");
                if (periodElement != null)
                {
                    var instantElement = periodElement.Element(XbrliNamespace + "instant");
                    if (instantElement != null && DateTime.TryParse(instantElement.Value, out var instantDate))
                    {
                        context.PeriodInstant = instantDate;
                    }

                    var startDateElement = periodElement.Element(XbrliNamespace + "startDate");
                    if (startDateElement != null && DateTime.TryParse(startDateElement.Value, out var startDate))
                    {
                        context.PeriodStartDate = startDate;
                    }

                    var endDateElement = periodElement.Element(XbrliNamespace + "endDate");
                    if (endDateElement != null && DateTime.TryParse(endDateElement.Value, out var endDate))
                    {
                        context.PeriodEndDate = endDate;
                    }

                    var foreverElement = periodElement.Element(XbrliNamespace + "forever");
                    context.PeriodForever = foreverElement != null;
                }

                // Парсим scenario
                var scenarioElement = contextElement.Element(XbrliNamespace + "scenario");
                if (scenarioElement != null)
                {
                    ParseScenario(scenarioElement, context);
                }

                instance.Contexts.Add(context);
            }
        }

        /// <summary>
        /// Парсит scenario из контекста
        /// </summary>
        private void ParseScenario(XElement scenarioElement, Context context)
        {
            // Парсим explicitMember
            foreach (var explicitMember in scenarioElement.Descendants(XbrldiNamespace + "explicitMember"))
            {
                var dimensionAttr = explicitMember.Attribute("dimension");
                var dimensionValue = explicitMember.Value;

                var scenarioItem = new Scenario
                {
                    DimensionType = "explicitMember",
                    DimensionName = dimensionAttr?.Value ?? string.Empty,
                    DimensionValue = dimensionValue
                };

                context.Scenarios.Add(scenarioItem);
            }

            // Парсим typedMember
            foreach (var typedMember in scenarioElement.Descendants(XbrldiNamespace + "typedMember"))
            {
                var dimensionAttr = typedMember.Attribute("dimension");
                var dimensionName = dimensionAttr?.Value ?? string.Empty;

                // Получаем первый дочерний элемент как значение
                var childElement = typedMember.Elements().FirstOrDefault();
                if (childElement != null)
                {
                    var scenarioItem = new Scenario
                    {
                        DimensionType = "typedMember",
                        DimensionName = dimensionName,
                        DimensionCode = childElement.Name.LocalName,
                        DimensionValue = childElement.Value
                    };

                    context.Scenarios.Add(scenarioItem);
                }
            }
        }

        /// <summary>
        /// Парсит единицы измерения из XBRL документа
        /// </summary>
        private void ParseUnits(XDocument doc, Instance instance)
        {
            var units = doc.Descendants(XbrliNamespace + "unit");

            foreach (var unitElement in units)
            {
                var unit = new Unit
                {
                    Id = unitElement.Attribute("id")?.Value ?? string.Empty
                };

                // Парсим measure
                var measureElements = unitElement.Elements(XbrliNamespace + "measure").ToList();
                if (measureElements.Count > 0)
                {
                    // Проверяем, есть ли divide
                    var divideElement = unitElement.Element(XbrliNamespace + "divide");
                    if (divideElement == null)
                    {
                        // Если нет divide, просто берем все measures
                        unit.Measure = string.Join(" ", measureElements.Select(m => m.Value));
                    }
                    else
                    {
                        // Если есть divide, парсим numerator и denominator
                        var numeratorElement = divideElement.Element(XbrliNamespace + "unitNumerator");
                        if (numeratorElement != null)
                        {
                            var numeratorMeasures = numeratorElement.Elements(XbrliNamespace + "measure");
                            unit.Numerator = string.Join(" ", numeratorMeasures.Select(m => m.Value));
                        }

                        var denominatorElement = divideElement.Element(XbrliNamespace + "unitDenominator");
                        if (denominatorElement != null)
                        {
                            var denominatorMeasures = denominatorElement.Elements(XbrliNamespace + "measure");
                            unit.Denominator = string.Join(" ", denominatorMeasures.Select(m => m.Value));
                        }
                    }
                }

                instance.Units.Add(unit);
            }
        }

        /// <summary>
        /// Парсит факты из XBRL документа
        /// </summary>
        private void ParseFacts(XDocument doc, Instance instance)
        {
            // Получаем все элементы, которые не являются контекстами, единицами или служебными элементами
            var root = doc.Root;
            if (root == null) return;

            var knownElements = new HashSet<XName>
            {
                XbrliNamespace + "context",
                XbrliNamespace + "unit",
                XNamespace.Get("http://www.xbrl.org/2003/linkbase") + "schemaRef"
            };

            // Получаем все факты (элементы с атрибутом contextRef)
            var facts = root.Elements()
                .Where(e => !knownElements.Contains(e.Name) && e.Attribute("contextRef") != null);

            foreach (var factElement in facts)
            {
                var fact = new Fact
                {
                    Id = factElement.Attribute("id")?.Value ?? string.Empty,
                    ContextRef = factElement.Attribute("contextRef")?.Value ?? string.Empty,
                    UnitRef = factElement.Attribute("unitRef")?.Value ?? string.Empty
                };

                // Парсим decimals и precision
                var decimalsAttr = factElement.Attribute("decimals");
                if (decimalsAttr != null && int.TryParse(decimalsAttr.Value, out var decimals))
                {
                    fact.Decimals = decimals;
                }

                var precisionAttr = factElement.Attribute("precision");
                if (precisionAttr != null && int.TryParse(precisionAttr.Value, out var precision))
                {
                    fact.Precision = precision;
                }

                // Значение факта
                fact.Value = factElement.Value;

                // Связываем с контекстом
                if (!string.IsNullOrEmpty(fact.ContextRef))
                {
                    fact.Context = instance.Contexts.FirstOrDefault(c => c.Id == fact.ContextRef);
                }

                instance.Facts.Add(fact);
            }

            // Создаем словарь для связи unitRef с Unit
            var unitRefToUnitMap = instance.Units.ToDictionary(u => u.Id, u => u);

            // Связываем факты с единицами измерения
            foreach (var fact in instance.Facts)
            {
                if (!string.IsNullOrEmpty(fact.UnitRef) && unitRefToUnitMap.ContainsKey(fact.UnitRef))
                {
                    fact.Unit = unitRefToUnitMap[fact.UnitRef];
                }
            }
        }
    }
}
