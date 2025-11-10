using AuditLog.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuditLog.Infrastructure.Persistence;

public class RekrutacjaDbContext : DbContext
{
    public RekrutacjaDbContext()
    {
    }

    public RekrutacjaDbContext(DbContextOptions<RekrutacjaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLogEntity> AuditLogs { get; set; }

    public virtual DbSet<DocumentHeaderEntity> DocumentHeaders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLogEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_auditlog");

            entity.ToTable("audit_log");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.AffectedColumns).HasColumnName("affected_columns");
            entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp(6) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType).HasColumnName("entity_type");
            entity.Property(e => e.NewValues).HasColumnName("new_values");
            entity.Property(e => e.OldValues).HasColumnName("old_values");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.PrimaryKey)
                .HasMaxLength(1024)
                .HasColumnName("primary_key");
            entity.Property(e => e.SubUnitId).HasColumnName("sub_unit_id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .HasColumnName("user_email");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<DocumentHeaderEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_documentheader");

            entity.ToTable("document_header");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ConclusionDate).HasColumnName("conclusion_date");
            entity.Property(e => e.Confidentiality)
                .HasDefaultValue((short)0)
                .HasColumnName("confidentiality");
            entity.Property(e => e.ContractFlags)
                .HasDefaultValue(0)
                .HasColumnName("contract_flags");
            entity.Property(e => e.ContractNetValue)
                .HasColumnType("money")
                .HasColumnName("contract_net_value");
            entity.Property(e => e.ContractType).HasColumnName("contract_type");
            entity.Property(e => e.ContractValue)
                .HasColumnType("money")
                .HasColumnName("contract_value");
            entity.Property(e => e.ContractorId).HasMaxLength(255);
            entity.Property(e => e.ContractorId1).HasColumnName("contractor_id");
            entity.Property(e => e.ContractorName).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("timestamp(0) without time zone")
                .HasColumnName("deleted_date");
            entity.Property(e => e.DocumentType).HasColumnName("document_type");
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.EngagementId).HasColumnName("engagement_id");
            entity.Property(e => e.ExecutionDate).HasColumnName("execution_date");
            entity.Property(e => e.IsFunded)
                .HasDefaultValue(false)
                .HasColumnName("is_funded");
            entity.Property(e => e.IsMultiyear)
                .HasDefaultValue(false)
                .HasColumnName("is_multiyear");
            entity.Property(e => e.Number)
                .HasMaxLength(255)
                .HasColumnName("number");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.PaymentDueDate).HasColumnName("payment_due_date");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");
            entity.Property(e => e.SubUnitId).HasColumnName("sub_unit_id");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasColumnName("subject");
        });
    }
}
