using System;
using InstitutionalEvaluation.Domain;
using Xunit;

namespace UniversalSystem.UnitTests;

public class InstitutionalEvaluationTests
{
    [Fact]
    public void CreateSnapshot_ValidParameters_ShouldInitializeCorrectly()
    {
        var snapshot = EvaluationMetricSnapshot.Create(
            "Öğrenci İstatistikleri",
            "ToplamÖğrenci",
            "Üniversite Toplam Öğrenci Sayısı",
            14250,
            "Kişi",
            "{\"Lisans\":10450,\"ÖnLisans\":2000,\"Lisansüstü\":1800}");

        Assert.NotEqual(Guid.Empty, snapshot.Id);
        Assert.Equal("Öğrenci İstatistikleri", snapshot.Category);
        Assert.Equal("ToplamÖğrenci", snapshot.MetricKey);
        Assert.Equal(14250, snapshot.Value);
        Assert.Equal("Kişi", snapshot.Unit);
        Assert.NotNull(snapshot.BreakdownJson);
    }

    [Fact]
    public void UpdateSnapshot_ShouldUpdateValueAndTimestamp()
    {
        var snapshot = EvaluationMetricSnapshot.Create(
            "BAP İstatistikleri",
            "ProjeSayisi",
            "Toplam BAP Proje Sayısı",
            142,
            "Adet");

        var oldDate = snapshot.SnapshotDateUtc;

        snapshot.Update(150, "{\"Tamamlanan\":88,\"DevamEden\":62}");

        Assert.Equal(150, snapshot.Value);
        Assert.True(snapshot.SnapshotDateUtc >= oldDate);
        Assert.NotNull(snapshot.BreakdownJson);
    }
}
