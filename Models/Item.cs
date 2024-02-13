
namespace MagellanTest.Models 
{
    /// <summary>
    /// Represents item in the database
    /// </summary>
    public class Item 
    {
        public int Id {get; set;}
        public string ItemName {get; set;} = string.Empty;
        public int? ParentItem {get; set;}
        public int Cost {get; set;}
        public DateTime ReqDate {get; set;}
    }
}