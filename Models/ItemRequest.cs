
namespace MagellanTest.Models 
{
    public record ItemRequest
    (
        string ItemName,
        int? ParentItem,
        int Cost,
        DateTime ReqDate
    );
}