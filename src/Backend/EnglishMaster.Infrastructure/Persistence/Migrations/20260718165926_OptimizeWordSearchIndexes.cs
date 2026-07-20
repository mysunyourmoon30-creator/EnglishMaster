using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishMaster.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeWordSearchIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Words_IsActive_Slug",
                table: "Words",
                columns: new[] { "IsActive", "Slug" });

            migrationBuilder.CreateIndex(
                name: "IX_Words_IsActive_Text",
                table: "Words",
                columns: new[] { "IsActive", "Text" });

            migrationBuilder.Sql(
                """
                IF FULLTEXTSERVICEPROPERTY('IsFullTextInstalled') = 1
                    AND NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = N'EnglishMasterFullText')
                BEGIN
                    CREATE FULLTEXT CATALOG [EnglishMasterFullText] AS DEFAULT;
                END;

                IF FULLTEXTSERVICEPROPERTY('IsFullTextInstalled') = 1
                    AND OBJECT_ID(N'[dbo].[Words]') IS NOT NULL
                    AND EXISTS (SELECT 1 FROM sys.indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[Words]') AND [name] = N'PK_Words')
                    AND NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[Words]'))
                BEGIN
                    CREATE FULLTEXT INDEX ON [dbo].[Words]
                    (
                        [Text] LANGUAGE 1033,
                        [Slug] LANGUAGE 1033,
                        [MeaningTh] LANGUAGE 1054,
                        [MeaningEn] LANGUAGE 1033
                    )
                    KEY INDEX [PK_Words]
                    ON [EnglishMasterFullText]
                    WITH CHANGE_TRACKING AUTO;
                END;
                """,
                suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_IsActive_Slug",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_IsActive_Text",
                table: "Words");

            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE [object_id] = OBJECT_ID(N'[dbo].[Words]'))
                BEGIN
                    DROP FULLTEXT INDEX ON [dbo].[Words];
                END;

                IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = N'EnglishMasterFullText')
                BEGIN
                    DROP FULLTEXT CATALOG [EnglishMasterFullText];
                END;
                """,
                suppressTransaction: true);
        }
    }
}
