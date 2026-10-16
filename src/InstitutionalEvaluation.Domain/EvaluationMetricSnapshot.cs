using BuildingBlocks.Domain.Primitives;

namespace InstitutionalEvaluation.Domain;

public sealed class EvaluationMetricSnapshot : AggregateRoot<Guid>
{
    public string Category { get; private set; } = string.Empty; // Öğrenci, Personel, Yayın, Birim, Gösterge, BAP, Uzaktan Eğitim
    public string MetricKey { get; private set; } = string.Empty; // ToplamÖğrenci, ÖnLisans, Lisans vb.
    public string Title { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public string? Unit { get; private set; } // Kişi, Adet, m2, Oran (%)
    public string? BreakdownJson { get; private set; } // Alt kırılımlar (Fakülte bazında, program bazında, il/bölge bazında JSON)
    public DateTime SnapshotDateUtc { get; private set; }

    private EvaluationMetricSnapshot() { }

    public static EvaluationMetricSnapshot Create(
        string category,
        string metricKey,
        string title,
        decimal value,
        string? unit = null,
        string? breakdownJson = null)
    {
        return new EvaluationMetricSnapshot
        {
            Id = Guid.NewGuid(),
            Category = category.Trim(),
            MetricKey = metricKey.Trim(),
            Title = title.Trim(),
            Value = value,
            Unit = unit?.Trim(),
            BreakdownJson = breakdownJson,
            SnapshotDateUtc = DateTime.UtcNow
        };
    }

    public void Update(decimal value, string? breakdownJson = null)
    {
        Value = value;
        BreakdownJson = breakdownJson;
        SnapshotDateUtc = DateTime.UtcNow;
    }
}
