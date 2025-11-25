using System.Text;

namespace Parqueadero.Builder;

public class ReporteContentBuilder : IContentBuilder
{
    private readonly StringBuilder _content = new();

    public IContentBuilder AddTitle(string title)
    {
        _content.AppendLine(title);
        return this;
    }

    public IContentBuilder AddParagraph(string paragraph)
    {
        _content.AppendLine(paragraph);
        return this;
    }

    public IContentBuilder AddPrice(decimal price)
    {
        _content.AppendLine(price.ToString("C"));
        return this;
    }

    public IContentBuilder AddDate(DateTime date)
    {
        _content.AppendLine(date.ToString("dd/MM/yyyy"));
        return this;
    }

    public IContentBuilder AddUser(string user)
    {
        _content.AppendLine(user);
        return this;
    }

    public IContentBuilder AddNumberBill(string numberBill)
    {
        _content.AppendLine(numberBill);
        return this;
    }

    public string BuildContent()
    {
        return _content.ToString();
    }
}
