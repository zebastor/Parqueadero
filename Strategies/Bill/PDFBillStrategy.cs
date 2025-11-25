using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Parqueadero.Strategies.Bill;

public class PDFBillStrategy : IBillStrategy
{
    public byte[] GenerarFactura(string detalle)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Header()
                .Text("Comprobante de Transacción")
                .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                .PaddingVertical(10)
                .Column(column =>
                {
                    column.Spacing(15);

                    column.Item().Text("Detalles:")
                        .Bold().FontSize(14);

                    column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Text(detalle);

                    column.Item().Text(text =>
                    {
                        text.Span("Fecha de emisión: ").SemiBold();
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy"));
                    });
                });

                page.Footer()
                .AlignCenter()
                .Text("Este documento ha sido generado automáticamente.")
                .FontSize(10).FontColor(Colors.Grey.Medium);
            });
        });

        return document.GeneratePdf();
    }

    public string TipoContenido() => "application/pdf";
    public string ObtenerExtension() => "pdf";
}