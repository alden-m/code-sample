using Microsoft.AspNetCore.Mvc;
using BizName.Studio.Api.Extensions;
using BizName.Studio.App.Services;

namespace BizName.Studio.Api.Controllers;

[ApiController]
[Route("api/tenant-eligibility")]
public class TenantEligibilityController(ILogger<TenantEligibilityController> logger) : ControllerBase
{
    [HttpPost("evaluate")]
    public async Task<IActionResult> EvaluateEligibility([FromBody] EligibilityRequest request)
    {
        if (string.IsNullOrEmpty(request.TenantId) || string.IsNullOrEmpty(request.PlanType))
            return BadRequest("TenantId and PlanType are required");

        await Task.Delay(100); // Simulate async processing

        var criteria = GetPlanCriteria(request.PlanType);
        var evaluation = EvaluateTenantAgainstCriteria(request, criteria);

        logger.LogInformation("Eligibility evaluated for tenant {TenantId} on plan {PlanType}: {IsEligible}", 
            request.TenantId, request.PlanType, evaluation.IsEligible);

        return Ok(evaluation);
    }

    [HttpGet("plans")]
    public IActionResult GetAvailablePlans()
    {
        var plans = new[]
        {
            new PlanInfo 
            { 
                PlanType = "STARTER", 
                MonthlyFee = 29.99m, 
                MaxUsers = 5, 
                MaxStorage = 1000, 
                MaxApiCalls = 10000,
                Features = new[] { "Basic Analytics", "Email Support", "Standard SLA" }
            },
            new PlanInfo 
            { 
                PlanType = "PROFESSIONAL", 
                MonthlyFee = 99.99m, 
                MaxUsers = 25, 
                MaxStorage = 10000, 
                MaxApiCalls = 100000,
                Features = new[] { "Advanced Analytics", "Priority Support", "Custom Integrations", "Enhanced SLA" }
            },
            new PlanInfo 
            { 
                PlanType = "ENTERPRISE", 
                MonthlyFee = 299.99m, 
                MaxUsers = -1, 
                MaxStorage = -1, 
                MaxApiCalls = -1,
                Features = new[] { "Full Analytics Suite", "24/7 Dedicated Support", "Custom Development", "Premium SLA", "On-premise Deployment" }
            }
        };

        return Ok(plans);
    }

    [HttpPost("upgrade-assessment")]
    public async Task<IActionResult> AssessUpgradeOpportunity([FromBody] UpgradeAssessmentRequest request)
    {
        if (string.IsNullOrEmpty(request.TenantId) || string.IsNullOrEmpty(request.CurrentPlan))
            return BadRequest("TenantId and CurrentPlan are required");

        await Task.Delay(150);

        var assessment = new UpgradeAssessment
        {
            TenantId = request.TenantId,
            CurrentPlan = request.CurrentPlan,
            RecommendedPlan = DetermineRecommendedPlan(request),
            UpgradeReasons = GenerateUpgradeReasons(request),
            EstimatedSavings = CalculateEstimatedSavings(request),
            ImplementationComplexity = AssessImplementationComplexity(request)
        };

        return Ok(assessment);
    }

    [HttpPost("compliance-check")]
    public async Task<IActionResult> CheckComplianceRequirements([FromBody] ComplianceRequest request)
    {
        if (string.IsNullOrEmpty(request.TenantId))
            return BadRequest("TenantId is required");

        await Task.Delay(200);

        var compliance = new ComplianceResult
        {
            TenantId = request.TenantId,
            ComplianceScore = Random.Shared.Next(75, 100),
            RequiredCertifications = GetRequiredCertifications(request),
            MissingRequirements = GetMissingRequirements(request),
            RecommendedActions = GetRecommendedActions(request),
            NextAuditDate = DateTime.UtcNow.AddMonths(3)
        };

        return Ok(compliance);
    }

    private static PlanCriteria GetPlanCriteria(string planType) => planType.ToUpper() switch
    {
        "STARTER" => new PlanCriteria { MinMonthlyRevenue = 0, MaxUsers = 5, RequiredCertifications = Array.Empty<string>() },
        "PROFESSIONAL" => new PlanCriteria { MinMonthlyRevenue = 1000, MaxUsers = 25, RequiredCertifications = new[] { "SOC2" } },
        "ENTERPRISE" => new PlanCriteria { MinMonthlyRevenue = 10000, MaxUsers = -1, RequiredCertifications = new[] { "SOC2", "ISO27001" } },
        _ => new PlanCriteria { MinMonthlyRevenue = 0, MaxUsers = 1, RequiredCertifications = Array.Empty<string>() }
    };

    private static EligibilityResult EvaluateTenantAgainstCriteria(EligibilityRequest request, PlanCriteria criteria)
    {
        var result = new EligibilityResult { TenantId = request.TenantId, PlanType = request.PlanType };
        
        var revenueCheck = request.MonthlyRevenue >= criteria.MinMonthlyRevenue;
        var userCheck = criteria.MaxUsers == -1 || request.UserCount <= criteria.MaxUsers;
        var certificationCheck = criteria.RequiredCertifications.All(cert => 
            request.Certifications?.Contains(cert, StringComparer.OrdinalIgnoreCase) == true);

        result.IsEligible = revenueCheck && userCheck && certificationCheck;
        result.EligibilityScore = CalculateEligibilityScore(request, criteria);
        
        if (!revenueCheck) result.Reasons.Add($"Monthly revenue of ${request.MonthlyRevenue} is below required ${criteria.MinMonthlyRevenue}");
        if (!userCheck) result.Reasons.Add($"User count of {request.UserCount} exceeds plan limit of {criteria.MaxUsers}");
        if (!certificationCheck) result.Reasons.Add($"Missing required certifications: {string.Join(", ", criteria.RequiredCertifications)}");

        return result;
    }

    private static int CalculateEligibilityScore(EligibilityRequest request, PlanCriteria criteria) =>
        (int)((request.MonthlyRevenue / Math.Max(criteria.MinMonthlyRevenue, 1)) * 40 +
              (criteria.MaxUsers == -1 ? 30 : Math.Max(0, 30 - (request.UserCount - criteria.MaxUsers) * 5)) +
              (request.Certifications?.Length ?? 0) * 10);

    private static string DetermineRecommendedPlan(UpgradeAssessmentRequest request) =>
        request.MonthlyUsage > 50000 ? "ENTERPRISE" : 
        request.UserGrowthRate > 0.2 ? "PROFESSIONAL" : 
        request.CurrentPlan;

    private static List<string> GenerateUpgradeReasons(UpgradeAssessmentRequest request) =>
        new List<string>() 
        {
            request.MonthlyUsage > 50000 ? "High usage volume detected" : null,
            request.UserGrowthRate > 0.2 ? "Rapid user growth observed" : null,
            request.SupportTickets > 10 ? "Frequent support requests indicate need for premium support" : null
        }.Where(r => r != null).ToList()!;

    private static decimal CalculateEstimatedSavings(UpgradeAssessmentRequest request) =>
        request.MonthlyUsage > 100000 ? request.MonthlyUsage * 0.001m : 0;

    private static string AssessImplementationComplexity(UpgradeAssessmentRequest request) =>
        request.UserGrowthRate > 0.5 ? "High" : request.UserGrowthRate > 0.2 ? "Medium" : "Low";

    private static string[] GetRequiredCertifications(ComplianceRequest request) =>
        request.Industry?.ToUpper() switch
        {
            "HEALTHCARE" => new[] { "HIPAA", "SOC2", "ISO27001" },
            "FINANCE" => new[] { "PCI-DSS", "SOC2", "ISO27001" },
            "GOVERNMENT" => new[] { "FedRAMP", "SOC2", "ISO27001" },
            _ => new[] { "SOC2" }
        };

    private static string[] GetMissingRequirements(ComplianceRequest request) =>
        new[] { "Data Encryption Policy", "Access Control Review", "Incident Response Plan" }
            .Where(_ => Random.Shared.NextDouble() > 0.7).ToArray();

    private static string[] GetRecommendedActions(ComplianceRequest request) =>
        new[] { "Schedule security audit", "Update data retention policies", "Implement MFA for all users" };
}

public class EligibilityRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public decimal MonthlyRevenue { get; set; }
    public int UserCount { get; set; }
    public string[]? Certifications { get; set; }
    public string? Industry { get; set; }
}

public class EligibilityResult
{
    public string TenantId { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public bool IsEligible { get; set; }
    public int EligibilityScore { get; set; }
    public List<string> Reasons { get; set; } = new();
}

public class PlanCriteria
{
    public decimal MinMonthlyRevenue { get; set; }
    public int MaxUsers { get; set; }
    public string[] RequiredCertifications { get; set; } = Array.Empty<string>();
}

public class PlanInfo
{
    public string PlanType { get; set; } = string.Empty;
    public decimal MonthlyFee { get; set; }
    public int MaxUsers { get; set; }
    public int MaxStorage { get; set; }
    public int MaxApiCalls { get; set; }
    public string[] Features { get; set; } = Array.Empty<string>();
}

public class UpgradeAssessmentRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string CurrentPlan { get; set; } = string.Empty;
    public int MonthlyUsage { get; set; }
    public double UserGrowthRate { get; set; }
    public int SupportTickets { get; set; }
}

public class UpgradeAssessment
{
    public string TenantId { get; set; } = string.Empty;
    public string CurrentPlan { get; set; } = string.Empty;
    public string RecommendedPlan { get; set; } = string.Empty;
    public List<string> UpgradeReasons { get; set; } = new();
    public decimal EstimatedSavings { get; set; }
    public string ImplementationComplexity { get; set; } = string.Empty;
}

public class ComplianceRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Region { get; set; }
}

public class ComplianceResult
{
    public string TenantId { get; set; } = string.Empty;
    public int ComplianceScore { get; set; }
    public string[] RequiredCertifications { get; set; } = Array.Empty<string>();
    public string[] MissingRequirements { get; set; } = Array.Empty<string>();
    public string[] RecommendedActions { get; set; } = Array.Empty<string>();
    public DateTime NextAuditDate { get; set; }
}