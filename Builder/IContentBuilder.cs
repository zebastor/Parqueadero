namespace Parqueadero.Builder;

public interface IContentBuilder
{
    IContentBuilder AddTitle(string title);
    IContentBuilder AddParagraph(string paragraph);
    IContentBuilder AddPrice(decimal price);
    IContentBuilder AddDate(DateTime date);
    IContentBuilder AddUser(string user);
    IContentBuilder AddNumberBill(string numberBill);
    string BuildContent();
}
    