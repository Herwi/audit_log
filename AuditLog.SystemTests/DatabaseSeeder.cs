using AuditLog.Infrastructure.Persistence;
using AuditLog.Infrastructure.Persistence.Entities;

namespace AuditLog.SystemTests;

/// <summary>
/// Seeds the database with test data for user actions testing
/// </summary>
public static class DatabaseSeeder
{
    private static readonly Guid Organization1Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
    private static readonly Guid Organization2Id = Guid.Parse("7b91c8e2-4d3a-4f1e-9c8b-5a6d7e8f9a0b");
    private static readonly Guid Organization3Id = Guid.Parse("9d2e4f6a-8b1c-4e5d-a7f9-3c5b8d1e4a7b");

    public static async Task SeedAsync(RekrutacjaDbContext context)
    {
        // Clear existing data
        context.AuditLogs.RemoveRange(context.AuditLogs);
        context.DocumentHeaders.RemoveRange(context.DocumentHeaders);
        await context.SaveChangesAsync();

        // Seed document headers for contracts
        await SeedDocumentHeaders(context);

        // Seed audit log entries
        await SeedAuditLogs(context);

        await context.SaveChangesAsync();
    }

    private static async Task SeedDocumentHeaders(RekrutacjaDbContext context)
    {
        var headers = new[]
        {
            new DocumentHeaderEntity
            {
                Id = Guid.Parse("c0000001-0000-0000-0000-000000000001"),
                Number = "2024/01/001",
                OrganizationId = Organization1Id,
                CreatedDate = DateTime.Parse("2024-01-15 10:00:00")
            },
            new DocumentHeaderEntity
            {
                Id = Guid.Parse("c0000002-0000-0000-0000-000000000001"),
                Number = "2024/01/002",
                OrganizationId = Organization1Id,
                CreatedDate = DateTime.Parse("2024-01-18 10:00:00")
            },
            new DocumentHeaderEntity
            {
                Id = Guid.Parse("c0000003-0000-0000-0000-000000000001"),
                Number = "2024/01/003",
                OrganizationId = Organization1Id,
                CreatedDate = DateTime.Parse("2024-01-20 08:00:00")
            }
        };

        await context.DocumentHeaders.AddRangeAsync(headers);
    }

    private static async Task SeedAuditLogs(RekrutacjaDbContext context)
    {
        var auditLogs = new List<AuditLogEntity>();

        // ACTION 1: Contract Created with Payment Schedule and Files (3 entries)
        auditLogs.AddRange([
            CreateAuditLog("a1b2c3d4-e5f6-7890-abcd-ef1234567890", "jan.kowalski@company.pl",
                OperationType.Added, EntityType.ContractHeaderEntity, "2024-01-15 10:30:35.100",
                "c0000001-0000-0000-0000-000000000001", "11111111-1111-1111-1111-111111111111"),
            CreateAuditLog("a1b2c3d4-e5f6-7890-abcd-ef1234567890", "jan.kowalski@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-15 10:30:35.200",
                "f0000001-0000-0000-0000-000000000001", "11111111-1111-1111-1111-111111111111"),
            CreateAuditLog("a1b2c3d4-e5f6-7890-abcd-ef1234567890", "jan.kowalski@company.pl",
                OperationType.Added, EntityType.PaymentScheduleEntity, "2024-01-15 10:30:35.250",
                "b0000001-0000-0000-0000-000000000001", "11111111-1111-1111-1111-111111111111")
        ]);

        // ACTION 2: Annex Added (2 entries)
        auditLogs.AddRange([
            CreateAuditLog("b2c3d4e5-f6a7-8901-bcde-f23456789012", "anna.nowak@company.pl",
                OperationType.Added, EntityType.AnnexHeaderEntity, "2024-01-15 11:15:22.000",
                "a0000001-0000-0000-0000-000000000001", "22222222-2222-2222-2222-222222222222"),
            CreateAuditLog("b2c3d4e5-f6a7-8901-bcde-f23456789012", "anna.nowak@company.pl",
                OperationType.Added, EntityType.AnnexChangeEntity, "2024-01-15 11:15:22.050",
                "a0000002-0000-0000-0000-000000000001", "22222222-2222-2222-2222-222222222222")
        ]);

        // ACTION 3: Contract Modified with Payment Schedule Update (3 entries)
        auditLogs.AddRange([
            CreateAuditLog("c3d4e5f6-a7b8-9012-cdef-34567890abcd", "piotr.wisniewski@company.pl",
                OperationType.Modified, EntityType.ContractHeaderEntity, "2024-01-15 14:20:10.000",
                "c0000001-0000-0000-0000-000000000001", "33333333-3333-3333-3333-333333333333"),
            CreateAuditLog("c3d4e5f6-a7b8-9012-cdef-34567890abcd", "piotr.wisniewski@company.pl",
                OperationType.Modified, EntityType.PaymentScheduleEntity, "2024-01-15 14:20:10.100",
                "b0000001-0000-0000-0000-000000000001", "33333333-3333-3333-3333-333333333333"),
            CreateAuditLog("c3d4e5f6-a7b8-9012-cdef-34567890abcd", "piotr.wisniewski@company.pl",
                OperationType.Modified, EntityType.PaymentScheduleEntity, "2024-01-15 14:20:10.150",
                "b0000002-0000-0000-0000-000000000001", "33333333-3333-3333-3333-333333333333")
        ]);

        // ACTION 4: Invoice Created with Multiple Files (4 entries)
        auditLogs.AddRange([
            CreateAuditLog("d4e5f6a7-b8c9-0123-defa-4567890bcdef", "maria.dabrowska@company.pl",
                OperationType.Added, EntityType.InvoiceEntity, "2024-01-16 09:45:30.000",
                "10000001-0000-0000-0000-000000000001", "44444444-4444-4444-4444-444444444444"),
            CreateAuditLog("d4e5f6a7-b8c9-0123-defa-4567890bcdef", "maria.dabrowska@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-16 09:45:30.100",
                "f0000002-0000-0000-0000-000000000001", "44444444-4444-4444-4444-444444444444"),
            CreateAuditLog("d4e5f6a7-b8c9-0123-defa-4567890bcdef", "maria.dabrowska@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-16 09:45:30.150",
                "f0000003-0000-0000-0000-000000000001", "44444444-4444-4444-4444-444444444444"),
            CreateAuditLog("d4e5f6a7-b8c9-0123-defa-4567890bcdef", "maria.dabrowska@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-16 09:45:30.200",
                "f0000004-0000-0000-0000-000000000001", "44444444-4444-4444-4444-444444444444")
        ]);

        // ACTION 5: Complex Funding Restructure (4 entries)
        auditLogs.AddRange([
            CreateAuditLog("e5f6a7b8-c9d0-1234-efab-567890cdef01", "tomasz.kaczmarek@company.pl",
                OperationType.Deleted, EntityType.ContractFundingEntity, "2024-01-16 15:30:45.000",
                "cf000001-0000-0000-0000-000000000001", "55555555-5555-5555-5555-555555555555"),
            CreateAuditLog("e5f6a7b8-c9d0-1234-efab-567890cdef01", "tomasz.kaczmarek@company.pl",
                OperationType.Deleted, EntityType.ContractFundingEntity, "2024-01-16 15:30:45.050",
                "cf000002-0000-0000-0000-000000000001", "55555555-5555-5555-5555-555555555555"),
            CreateAuditLog("e5f6a7b8-c9d0-1234-efab-567890cdef01", "tomasz.kaczmarek@company.pl",
                OperationType.Added, EntityType.ContractFundingEntity, "2024-01-16 15:30:45.100",
                "cf000003-0000-0000-0000-000000000001", "55555555-5555-5555-5555-555555555555"),
            CreateAuditLog("e5f6a7b8-c9d0-1234-efab-567890cdef01", "tomasz.kaczmarek@company.pl",
                OperationType.Added, EntityType.ContractFundingEntity, "2024-01-16 15:30:45.150",
                "cf000004-0000-0000-0000-000000000001", "55555555-5555-5555-5555-555555555555")
        ]);

        // ACTION 6: Annex Modified with Payment Schedule (3 entries)
        auditLogs.AddRange([
            CreateAuditLog("f6a7b8c9-d0e1-2345-fabc-67890def0123", "krzysztof.lewandowski@company.pl",
                OperationType.Modified, EntityType.AnnexHeaderEntity, "2024-01-17 10:00:00.000",
                "a0000001-0000-0000-0000-000000000001", "66666666-6666-6666-6666-666666666666"),
            CreateAuditLog("f6a7b8c9-d0e1-2345-fabc-67890def0123", "krzysztof.lewandowski@company.pl",
                OperationType.Modified, EntityType.AnnexChangeEntity, "2024-01-17 10:00:00.050",
                "a0000002-0000-0000-0000-000000000001", "66666666-6666-6666-6666-666666666666"),
            CreateAuditLog("f6a7b8c9-d0e1-2345-fabc-67890def0123", "krzysztof.lewandowski@company.pl",
                OperationType.Added, EntityType.PaymentScheduleEntity, "2024-01-17 10:00:00.100",
                "b0000003-0000-0000-0000-000000000001", "66666666-6666-6666-6666-666666666666")
        ]);

        // ACTION 7: Payment Schedule Restructure (3 entries)
        auditLogs.AddRange([
            CreateAuditLog("a7b8c9d0-e1f2-3456-abcd-7890ef012345", "magdalena.wozniak@company.pl",
                OperationType.Deleted, EntityType.PaymentScheduleEntity, "2024-01-17 14:30:00.000",
                "b0000004-0000-0000-0000-000000000001", "77777777-7777-7777-7777-777777777777"),
            CreateAuditLog("a7b8c9d0-e1f2-3456-abcd-7890ef012345", "magdalena.wozniak@company.pl",
                OperationType.Added, EntityType.PaymentScheduleEntity, "2024-01-17 14:30:00.050",
                "b0000005-0000-0000-0000-000000000001", "77777777-7777-7777-7777-777777777777"),
            CreateAuditLog("a7b8c9d0-e1f2-3456-abcd-7890ef012345", "magdalena.wozniak@company.pl",
                OperationType.Modified, EntityType.PaymentScheduleEntity, "2024-01-17 14:30:00.100",
                "b0000005-0000-0000-0000-000000000001", "77777777-7777-7777-7777-777777777777")
        ]);

        // ACTION 8: Contract Creation with Everything (6 entries)
        auditLogs.AddRange([
            CreateAuditLog("b8c9d0e1-f2a3-4567-bcde-890fab123456", "robert.kaminski@company.pl",
                OperationType.Added, EntityType.ContractHeaderEntity, "2024-01-18 11:00:00.000",
                "c0000002-0000-0000-0000-000000000001", "88888888-8888-8888-8888-888888888888"),
            CreateAuditLog("b8c9d0e1-f2a3-4567-bcde-890fab123456", "robert.kaminski@company.pl",
                OperationType.Added, EntityType.PaymentScheduleEntity, "2024-01-18 11:00:00.100",
                "b0000006-0000-0000-0000-000000000001", "88888888-8888-8888-8888-888888888888"),
            CreateAuditLog("b8c9d0e1-f2a3-4567-bcde-890fab123456", "robert.kaminski@company.pl",
                OperationType.Added, EntityType.PaymentScheduleEntity, "2024-01-18 11:00:00.150",
                "b0000007-0000-0000-0000-000000000001", "88888888-8888-8888-8888-888888888888"),
            CreateAuditLog("b8c9d0e1-f2a3-4567-bcde-890fab123456", "robert.kaminski@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-18 11:00:00.200",
                "f0000005-0000-0000-0000-000000000001", "88888888-8888-8888-8888-888888888888"),
            CreateAuditLog("b8c9d0e1-f2a3-4567-bcde-890fab123456", "robert.kaminski@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-18 11:00:00.250",
                "f0000006-0000-0000-0000-000000000001", "88888888-8888-8888-8888-888888888888"),
            CreateAuditLog("b8c9d0e1-f2a3-4567-bcde-890fab123456", "robert.kaminski@company.pl",
                OperationType.Added, EntityType.ContractFundingEntity, "2024-01-18 11:00:00.300",
                "cf000005-0000-0000-0000-000000000001", "88888888-8888-8888-8888-888888888888")
        ]);

        // ACTION 9: Invoice Modified (2 entries)
        auditLogs.AddRange([
            CreateAuditLog("c9d0e1f2-a3b4-5678-cdef-90abcd234567", "aleksandra.zielinska@company.pl",
                OperationType.Modified, EntityType.InvoiceEntity, "2024-01-19 09:15:00.000",
                "10000001-0000-0000-0000-000000000001", "99999999-9999-9999-9999-999999999999"),
            CreateAuditLog("c9d0e1f2-a3b4-5678-cdef-90abcd234567", "aleksandra.zielinska@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-19 09:15:00.100",
                "f0000007-0000-0000-0000-000000000001", "99999999-9999-9999-9999-999999999999")
        ]);

        // ACTION 10: Annex with Files (4 entries)
        auditLogs.AddRange([
            CreateAuditLog("d0e1f2a3-b4c5-6789-defa-0bcde3456789", "wojciech.jankowski@company.pl",
                OperationType.Added, EntityType.AnnexHeaderEntity, "2024-01-19 16:45:00.000",
                "a0000003-0000-0000-0000-000000000001", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            CreateAuditLog("d0e1f2a3-b4c5-6789-defa-0bcde3456789", "wojciech.jankowski@company.pl",
                OperationType.Added, EntityType.AnnexChangeEntity, "2024-01-19 16:45:00.050",
                "a0000004-0000-0000-0000-000000000001", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            CreateAuditLog("d0e1f2a3-b4c5-6789-defa-0bcde3456789", "wojciech.jankowski@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-19 16:45:00.100",
                "f0000008-0000-0000-0000-000000000001", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            CreateAuditLog("d0e1f2a3-b4c5-6789-defa-0bcde3456789", "wojciech.jankowski@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-19 16:45:00.150",
                "f0000009-0000-0000-0000-000000000001", "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
        ]);

        // ACTION 11: Contract Created (2 entries)
        auditLogs.AddRange([
            CreateAuditLog("e1f2a3b4-c5d6-789a-efab-cdef456789ab", "katarzyna.nowak@company.pl",
                OperationType.Added, EntityType.ContractHeaderEntity, "2024-01-20 08:30:00.000",
                "c0000003-0000-0000-0000-000000000001", "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            CreateAuditLog("e1f2a3b4-c5d6-789a-efab-cdef456789ab", "katarzyna.nowak@company.pl",
                OperationType.Added, EntityType.PaymentScheduleEntity, "2024-01-20 08:30:00.100",
                "b0000008-0000-0000-0000-000000000001", "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
        ]);

        // ACTION 12: Annex Created (2 entries)
        auditLogs.AddRange([
            CreateAuditLog("f2a3b4c5-d6e7-89ab-fabc-def56789abcd", "marcin.kowalczyk@company.pl",
                OperationType.Added, EntityType.AnnexHeaderEntity, "2024-01-20 13:45:00.000",
                "a0000005-0000-0000-0000-000000000001", "cccccccc-cccc-cccc-cccc-cccccccccccc"),
            CreateAuditLog("f2a3b4c5-d6e7-89ab-fabc-def56789abcd", "marcin.kowalczyk@company.pl",
                OperationType.Added, EntityType.AnnexChangeEntity, "2024-01-20 13:45:00.050",
                "a0000006-0000-0000-0000-000000000001", "cccccccc-cccc-cccc-cccc-cccccccccccc")
        ]);

        // ACTION 13: Invoice Created (3 entries)
        auditLogs.AddRange([
            CreateAuditLog("a3b4c5d6-e7f8-9abc-abcd-ef6789abcdef", "beata.wojcik@company.pl",
                OperationType.Added, EntityType.InvoiceEntity, "2024-01-21 10:00:00.000",
                "10000002-0000-0000-0000-000000000001", "dddddddd-dddd-dddd-dddd-dddddddddddd"),
            CreateAuditLog("a3b4c5d6-e7f8-9abc-abcd-ef6789abcdef", "beata.wojcik@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-21 10:00:00.100",
                "f0000010-0000-0000-0000-000000000001", "dddddddd-dddd-dddd-dddd-dddddddddddd"),
            CreateAuditLog("a3b4c5d6-e7f8-9abc-abcd-ef6789abcdef", "beata.wojcik@company.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-21 10:00:00.150",
                "f0000011-0000-0000-0000-000000000001", "dddddddd-dddd-dddd-dddd-dddddddddddd")
        ]);

        // ACTION 14: Contract Modified (2 entries)
        auditLogs.AddRange([
            CreateAuditLog("b4c5d6e7-f8a9-abcd-bcde-f789abcdef01", "lukasz.mazur@company.pl",
                OperationType.Modified, EntityType.ContractHeaderEntity, "2024-01-21 15:30:00.000",
                "c0000003-0000-0000-0000-000000000001", "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            CreateAuditLog("b4c5d6e7-f8a9-abcd-bcde-f789abcdef01", "lukasz.mazur@company.pl",
                OperationType.Modified, EntityType.PaymentScheduleEntity, "2024-01-21 15:30:00.100",
                "b0000008-0000-0000-0000-000000000001", "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")
        ]);

        // ACTION 15: Payment Schedule Created (2 entries)
        auditLogs.AddRange([
            CreateAuditLog("c5d6e7f8-a9bc-def0-cdef-89abcdef0123", "iwona.krawczyk@company.pl",
                OperationType.Added, EntityType.PaymentScheduleEntity, "2024-01-22 09:00:00.000",
                "b0000009-0000-0000-0000-000000000001", "ffffffff-ffff-ffff-ffff-ffffffffffff"),
            CreateAuditLog("c5d6e7f8-a9bc-def0-cdef-89abcdef0123", "iwona.krawczyk@company.pl",
                OperationType.Modified, EntityType.PaymentScheduleEntity, "2024-01-22 09:00:00.100",
                "b0000009-0000-0000-0000-000000000001", "ffffffff-ffff-ffff-ffff-ffffffffffff")
        ]);

        // ACTION 16: Invoice Created for Organization 2 (2 entries)
        auditLogs.AddRange([
            CreateAuditLog("d6e7f8a9-bcde-f012-def0-9abcdef01234", "adam.nowicki@company2.pl",
                OperationType.Added, EntityType.InvoiceEntity, "2024-01-23 10:00:00.000",
                "10000003-0000-0000-0000-000000000001", "12345678-1234-1234-1234-123456789012",
                Organization2Id),
            CreateAuditLog("d6e7f8a9-bcde-f012-def0-9abcdef01234", "adam.nowicki@company2.pl",
                OperationType.Modified, EntityType.FileEntity, "2024-01-23 10:00:00.100",
                "f0000012-0000-0000-0000-000000000001", "12345678-1234-1234-1234-123456789012",
                Organization2Id)
        ]);

        // ACTION 17: Annex Created for Organization 3 (2 entries)
        auditLogs.AddRange([
            CreateAuditLog("e7f8a9bc-def0-1234-ef01-abcdef012345", "ewa.kowalska@company3.pl",
                OperationType.Added, EntityType.AnnexHeaderEntity, "2024-01-24 11:00:00.000",
                "a0000007-0000-0000-0000-000000000001", "23456789-2345-2345-2345-234567890123",
                Organization3Id),
            CreateAuditLog("e7f8a9bc-def0-1234-ef01-abcdef012345", "ewa.kowalska@company3.pl",
                OperationType.Added, EntityType.AnnexChangeEntity, "2024-01-24 11:00:00.100",
                "a0000008-0000-0000-0000-000000000001", "23456789-2345-2345-2345-234567890123",
                Organization3Id)
        ]);

        await context.AuditLogs.AddRangeAsync(auditLogs);
    }

    private static AuditLogEntity CreateAuditLog(
        string correlationId,
        string userEmail,
        OperationType operationType,
        EntityType entityType,
        string createdDate,
        string entityId,
        string userId,
        Guid? organizationId = null)
    {
        return new AuditLogEntity
        {
            CorrelationId = Guid.Parse(correlationId),
            OrganizationId = organizationId ?? Organization1Id,
            UserEmail = userEmail,
            Type = (int)operationType,
            EntityType = (int)entityType,
            CreatedDate = DateTime.Parse(createdDate),
            EntityId = Guid.Parse(entityId),
            UserId = Guid.Parse(userId)
        };
    }
}