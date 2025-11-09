using AuditLog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuditLog.Domain;

public class AuditLogDbContext : DbContext
{
    public AuditLogDbContext(DbContextOptions<AuditLogDbContext> options) : base(options)
    {
    }

    public DbSet<Entities.AuditLog> AuditLogs { get; set; }
    public DbSet<DocumentHeader> DocumentHeaders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AuditLog entity configuration
        modelBuilder.Entity<Entities.AuditLog>(entity =>
        {
            entity.ToTable("audit_log");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserEmail).HasColumnName("user_email").HasMaxLength(255);
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.EntityType).HasColumnName("entity_type");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.OldValues).HasColumnName("old_values");
            entity.Property(e => e.NewValues).HasColumnName("new_values");
            entity.Property(e => e.AffectedColumns).HasColumnName("affected_columns");
            entity.Property(e => e.PrimaryKey).HasColumnName("primary_key").HasMaxLength(1024);
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
            entity.Property(e => e.SubUnitId).HasColumnName("sub_unit_id");
        });

        // DocumentHeader entity configuration
        modelBuilder.Entity<DocumentHeader>(entity =>
        {
            entity.ToTable("document_header");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Number).HasColumnName("number").HasMaxLength(255);
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.ExecutionDate).HasColumnName("execution_date");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.ConclusionDate).HasColumnName("conclusion_date");
            entity.Property(e => e.ContractorIdString).HasColumnName("ContractorId").HasMaxLength(255);
            entity.Property(e => e.DocumentType).HasColumnName("document_type");
            entity.Property(e => e.Subject).HasColumnName("subject").HasMaxLength(255);
            entity.Property(e => e.ContractValue).HasColumnName("contract_value").HasColumnType("money");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(255);
            entity.Property(e => e.DeletedDate).HasColumnName("deleted_date");
            entity.Property(e => e.PaymentDueDate).HasColumnName("payment_due_date");
            entity.Property(e => e.ContractorName).HasColumnName("ContractorName").HasMaxLength(255);
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.EngagementId).HasColumnName("engagement_id");
            entity.Property(e => e.IsMultiyear).HasColumnName("is_multiyear").HasDefaultValue(false);
            entity.Property(e => e.Confidentiality).HasColumnName("confidentiality").HasDefaultValue((short)0);
            entity.Property(e => e.SubUnitId).HasColumnName("sub_unit_id");
            entity.Property(e => e.ContractType).HasColumnName("contract_type");
            entity.Property(e => e.IsFunded).HasColumnName("is_funded").HasDefaultValue(false);
            entity.Property(e => e.ContractorId).HasColumnName("contractor_id");
            entity.Property(e => e.ContractFlags).HasColumnName("contract_flags").HasDefaultValue(0);
            entity.Property(e => e.ContractNetValue).HasColumnName("contract_net_value").HasColumnType("money");
        });
    }
}
