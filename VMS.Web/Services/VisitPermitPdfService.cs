using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VMS.Web.ViewModels.Permit;

namespace VMS.Web.Services
{
    public class VisitPermitPdfService : IVisitPermitPdfService
    {
        public byte[] Generate(VisitPermitViewModel model)
        {
            var qrBytes = Convert.FromBase64String(model.QrBase64);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Content().Column(column =>
                    {
                        column.Spacing(8);

                        // ==========================
                        // HEADER + QR
                        // ==========================

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item()
                                    .Text("Data Center Work / Visitor Access Permit")
                                    .FontSize(18)
                                    .Bold();

                                left.Item()
                                    .Text("Visitor Management System")
                                    .FontColor(Colors.Grey.Darken1);

                                left.Item()
                                    .PaddingTop(8)
                                    .Text($"Permit No: {model.VisitReference}")
                                    .Bold();
                            });

                            row.ConstantItem(100)
                                .AlignRight()
                                .Column(qr =>
                                {
                                    qr.Item()
                                        .Width(80)
                                        .Height(80)
                                        .Image(qrBytes)
                                        .FitArea();

                                    qr.Item()
                                        .PaddingTop(3)
                                        .AlignCenter()
                                        .Text(model.VisitReference)
                                        .FontSize(6);
                                });
                        });

                        // ==========================
                        // REQUEST INFORMATION
                        // ==========================

                        column.Item()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(6)
                            .Text("Request Information")
                            .Bold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            AddField(
                                table,
                                "Requested By",
                                model.RequestedBy);

                            AddField(
                                table,
                                "Host",
                                model.HostName);

                            AddField(
                                table,
                                "Department",
                                model.DepartmentName);

                            AddField(
                                table,
                                "Valid From",
                                model.VisitFromDateTime
                                    .ToString("dd-MMM-yyyy hh:mm tt"));

                            AddField(
                                table,
                                "Valid To",
                                model.VisitToDateTime
                                    .ToString("dd-MMM-yyyy hh:mm tt"));

                            AddField(
                                table,
                                "Location",
                                model.MeetingLocation);

                            AddField(
                                table,
                                "Purpose / Work Description",
                                model.Purpose);

                            AddField(
                                table,
                                "Status",
                                model.Status);
                        });

                        // ==========================
                        // VISITORS
                        // ==========================

                        column.Item()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(6)
                            .Text($"Visitors ({model.Visitors.Count})")
                            .Bold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(20);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn();
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn(1.3f);
                                columns.RelativeColumn();
                                columns.RelativeColumn(1.1f);
                            });

                            table.Header(header =>
                            {
                                HeaderCell(header, "#");
                                HeaderCell(header, "Name");
                                HeaderCell(header, "ID Type");
                                HeaderCell(header, "ID Number");
                                HeaderCell(header, "ID Expiry");
                                HeaderCell(header, "Nationality");
                                HeaderCell(header, "Company");
                                HeaderCell(header, "Designation");
                                HeaderCell(header, "Mobile");
                            });

                            for (var i = 0;
                                 i < model.Visitors.Count;
                                 i++)
                            {
                                var visitor = model.Visitors[i];

                                BodyCell(
                                    table,
                                    (i + 1).ToString());

                                BodyCell(
                                    table,
                                    visitor.FullName);

                                BodyCell(
                                    table,
                                    visitor.IdType);

                                BodyCell(
                                    table,
                                    visitor.IdNumber);

                                BodyCell(
                                    table,
                                    visitor.IdExpiryDate
                                        .ToString("dd-MMM-yyyy"));

                                BodyCell(
                                    table,
                                    visitor.Nationality);

                                BodyCell(
                                    table,
                                    visitor.CompanyName);

                                BodyCell(
                                    table,
                                    visitor.Designation);

                                BodyCell(
                                    table,
                                    visitor.PhoneNumber);
                            }
                        });

                        // ==========================
                        // FOOTER NOTE
                        // ==========================

                        column.Item()
                            .PaddingTop(5)
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(6)
                            .Text(
                                "This permit is valid only during the approved access period. " +
                                "Reception will record each visitor's individual entry and exit.")
                            .FontSize(8);
                    });
                });
            });

            return document.GeneratePdf();
        }


        private static void AddField(
            TableDescriptor table,
            string label,
            string? value)
        {
            table.Cell()
                .Padding(5)
                .Column(column =>
                {
                    column.Item()
                        .Text(label)
                        .FontSize(7)
                        .Bold();

                    column.Item()
                        .Text(value ?? "-")
                        .FontSize(9);
                });
        }


        private static void HeaderCell(
            TableCellDescriptor header,
            string text)
        {
            header.Cell()
                .Background(Colors.Grey.Lighten2)
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Padding(4)
                .Text(text)
                .Bold()
                .FontSize(7);
        }


        private static void BodyCell(
            TableDescriptor table,
            string? text)
        {
            table.Cell()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(4)
                .Text(text ?? "-")
                .FontSize(7);
        }
    }
}